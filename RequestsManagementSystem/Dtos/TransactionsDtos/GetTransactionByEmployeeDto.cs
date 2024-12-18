namespace RequestsManagementSystem.Dtos.TransactionsDtos
{
    public class GetTransactionByEmployeeDto
    {
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string DueDate { get; set; } = string.Empty;
        public DateTime ?ResponseDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
