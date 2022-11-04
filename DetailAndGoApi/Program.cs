using DetailAndGo.Services.Interfaces;
using DetailAndGo.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using DetailAndGo.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer("Server=88.208.214.244;Initial Catalog=DetailAndGo;User ID=sa;Password=Gordon2Freeman1*;TrustServerCertificate=true;"));
builder.Services.AddTransient<ICustomerService, CustomerService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
