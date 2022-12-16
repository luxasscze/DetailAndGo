using DetailAndGo.Models;

namespace DetailAndGo.Services.Interfaces
{
    public interface IBookingService
    {
        public Task CreateBooking(Booking booking);
        public Task<List<Booking>> GetCustomerAllBookings(string aspNetUserId);
        public Task<Booking?> GetCustomerActiveBooking(string aspNetUserId);
        public Task<List<Booking>> GetAllActiveBookings();
        public Task<string> GetAllActiveBookinsAsJSON();
        public Task<string> GetAllActiveBookingsAsCalendarEvents();
    }
}
