using DetailAndGo.Services;
using DetailAndGo.Services.Interfaces;
using DetailAndGoAdmin;
using DetailAndGoAdmin.Data;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DetailAndGo.Data.ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DetailAndGo.Data.ApplicationDbContext>();
builder.Services.AddScoped<DetailAndGo.Services.Interfaces.IStripeService, DetailAndGo.Services.StripeService>();
builder.Services.AddScoped<DetailAndGo.Services.Interfaces.IDAGService, DetailAndGo.Services.DAGService>();
builder.Services.AddScoped<DetailAndGo.Services.Interfaces.IJobService, DetailAndGo.Services.JobService>();
//builder.Services.AddTransient<IDAGService, DAGService>();
builder.Services.AddRazorPages();
builder.Services.AddSingleton<Utility>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

if (!app.Environment.IsDevelopment())
{
    //app.UseHttpsRedirection();
}
//app.UseHttpsRedirection(); // MAYBE USE THIS!!!!
//app.UseRewriter(new RewriteOptions().AddRedirectToWwwPermanent().AddRedirectToHttpsPermanent()); //CONTINUE HERE TO SORT OUT THE SSL PROBLEM
app.UseStaticFiles();

app.UseRouting();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

string locale = "en-GB";
RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions
{
    SupportedCultures = new List<CultureInfo> { new CultureInfo(locale) },
    SupportedUICultures = new List<CultureInfo> { new CultureInfo(locale) },
    DefaultRequestCulture = new RequestCulture(locale)
};
app.UseRequestLocalization(localizationOptions);

app.Run();
