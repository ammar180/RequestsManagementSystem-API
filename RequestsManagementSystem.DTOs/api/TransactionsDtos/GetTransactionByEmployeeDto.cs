namespace RequestsManagementSystem.DTOs.api.TransactionsDtos
{
    public class GetTransactionByEmployeeDto
    {
		public int TransactionId { get; set; }
		public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string DueDate { get; set; } = string.Empty;
        public string TakenDays { get; set; } = string.Empty;
        public string SendDate { get; set; } = string.Empty;
	}
}
