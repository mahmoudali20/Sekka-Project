using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.BLL.Interfaces;
using Sekka.BLL.ViewModels.DriverVM;

namespace Sekka.PL.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DriverController : Controller
    {

        private readonly IDriverService _driverService;
        private readonly IAttachmentService _attachmentService;
        public DriverController(IDriverService driverService, IAttachmentService attachmentService)
        {
            _driverService = driverService;
            _attachmentService = attachmentService;
        }



        [HttpGet]
        public async Task<IActionResult> Picture(int id, CancellationToken ct = default)
        {
            var driver = await _driverService.GetDriverDetailsByIdAsync(id, ct);
            if (driver is null || string.IsNullOrWhiteSpace(driver.Value.ProfilePicture))
                return NotFound();
            var result = _attachmentService.GetAttachment(driver.Value.ProfilePicture, "DriversPhoto");
            if (result is null)
                return NotFound();
            return File(result.Value.stream, result.Value.ContentType);
        }
        [HttpGet]
        public async Task<IActionResult> CarPicture(int id, CancellationToken ct = default)
        {
            var driver = await _driverService.GetDriverDetailsByIdAsync(id, ct);
            if (driver is null || string.IsNullOrWhiteSpace(driver.Value.CarImage))
                return NotFound();
            var result = _attachmentService.GetAttachment(driver.Value.CarImage, "CarsPhoto");
            if (result is null)
                return NotFound();
            return File(result.Value.stream, result.Value.ContentType);
        }

        public async Task<IActionResult> Index()
        {
            var result = await _driverService.GetAllDriverAsync();
            return View(result.Value);
        }

        [HttpGet]

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDriverVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _driverService.CreateDriverAsync(model);
            if (!result.success)
                TempData["ErrorMessage"] = result.error;
            else
                TempData["SuccessMessage"] = "Driver Created Succesfully";

            return RedirectToAction("index");

        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct = default)
        {
            var driver = await _driverService.GetDriverDetailsByIdAsync(id, ct);

            if (!driver.success)
                TempData["ErrorMessage"] = driver.error;

            return View(driver.Value);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var driver = await _driverService.GetForUpdateAsync(id);
            if (!driver.success)
                return NotFound();

            return View(driver.Value);

        }


        [HttpPost]
        public async Task<IActionResult> Edit(int id, DriverEditVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _driverService.UpdateDriverAsync(id, model);
            if (!result.success)
                TempData["ErrorMessage"] = result.error;
            else
                TempData["SuccessMessage"] = "Driver update Succesfully";

            return RedirectToAction("index");

        }


        [HttpGet]
        public IActionResult Delete()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {

            var result = await _driverService.DeleteDriverAsync(id);

            if (!result.success)
                TempData["ErrorMessage"] = result.error;
            else
                TempData["SuccessMessage"] = "Driver Deleted Successfully";

            return RedirectToAction("index");
        }
    }
}
