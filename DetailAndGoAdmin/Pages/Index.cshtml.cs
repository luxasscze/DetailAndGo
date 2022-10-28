using DetailAndGo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DetailAndGoAdmin.Pages
{
    

    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly DetailAndGoAdmin.Data.ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;
        private readonly Utility _utility = new Utility();
        private IWebHostEnvironment _webHostEnvironment;        

        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment webHostEnvironment, DetailAndGoAdmin.Data.ApplicationDbContext context)
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
                Jobs = _context.Jobs.ToList();
            }
            ViewData["Greeting"] = _utility.GetGreetings();
            string[] quote = await _utility.GetRandomQuote(_webHostEnvironment.WebRootPath);
            ViewData["Quote"] = quote[0];
            ViewData["QuoteAuthor"] = quote[1];
        }
    }
}