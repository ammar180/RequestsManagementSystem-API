using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using RequestsManagementSystem.Core.Entities;

namespace RequestsManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public ApplicationDbContext(IConfiguration configuration, DbContextOptions options) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
                .HasIndex(e => e.Code)
                .IsUnique();

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.SubstituteEmployee)
                .WithMany()
                .HasForeignKey(t => t.SubstituteEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data for EmployeeLevel
            // Load EmployeeLevels from appsettings.json
            var employeeLevels = _configuration.GetSection("EmployeeLevels").GetChildren();
            modelBuilder.Entity<EmployeeLevel>().HasData(
                employeeLevels.Select(e => new EmployeeLevel
                {
                    Id = int.Parse(e["Id"]!),
                    LevelName = e["LevelName"]!,
                    LevelDescription = e["LevelDescription"]!,
                    RegularLeaveperYear = int.Parse(e["RegularLeaveperYear"]!),
                    CasualLeavePerYear = int.Parse(e["CasualLeavePerYear"]!),
                    OrderId = int.Parse(e["OrderId"]!)
                }).ToArray()
            );

            // Seed data for TransactionType
            // Load transaction types from appsettings.json
            var transactionTypes = _configuration.GetSection("TransactionTypes").GetChildren();
            modelBuilder.Entity<TransactionType>().HasData(
                transactionTypes.Select(t =>
                {
                    var tType = new TransactionType
                    {
                        Name = t["Name"]!,
                        Description = t["Description"]!,
                        Unit = double.Parse(t["Unit"]!),
                        Sign = int.Parse(t["Sign"]!),
                        ParentType = t["Parent"] ?? "",
                    };

                    tType.Id = (int)tType.EType;                    
                    return tType;
                }).ToArray()
            );

            modelBuilder.Entity<Transaction>()
                .Navigation(t => t.Type)
                .AutoInclude();
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Type)
                .WithMany()
                .HasForeignKey(t => t.TypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .Property(e => e.EmployeeRole)
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
        public DbSet<TransactionType> TransactionTypes { get; set; } = default!;
    }
}
