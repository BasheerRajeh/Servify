using Microsoft.EntityFrameworkCore;
using Servify.Models;

namespace Servify.Data
{
    public class ServifyDbContext: DbContext
    {
        public ServifyDbContext(DbContextOptions<ServifyDbContext> options): base(options)
        {

        }

        public DbSet<Restaurant> Restaurants{ get; set; }
        public DbSet<Employee> Employees{ get; set; }
    }
}
