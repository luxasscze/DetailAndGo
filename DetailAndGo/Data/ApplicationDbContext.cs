using DetailAndGo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DetailAndGo.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Lukas> Lukases { get; set; }
        public DbSet<TestingDb> TestingDb { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarHistory> CarHistories { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}