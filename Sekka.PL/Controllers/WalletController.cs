
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.BLL.Interfaces;
using Sekka.BLL.ViewModels.WalletVM;
using System.Security.Claims;

namespace Sekka.PL.Controllers
{
    [Authorize]
    public class WalletController : Controller
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet]
        [ResponseCache(
            NoStore = true,
            Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            var balance =
                await _walletService.GetBalanceAsync(userId!);

            var transactions =
                await _walletService.GetTransactionsAsync(userId!);

            ViewBag.Balance = balance;

            return View(transactions);
        }

        [HttpGet]
        public IActionResult Deposit()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deposit(DepositVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            var result =
                await _walletService.DepositAsync(
                    userId!,
                    model);

            if (result.success)
            {
                TempData["SuccessMessage"] =
                    $"Deposited {model.Amount:C} into your wallet.";
            }
            else
            {
                TempData["ErrorMessage"] =
                    result.error;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}


