using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using User.Application.Mappers;
using User.Application.Providers;
using User.Core;
using User.Core.Enums;
using User.Core.Exceptions;
using User.Core.Handlers;
using User.Core.Requests;
using User.Core.Responses;
using User.Infrastructure.Data;
using UserEntity = User.Core.Entities.User;

namespace User.Application.Handlers
{
    public class UserHandler : IUserHandler
    {
        private readonly UserContext _context;
        private readonly IPasswordProvider _passwordProvider;
        private readonly UserSettings _settings;

        public UserHandler(
            UserContext context,
            IPasswordProvider passwordProvider,
            IOptions<UserSettings> settings)
        {
            _context = context;
            _passwordProvider = passwordProvider;
            _settings = settings.Value;
        }

        public async Task<UserResponse> CreateAsync(UserRequest request)
        {
            var user = request.ToUser();
            user.Password = _passwordProvider.HashPassword(request.Password);

            await ValidExistsAsync(user);

            _context.Add(user);
            await _context.SaveChangesAsync();

            return user.ToUserResponse(_settings.FileApiUrl);
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(w => w.Id == id && w.IsDeleted == false);

            if (user == null)
                throw new NotFoundException("User not found");

            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedResponse<UserResponse>> GetAsync(PaginatedRequest request)
        {
            var query = _context.Users.AsNoTracking()
                .Include(i => i.UserPhoto)
                .Where(w => w.IsDeleted == false);

            if (request.UserId.HasValue)
                query = query.Where(w => w.Id == request.UserId.Value);

            if (request.UserIds is not null && request.UserIds.Any())
                query = query.Where(w => request.UserIds.Contains(w.Id));

            if (request.Search is not null)
                query = query.Where(w => w.Name.ToLower() == request.Search.ToLower());

            var users = await query
                .OrderBy(o => o.Name)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var totalItems = await query.CountAsync();
            var response = users.Select(s => s.ToUserResponse(_settings.FileApiUrl)).ToList();
            return new PaginatedResponse<UserResponse>(response, totalItems, request.PageIndex, request.PageSize, request);
        }

        public async Task<UserResponse> GetByIdAsync(Guid id)
        {
            var user = await _context.Users.AsNoTracking()
                .Include(i => i.UserPhoto)
                .FirstOrDefaultAsync(w => w.Id == id && w.IsDeleted == false);

            if (user == null)
                throw new NotFoundException("User not found");

            return user.ToUserResponse(_settings.FileApiUrl);
        }

        public async Task UpdateAsync(Guid id, UserUpdateRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(w => w.Id == id && w.IsDeleted == false);

            if (user == null)
                throw new NotFoundException("User not found");

            user.Update(request);

            if (!string.IsNullOrWhiteSpace(request.Password))
                user.Password = _passwordProvider.HashPassword(request.Password);

            await ValidExistsAsync(user);

            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        private async Task ValidExistsAsync(UserEntity user)
        {
            var exist = await _context.Users.AsNoTracking()
                .AnyAsync(w => w.Email == user.Email && w.Id != user.Id && w.IsDeleted == false);

            if (exist)
                throw new BusinessException("User email already exists");
        }
    }
}
