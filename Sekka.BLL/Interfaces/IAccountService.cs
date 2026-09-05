using Microsoft.AspNetCore.Authentication;
using Sekka.BLL.Common;
using Sekka.BLL.ViewModels.AccountVM;
using Sekka.DAL.Models;

namespace Sekka.BLL.Interfaces
{
    public interface IAccountService
    {



        Task<Result<ApplicationUser>> CreateUserAsync(string userName, string fullName, string email, string phoneNumber, string address, string password);
        Task<Result> AddRoleAsync(ApplicationUser user, string role);
        Task<Result> RegisterPassengerAsync(RegisterVM model);


        Task<Result> CheckPasswordAsync(ApplicationUser user, string password);
        Task<Result<ApplicationUser>> FindByUserNameAsync(string UserName);
        Task<Result<ApplicationUser>> FindByIdAsync(string id);
        Task<Result<ApplicationUser>> FindByEmailAsync(string email);


        Task<bool> IsPhoneNumberExistAsync(string? phoneNumber, string currentUserId);


        Task<Result> SetEmailAsync(ApplicationUser user, string? email);
        Task<Result> SetUserNameAsync(ApplicationUser user, string userName);
        Task<Result> UpdateAsync(ApplicationUser user);

        Task<Result> SignInAsync(ApplicationUser user, bool isPersistent = false);
        Task<Result> SignOutAsync(string userId);

        Task<Result<bool>> IsInRoleAsync(ApplicationUser user, string role);
        Task<Result> DeleteUserAsync(ApplicationUser user);
        Task<Result<IEnumerable<ApplicationUser>>> GetUsersInRoleAsync(string role);

        Task<AuthenticationProperties> ConfigureExternalLoginAsync(string provider, string redirectUrl);
        Task<Result> ExternalLoginAsync();


    }
}
