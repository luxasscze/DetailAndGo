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
        public Task<List<Booking>> GetAllDeclinedBookings(int take);
        public Task<List<Booking>> GetAllAcceptedBookings(int take);
        public Task<bool> HasActiveBooking(string aspNetUserId);
        public Task<Booking> GetBookingById(int id);
        public Task<List<Booking>> GetAllBookingsByStatus(BookingStatus status);
        public Task<Booking> DeclineBooking(int bookingId, string reason);
        public Task<bool> ReinstateBooking(int bookingId);
        public Task<bool> AcceptBooking(int bookingId);
        public Task<string> GetAllActiveBookinsAsJSON();
        public Task<string> GetAllActiveBookingsAsCalendarEvents();
    }
}
