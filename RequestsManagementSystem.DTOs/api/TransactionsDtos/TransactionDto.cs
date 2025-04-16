using RequestsManagementSystem.DTOs.api.EmployeeDtos;

namespace RequestsManagementSystem.DTOs.api.TransactionsDtos
{
    public class TransactionDto
    {
        public int TransactionId { get; set; }
        public string Type { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public List<string>? Itinerary { get; set; } = default!;
        public string RespondDate { get; set; } = string.Empty;
        public string RespondMessage { get; set; } = string.Empty;
        public string Status { get; set; }
        public string CreationDate { get; set; } = string.Empty;

        public EmployeeIdAndNameDto Employee { get; set; }

        public EmployeeIdAndNameDto SubstituteEmployee{ get; set; }
        public string TakenDays { get; set; }
        public string SeenStatus { get; set; }
    }
}