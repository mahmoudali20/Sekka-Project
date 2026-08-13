using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sekka.BLL.Interfaces;
using Sekka.BLL.ViewModels.AccountVM;
using Sekka.DAL.Models;

namespace Sekka.BLL.Classes
{
    public class AccountService : IAccountService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public async Task AddRoleAsync(ApplicationUser user, string role)
        {
            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password) => await _userManager.CheckPasswordAsync(user, password);

        public async Task<ApplicationUser> CreateUserAsync(string userName, string fullName, string email, string phoneNumber, string address, string password)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                FullName = fullName,
                Email = email,
                PhoneNumber = phoneNumber,
                Address = address
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                throw new Exception(errors);
            }

            return user;
        }

        public async Task<ApplicationUser?> FindByUserNameAsync(string username) => await _userManager.FindByNameAsync(username);


        public async Task SignInAsync(ApplicationUser user, bool isPersistent = false) => await _signInManager.SignInAsync(user, isPersistent);


        public async Task SignOutAsync() => await _signInManager.SignOutAsync();


        public async Task RegisterPassengerAsync(RegisterVM model)
        {
            var user = await CreateUserAsync(model.UserName, model.FullName, model.Email, model.PhoneNumber, model.Address, model.Password);

            await AddRoleAsync(user, "Passenger");
        }

        public async Task<ApplicationUser?> FindByIdAsync(string id) => await _userManager.FindByIdAsync(id);


        public async Task<ApplicationUser?> FindByEmailAsync(string email) => await _userManager.FindByEmailAsync(email);


        public async Task<IdentityResult> SetEmailAsync(ApplicationUser user, string? email) => await _userManager.SetEmailAsync(user, email);


        public async Task<IdentityResult> SetUserNameAsync(ApplicationUser user, string userName) => await _userManager.SetUserNameAsync(user, userName);

        public async Task<bool> IsPhoneNumberExistAsync(string? phoneNumber, string currentUserId)
        {
            if (string.IsNullOrEmpty(phoneNumber)) return false;

            return await _userManager.Users
                .AnyAsync(x => x.PhoneNumber == phoneNumber && x.Id != currentUserId);
        }


        public async Task<IdentityResult> UpdateAsync(ApplicationUser user) => await _userManager.UpdateAsync(user);




    }
}
