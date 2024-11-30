using Microsoft.EntityFrameworkCore;
using RequestsManagementSystem.Core.Entities;

namespace RequestsManagementSystem.Data
{
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
		protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			modelBuilder.Entity<Request>()
				.Property(p => p.Title)
				.HasConversion<int>();
			modelBuilder.Entity<Request>()
				.Property(p => p.Type)
				.HasConversion<int>();
			modelBuilder.Entity<Request>()
				.Property(p => p.Status)
				.HasConversion<int>();

			modelBuilder.Entity<Request>()
				.Property(p => p.Itinerary)
				.HasConversion(
					v => string.Join(';', v),
					v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
				);
		}
        public DbSet<Employee> Employees { get; set; } = default!;
        public DbSet<Request> Requests { get; set; } = default!;
    }
}
