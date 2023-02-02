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
        private readonly DetailAndGo.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IDAGService _serviceService;

        public IndexModel(DetailAndGo.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager, IDAGService serviceService)
        {
            _context = context;
            _userManager = userManager;
            _serviceService = serviceService;
        }

        public List<Service> AllServices { get; set; }
        public List<Service> AllSubServices { get; set; }

        public async Task OnGetAsync()
        {

            AllServices = await _serviceService.GetAllServicesOrderedByCreated();
            AllServices = AllServices.OrderBy(s => s.IsSubService).ToList();
            AllSubServices = await _serviceService.GetAllSubServices();
        }
    }
}
