using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DetailAndGo.Data;
using DetailAndGo.Models;
using Stripe;

namespace DetailAndGoAdmin.Pages.Services
{
    public class EditModel : PageModel
    {
        private readonly DetailAndGo.Data.ApplicationDbContext _context;
        private readonly DetailAndGo.Services.Interfaces.IStripeService _stripeService;
        private readonly DetailAndGo.Services.Interfaces.IDAGService _serviceService;

        public EditModel(DetailAndGo.Data.ApplicationDbContext context, DetailAndGo.Services.Interfaces.IStripeService stripeService, DetailAndGo.Services.Interfaces.IDAGService serviceService)
        {
            _context = context;
            _stripeService = stripeService;
            _serviceService = serviceService;
        }

        [BindProperty]
        public Service Service { get; set; } = default!;
        public Service ServiceBeforeChange { get; set; }
        public List<Service> AllSubServices { get; set; }
        public List<Service> SubServices { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Services == null)
            {
                return NotFound();
            }

            var service = await _serviceService.GetServiceById((int)id);
            if (service == null)
            {
                return NotFound();
            }
            Service = service;
            ServiceBeforeChange = await _serviceService.GetServiceById((int)id);
            AllSubServices = await _serviceService.GetAllSubServices();
            if (Service.SubServices != null && Service.SubServices.Length > 0)
            {
                string[] subserviceIds = Service.SubServices.Split(',');

                SubServices = new List<Service>();

                foreach (var item in subserviceIds)
                {
                    Service serviceToAdd = await _serviceService.GetServiceById(Convert.ToInt32(item));
                    SubServices.Add(serviceToAdd);
                }
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {

            /*if (!ModelState.IsValid)
            {
                return Page();
            }*/

            _context.Attach(Service).State = EntityState.Modified;

            try
            {
                Dictionary<string, string> metadata = new Dictionary<string, string>()
                {
                    { "timeToFinishMinsS", Service.TimeToFinishMinsS.ToString() },
                    { "timeToFinishMinsM", Service.TimeToFinishMinsM.ToString() },
                    { "timeToFinishMinsL", Service.TimeToFinishMinsL.ToString() }
                };
                Stripe.Product changedProduct = await _stripeService.UpdateProduct(Service.StripeServiceId, Service.Name, Service.Description, Service.Price, Service.PriceMedium, Service.PriceLarge, metadata);

                StripeList<Stripe.Price> newPrices = await _stripeService.GetPricesByProductId(changedProduct.Id);

                Service.Price = (decimal)newPrices.FirstOrDefault(s => s.Nickname == "small").UnitAmount / 100;
                Service.PriceMedium = (decimal)newPrices.FirstOrDefault(s => s.Nickname == "medium").UnitAmount / 100;
                Service.PriceLarge = (decimal)newPrices.FirstOrDefault(s => s.Nickname == "large").UnitAmount / 100;
                Service.PriceId = newPrices.FirstOrDefault(s => s.Nickname == "small").Id;
                Service.PriceMediumId = newPrices.FirstOrDefault(s => s.Nickname == "medium").Id;
                Service.PriceLargeId = newPrices.FirstOrDefault(s => s.Nickname == "large").Id;
                Service.IsCustomisable = Service.SubServices != null ? true : false;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(Service.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.Id == id);
        }
    }
}
