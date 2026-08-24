using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.BLL.Interfaces;
using Sekka.BLL.ViewModels.AccountVM;
using System.Security.Claims;

namespace Sekka.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var admins = await _adminService.GetAdminsAsync();
            if (!admins.success)
            {
                TempData["ErrorMessage"] = admins.error;
                return View(Enumerable.Empty<AdminVM>());
            }
            return View(admins.Value);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAdminVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _adminService.RegisterAdminAsync(model);
            if (!result.success)
            {
                ModelState.AddModelError(string.Empty, result.error);
                return View(model);
            }

            TempData["SuccessMessage"] = "Admin created successfully";
            return RedirectToAction("Index");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == id)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account!";
                return RedirectToAction(nameof(Index));
            }

            var result = await _adminService.DeleteAdminAsync(id);
            if (!result.success)
            {
                TempData["ErrorMessage"] = result.error;
            }
            else
            {
                TempData["SuccessMessage"] = "Admin deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
