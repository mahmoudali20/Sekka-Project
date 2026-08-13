using Microsoft.AspNetCore.Identity;
using Sekka.BLL.ViewModels.AccountVM;
using Sekka.DAL.Models;

namespace Sekka.BLL.Interfaces
{
    public interface IAccountService
    {

        Task<ApplicationUser> CreateUserAsync(string userName, string fullName, string email, string phoneNumber, string address, string password);
        Task AddRoleAsync(ApplicationUser user, string role);

        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<ApplicationUser?> FindByUserNameAsync(string UserName);
        Task<ApplicationUser?> FindByIdAsync(string id);
        Task<ApplicationUser?> FindByEmailAsync(string email);
        Task<bool> IsPhoneNumberExistAsync(string? phoneNumber, string currentUserId);



        Task<IdentityResult> SetEmailAsync(ApplicationUser user, string? email);
        Task<IdentityResult> SetUserNameAsync(ApplicationUser user, string userName);
        Task<IdentityResult> UpdateAsync(ApplicationUser user);

        Task SignInAsync(ApplicationUser user, bool isPersistent = false);
        Task SignOutAsync();

        Task RegisterPassengerAsync(RegisterVM model);

    }
}
