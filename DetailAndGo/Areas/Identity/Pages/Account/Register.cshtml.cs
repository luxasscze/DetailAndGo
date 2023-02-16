// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using DetailAndGo.Models;
using DetailAndGo.Models.Enums;
using DetailAndGo.Services;
using DetailAndGo.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace DetailAndGo.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IEmailService _emailService;
        private readonly ICustomerService _customerService;
        private readonly ICarService _carService;
        private readonly IStripeService _stripeService;
        private readonly Data.ApplicationDbContext _context;
        private IWebHostEnvironment _webHostEnvironment;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ICustomerService customerService,
            IStripeService stripeService,
            IEmailService emailService,
            IWebHostEnvironment webHostEnvironment,
            ICarService carService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _emailService = emailService;
            _customerService = customerService;
            _carService = carService;
            _stripeService = stripeService;
            _webHostEnvironment = webHostEnvironment;

            //_customerService = new CustomerService();
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "First Line Address")]
            public string Address1 { get; set; }

            [Display(Name = "Second Line Address")]
            public string Address2 { get; set; }

            [Display(Name = "Third Line Address")]
            public string Address3 { get; set; }

            [Required]
            [Display(Name = "Post Code")]
            public string PostCode { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "Car Model")]
            public string CarModel { get; set; }

            [Required]
            [Display(Name = "Car Family")]
            public string CarFamily { get; set; }

            [Required]
            [Display(Name = "Car Size")]
            public string CarSize { get; set; }

            [Required]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Card number")]
            public string CardNumber { get; set; }

            [Required]
            [Display(Name = "Expiry")]
            public string Expiry { get; set; }

            [Required]
            [Display(Name = "CVC")]
            public string CVC { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {            
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Page("/Account/RegisterConfirmation", pageHandler: null, values: new { area = "Identity" }, protocol: "https");
            //ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            //var test = Input;

            var errors = ModelState.Values;

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    _userManager.Options.SignIn.RequireConfirmedEmail = true;
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var test = Request.Scheme;
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: "https");                   
                    string encodedCallBackUrl = HtmlEncoder.Default.Encode(callbackUrl);
                    //encodedCallBackUrl = $"{Request.Scheme}://{Request.Host}/{encodedCallBackUrl.TrimStart('/')}";

                    Email email = new Email();
                    using (StreamReader reader = System.IO.File.OpenText(_webHostEnvironment.WebRootPath + "/Email/index.html"))
                    {
                        email.From = "info@detailandgo.co.uk";
                        email.Body = reader.ReadToEnd()
                            .Replace("{callbackUrl}", encodedCallBackUrl)
                            .Replace("{firstName}", Input.FirstName)
                            .Replace("{callbackBook}", Url.Page("/Index"));
                        email.IsHtml = true;
                        email.Subject = Input.FirstName + ", confirm your Detail&Go account";
                        email.To = Input.Email;
                    }
                    try
                    {
                        await _emailService.SendSingleEmail(email);
                    }
                    catch(Exception ex)
                    {
                        _userManager.Options.SignIn.RequireConfirmedEmail = false;
                    }

                    Customer customerToRegister = new Customer()
                    {
                        AspNetUserId = userId,
                        Email = Input.Email,
                        Registered = DateTime.Now,
                        Address1 = Input.Address1,
                        Address2 = Input.Address2,
                        Address3 = Input.Address3,
                        PostCode = Input.PostCode,
                        FirstName = Input.FirstName,
                        LastName = Input.LastName,
                        CarModel = Input.CarModel,
                        CarFamily = Input.CarFamily,
                        PhoneNumber = Input.PhoneNumber,
                        CarSize = Input.CarSize
                    };

                    Car initialCar = new Car()
                    {
                        Created = DateTime.Now,
                        AspNetUserId = userId,
                        CarFamily = Input.CarFamily,
                        CarModel = Input.CarModel,
                        IsPrimary = true,
                        Notes = string.Empty,
                        ManufactureYear = "",
                        CarSize = GetCarSize(Input.CarSize)
                    };

                    await _carService.SaveCar(initialCar);

                    int expM = int.Parse(Input.Expiry.Split('/')[0]);
                    int expY = int.Parse(Input.Expiry.Split('/')[1]);
                    string stripeId = await _stripeService.CreateCustomerAsync(customerToRegister);
                    string paymentMethod = await _stripeService.CreatePaymentMethod(Input.CardNumber, expM, expY, Input.CVC);
                    await _stripeService.AttachPaymentMethodToCustomer(stripeId, paymentMethod);
                    //await _stripeService.SetCustomerDefaultPaymentMethod(stripeId, paymentMethod);
                    customerToRegister.StripeId = stripeId;
                    await _customerService.RegisterCustomerAsync(customerToRegister);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount) // REQUIRES EMAIL CONFIRMATION
                    {
                        return RedirectToPage("/Account/RegisterConfirmation", pageHandler: null, new { area = "Identity" });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToPage("Index");
                    }                    
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        public CarSize GetCarSize(string carSize)
        {
            switch (carSize)
            {
                case "small": return CarSize.Small;
                case "medium": return CarSize.Medium;
                case "large": return CarSize.Large;
            }
            return CarSize.Unknown;
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }

        public JsonResult OnGetCheckCreditCard(string cardNumber, string expiry, string cvc)
        {
            if (string.IsNullOrEmpty(cardNumber) || string.IsNullOrEmpty(expiry) || string.IsNullOrEmpty(cvc))
            {
                return new JsonResult(false);
            }
            int expM = int.Parse(expiry.Split('/')[0]);
            int expY = int.Parse(expiry.Split('/')[1]);
            if (_stripeService.CreatePaymentMethod(cardNumber, expM, expY, cvc).Result == "invalid_card")
            {
                return new JsonResult(false);
            }
            else
            {
                return new JsonResult(true);
            }
        }

        public async Task<JsonResult> OnGetCheckEmailExists(string email)
        {
            bool result = await _customerService.CheckEmailExists(email);
            if (result)
            {
                return new JsonResult(true);
            }
            else
            {
                return new JsonResult(false);
            }
        }
    }
}
