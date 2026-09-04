namespace Sekka.BLL.Interfaces
{
    /// <summary>
    /// AI features for complaints (and Lost &amp; Found matching), backed by Hugging Face's
    /// free hosted Inference API — no local Python service required.
    /// Configure AI:HuggingFace:ApiKey using Visual Studio User Secrets (see README-AI.md).
    /// </summary>
    public interface IComplaintAiService
    {
        bool IsConfigured { get; }

        /// <summary>Summarizes complaint text. Model: facebook/bart-large-cnn.</summary>
        Task<string> SummarizeAsync(string text, CancellationToken ct = default);

        /// <summary>
        /// Classifies complaint text into one of the fixed complaint categories using
        /// zero-shot classification. Model: facebook/bart-large-mnli.
        /// Returns the top-scoring label (see <see cref="ComplaintCategories"/>).
        /// </summary>
        Task<string> ClassifyAsync(string text, CancellationToken ct = default);

        /// <summary>
        /// Gets a sentence embedding for Lost &amp; Found matching.
        /// Model: sentence-transformers/all-MiniLM-L6-v2.
        /// Compare two embeddings with <see cref="ComplaintCategories.CosineSimilarity"/>.
        /// </summary>
        Task<float[]> GetEmbeddingAsync(string text, CancellationToken ct = default);
    }

    /// <summary>
    /// Fixed zero-shot classification labels for complaints, and the cosine similarity
    /// helper used to compare Lost &amp; Found embeddings (no extra library needed).
    /// </summary>
    public static class ComplaintCategories
    {
        public static readonly string[] Labels =
        {
            "late driver",
            "payment",
            "unsafe driving",
            "harassment",
            "vehicle"
        };

        public static double CosineSimilarity(float[] a, float[] b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vectors must be the same length.");

            double dot = 0, magA = 0, magB = 0;
            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                magA += a[i] * a[i];
                magB += b[i] * b[i];
            }

            if (magA == 0 || magB == 0) return 0;
            return dot / (Math.Sqrt(magA) * Math.Sqrt(magB));
        }
    }
}
