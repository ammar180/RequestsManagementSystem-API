using Microsoft.EntityFrameworkCore;
using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Core.Interfaces;
using RequestsManagementSystem.Core.Enums;

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

        public async Task<Transaction?> GetTransactionById(int transactionId)
        {
            return await _context.Transactions.FindAsync(transactionId);
        }

        public async Task<bool> RemoveTransactionAsync(int transactionId)
        {
            var transaction = await _context.Transactions.FindAsync(transactionId);
            if (transaction == null)
            {
                return false;
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Transaction>> GetStaffTransaction(int managerId)
        {
            return await _context.Employees
                .Include(e => e.Transactions)
                .ThenInclude(e=> e.Employee)
                .Where(e => e.ManagerId == managerId)
                .SelectMany(e => e.Transactions)
                .Where(t => t.Status == TransactionStatus.Edited)
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

        public async Task<Transaction?> GetTransactionByIdAsync(int id, string[]? includes = null)
        {
            IQueryable<Transaction> query = _context.Transactions;

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync(x => x.TransactionId == id);
        }

        public async Task SaveChanges()
		{
            await _context.SaveChangesAsync();
		}

        public async Task<bool> UpdateTransactionAsync(Transaction transaction)
        {
            try
            {
                _context.Transactions.Update(transaction);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
