﻿using DetailAndGo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DetailAndGoAdmin.Pages
{
    

    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly DetailAndGo.Data.ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;
        private readonly Utility _utility = new Utility();
        private IWebHostEnvironment _webHostEnvironment;        

        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment webHostEnvironment, DetailAndGo.Data.ApplicationDbContext context)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        public IList<Job> Jobs { get; set; }       

        public async Task OnGetAsync()
        {
            if (_context.Jobs != null)
            {
                Jobs = _context.Jobs.OrderBy(r => r.BookingDate).ToList();     
            }
            await GreetingHeader();            
        }

       
        public async Task OnGetNearJobsAsync()
        {
            if(_context.Jobs != null)
            {
                Jobs = _context.Jobs.Take(3).ToList();
            }
            await GreetingHeader();
        }

        public async Task OnGetNextClosestJobAsync()
        {
            if (_context.Jobs != null)
            {
                Jobs = _context.Jobs.OrderBy(s => s.BookingDate).Take(1).ToList();
            }
            await GreetingHeader();
        }

        public async Task GreetingHeader()
        {
            ViewData["Greeting"] = _utility.GetGreetings();
            string[] quote = await _utility.GetRandomQuote(_webHostEnvironment.WebRootPath);
            ViewData["Quote"] = quote[0];
            ViewData["QuoteAuthor"] = quote[1];
        }
    }
}