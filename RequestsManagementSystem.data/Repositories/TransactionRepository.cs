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
    }
}
