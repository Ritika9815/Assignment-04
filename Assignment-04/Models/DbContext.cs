using Microsoft.EntityFrameworkCore;

namespace Assignment_04.Models
{
    public class InsuranceContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<InsuranceQuote> InsuranceQuotes { get; set; }

        public InsuranceContext(DbContextOptions<InsuranceContext> options) : base(options)
        {
        }
    }
}
