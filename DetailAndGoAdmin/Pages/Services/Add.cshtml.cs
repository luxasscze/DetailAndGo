using DetailAndGo.Services;
using DetailAndGo.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DetailAndGoAdmin.Pages.Services
{
    public class AddModel : PageModel
    {
        private readonly IDAGService _serviceService;

        public AddModel(IDAGService serviceService)
        {            
            _serviceService = serviceService;            
        }

        public void OnGet()
        {

        }

        public void OnPost() 
        {
        
        }
    }
}
