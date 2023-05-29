using DetailAndGo.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DetailAndGo.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string AspNetUserId { get; set; }
        public int CarId { get; set; }
        public List<Service> Services { get; set; }
        public string ServicesArray { get; set; }
        public string SubServicesArray { get; set; }
        public DateTime BookedFor { get; set; }
        public DateTime Created { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime StatusChanged { get; set; }
        public string Notes { get; set; }
        public string Image { get; set; }
        public string PaymentMethodId { get; set; }
        public long TotalAmount { get; set; }
    }
}
