using DetailAndGoAdmin.Data;
using DetailAndGoServices;
using DetailAndGoServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DetailAndGoAdmin.Pages
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private Utility _utility = new Utility();
        private IWebHostEnvironment _webHostEnvironment;
        private ApplicationDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        public void OnGet()
        {
            
            ViewData["Greeting"] = _utility.GetGreetings();
            string[] quote = _utility.GetRandomQuote(_webHostEnvironment.WebRootPath).Result;
            ViewData["Quote"] = quote[0];
            ViewData["QuoteAuthor"] = quote[1];
        }
    }
}