using System.ComponentModel.DataAnnotations;

namespace Sekka.BLL.ViewModels.TripVM
{
    public class RateDriverVM
    {
        [Required]
        public int RideId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Score { get; set; }

        public string? Comment { get; set; }
    }
}
