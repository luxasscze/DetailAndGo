using DetailAndGo.Models;
using DetailAndGo.Models.Enums;

namespace DetailAndGo.Services.Interfaces
{
    public interface IBookingService
    {
        public Task CreateBooking(CreateBooking booking, string aspNetUserId);
        public Task<List<Booking>> GetCustomerAllBookings(string aspNetUserId);
        public Task<Booking?> GetCustomerActiveBooking(string aspNetUserId);
        public Task<List<Booking>> GetAllActiveBookings();
        public Task<List<Booking>> GetAllCreatedBookings();
        public Task<List<Booking>> GetAllDeclinedBookings();
        public Task<Booking> GetBookingById(int id);
        public Task<List<Booking>> GetAllBookingsByStatus(BookingStatus status);
        public Task<Booking> DeclineBooking(int bookingId, string reason);
        public Task<string> GetAllActiveBookinsAsJSON();
        public Task<string> GetAllActiveBookingsAsCalendarEvents();
    }
}
