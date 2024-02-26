using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Servify.Models;

namespace Servify.Data
{
    public class ServifyDbContext: IdentityDbContext<User>
    {
        public ServifyDbContext(DbContextOptions<ServifyDbContext> options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<Restaurant> Restaurants{ get; set; }
        public DbSet<Employee> Employees{ get; set; }
    }
}
