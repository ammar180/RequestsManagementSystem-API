namespace RequestsManagementSystem.DTOs.api.TransactionsDtos
{
    public class StaffTransactionDto : GetTransactionByEmployeeDto
    {
        public string EmployeeName { get; set; } = string.Empty;
        public bool Seen { get; set; } = false;
    }
}
