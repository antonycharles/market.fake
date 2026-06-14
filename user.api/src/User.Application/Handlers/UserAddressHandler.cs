using Microsoft.EntityFrameworkCore;
using User.Application.Mappers;
using User.Core.Exceptions;
using User.Core.Handlers;
using User.Core.Requests;
using User.Core.Responses;
using User.Infrastructure.Data;

namespace User.Application.Handlers
{
    public class UserAddressHandler : IUserAddressHandler
    {
        private readonly UserContext _context;

        public UserAddressHandler(UserContext context)
        {
            _context = context;
        }

        public async Task DeleteAsync(Guid id)
        {
            var address = await _context.UserAddresses
                .FirstOrDefaultAsync(w => w.Id == id && w.IsDeleted == false);

            if (address == null)
                throw new NotFoundException("Address not found");

            address.IsDeleted = true;
            address.UpdatedAt = DateTime.UtcNow;

            _context.Update(address);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMeAsync(Guid id, Guid userId)
        {
            var address = await _context.UserAddresses
                .FirstOrDefaultAsync(w =>
                    w.Id == id &&
                    w.UserId == userId &&
                    w.IsDeleted == false);

            if (address == null)
                throw new NotFoundException("Address not found");

            address.IsDeleted = true;
            address.UpdatedAt = DateTime.UtcNow;

            _context.Update(address);
            await _context.SaveChangesAsync();
        }

        public async Task<UserAddressResponse> GetByIdAsync(Guid id)
        {
            var address = await _context.UserAddresses.AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == id && w.IsDeleted == false);

            if (address == null)
                throw new NotFoundException("Address not found");

            return address.ToUserAddressResponse();
        }

        public async Task<List<UserAddressResponse>> GetByUserIdAsync(Guid userId)
        {
            var userExists = await _context.Users.AsNoTracking()
                .AnyAsync(w => w.Id == userId && w.IsDeleted == false);

            if (!userExists)
                throw new BusinessException("User not found");

            var addresses = await _context.UserAddresses.AsNoTracking()
                .Where(w => w.UserId == userId && w.IsDeleted == false)
                .OrderByDescending(o => o.IsPrimary)
                .ThenByDescending(o => o.CreatedAt)
                .ToListAsync();

            return addresses.Select(s => s.ToUserAddressResponse()).ToList();
        }

        public async Task UpdateOrCreateAsync(UserAddressRequest request)
        {
            var userExists = await _context.Users.AsNoTracking()
                .AnyAsync(w => w.Id == request.UserId && w.IsDeleted == false);

            if (!userExists)
                throw new BusinessException("User not found");

            var address = request.Id.HasValue
                ? await _context.UserAddresses
                    .FirstOrDefaultAsync(w =>
                        w.Id == request.Id.Value &&
                        w.UserId == request.UserId &&
                        w.IsDeleted == false)
                : null;

            if (request.Id.HasValue && address == null)
                throw new NotFoundException("Address not found");

            if (address == null)
            {
                address = request.ToUserAddress();
                _context.UserAddresses.Add(address);
            }
            else
            {
                address.UpdateUserAddress(request);
                _context.UserAddresses.Update(address);
            }

            if (request.IsPrimary)
                await UnsetPrimaryAddressesAsync(request.UserId, address.Id);

            await _context.SaveChangesAsync();
        }

        private async Task UnsetPrimaryAddressesAsync(Guid userId, Guid currentAddressId)
        {
            var addresses = await _context.UserAddresses
                .Where(w =>
                    w.UserId == userId &&
                    w.Id != currentAddressId &&
                    w.IsPrimary &&
                    w.IsDeleted == false)
                .ToListAsync();

            foreach (var address in addresses)
            {
                address.IsPrimary = false;
                address.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
