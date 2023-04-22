using DetailAndGo.Data;
using DetailAndGo.Models;
using DetailAndGo.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DetailAndGo.Models.Enums;
using Newtonsoft.Json;

namespace DetailAndGo.Services
{
    public class BookingService : IBookingService
    {
        public ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private IDAGService _serviceService;

        public BookingService(ApplicationDbContext context, UserManager<IdentityUser> userManager, IDAGService serviceService)
        {
            _context = context;
            _userManager = userManager;
            _serviceService = serviceService;
        }

        public string DecodeServicesToServicesArray(int[] services)
        {
            string result = string.Empty;

            if(services == null)
            {
                return "0";
            }

            int lastService = services.Last();

            foreach (var service in services)
            {
                if (service == lastService)
                {
                    result += service.ToString();
                }
                else
                {
                    result += service.ToString() + ",";
                }
            }

            return result;
        }

        public async Task CreateBooking(CreateBooking bookingInput, string aspNetUserId)
        {
            if (bookingInput != null)
            {
                Booking booking = new Booking()
                {
                    AspNetUserId = aspNetUserId,
                    BookedFor = bookingInput.dateTime,
                    CarId = bookingInput.carId,
                    Created = DateTime.Now,
                    Image = "",
                    Notes = "",
                    PaymentMethodId = bookingInput.paymentMethod,
                    Status = BookingStatus.AwaitingApproval,
                    ServicesArray = DecodeServicesToServicesArray(bookingInput.services),
                    SubServicesArray = bookingInput.subServices == null ? "0" : DecodeServicesToServicesArray(bookingInput.subServices)
                };

                await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Booking>> GetCustomerAllBookings(string aspNetUserId)
        {
            List<Booking> allBookings = await _context.Bookings.Where(s => s.AspNetUserId == aspNetUserId).ToListAsync();
            return allBookings;
        }

        public async Task<Booking?> GetCustomerActiveBooking(string aspNetUserId)
        {
            Booking? booking = await _context.Bookings.FirstOrDefaultAsync(s => s.AspNetUserId == aspNetUserId && s.Status == BookingStatus.Created);
            return booking;
        }

        public async Task<List<Booking>> GetAllActiveBookings()
        {
            List<Booking> allBookings = await _context.Bookings.Where(s => (s.Status == BookingStatus.Created || s.Status == BookingStatus.Accepted) && s.BookedFor > DateTime.Now).ToListAsync();
            return allBookings;
        }

        public async Task<List<Booking>> GetAllCreatedBookings()
        {
            List<Booking> allBookings = await _context.Bookings.Where(s => (s.Status == BookingStatus.AwaitingApproval) && s.BookedFor > DateTime.Now).ToListAsync();
            return allBookings;
        }

        public async Task<List<Booking>> GetAllBookingsByStatus(BookingStatus status)
        {
            List<Booking> allBookings = await _context.Bookings.Where(s => (s.Status == status) && s.BookedFor > DateTime.Now).ToListAsync();
            return allBookings;
        }

        public async Task<string> GetAllActiveBookinsAsJSON()
        {
            List<Booking> allBookings = await _context.Bookings.Where(s => (s.Status == BookingStatus.Created || s.Status == BookingStatus.Accepted) && s.BookedFor > DateTime.Now).ToListAsync();
            string output = JsonConvert.SerializeObject(allBookings, Formatting.Indented);
            return output;
        }

        public async Task<string> GetAllActiveBookingsAsCalendarEvents()
        {
            List<Booking> allBookings = await _context.Bookings.Where(s => (s.Status == BookingStatus.Created || s.Status == BookingStatus.Accepted) && s.BookedFor.Date >= DateTime.Now.Date).ToListAsync();

            List<CalendarBooking> calendarBookings = new List<CalendarBooking>();

            foreach (Booking booking in allBookings)
            {
                calendarBookings.Add(new CalendarBooking()
                {
                    allDay = true,
                    backgroundColor = "transparent",
                    borderColor = "#222",
                    classNames = new string[] { "text-light", "bg-danger" },
                    editable = false,
                    end = booking.BookedFor.AddHours(8),
                    start = booking.BookedFor,
                    extendedProps = new Dictionary<string, string>() { },
                    groupId = "a",
                    textColor = "#fff",
                    title = "Fully Booked",
                    url = "#",
                    display = "background"
                });
            }

            string output = JsonConvert.SerializeObject(calendarBookings, Formatting.Indented);
            return output;
        }
    }
}
