using DetailAndGo.Models;
using DetailAndGo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DetailAndGoAdmin.Pages
{
    [Authorize(Roles = "Admin")]
    public class SettingsModel : PageModel
    {
        private readonly DetailAndGo.Data.ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;        
        private IWebHostEnvironment _webHostEnvironment;
        private readonly IJobService _jobService;

        public SettingsModel(ILogger<IndexModel> logger, IWebHostEnvironment webHostEnvironment, DetailAndGo.Data.ApplicationDbContext context, IJobService jobService)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _jobService = jobService;
        }

        public IList<Job> Jobs { get; set; }
        public Dictionary<string, string> AllSettings { get; set; } = new Dictionary<string, string>();

        [BindProperty]
        public SettingsClass SettingsToSave { get; set; }

        public async Task OnGetAsync()
        {
            AllSettings = await _jobService.GetAllSettings();
            
        }

        public ActionResult OnPostAsync()
        {
            _jobService.UpdateTimes(SettingsToSave.TimeStart, SettingsToSave.TimeEnd, SettingsToSave.TimeBetweenJobs);
            return RedirectToPage();
        }

        public class SettingsClass
        {
            public string TimeStart { get; set; }
            public string TimeEnd { get; set; }
            public string TimeBetweenJobs { get; set; }
        }
    }
}
