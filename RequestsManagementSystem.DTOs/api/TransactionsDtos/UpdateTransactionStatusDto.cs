

namespace RequestsManagementSystem.DTOs.api.TransactionsDtos
{
    public class UpdateTransactionStatusDto
    {
        public string Status { get; set; } = "Pending";
        public string ResponceMessage {  get; set; } = string.Empty;
    }
}
