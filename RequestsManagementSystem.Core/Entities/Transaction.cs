using RequestsManagementSystem.Core.Enums;

namespace RequestsManagementSystem.Core.Entities
{
    public class Transaction
	{
		public int TransactionId { get; set; }
		public TransactionTitle Title { get; set; }
		public TransactionType Type { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int SubstituteEmployeeId { get; set; }
		public string Itinerary { get; set; } = default!;
		public string RespondMessage { get; set; } = string.Empty;
		public TransactionStatus? Status { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public int EmployeeId { get; set; }
		public Employee Employee { get; set; } = default!;
	}
}
