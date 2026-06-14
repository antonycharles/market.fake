using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounts.Core.Entities;
using Accounts.Core.Enums;
using Accounts.Core.Requests;
using Accounts.Core.Responses;

namespace Accounts.Application.Mappers
{
    public static class UserMap
    {
        public static User ToUser(this UserRequest request) => new User{
            Name = request.Name,
            Email = request.Email,
            Status = request.Status != null ? request.Status.Value : StatusEnum.Active
        };


        public static UserResponse ToUserResponse(this User user, string? fileUrl = "") => new UserResponse{
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Status = user.Status,
            Profiles = user.UserProfiles?.Select(s => s.ToUserProfileResponse()).ToList(),
        };

        public static void Update(this User user, UserUpdateRequest request){
            user.Name = request.Name;
            user.Email = request.Email;
        }
    }
}