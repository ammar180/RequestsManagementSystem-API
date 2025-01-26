using RequestsManagementSystem.Dtos.EmployeeDtos;

namespace RequestsManagementSystem.Dtos.TransactionsDtos
{
    public class TransactionDto
    {
        public int TransactionId { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public List<string>? Itinerary { get; set; } = default!;
        public DateTime? RespondDate { get; set; } = null;
        public string RespondMessage { get; set; } = string.Empty;
        public string Status { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public EmployeeIdAndNameDto Employee { get; set; }

        public EmployeeIdAndNameDto SubstituteEmployee{ get; set; }
        public string TakenDays { get; set; }
        public string SeenStatus { get; set; }
    }
}