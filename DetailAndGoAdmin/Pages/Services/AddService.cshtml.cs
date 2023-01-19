using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DetailAndGo.Data;
using DetailAndGo.Models;
using DetailAndGo.Services.Interfaces;
using Stripe;

namespace DetailAndGoAdmin.Pages.Services
{
    public class AddServiceModel : PageModel
    {
        private readonly DetailAndGo.Data.ApplicationDbContext _context;
        private readonly DetailAndGo.Services.Interfaces.IStripeService _stripeService;

        public AddServiceModel(DetailAndGo.Data.ApplicationDbContext context, IStripeService stripeService)
        {
            _context = context;
            _stripeService = stripeService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Service Service { get; set; }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>
            {
                { "timeToFinishMinsS", Service.TimeToFinishMinsS.ToString() },
                { "timeToFinishMinsM", Service.TimeToFinishMinsM.ToString() },
                { "timeToFinishMinsL", Service.TimeToFinishMinsL.ToString() }
            };

            List<decimal> price = new List<decimal>();            
            if(Service.Price > 0)
            {
                price.Add(Service.Price);
            }

            if(Service.PriceMedium > 0)
            {
                price.Add(Service.PriceMedium);
            }

            if(Service.PriceLarge > 0)
            {
                price.Add(Service.PriceLarge); //CONTINUE HERE!!!!!!!!!!
            }
            

            Product product = await _stripeService.CreateProduct(Service.Name, Service.Description, price, metadata);
            Service.StripeServiceId = product.Id;
            Service.CreatedDate = DateTime.Now;
            Service.IsActive = true;
            Service.Category = "default";
            Service.Image = "none";

            var test = ModelState.Values;
            /*if (!ModelState.IsValid)
            {
                return Page();
            }*/

            _context.Services.Add(Service);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
