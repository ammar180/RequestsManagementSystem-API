using RequestsManagementSystem.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RequestsManagementSystem.Core.Entities
{
    public class Transaction
	{
		[Key]
		public int TransactionId { get; set; }
		public TransactionTitle Title { get; set; }
		public TransactionType Type { get; set; }
		public DateTime StartDate { get; set; } 
		public DateTime EndDate { get; set; }
		public int SubstituteEmployeeId { get; set; }
        public List<string> Itinerary { get; set; } = default!;
		public DateTime? RespondDate { get; set; } = null;
        public string RespondMessage { get; set; } = string.Empty;
		public TransactionStatus Status { get; set; } = 0;
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public int EmployeeId { get; set; }
		[ForeignKey("EmployeeId")]
		public Employee Employee { get; set; } = default!;
		public TransactionSeenStatus SeenStatus { get; set; } = 0;
	}
}
