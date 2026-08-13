using Microsoft.AspNetCore.Identity;
using Sekka.BLL.Common;
using Sekka.BLL.Interfaces;
using Sekka.BLL.ViewModels.AccountVM;
using Sekka.DAL.Models;

namespace Sekka.BLL.Classes
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountService _accountService;

        public AdminService(UserManager<ApplicationUser> userManager, IAccountService account)
        {
            _userManager = userManager;
            _accountService = account;
        }


        public async Task<Result> RegisterAdminAsync(CreateAdminVM model)
        {
            var emailExist = await _accountService.FindByEmailAsync(model.Email);
            if (emailExist is not null)
                return Result.Validation("Email already exists.");

            var usernameExist = await _accountService.FindByUserNameAsync(model.UserName);
            if (usernameExist is not null)
                return Result.Validation("Username already exists.");

            var phoneExist = await _accountService.IsPhoneNumberExistAsync(model.PhoneNumber, string.Empty);
            if (phoneExist)
                return Result.Validation("Phone number already exists.");

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,

            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Fail(errors);
            }


            await _accountService.AddRoleAsync(user, "Admin");

            return Result.OK();
        }


        public async Task<IEnumerable<AdminVM>> GetAdminsAsync()
        {

            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");


            return adminUsers.Select(user => new AdminVM
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Address = user.Address
            });
        }


        public async Task<Result> DeleteAdminAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return Result.Fail("Admin not found.");

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (!isAdmin)
                return Result.Fail("This user is not an Admin.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Fail(errors);
            }

            return Result.OK();
        }
    }
}
