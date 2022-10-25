using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DetailAndGoAdmin.Pages
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly Utility _utility = new Utility();
        private IWebHostEnvironment _webHostEnvironment;

        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        


        public void OnGet()
        {
            string rootPath = _webHostEnvironment.WebRootPath;
            ViewData["Greeting"] = _utility.GetGreetings();
            string[] quote = _utility.GetRandomQuote(rootPath).Result;
            ViewData["Quote"] = quote[0];
            ViewData["QuoteAuthor"] = quote[1];
        }
    }
}