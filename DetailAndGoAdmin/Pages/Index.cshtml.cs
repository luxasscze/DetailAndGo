using DetailAndGoServices;
using DetailAndGoServices.DAL;
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
        private ContentContext _context;

        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment webHostEnvironment, ContentContext context)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        public void OnGet()
        {
            CustomerService customerService = new CustomerService(_context);
            Customer customer = customerService.Test();
            ViewData["Greeting"] = _utility.GetGreetings();
            string[] quote = _utility.GetRandomQuote(_webHostEnvironment.WebRootPath).Result;
            ViewData["Quote"] = quote[0];
            ViewData["QuoteAuthor"] = quote[1];
        }
    }
}