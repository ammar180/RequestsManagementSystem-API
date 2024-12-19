using Microsoft.EntityFrameworkCore;
using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Core.Interfaces;

namespace RequestsManagementSystem.Data.Repositories
{
	public class TransactionRepository : ITransactionRepository
    {

        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddTransactionAsync(Transaction transaction)
        {
            try
            {
                await _context.AddAsync(transaction);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Transaction>> GetStaffTransaction(int managerId)
        {
            return await _context.Employees
                .Include(e => e.Transactions)
                .ThenInclude(e=> e.Employee)
                .Where(e => e.ManagerId == managerId)
                .SelectMany(e => e.Transactions)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionByEmployeeIdAsync(int EmployeeId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.EmployeeId == EmployeeId)
                .Include(t => t.Employee)
                .ToListAsync();
            return transactions;
        
        }

		public async Task<Transaction?> GetTransactionByIdAsync(int id)
		{
            // lazy loading
            return await _context.Transactions.FindAsync(id);
		}

		public async Task SaveChanges()
		{
            await _context.SaveChangesAsync();
		}
	}
}
