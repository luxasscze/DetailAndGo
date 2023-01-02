using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DetailAndGo.Models;
using DetailAndGoAdmin.Data;
using Microsoft.AspNetCore.Authorization;

namespace DetailAndGoAdmin.Pages.Customers
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly DetailAndGo.Data.ApplicationDbContext _context;

        public IndexModel(DetailAndGo.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Customer> Customers { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Customers != null)
            {
                Customers = await _context.Customers.ToListAsync();
            }
        }
    }
}
