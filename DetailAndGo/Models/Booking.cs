using DetailAndGo.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace DetailAndGo.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string AspNetUserId { get; set; }
        public int CarId { get; set; }
        public List<Service> Services { get; set; }
        public DateTime BookedFor { get; set; }
        public DateTime Created { get; set; }
        public BookingStatus Status { get; set; }
        public string Notes { get; set; }        
        public string Image { get; set; }
    }
}
