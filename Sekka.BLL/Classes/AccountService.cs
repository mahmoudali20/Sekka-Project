using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sekka.BLL.Common;
using Sekka.BLL.Interfaces;
using Sekka.BLL.ViewModels.AccountVM;
using Sekka.DAL.Models;
using System.Security.Claims;

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


        public async Task<Result> AddRoleAsync(ApplicationUser user, string role)
        {
            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Fail(errors);
            }
            return Result.OK();
        }
        public async Task<Result<ApplicationUser>> CreateUserAsync(string userName, string fullName, string email, string phoneNumber, string address, string password)
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
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));

                return Result<ApplicationUser>.Fail(errors);
            }

            return Result<ApplicationUser>.OK(user);
        }
        public async Task<Result> RegisterPassengerAsync(RegisterVM model)
        {
            var userResult = await CreateUserAsync(model.UserName, model.FullName, model.Email, model.PhoneNumber, model.Address, model.Password);

            if (!userResult.success)
                return Result.Fail(userResult.error!);

            var roleResult = await AddRoleAsync(userResult.Value!, "Passenger");

            if (!roleResult.success)
                return Result.Fail(roleResult.error!);

            return Result.OK();
        }

        public async Task<Result> CheckPasswordAsync(ApplicationUser user, string password)
        {
            var isValid = await _userManager.CheckPasswordAsync(user, password);

            if (!isValid)
                return Result.Fail("Invalid  password.");

            return Result.OK();
        }
        public async Task<Result<ApplicationUser>> FindByUserNameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user is null)
                return Result<ApplicationUser>.NotFound("User not found.");

            return Result<ApplicationUser>.OK(user);

        }
        public async Task<Result<ApplicationUser>> FindByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return Result<ApplicationUser>.NotFound("User Not Found");
            return Result<ApplicationUser>.OK(user);
        }
        public async Task<Result<ApplicationUser>> FindByEmailAsync(string email)
        {

            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Result<ApplicationUser>.NotFound("User not found.");
            return Result<ApplicationUser>.OK(user);
        }

        public async Task<bool> IsPhoneNumberExistAsync(string? phoneNumber, string currentUserId)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return false;
            return await _userManager.Users.AnyAsync(x => x.PhoneNumber == phoneNumber && x.Id != currentUserId);
        }

        public async Task<Result> SetEmailAsync(ApplicationUser user, string? email)
        {
            var result = await _userManager.SetEmailAsync(user, email);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Fail(errors);
            }
            return Result.OK();
        }
        public async Task<Result> SetUserNameAsync(ApplicationUser user, string userName)
        {
            var result = await _userManager.SetUserNameAsync(user, userName);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Fail(errors);
            }
            return Result.OK();
        }
        public async Task<Result> UpdateAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Fail(errors);
            }
            return Result.OK();
        }

        public async Task<Result> SignInAsync(ApplicationUser user, bool isPersistent = false)
        {
            await _signInManager.SignInAsync(user, isPersistent);
            return Result.OK();
        }
        public async Task<Result> SignOutAsync()
        {
            await _signInManager.SignOutAsync();
            return Result.OK();
        }

        public async Task<Result<bool>> IsInRoleAsync(ApplicationUser user, string role)
        {
            var result = await _userManager.IsInRoleAsync(user, role);
            return Result<bool>.OK(result);
        }

        public async Task<Result> DeleteUserAsync(ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));

                return Result.Fail(errors);
            }

            return Result.OK();
        }

        public async Task<Result<IEnumerable<ApplicationUser>>> GetUsersInRoleAsync(string role)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);

            return Result<IEnumerable<ApplicationUser>>.OK(users);
        }

        public Task<AuthenticationProperties> ConfigureExternalLoginAsync(string provider, string redirectUrl) => Task.FromResult(_signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl));
        public async Task<Result> ExternalLoginAsync()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info is null)
                return Result.Fail("External login information not found.");

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            if (result.Succeeded)
                return Result.OK();

            var email = info.Principal.FindFirstValue(System.Security.Claims.ClaimTypes.Email);

            var name = info.Principal.FindFirstValue(System.Security.Claims.ClaimTypes.Name);

            if (string.IsNullOrEmpty(email))
                return Result.Validation("Email was not provided by the external provider.");

            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                var userName = email.Split('@')[0];

                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = email,
                    FullName = name ?? userName,
                    Address = "Not Provided",
                    PhoneNumber = "Not Provided",
                    EmailConfirmed = true
                };


                var createResult = await _userManager.CreateAsync(user);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));

                    return Result.Fail(errors);
                }

                var roleResult = await AddRoleAsync(user, "Passenger");
                if (!roleResult.success)
                    return Result.Fail(roleResult.error!);

            }

            var addLoginResult = await _userManager.AddLoginAsync(user, info);

            if (!addLoginResult.Succeeded)
            {
                var errors = string.Join(", ", addLoginResult.Errors.Select(e => e.Description));

                return Result.Fail(errors);
            }

            var signInResult = await SignInAsync(user, isPersistent: false);

            if (!signInResult.success)
                return signInResult;

            return Result.OK();
        }


    }
}
