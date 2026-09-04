namespace Sekka.BLL.Interfaces
{
    public class KashierInitResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public string? OrderId { get; set; }
        public string? ApproveUrl { get; set; }
    }

    /// <summary>
    /// Kashier Hosted Payment Page integration.
    /// Configure Payment:Kashier:MerchantId, Payment:Kashier:ApiKey and Payment:Kashier:Mode.
    /// </summary>
    public interface IKashierService
    {
        bool IsConfigured { get; }

        KashierInitResult CreateOrder(int rideId, decimal amountEgp, string requestBaseUrl);

        bool VerifyCallbackSignature(
            IEnumerable<KeyValuePair<string, string?>> parameters,
            string? signature);
    }
}
