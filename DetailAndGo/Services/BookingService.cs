using DetailAndGo.Data;
using DetailAndGo.Models;
using DetailAndGo.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DetailAndGo.Models.Enums;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Stripe;

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

            if (services == null)
            {
                return "0";
            }

            foreach (var service in services)
            {
                result += service.ToString() + ",";
            }

            return result.Remove(result.Length - 1, 1);
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
                    SubServicesArray = bookingInput.subServices == null ? "0" : DecodeServicesToServicesArray(bookingInput.subServices),
                    TotalAmount = (long)(bookingInput.totalPrice * 100)
                };                

                await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync();


            }
        }

        public async Task<bool> UpdateBookingStatus(int bookingId, BookingStatus status, string description)
        {
            Booking booking = await _context.Bookings.FirstOrDefaultAsync(s => s.Id == bookingId);
            if (booking != null)
            {
                booking.Status = status;
                booking.Notes = description;
                _context.Entry(booking).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<Booking>> GetCustomerAllBookings(string aspNetUserId)
        {
            List<Booking> allBookings = await _context.Bookings.Where(s => s.AspNetUserId == aspNetUserId).ToListAsync();
            return allBookings;
        }

        public async Task<Booking?> GetCustomerActiveBooking(string aspNetUserId)
        {
            Booking? booking = await _context.Bookings.OrderBy(s => s.Id).LastOrDefaultAsync(s => s.AspNetUserId == aspNetUserId);
            return booking;
        }

        public async Task<bool> HasActiveBooking(string aspNetUserId)
        {
            Booking? booking = await _context.Bookings.Where(s => s.AspNetUserId == aspNetUserId && (s.Status == BookingStatus.Approved || s.Status == BookingStatus.AwaitingApproval)).OrderBy(s => s.Id).LastOrDefaultAsync();
            return booking == null ? false : true;  
        }

        public async Task<List<Booking>> GetAllActiveBookings()
        {
            List<Booking> allBookings = await _context.Bookings.Where(s => (s.Status == BookingStatus.Created || s.Status == BookingStatus.Approved) && s.BookedFor > DateTime.Now).ToListAsync();
            return allBookings;
        }

        public async Task<List<Booking>> GetAllCreatedBookings()
        {
            List<Booking> allBookings = await _context.Bookings.Where(s => (s.Status == BookingStatus.AwaitingApproval || s.Status == BookingStatus.CardDeclined) && s.BookedFor > DateTime.Now).OrderBy(s => s.BookedFor).ToListAsync();
            return allBookings;
        }

        public async Task<List<Booking>> GetAllDeclinedBookings(int take)
        {
            List<Booking> allBookings = await _context.Bookings.Where(s => s.Status == BookingStatus.Declined).OrderByDescending(s => s.Id).Take(take).ToListAsync();
            return allBookings;
        }

        public async Task<List<Booking>> GetAllAcceptedBookings(int take)
        {
            List<Booking> allBookings = await _context.Bookings.Where(s => s.Status == BookingStatus.Approved).OrderByDescending(s => s.Id).Take(take).ToListAsync();
            return allBookings;
        }

        public async Task<List<Booking>> GetAllBookingsByStatus(BookingStatus status)
        {
            List<Booking> allBookings = await _context.Bookings.Where(s => s.Status == status).ToListAsync();
            return allBookings;
        }

        public async Task<string> GetAllActiveBookinsAsJSON()
        {
            List<Booking> allBookings = await _context.Bookings.Where(s => (s.Status == BookingStatus.Created || s.Status == BookingStatus.Approved) && s.BookedFor > DateTime.Now).ToListAsync();
            string output = JsonConvert.SerializeObject(allBookings, Formatting.Indented);
            return output;
        }

        public async Task<Booking> GetBookingById(int id)
        {
            Booking booking = await _context.Bookings.FirstOrDefaultAsync(s => s.Id == id);
            return booking;
        }

        public async Task<Booking> DeclineBooking(int bookingId, string reason)
        {
            Booking booking = await _context.Bookings.FirstOrDefaultAsync(s => s.Id == bookingId);           
            BookingHistory historyToAdd = new BookingHistory()
            {
                BookingId = bookingId,
                Created = DateTime.Now,
                Description = "Booking has been declined --- " + reason,
                Status = BookingStatus.Declined
            };
            booking.Notes = reason;
            booking.Status = BookingStatus.Declined;
            booking.StatusChanged = DateTime.Now;
            _context.Entry(booking).State = EntityState.Modified;
            _context.BookingHistories.Add(historyToAdd);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<bool> ReinstateBooking(int bookingId)
        {
            Booking booking = await _context.Bookings.FirstOrDefaultAsync(s => s.Id == bookingId);
            BookingHistory historyToAdd = new BookingHistory()
            {
                BookingId = bookingId,
                Created = DateTime.Now,
                Description = "Booking has been reinstated",
                Status = BookingStatus.Reinstated
            };
            booking.Notes = "***REINSTATED***";
            booking.Status = BookingStatus.AwaitingApproval;
            booking.StatusChanged = DateTime.Now;
            _context.Entry(booking).State = EntityState.Modified;
            _context.BookingHistories.Add(historyToAdd);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Booking>> GetBookingsOnTheWay()
        {
            List<Booking> bookings = await _context.Bookings.Where(s => s.Status == BookingStatus.OnTheWay).ToListAsync();
                     
            return bookings;
        }

        public async Task<Booking> GetBookingInProgress()
        {
            Booking booking = await _context.Bookings.FirstOrDefaultAsync(s => s.Status == BookingStatus.InProgress);
            return booking;
        }

        public async Task<List<Booking>> GetCancelledBookings()
        {
            List<Booking> bookings = await _context.Bookings.Where(s => s.Status == BookingStatus.Cancelled).ToListAsync();
            return bookings;
        }

        public async Task<List<Booking>> GetFinishedBookings(int take)
        {
            List<Booking> bookings = await _context.Bookings.Where(s => s.Status == BookingStatus.Finished).OrderByDescending(s => s.Created).Take(take).ToListAsync();
            return bookings;
        }

        public async Task<bool> SetBookingOnTheWay(int bookingId)
        {
            Booking booking = await _context.Bookings.FirstOrDefaultAsync(s => s.Id == bookingId);
            BookingHistory history = new BookingHistory()
            {
                BookingId = bookingId,
                Created = DateTime.Now,
                Description = "On the way to the customer",
                Status = BookingStatus.OnTheWay
            };
            booking.Status = BookingStatus.OnTheWay;
            booking.StatusChanged = DateTime.Now;
            booking.Notes = "***ON THE WAY***";
            _context.Entry(booking).State = EntityState.Modified;
            _context.BookingHistories.Add(history);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AcceptBooking(int bookingId)
        {
            Booking booking = await _context.Bookings.FirstOrDefaultAsync(s => s.Id == bookingId);
            BookingHistory historyToAdd = new BookingHistory()
            {
                BookingId = bookingId,
                Created = DateTime.Now,
                Description = "Booking has been accepted and paid for",
                Status = BookingStatus.Approved
            };
            booking.Status = BookingStatus.Approved;
            booking.StatusChanged = DateTime.Now;
            booking.Notes = "***ACCEPTED***PAID***";
            _context.Entry(booking).State = EntityState.Modified;
            _context.BookingHistories.Add(historyToAdd);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<bool> CancelBooking(int bookingID, string notes)
        {
            Booking booking = await _context.Bookings.FirstOrDefaultAsync(s => s.Id == bookingID);
            BookingHistory historyToAdd = new BookingHistory()
            {
                BookingId = bookingID,
                Created = DateTime.Now,
                Description = notes,
                Status = BookingStatus.Cancelled                
            };
            booking.Status = BookingStatus.Cancelled;
            booking.StatusChanged = DateTime.Now;
            booking.Notes = notes;
            _context.Entry(booking).State = EntityState.Modified;
            _context.BookingHistories.Add(historyToAdd);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> StartJob(int bookingId)
        {
            Booking booking = await _context.Bookings.FirstOrDefaultAsync(s => s.Id == bookingId);
            BookingHistory historyToAdd = new BookingHistory()
            {
                BookingId = bookingId,
                Created = DateTime.Now,
                Description = "Job Started",
                Status = BookingStatus.InProgress
            };
            booking.Status = BookingStatus.InProgress;
            booking.StatusChanged = DateTime.Now;
            booking.Notes = "***IN PROGRESS***";
            _context.Entry(booking).State = EntityState.Modified;
            _context.BookingHistories.Add(historyToAdd);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> FinishBooking(int bookingId)
        {
            Booking booking = await _context.Bookings.FirstOrDefaultAsync(s => s.Id == bookingId);
            BookingHistory historyToAdd = new BookingHistory()
            {
                BookingId = bookingId,
                Created = DateTime.Now,
                Description = "Booking finished",
                Status = BookingStatus.Finished
            };
            booking.Status = BookingStatus.Finished;
            booking.StatusChanged = DateTime.Now;
            booking.Notes = "***FINISHED***";
            _context.Entry(booking).State = EntityState.Modified;
            _context.BookingHistories.Add(historyToAdd);
            await _context.SaveChangesAsync();
            return true;
        }

       
        public async Task<bool> AddToBookingHistory(BookingHistory history)
        {
            if (history != null)
            {
                _context.BookingHistories.Add(history);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<BookingHistory>> GetBookingHistoryByBookingId(int bookingId)
        {
            List<BookingHistory> history = await _context.BookingHistories.Where(s => s.BookingId == bookingId).OrderByDescending(s => s.Created).ToListAsync();
            return history;
        }

        public async Task<string> GetAllActiveBookingsAsCalendarEvents()
        {
            List<Booking> allBookings = await _context.Bookings.Where(s => (s.Status == BookingStatus.Created || s.Status == BookingStatus.Approved) && s.BookedFor.Date >= DateTime.Now.Date).ToListAsync();

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
