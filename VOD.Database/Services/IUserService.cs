using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VOD.Common.DTOModels;
using VOD.Common.Entities;

namespace VOD.Database.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetUsersAsync();
        Task<UserDTO> GetUserAsync(string userId);
        Task<IdentityResult> AddUserAsync(RegisterUserDTO user);
        Task<bool> UpdateUserAsync(UserDTO user);
        Task<VODUser> GetUserAsync(LoginUserDTO loginUser, bool includeClaims = false);
        Task<bool> DeleteUserAsync(string userId);
    }
}
