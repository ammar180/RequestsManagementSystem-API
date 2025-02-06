using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Data;

namespace RequestsManagementSystem.Services
{
    public interface IAppConfigRepository
    {
        List<TransactionType> TransactionTypes { get; }
    }
    public class AppConfigRepository : IAppConfigRepository
    {
        private readonly List<TransactionType> _transactionTypes;
        public List<TransactionType> TransactionTypes => _transactionTypes;

        public AppConfigRepository(ApplicationDbContext context)
        {
            _transactionTypes = context.TransactionTypes.ToList();
        }
    }
}
