using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DetailAndGo.Models;
using DetailAndGoAdmin.Data;
using Microsoft.AspNetCore.Authorization;

namespace DetailAndGoAdmin.Pages.Jobs
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly DetailAndGo.Data.ApplicationDbContext _context;

        public CreateModel(DetailAndGo.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            var test = _context.Jobs.ToList();
            return Page();

        }

        [BindProperty]
        public Job Job { get; set; }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            var test = ModelState.Values;

            if (!ModelState.IsValid)
            {
                return Page();
            }
             
            _context.Jobs.Add(Job);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
