using User.Core.Entities;
using User.Core.Requests;
using User.Core.Responses;

namespace User.Application.Mappers
{
    public static class UserCreditCardMap
    {
        public static UserCreditCard ToUserCreditCard(this UserCreditCardRequest request)
        {
            return new UserCreditCard
            {
                UserId = request.UserId,
                HolderName = request.HolderName,
                Brand = request.Brand,
                LastFourDigits = request.LastFourDigits,
                ExpirationMonth = request.ExpirationMonth,
                ExpirationYear = request.ExpirationYear,
                IsPrimary = request.IsPrimary
            };
        }

        public static void UpdateUserCreditCard(this UserCreditCard userCreditCard, UserCreditCardRequest request)
        {
            userCreditCard.HolderName = request.HolderName;
            userCreditCard.Brand = request.Brand;
            userCreditCard.LastFourDigits = request.LastFourDigits;
            userCreditCard.ExpirationMonth = request.ExpirationMonth;
            userCreditCard.ExpirationYear = request.ExpirationYear;
            userCreditCard.IsPrimary = request.IsPrimary;
            userCreditCard.UpdatedAt = DateTime.UtcNow;
        }

        public static UserCreditCardResponse ToUserCreditCardResponse(this UserCreditCard userCreditCard) => new()
        {
            Id = userCreditCard.Id,
            UserId = userCreditCard.UserId,
            HolderName = userCreditCard.HolderName,
            Brand = userCreditCard.Brand,
            LastFourDigits = userCreditCard.LastFourDigits,
            ExpirationMonth = userCreditCard.ExpirationMonth,
            ExpirationYear = userCreditCard.ExpirationYear,
            IsPrimary = userCreditCard.IsPrimary
        };
    }
}
