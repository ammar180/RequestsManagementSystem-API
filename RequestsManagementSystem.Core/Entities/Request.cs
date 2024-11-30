using RequestsManagementSystem.Core.Enums;

namespace RequestsManagementSystem.Core.Entities
{
    public class Request
	{
		public int RequestId { get; set; }
		public ERequestTitle Title { get; set; }
		public ERequestType Type { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int SubstituteEmployeeId { get; set; }
		public List<string> Itinerary { get; set; }
		public string RespondMessage { get; set; }
		public ERequestStatus? Status { get; set; }
		public int EmployeeId { get; set; }
		public Employee Employee { get; set; } = default!;
	}
}
