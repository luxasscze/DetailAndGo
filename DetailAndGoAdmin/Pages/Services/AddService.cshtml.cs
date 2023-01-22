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
        private readonly IDAGService _serviceService;

        public AddServiceModel(DetailAndGo.Data.ApplicationDbContext context, IStripeService stripeService, IDAGService serviceService)
        {
            _context = context;
            _stripeService = stripeService;
            _serviceService = serviceService;
        }        

        [BindProperty]
        public Service Service { get; set; }
        public List<Service> SubServices { get; set; }

        public async Task<IActionResult> OnGetAsync(List<Service>? subservices)
        {
            SubServices = subservices.Count == 0 ? await _serviceService.GetAllSubServices() : subservices;
            return Page();
        }
        
        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            Service.StripeServiceId = string.Empty;
            Service.CreatedDate = DateTime.Now;
            Service.IsActive = true;
            Service.Category = "default";
            Service.Image = "none";
            Service.PriceId = string.Empty;
            Service.PriceMediumId = string.Empty;
            Service.PriceLargeId = string.Empty;

            if (!ModelState.IsValid)
            {
                var test = ModelState.Values;
                return Page();
            }

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
                price.Add(Service.PriceLarge);
            }

            if(string.IsNullOrEmpty(Service.SubServices))
            {
                Service.SubServices = "";
                Service.IsCustomisable = false;
            }
            else
            {
                Service.IsCustomisable = true;
            }

            

            

            Product product = await _stripeService.CreateProduct(Service.Name, Service.Description, price, metadata);
            StripeList<Price> prices = await _stripeService.GetPricesByProductId(product.Id);
            Service.StripeServiceId = product.Id;                  
            Service.PriceId = prices.FirstOrDefault(s => s.Nickname == "small").Id;
            Service.PriceMediumId = prices.FirstOrDefault(s => s.Nickname == "medium").Id;
            Service.PriceLargeId = prices.FirstOrDefault(s => s.Nickname == "large").Id;

            
            

            _context.Services.Add(Service);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
