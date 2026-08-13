namespace Sekka.DAL.Models
{
    public class Car
    {

        public int Id { get; set; }
        public string Model { get; set; } = default!;
        public string Color { get; set; } = default!;
        public string PlateNumber { get; set; } = default!;
        public string LicensePlate { get; set; } = default!;
        public string Image { get; set; } = default!;


        public Driver Driver { get; set; } = default!;



    }
}
