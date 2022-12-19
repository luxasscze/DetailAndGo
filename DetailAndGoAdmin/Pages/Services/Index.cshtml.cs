using DetailAndGo.Models;
using DetailAndGo.Services;
using DetailAndGo.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DetailAndGoAdmin.Pages.Services
{
    public class IndexModel : PageModel
    {
        private readonly DetailAndGoAdmin.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(DetailAndGoAdmin.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Service> AllServices { get; set; }

        public async Task OnGetAsync()
        {

            AllServices = await _context.Services.ToListAsync();
        }
    }
}
