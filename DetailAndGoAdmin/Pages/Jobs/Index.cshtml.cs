using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DetailAndGo.Models;
using DetailAndGoAdmin.Data;
using DetailAndGo.Enums;
using Microsoft.AspNetCore.Authorization;

namespace DetailAndGoAdmin.Pages.Jobs
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly DetailAndGo.Data.ApplicationDbContext _context;

        public IndexModel(DetailAndGo.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Job> Jobs { get;set; }

        public async Task OnGetAsync()
        {
            if (_context.Jobs != null)
            {
                Jobs = _context.Jobs.ToList();               
            }
        }
    }
}
