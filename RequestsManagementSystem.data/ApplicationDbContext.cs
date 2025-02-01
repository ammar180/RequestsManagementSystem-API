using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
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
               .HasMany(e => e.Transactions)
               .WithOne(t => t.Employee)
               .HasForeignKey(t => t.EmployeeId);
            
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.EmployeeCode)
                .IsUnique();

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.SubstituteEmployee)
                .WithMany()
                .HasForeignKey(t => t.SubstituteEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data for EmployeeLevel
            modelBuilder.Entity<EmployeeLevel>().HasData(
                new EmployeeLevel
                {
                    Id = 1,
                    LevelName = "A",
                    LevelDescription = "الفئة أ",
                    RegularLeaveperYear = 15,
                    RegularLeaveperMonth = 15f / 12f,
                    CasualLeavePerYear = 6,
                    CasualLeavePerMonth = 6f / 12f,
                    OrderId = 1
                },
                new EmployeeLevel
                {
                    Id = 2,
                    LevelName = "B",
                    LevelDescription = "الفئة ب",
                    RegularLeaveperYear = 24,
                    RegularLeaveperMonth = 24f / 12f,
                    CasualLeavePerYear = 6,
                    CasualLeavePerMonth = 6f / 12f,
                    OrderId = 2
                }
            );

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
                .Property(p => p.SeenStatus)
                .HasConversion<short>();
            modelBuilder.Entity<Transaction>()
                .Property(p => p.Itinerary)
                .HasConversion(
                    v => string.Join(';', v ?? new List<string> { "" }),
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
