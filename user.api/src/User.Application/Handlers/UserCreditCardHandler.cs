using Microsoft.EntityFrameworkCore;
using User.Application.Mappers;
using User.Core.Exceptions;
using User.Core.Handlers;
using User.Core.Requests;
using User.Core.Responses;
using User.Infrastructure.Data;

namespace User.Application.Handlers
{
    public class UserCreditCardHandler : IUserCreditCardHandler
    {
        private readonly UserContext _context;

        public UserCreditCardHandler(UserContext context)
        {
            _context = context;
        }

        public async Task DeleteAsync(Guid id)
        {
            var creditCard = await _context.UserCreditCards
                .FirstOrDefaultAsync(w => w.Id == id && w.IsDeleted == false);

            if (creditCard == null)
                throw new NotFoundException("Credit card not found");

            creditCard.IsDeleted = true;
            creditCard.UpdatedAt = DateTime.UtcNow;

            _context.Update(creditCard);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMeAsync(Guid id, Guid userId)
        {
            var creditCard = await _context.UserCreditCards
                .FirstOrDefaultAsync(w =>
                    w.Id == id &&
                    w.UserId == userId &&
                    w.IsDeleted == false);

            if (creditCard == null)
                throw new NotFoundException("Credit card not found");

            creditCard.IsDeleted = true;
            creditCard.UpdatedAt = DateTime.UtcNow;

            _context.Update(creditCard);
            await _context.SaveChangesAsync();
        }

        public async Task<UserCreditCardResponse> GetByIdAsync(Guid id)
        {
            var creditCard = await _context.UserCreditCards.AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == id && w.IsDeleted == false);

            if (creditCard == null)
                throw new NotFoundException("Credit card not found");

            return creditCard.ToUserCreditCardResponse();
        }

        public async Task<List<UserCreditCardResponse>> GetByUserIdAsync(Guid userId)
        {
            var userExists = await _context.Users.AsNoTracking()
                .AnyAsync(w => w.Id == userId && w.IsDeleted == false);

            if (!userExists)
                throw new BusinessException("User not found");

            var creditCards = await _context.UserCreditCards.AsNoTracking()
                .Where(w => w.UserId == userId && w.IsDeleted == false)
                .OrderByDescending(o => o.IsPrimary)
                .ThenByDescending(o => o.CreatedAt)
                .ToListAsync();

            return creditCards.Select(s => s.ToUserCreditCardResponse()).ToList();
        }

        public async Task<UserCreditCardResponse> GetByIdMeAsync(Guid id, Guid userId)
        {
            var creditCard = await _context.UserCreditCards.AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId && w.IsDeleted == false);

            if (creditCard == null)
                throw new NotFoundException("Credit card not found");

            return creditCard.ToUserCreditCardResponse();
        }

        public async Task UpdateOrCreateAsync(UserCreditCardRequest request)
        {
            var userExists = await _context.Users.AsNoTracking()
                .AnyAsync(w => w.Id == request.UserId && w.IsDeleted == false);

            if (!userExists)
                throw new BusinessException("User not found");

            var creditCard = request.Id.HasValue
                ? await _context.UserCreditCards
                    .FirstOrDefaultAsync(w =>
                        w.Id == request.Id.Value &&
                        w.UserId == request.UserId &&
                        w.IsDeleted == false)
                : null;

            if (request.Id.HasValue && creditCard == null)
                throw new NotFoundException("Credit card not found");

            if (creditCard == null)
            {
                creditCard = request.ToUserCreditCard();
                _context.UserCreditCards.Add(creditCard);
            }
            else
            {
                creditCard.UpdateUserCreditCard(request);
                _context.UserCreditCards.Update(creditCard);
            }

            if (request.IsPrimary)
                await UnsetPrimaryCreditCardsAsync(request.UserId, creditCard.Id);

            await _context.SaveChangesAsync();
        }

        private async Task UnsetPrimaryCreditCardsAsync(Guid userId, Guid currentCreditCardId)
        {
            var creditCards = await _context.UserCreditCards
                .Where(w =>
                    w.UserId == userId &&
                    w.Id != currentCreditCardId &&
                    w.IsPrimary &&
                    w.IsDeleted == false)
                .ToListAsync();

            foreach (var creditCard in creditCards)
            {
                creditCard.IsPrimary = false;
                creditCard.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
