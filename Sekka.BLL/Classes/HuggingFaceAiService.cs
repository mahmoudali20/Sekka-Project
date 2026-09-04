using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Sekka.BLL.Interfaces;

namespace Sekka.BLL.Classes
{
   
    public class HuggingFaceAiService : IComplaintAiService
    {
        private const string BaseUrl = "https://router.huggingface.co/hf-inference/";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        private string ApiKey => _config["AI:HuggingFace:ApiKey"] ?? "";

        public bool IsConfigured => !string.IsNullOrWhiteSpace(ApiKey);

        public HuggingFaceAiService(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        private HttpClient CreateClient()
        {
            if (!IsConfigured)
                throw new InvalidOperationException(
                    "Hugging Face is not configured. Add AI:HuggingFace:ApiKey using Visual Studio User Secrets (see README-AI.md).");

            var http = _httpClientFactory.CreateClient();
            http.BaseAddress = new Uri(BaseUrl);
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
            return http;
        }

        public async Task<string> SummarizeAsync(string text, CancellationToken ct = default)
        {
            var http = CreateClient();

            var res = await http.PostAsJsonAsync("models/facebook/bart-large-cnn",
                new { inputs = text }, ct);
            res.EnsureSuccessStatusCode();

            var result = await res.Content.ReadFromJsonAsync<List<SummarizationResult>>(cancellationToken: ct);
            return result?.FirstOrDefault()?.SummaryText ?? "";
        }

        public async Task<string> ClassifyAsync(string text, CancellationToken ct = default)
        {
            var http = CreateClient();

            var res = await http.PostAsJsonAsync("models/facebook/bart-large-mnli", new
            {
                inputs = text,
                parameters = new { candidate_labels = ComplaintCategories.Labels }
            }, ct);
            res.EnsureSuccessStatusCode();

            var result = await res.Content.ReadFromJsonAsync<List<ZeroShotLabelScore>>(cancellationToken: ct);
            return result?.FirstOrDefault()?.Label ?? "";
        }

        public async Task<float[]> GetEmbeddingAsync(string text, CancellationToken ct = default)
        {
            var http = CreateClient();

            var res = await http.PostAsJsonAsync("models/sentence-transformers/all-MiniLM-L6-v2/pipeline/feature-extraction", new
            {
                inputs = text,
                options = new { wait_for_model = true }
            }, ct);
            res.EnsureSuccessStatusCode();

            var embedding = await res.Content.ReadFromJsonAsync<float[]>(cancellationToken: ct);
            return embedding ?? Array.Empty<float>();
        }

        private class SummarizationResult
        {
            [JsonPropertyName("summary_text")]
            public string? SummaryText { get; set; }
        }

        private class ZeroShotLabelScore
        {
            [JsonPropertyName("label")]
            public string? Label { get; set; }

            [JsonPropertyName("score")]
            public double Score { get; set; }
        }
    }
}