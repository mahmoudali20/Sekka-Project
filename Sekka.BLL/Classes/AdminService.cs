using Sekka.BLL.Common;
using Sekka.BLL.Interfaces;
using Sekka.BLL.ViewModels.AccountVM;

namespace Sekka.BLL.Classes
{
    public class AdminService : IAdminService
    {

        private readonly IAccountService _accountService;
        public AdminService(IAccountService account)
        {

            _accountService = account;
        }


        public async Task<Result> RegisterAdminAsync(CreateAdminVM model)
        {
            var emailExist = await _accountService.FindByEmailAsync(model.Email);
            if (emailExist.success)
                return Result.Validation("Email already exists.");

            var usernameExist = await _accountService.FindByUserNameAsync(model.UserName);
            if (usernameExist.success)
                return Result.Validation("Username already exists.");

            var phoneExist = await _accountService.IsPhoneNumberExistAsync(model.PhoneNumber, string.Empty);
            if (phoneExist)
                return Result.Validation("Phone number already exists.");

            var userResult = await _accountService.CreateUserAsync(model.UserName, model.FullName, model.Email, model.PhoneNumber, model.Address, model.Password);
            if (!userResult.success)
                return Result.Fail("Failed to create Admin");

            var user = userResult.Value;


            await _accountService.AddRoleAsync(user, "Admin");

            return Result.OK();
        }


        public async Task<Result<IEnumerable<AdminVM>>> GetAdminsAsync()
        {
            var result = await _accountService.GetUsersInRoleAsync("Admin");

            if (!result.success)
                return Result<IEnumerable<AdminVM>>.Fail("Failed to get admin");


            var admins = result.Value.Select(user => new AdminVM
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Address = user.Address
            });

            return Result<IEnumerable<AdminVM>>.OK(admins);
        }


        public async Task<Result> DeleteAdminAsync(string id)
        {
            var userResult = await _accountService.FindByIdAsync(id);
            if (!userResult.success || userResult.Value is null)
                return Result.Fail("Admin not found.");
            var user = userResult.Value;

            var roleResult = await _accountService.IsInRoleAsync(user, "Admin");
            if (!roleResult.success || !roleResult.Value)
                return Result.Fail("This user is not an Admin.");

            var deleteResult = await _accountService.DeleteUserAsync(user);

            if (!deleteResult.success)
                return Result.Fail("Faided to delete this admon");

            return Result.OK();

        }
    }
}
