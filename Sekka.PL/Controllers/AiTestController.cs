
using Microsoft.AspNetCore.Mvc;
using Sekka.BLL.Interfaces;

namespace Sekka.PL.Controllers
{
    public class AiTestController : Controller
    {
        private readonly IComplaintAiService _aiService;

        public AiTestController(IComplaintAiService aiService)
        {
            _aiService = aiService;
        }

        public IActionResult Index()
        {
            AiTestViewModel model = new AiTestViewModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Summarize(
            AiTestViewModel model,
            CancellationToken ct)
        {
            // Check if Hugging Face is configured
            if (!_aiService.IsConfigured)
            {
                model.Error =
                    "Hugging Face is not configured. Add AI:HuggingFace:ApiKey in User Secrets.";

                return View("Index", model);
            }

            try
            {
                string text = model.InputText ?? "";

                string result =
                    await _aiService.SummarizeAsync(text, ct);

                model.Result = result;
            }
            catch (Exception ex)
            {
                model.Error = ex.Message;
            }

            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> Classify(
            AiTestViewModel model,
            CancellationToken ct)
        {
            // Check if Hugging Face is configured
            if (!_aiService.IsConfigured)
            {
                model.Error =
                    "Hugging Face is not configured. Add AI:HuggingFace:ApiKey in User Secrets.";

                return View("Index", model);
            }

            try
            {
                string text = model.InputText ?? "";

                string result =
                    await _aiService.ClassifyAsync(text, ct);

                model.Result = result;
            }
            catch (Exception ex)
            {
                model.Error = ex.Message;
            }

            return View("Index", model);
        }
    }

    public class AiTestViewModel
    {
        public string? InputText { get; set; }

        public string? Result { get; set; }

        public string? Error { get; set; }
    }
}


