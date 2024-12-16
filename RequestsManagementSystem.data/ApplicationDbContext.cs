using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RequestsManagementSystem.Core.Entities;

namespace RequestsManagementSystem.Data
{
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Manager)
                .WithMany(m => m.ManagerStaff)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .Property(e => e.EmployeeRole)
                .HasConversion<short>();
            modelBuilder.Entity<Transaction>()
                .Property(p => p.Title)
                .HasConversion<short>();
            modelBuilder.Entity<Transaction>()
                .Property(p => p.Type)
                .HasConversion<short>();
            modelBuilder.Entity<Transaction>()
                .Property(p => p.Status)
                .HasConversion<short>();
            modelBuilder.Entity<Transaction>()
                .Property(p => p.Itinerary)
                .HasConversion(
                    v => string.Join(';', v),
                    v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
                )
                .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                    (c1, c2) => c1 != null && c2 != null ? c1.SequenceEqual(c2) : c1 == c2,
                    c => c != null ? c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())) : 0,
                    c => c != null ? c.ToList() : new List<string>()
                ));
        }
        public DbSet<Employee> Employees { get; set; } = default!;
        public DbSet<Transaction> Transactions { get; set; } = default!;
    }
}
