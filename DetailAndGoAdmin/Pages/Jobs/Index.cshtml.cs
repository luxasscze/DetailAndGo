﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DetailAndGo.Models;
using DetailAndGoAdmin.Data;
using DetailAndGo.Enums;
using Microsoft.AspNetCore.Authorization;
using DetailAndGo.Services.Interfaces;

namespace DetailAndGoAdmin.Pages.Jobs
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly DetailAndGo.Data.ApplicationDbContext _context;
        private IBookingService _bookingService;
        private ICustomerService _customerService;
        private IDAGService _serviceService;
        private ICarService _carService;

        public IndexModel(DetailAndGo.Data.ApplicationDbContext context, IBookingService bookingService, ICustomerService customerService, IDAGService serviceService, ICarService carService)
        {
            _context = context;
            _bookingService = bookingService;
            _customerService = customerService;
            _serviceService = serviceService;
            _carService = carService;
        }

        public IList<Booking> Bookings { get;set; }
        public IList<Booking> DeclinedBookings { get; set; }
        public IList<Booking> AcceptedBookings { get; set; }
        public IList<Booking> BookingsOnTheWay { get; set; }
        public IList<Booking> CancelledBookings { get; set; }
        public IList<Booking> FinishedBookings { get; set; }
        public Booking BookingInProgress { get; set; }

        public async Task OnGetAsync()
        {
            if (_context.Jobs != null)
            {
                Bookings = await _bookingService.GetAllCreatedBookings();
                DeclinedBookings = await _bookingService.GetAllDeclinedBookings(take: 12);
                AcceptedBookings = await _bookingService.GetAllAcceptedBookings(take: 12);
                BookingsOnTheWay = await _bookingService.GetBookingsOnTheWay();
                BookingInProgress = await _bookingService.GetBookingInProgress();
                CancelledBookings = await _bookingService.GetCancelledBookings();
                FinishedBookings = await _bookingService.GetFinishedBookings(take: 12);
            }           
            
        }

        public JsonResult OnGetGetUserFullName(string aspNetUserId)
        {
            Customer customer = _customerService.GetCustomerById(aspNetUserId);
            return new JsonResult(customer.FirstName + " " + customer.LastName);
        }

        public JsonResult OnGetGetServiceName(int serviceId)
        {
            return new JsonResult(_serviceService.GetServiceNameById(serviceId));
        }

        public JsonResult OnGetGetCarName(int carId)
        {
            return new JsonResult(_carService.GetCarName(carId));
        }

        public JsonResult OnGetGetPostCode(string aspNetUserId)
        {
            return new JsonResult(_customerService.GetCustomerById(aspNetUserId).PostCode);
        }
    }
}
