namespace Sekka.BLL.ViewModels.DriverVM
{
    public class DriverVM
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string ProfilePicture { get; set; }
		public bool IsAvailable { get; set; }
		public decimal RatingAverage { get; set; }
	}
}
