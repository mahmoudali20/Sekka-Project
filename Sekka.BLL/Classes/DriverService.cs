using AutoMapper;
using Sekka.BLL.Common;
using Sekka.BLL.Interfaces;
using Sekka.BLL.ViewModels.DriverVM;
using Sekka.DAL.Models;
using Sekka.DAL.Repositories.Interfaces;

namespace Sekka.BLL.Classes
{
    public class DriverService : IDriverService
    {
        private readonly IAccountService _accountService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAttachmentService _attachmentService;
        private readonly IMapper _mapper;

        public DriverService(IAccountService accountService, IUnitOfWork unitOfWork, IAttachmentService attachmentService, IMapper mapper)
        {
            _accountService = accountService;
            _unitOfWork = unitOfWork;
            _attachmentService = attachmentService;
            _mapper = mapper;

        }

        public async Task<Result> CreateDriverAsync(CreateDriverVM model)
        {

            var UsernameExist = await _unitOfWork.GetRepo<ApplicationUser, string>().AnyAsync(x => x.UserName == model.UserName);
            var PhoneExist = await _unitOfWork.GetRepo<ApplicationUser, string>().AnyAsync(x => x.PhoneNumber == model.PhoneNumber);
            var EmailExist = await _unitOfWork.GetRepo<ApplicationUser, string>().AnyAsync(x => x.Email == model.Email);

            if (UsernameExist)
                return Result.Validation("username already exists.");
            if (PhoneExist)
                return Result.Validation("Phone already exists.");
            if (EmailExist)
                return Result.Validation("Email already exists.");

            // 1. Upload Car Photo
            var storedCarPhotoName = await _attachmentService.UploadAttachmentAsync(model.Car.ProfilePicture.OpenReadStream(), model.Car.ProfilePicture.FileName, "CarsPhoto");
            if (string.IsNullOrWhiteSpace(storedCarPhotoName))
                return Result.Fail("Failed to upload car photo.");

            // 2. Upload Driver Photo
            var storedDriverPhotoName = await _attachmentService.UploadAttachmentAsync(model.ProfilePicture.OpenReadStream(), model.ProfilePicture.FileName, "DriversPhoto");
            if (string.IsNullOrWhiteSpace(storedDriverPhotoName))
            {
                _attachmentService.DeleteAttachment(storedCarPhotoName, "CarsPhoto");
                return Result.Fail("Failed to upload driver photo.");
            }

            // 3. Create User
            var user = await _accountService.CreateUserAsync(model.UserName, model.FullName, model.Email, model.PhoneNumber, model.Address, model.Password);
            if (user == null)
            {
                _attachmentService.DeleteAttachment(storedCarPhotoName, "CarsPhoto");
                _attachmentService.DeleteAttachment(storedDriverPhotoName, "DriversPhoto");
                return Result.Fail("Failed to create user.");
            }

            // 4. Save Driver Photo Name
            user.ProfilePicture = storedDriverPhotoName;

            // 5. Map Car
            var car = _mapper.Map<Car>(model.Car);
            car.Image = storedCarPhotoName;

            // 6. Map Driver
            var driver = _mapper.Map<Driver>(model);
            driver.UserId = user.Id;
            driver.IsAvailable = false;
            driver.RatingAverage = 0;
            driver.Car = car;

            _unitOfWork.GetRepo<Driver, int>().AddAsync(driver);

            await _unitOfWork.SaveChangesAsync();

            await _accountService.AddRoleAsync(user, "Driver");

            return Result.OK();
        }

        public async Task<Result> DeleteDriverAsync(int id, CancellationToken ct = default)
        {
            var driver = await _unitOfWork.GetRepo<Driver, int>().GetByIdAsync(id, ct);

            if (driver is null)
                return Result.Fail("Driver not found.");

            var car = await _unitOfWork.GetRepo<Car, int>().GetByIdAsync(driver.CarId, ct);

            var user = await _unitOfWork.GetRepo<ApplicationUser, string>().GetByIdAsync(driver.UserId, ct);

            // Delete Driver first
            _unitOfWork.GetRepo<Driver, int>().Delete(driver);

            // Delete Car
            if (car is not null)
                _unitOfWork.GetRepo<Car, int>().Delete(car);


            // Delete User
            if (user is not null)
                _unitOfWork.GetRepo<ApplicationUser, string>().Delete(user);


            var result = await _unitOfWork.SaveChangesAsync();

            return result > 0 ? Result.OK() : Result.Fail("Failed to delete driver.");
        }

        public async Task<Result<IEnumerable<DriverVM>>> GetAllDriverAsync()
        {
            var drivers = await _unitOfWork.DriverRepository.GetAllWithUserAsync();
            var result = _mapper.Map<IEnumerable<DriverVM>>(drivers);
            return Result<IEnumerable<DriverVM>>.OK(result);
        }

        public async Task<Result<DriverDetailsVM>> GetDriverDetailsByIdAsync(int id, CancellationToken ct = default)
        {
            var driver = await _unitOfWork.DriverRepository.GetDriverDetailsByIdAsync(id, ct);

            if (driver is null)
                return Result<DriverDetailsVM>.Fail("Driver not found.");

            var result = _mapper.Map<DriverDetailsVM>(driver);

            return Result<DriverDetailsVM>.OK(result);
        }


        public async Task<Result> UpdateDriverAsync(int id, DriverEditVM model, CancellationToken ct = default)
        {
            var driver = await _unitOfWork.GetRepo<Driver, int>().GetByIdAsync(id);
            if (driver is null)
                return Result.Fail("Driver not found.");

            ;

            var user = await _accountService.FindByIdAsync(driver.UserId);
            if (user is null)
                return Result.Fail("User not found.");

            var userWithSameEmail = await _accountService.FindByEmailAsync(model.Email);
            if (userWithSameEmail != null && userWithSameEmail.Id != user.Id)
                return Result.Validation("Email already exists.");

            var userWithSameUserName = await _accountService.FindByUserNameAsync(model.UserName);
            if (userWithSameUserName != null && userWithSameUserName.Id != user.Id)
                return Result.Validation("UserName already exists.");

            var phoneExist = await _unitOfWork.GetRepo<ApplicationUser, string>().AnyAsync(x => x.PhoneNumber == model.PhoneNumber && x.Id != user.Id, ct);
            if (phoneExist) return Result.Validation("Phone already exists.");


            // Update Driver
            _mapper.Map(model, driver);
            user.Address = model.Address;
            user.PhoneNumber = model.PhoneNumber;


            await _accountService.SetEmailAsync(user, model.Email);
            await _accountService.SetUserNameAsync(user, model.UserName);

            var userUpdateResult = await _accountService.UpdateAsync(user);
            if (!userUpdateResult.Succeeded)
                return Result.Fail("Failed to update User data.");


            var result = await _unitOfWork.SaveChangesAsync();

            return result > 0 || userUpdateResult.Succeeded ? Result.OK() : Result.Fail("Failed to update Driver.");
        }

        public async Task<Result<DriverEditVM?>> GetForUpdateAsync(int id, CancellationToken ct)
        {
            var driver = await _unitOfWork.GetRepo<Driver, int>().FirstOrDefaultAsync(x => x.Id == id, tracking: false, ct);
            if (driver is null)
                return Result<DriverEditVM>.Fail("Driver not found.");


            var user = await _unitOfWork.GetRepo<ApplicationUser, string>().FirstOrDefaultAsync(x => x.Id == driver.UserId, tracking: false, ct);
            if (user is null)
                return Result<DriverEditVM>.Fail("User not found.");


            var model = _mapper.Map<DriverEditVM>(driver);
            _mapper.Map(user, model);

            return Result<DriverEditVM>.OK(model);
        }


    }
}
