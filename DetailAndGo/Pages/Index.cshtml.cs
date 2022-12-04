using DetailAndGo.Models;
using DetailAndGo.Services;
using DetailAndGo.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DetailAndGo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ICustomerService _customerService;

        public IndexModel(ILogger<IndexModel> logger, ICustomerService customerService)
        {
            _logger = logger;
            _customerService = customerService;
        }

        public Customer Customer { get; set; }

        public void OnGetAsync()
        {
            Customer = _customerService.GetCustomerByEmail(User.Identity.Name);
            
            if (User.Identity.IsAuthenticated)
            {
                
            }
        }
    }
}