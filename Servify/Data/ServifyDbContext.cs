using Microsoft.EntityFrameworkCore;
using Servify.Models;

namespace Servify.Data
{
    public class ServifyDbContext: DbContext
    {
        public ServifyDbContext(DbContextOptions<ServifyDbContext> options): base(options)
        {

        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Models.ServiceProvider> ServiceProviders { get; set; }
    }
}
