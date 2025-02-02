using RequestsManagementSystem.Core.Enums;

namespace RequestsManagementSystem.Dtos.TransactionsDtos
{
    public class UpdateTransactionStatusDto
    {
        public string Status { get; set; } = TransactionStatus.Pending.ToString();
        public string ResponceMessage {  get; set; } = string.Empty;
    }
}
