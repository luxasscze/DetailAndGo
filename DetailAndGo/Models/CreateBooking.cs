namespace DetailAndGo.Models
{
    public class CreateBooking
    {
        public string AspNetUserId { get; set; }
        public int CarId { get; set; }
        public List<Service> Services { get; set; }
        public DateTime BookedFor { get; set; }
        public string PaymentMethodId { get; set; }
        public DateTime Created { get; set; }
    }
}
