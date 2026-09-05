using Microsoft.AspNetCore.Mvc;
using Sekka.BLL.Interfaces;
using Sekka.BLL.ViewModels.AccountVM;
using System.Security.Claims;

namespace Sekka.PL.Controllers
{
    public class AccountController : Controller
    {

        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var result = await _accountService.RegisterPassengerAsync(model);

            if (!result.success)
            {
                ModelState.AddModelError(string.Empty, result.error ?? "Registration failed.");
                return View(model);
            }
            return RedirectToAction("Login", "Account");
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userResult = await _accountService.FindByUserNameAsync(model.UserName);

            if (!userResult.success)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            var user = userResult.Value;
            var validPassword = await _accountService.CheckPasswordAsync(user, model.Password);

            if (!validPassword.success)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");

                return View(model);
            }

            var signInResult = await _accountService.SignInAsync(user, model.RememberMe);

            if (!signInResult.success)
            {
                ModelState.AddModelError(string.Empty, signInResult.error!);
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                var result = await _accountService.SignOutAsync(userId);

                if (!result.success)
                    TempData["Error"] = result.error;
            }

            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GoogleLogin()
        {
            var redirectUrl = Url.Action(nameof(GoogleLoginCallback), "Account");
            var properties = await _accountService.ConfigureExternalLoginAsync("Google", redirectUrl!);
            return Challenge(properties, "Google");
        }

        [HttpGet]
        public async Task<IActionResult> GoogleLoginCallback()
        {
            var result = await _accountService.ExternalLoginAsync();
            if (!result.success)
            {
                TempData["Error"] = result.error;
                return RedirectToAction(nameof(Login));
            }
            return RedirectToAction("Index", "Home");
        }

    }
}
