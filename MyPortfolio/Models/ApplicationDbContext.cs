using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
namespace MyPortfolio.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Stock> StockList { get; set; } // stock
        public DbSet<AppUser> AppUsers { get; set; } // app user
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
                
    }
}
