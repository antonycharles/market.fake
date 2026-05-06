using Microsoft.EntityFrameworkCore;
using User.Application.Mappers;
using User.Core.Exceptions;
using User.Core.Handlers;
using User.Core.Requests;
using User.Core.Responses;
using User.Infrastructure.Data;

namespace User.Application.Handlers
{
    public class UserPhotoHandler : IUserPhotoHandler
    {
        private readonly UserContext _context;

        public UserPhotoHandler(UserContext context)
        {
            _context = context;
        }

        public async Task DeleteAsync(Guid id)
        {
            var photo = await _context.UserPhotos
                .FirstOrDefaultAsync(w => w.Id == id && w.IsDeleted == false);

            if (photo == null)
                throw new NotFoundException("Photo not found");

            photo.IsDeleted = true;
            photo.UpdatedAt = DateTime.UtcNow;

            _context.Update(photo);
            await _context.SaveChangesAsync();
        }

        public async Task<UserPhotoResponse> GetByIdAsync(Guid id)
        {
            var photo = await _context.UserPhotos.AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == id && w.IsDeleted == false);

            if (photo == null)
                throw new NotFoundException("Photo not found");

            return photo.ToUserPhotoResponse();
        }

        public async Task UpdateOrCreateAsync(UserPhotoRequest request)
        {
            var userExists = await _context.Users.AsNoTracking()
                .AnyAsync(w => w.Id == request.UserId && w.IsDeleted == false);

            if (!userExists)
                throw new BusinessException("User not found");

            var photo = await _context.UserPhotos
                .FirstOrDefaultAsync(w => w.UserId == request.UserId && w.IsDeleted == false);

            if (photo == null)
            {
                photo = request.ToUserPhoto();
                _context.UserPhotos.Add(photo);
            }
            else
            {
                photo.UpdateUserPhoto(request);
                _context.UserPhotos.Update(photo);
            }

            await _context.SaveChangesAsync();
        }
    }
}
