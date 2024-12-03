using Microsoft.EntityFrameworkCore;
using RequestsManagementSystem.Core.Entities;

namespace RequestsManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			modelBuilder.Entity<Employee>()
				.Property(e => e.EmployeeRole)
				.HasConversion<int>();
			base.OnModelCreating(modelBuilder);
        }
        public DbSet<Employee> Employees { get; set; }
    }
}
