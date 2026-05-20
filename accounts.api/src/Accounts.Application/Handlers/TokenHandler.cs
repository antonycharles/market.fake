using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Accounts.Application.Extensions;
using Accounts.Core;
using Accounts.Core.Entities;
using Accounts.Core.Enums;
using Accounts.Core.Exceptions;
using Accounts.Core.Handlers;
using Accounts.Core.Requests;
using Accounts.Core.Responses;
using Accounts.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Accounts.Application.Handlers
{
    public class TokenHandler : ITokenHandler
    {
        private const string T_ISSUER = "accounts.api";
        private const string MSG_REDIRECT_URI_INVALID = "Invalid callback url";

        private readonly AccountsContext _context;
        private readonly ITokenKeyHandler _tokenKeyHandler;
        private readonly AccountsSettings _settings;

        public TokenHandler(ITokenKeyHandler tokenKeyHandler, AccountsContext context, IOptions<AccountsSettings> settings)
        {
            _tokenKeyHandler = tokenKeyHandler;
            _context = context;
            _settings = settings.Value;
        }

        public async Task<AuthenticationResponse> GenerateClientTokenAsync(Guid clientId, string appSlug)
        {
            var client = await _context.Clients.AsNoTracking()
                .Include(i => i.ClientProfiles.Where(w => w.Status == StatusEnum.Active && w.IsDeleted == false))
                .ThenInclude(i => i.Profile)
                .ThenInclude(i => i.ProfilePermissions.Where(w => w.Status == StatusEnum.Active && w.IsDeleted == false))
                .ThenInclude(i => i.Permission)
                .Include(i => i.ClientProfiles.Where(w => w.Status == StatusEnum.Active && w.IsDeleted == false))
                .ThenInclude(i => i.Profile)
                .ThenInclude(i => i.App)
                .FirstOrDefaultAsync(w => w.Id == clientId);

            client.ClientProfiles = client.ClientProfiles.Where(w => w.Profile.App.Slug == appSlug).ToList();

            var profile = client.ClientProfiles
                .Select(s => s.Profile).FirstOrDefault();

            var roles = GetRoles(profile);

            var claims = new List<Claim>
            {
                new Claim(CustomClaimTypes.Id, client.Id.ToString()),
                new Claim(CustomClaimTypes.Name, client.Name),
                new Claim(CustomClaimTypes.Type, UserTypeEnum.client.ToString()),
            };

            foreach (var role in roles)
                claims.Add(new Claim(CustomClaimTypes.Role, profile.App.Code + "-" + role));


            return GenerationAuthenticationResponse(claims, client.Id, UserTypeEnum.client, profile.App);
        }


        public async Task<AuthenticationResponse> GenerateUserTokenAsync(Guid userId, string appSlug, string? redirectUri, EnvironmentEnum? environment)
        {
            var user = await _context.Users.AsNoTracking()
                .Include(i => i.UserProfiles.Where(w => w.Status == StatusEnum.Active && w.IsDeleted == false))
                .ThenInclude(i => i.Profile)
                .ThenInclude(i => i.App)
                .Include(i => i.UserProfiles.Where(w => w.Status == StatusEnum.Active && w.IsDeleted == false))
                .Include(i => i.UserProfiles.Where(w => w.Status == StatusEnum.Active && w.IsDeleted == false))
                .ThenInclude(i => i.Profile)
                .ThenInclude(i => i.ProfilePermissions.Where(w => w.Status == StatusEnum.Active && w.IsDeleted == false))
                .ThenInclude(i => i.Permission)
                .Include(i => i.UserPhoto)
                .FirstOrDefaultAsync(w => w.Id == userId);

            var profileQuery = user?.UserProfiles?
                .Where(w => w.Status == StatusEnum.Active)
                .Where(w => w.Profile?.App?.Slug == appSlug && w.Profile.Status == StatusEnum.Active);

            var profile = profileQuery.Select(s => s.Profile).FirstOrDefault();

            var roles = GetRoles(profile);

            var claims = new List<Claim>
            {
                new Claim(CustomClaimTypes.Id, user.Id.ToString()),
                new Claim(CustomClaimTypes.Name, user.Name),
                new Claim(CustomClaimTypes.Email, user.Email),
                new Claim(CustomClaimTypes.Type, UserTypeEnum.user.ToString()),
                new Claim(CustomClaimTypes.Image, user.UserPhoto != null ? _settings.FileApiUrl + "/File/" + user.UserPhoto.DocumentId : "")
            };

            foreach (var role in roles)
                claims.Add(new Claim(CustomClaimTypes.Role, profile.App.Code + "-" + role));

            var callbackUrl = await DefineCallbackAsync(appSlug, redirectUri, environment);

            return GenerationAuthenticationResponse(claims, user.Id, UserTypeEnum.user, profile?.App, callbackUrl);
        }

        private AuthenticationResponse GenerationAuthenticationResponse(List<Claim> claims, Guid authId, UserTypeEnum userType, App? app, string? redirectUri = null)
        {
            SecurityTokenDescriptor jwt = GetSecurityTokenDescriptor(claims, app?.Slug);

            var tokenHandler = new JsonWebTokenHandler();

            var key = _tokenKeyHandler.GetPrivateKey();

            jwt.SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.EcdsaSha256);
            var lastJws = tokenHandler.CreateToken(jwt);

            return new AuthenticationResponse
            {
                ExpiresIn = jwt.Expires?.AddMinutes(-3),
                Token = lastJws,
                AuthId = authId,
                AppSlug = app?.Slug,
                CallbackUrl = redirectUri,
                UserType = userType,
                RefreshToken = Guid.NewGuid().ToString()
            };
        }

        private async Task<string> DefineCallbackAsync(string appSlug, string? redirectUrl, EnvironmentEnum? environment)
        {
            if (!string.IsNullOrWhiteSpace(redirectUrl))
            {
                await ValidateRedirectUriAsync(redirectUrl, appSlug);
                return redirectUrl;
            }
            else
            {
                var callback = await _context.AppCallbacks.AsNoTracking()
                    .Include(i => i.App)
                    .FirstOrDefaultAsync(w =>
                        w.Environment == environment &&
                        w.IsDeleted == false &&
                        w.IsDefault == true &&
                        w.Status == StatusEnum.Active &&
                        w.App.Slug == appSlug);

                if (callback == null)
                    throw new BusinessException(MSG_REDIRECT_URI_INVALID);

                return callback.Url;
            }
        }

        private async Task ValidateRedirectUriAsync(string? redirectUrl, string appSlug)
        {
            if (string.IsNullOrWhiteSpace(appSlug))
                throw new BusinessException(MSG_REDIRECT_URI_INVALID);

            redirectUrl = redirectUrl?.Trim();

            var callbackExists = await _context.AppCallbacks.AsNoTracking()
                .Include(i => i.App)
                .AnyAsync(w =>
                    w.IsDeleted == false &&
                    w.Status == StatusEnum.Active &&
                    w.Url == redirectUrl &&
                    w.App.IsDeleted == false &&
                    w.App.Status == StatusEnum.Active &&
                    w.App.Slug == appSlug);

            if (!callbackExists)
                throw new BusinessException(MSG_REDIRECT_URI_INVALID);
        }

        private SecurityTokenDescriptor GetSecurityTokenDescriptor(List<Claim> claims, string? appSlug = null)
        {
            return new SecurityTokenDescriptor
            {
                Issuer = T_ISSUER,
                Audience = appSlug ?? T_ISSUER,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddHours(2).AddMinutes(3),
                Subject = new ClaimsIdentity(claims)
            };
        }

        private List<string> GetRoles(Profile? profile)
        {
            var roles = new List<string>();

            if(profile == null)
                return roles;

            var rolesProfile = profile.ProfilePermissions
                .Where(w => w.Permission != null && w.Permission.Role != null && w.Status == StatusEnum.Active && w.Permission.Status == StatusEnum.Active)
                .Select(s => s.Permission?.Role);

            roles.AddRange(rolesProfile);

            return roles;
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JsonWebTokenHandler();

            var key = _tokenKeyHandler.GetPublicKeys().First();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = T_ISSUER,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}