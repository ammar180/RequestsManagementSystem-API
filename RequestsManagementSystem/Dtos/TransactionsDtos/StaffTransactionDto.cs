using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Core.Enums;

namespace RequestsManagementSystem.Dtos.TransactionsDtos
{
    public class StaffTransactionDto
    {
        public int TransactionId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string DueDate { get; set; } = string.Empty;
        public int SendDays { get; set; }= 0;
        public bool Seen { get; set; } = false;
    }
}
