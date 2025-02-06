using System.ComponentModel.DataAnnotations;

namespace RequestsManagementSystem.Dtos.TransactionsDtos
{
    [MissionRequestValidation]
    [LeaveRequestValidation]
    public class UpdateTransactionDto
    {
        public string? Title { get; set; }
        public string? Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SubstituteEmployeeId { get; set; }
        public List<string>? Itinerary { get; set; }
    }
}
