namespace DetailAndGo.Models
{
    public class CreateBooking
    {
        public string carModel { get; set; }
        public string carFamily { get; set; }
        public string carSize { get; set; }
        public int[] services { get; set; }
        public int[] subServices { get; set; }
        public long totalPrice { get; set; }
        public DateTime dateTime { get; set; }
        public string paymentMethod { get; set; }
    }
}
