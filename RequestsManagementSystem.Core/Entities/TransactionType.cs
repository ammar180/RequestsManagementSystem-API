using RequestsManagementSystem.Core.Enums;

namespace RequestsManagementSystem.Core.Entities
{
    public class TransactionType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Unit { get; set; } = 1;
        public int Sign { get; set; } = -1; // or 1 or 0

        private ETransactionType? _transactionType {  get; set; }
        public ETransactionType eType { get => _transactionType ??= Enum.TryParse(Name, true, out ETransactionType type) ? type : ETransactionType.Other; }
    }
}
