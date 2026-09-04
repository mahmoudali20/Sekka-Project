using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Sekka.BLL.Interfaces;

namespace Sekka.BLL.Classes
{
    
    public class KashierService : IKashierService
    {
        private readonly IConfiguration _config;

        private string MerchantId => _config["Payment:Kashier:MerchantId"] ?? "";
        private string ApiKey => _config["Payment:Kashier:ApiKey"] ?? "";
        private string Mode => _config["Payment:Kashier:Mode"] ?? "test";

        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(MerchantId) &&
            !string.IsNullOrWhiteSpace(ApiKey);

        public KashierService(IConfiguration config)
        {
            _config = config;
        }

        public KashierInitResult CreateOrder(int rideId, decimal amountEgp, string requestBaseUrl)
        {
            if (!IsConfigured)
            {
                return new KashierInitResult
                {
                    Success = false,
                    Error = "Kashier is not configured. Add Payment:Kashier:MerchantId and Payment:Kashier:ApiKey using Visual Studio User Secrets."
                };
            }

            if (amountEgp <= 0)
            {
                return new KashierInitResult
                {
                    Success = false,
                    Error = "The Kashier payment amount must be greater than zero."
                };
            }

            var merchantOrderId = $"ride{rideId}-{DateTime.UtcNow.Ticks}";
            var amount = amountEgp.ToString("F2", CultureInfo.InvariantCulture);
            const string currency = "EGP";

            var hash = ComputeOrderHash(merchantOrderId, amount, currency);
            var returnUrl = $"{requestBaseUrl.TrimEnd('/')}/Payment/KashierReturn";

            
            var approveUrl =
                "https://checkout.kashier.io" +
                $"?merchantId={Uri.EscapeDataString(MerchantId)}" +
                $"&orderId={Uri.EscapeDataString(merchantOrderId)}" +
                $"&mode={Uri.EscapeDataString(Mode)}" +
                $"&amount={Uri.EscapeDataString(amount)}" +
                $"&currency={Uri.EscapeDataString(currency)}" +
                $"&hash={Uri.EscapeDataString(hash)}" +
                $"&merchantRedirect={Uri.EscapeDataString(returnUrl)}" +
                "&allowedMethods=card" +
                "&display=en";

            return new KashierInitResult
            {
                Success = true,
                OrderId = merchantOrderId,
                ApproveUrl = approveUrl
            };
        }

        
        public bool VerifyCallbackSignature(IEnumerable<KeyValuePair<string, string?>> parameters, string? signature)
        {
            if (!IsConfigured || string.IsNullOrWhiteSpace(signature))
                return false;

            var canonical = new StringBuilder();

            foreach (var parameter in parameters)
            {
                if (parameter.Key.Equals("signature", StringComparison.OrdinalIgnoreCase) ||
                    parameter.Key.Equals("mode", StringComparison.OrdinalIgnoreCase))
                    continue;

                canonical.Append('&')
                         .Append(parameter.Key)
                         .Append('=')
                         .Append(parameter.Value ?? string.Empty);
            }

            if (canonical.Length > 0)
                canonical.Remove(0, 1);

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(ApiKey));
            var expected = Convert.ToHexString(
                hmac.ComputeHash(Encoding.UTF8.GetBytes(canonical.ToString())))
                .ToLowerInvariant();

            var supplied = signature.Trim();
            try
            {
                return CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(expected),
                    Encoding.UTF8.GetBytes(supplied.ToLowerInvariant()));
            }
            catch
            {
                return false;
            }
        }

        // Kashier HPP hash: HMAC-SHA256("/?payment=mid.orderId.amount.currency", apiKey).
        private string ComputeOrderHash(string merchantOrderId, string amount, string currency)
        {
            var path = $"/?payment={MerchantId}.{merchantOrderId}.{amount}.{currency}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(ApiKey));
            return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(path)))
                .ToLowerInvariant();
        }
    }
}
