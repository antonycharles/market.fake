using User.Core.Entities;
using User.Core.Requests;
using User.Core.Responses;

namespace User.Application.Mappers
{
    public static class UserAddressMap
    {
        public static UserAddress ToUserAddress(this UserAddressRequest request)
        {
            return new UserAddress
            {
                UserId = request.UserId,
                Street = request.Street,
                Number = request.Number,
                Complement = request.Complement,
                Neighborhood = request.Neighborhood,
                City = request.City,
                State = request.State,
                ZipCode = request.ZipCode,
                Country = request.Country,
                IsPrimary = request.IsPrimary
            };
        }

        public static void UpdateUserAddress(this UserAddress userAddress, UserAddressRequest request)
        {
            userAddress.Street = request.Street;
            userAddress.Number = request.Number;
            userAddress.Complement = request.Complement;
            userAddress.Neighborhood = request.Neighborhood;
            userAddress.City = request.City;
            userAddress.State = request.State;
            userAddress.ZipCode = request.ZipCode;
            userAddress.Country = request.Country;
            userAddress.IsPrimary = request.IsPrimary;
            userAddress.UpdatedAt = DateTime.UtcNow;
        }

        public static UserAddressResponse ToUserAddressResponse(this UserAddress userAddress) => new()
        {
            Id = userAddress.Id,
            UserId = userAddress.UserId,
            Street = userAddress.Street,
            Number = userAddress.Number,
            Complement = userAddress.Complement,
            Neighborhood = userAddress.Neighborhood,
            City = userAddress.City,
            State = userAddress.State,
            ZipCode = userAddress.ZipCode,
            Country = userAddress.Country,
            IsPrimary = userAddress.IsPrimary
        };
    }
}
