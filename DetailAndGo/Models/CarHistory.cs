using NuGet.Protocol.Plugins;

namespace DetailAndGo.Models
{
    public class CarHistory
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public DateTime BookingDate { get; set; }
        public string Service { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}
