using Microsoft.AspNetCore.Authentication;
using Sekka.BLL.Common;
using Sekka.BLL.ViewModels.AccountVM;
using Sekka.DAL.Models;

namespace Sekka.BLL.Interfaces
{
    public interface IAccountService
    {

        //Task<ApplicationUser> CreateUserAsync(string userName, string fullName, string email, string phoneNumber, string address, string password);
        //Task AddRoleAsync(ApplicationUser user, string role);

        //Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        //Task<ApplicationUser?> FindByUserNameAsync(string UserName);
        //Task<ApplicationUser?> FindByIdAsync(string id);
        //Task<ApplicationUser?> FindByEmailAsync(string email);
        //Task<bool> IsPhoneNumberExistAsync(string? phoneNumber, string currentUserId);



        //Task<IdentityResult> SetEmailAsync(ApplicationUser user, string? email);
        //Task<IdentityResult> SetUserNameAsync(ApplicationUser user, string userName);
        //Task<IdentityResult> UpdateAsync(ApplicationUser user);

        //Task SignInAsync(ApplicationUser user, bool isPersistent = false);
        //Task SignOutAsync();



        //Task<AuthenticationProperties> ConfigureExternalLoginAsync(string provider, string redirectUrl);
        //Task<bool> ExternalLoginAsync();

        //Task RegisterPassengerAsync(RegisterVM model);

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
        Task<Result> SignOutAsync();

        Task<Result<bool>> IsInRoleAsync(ApplicationUser user, string role);
        Task<Result> DeleteUserAsync(ApplicationUser user);
        Task<Result<IEnumerable<ApplicationUser>>> GetUsersInRoleAsync(string role);

        Task<AuthenticationProperties> ConfigureExternalLoginAsync(string provider, string redirectUrl);
        Task<Result> ExternalLoginAsync();


    }
}
