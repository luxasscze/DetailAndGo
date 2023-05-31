using DetailAndGo.Models.Enums;

namespace DetailAndGo.Models
{
    public class BookingHistory
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public DateTime Created { get; set; }
        public BookingStatus Status { get; set; }
        public string Description { get; set; }
    }
}
