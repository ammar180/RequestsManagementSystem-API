using Microsoft.EntityFrameworkCore;
using RequestsManagementSystem.Core.Entities;

namespace RequestsManagementSystem.Data
{
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Transaction>()
				.Property(p => p.Title)
				.HasConversion<int>();
			modelBuilder.Entity<Transaction>()
				.Property(p => p.Type)
				.HasConversion<int>();
			modelBuilder.Entity<Transaction>()
				.Property(p => p.Status)
				.HasConversion<int>();
		}
        public DbSet<Employee> Employees { get; set; } = default!;
        public DbSet<Transaction> Transactions { get; set; } = default!;
    }
}
