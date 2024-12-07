using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Core.Enums;

namespace RequestsManagementSystem.Dtos.TransactionsDtos
{
    public class CreateTransactionDto
    {
        public int TransactionId { get; set; }
        public TransactionTitle Title { get; set; }
        public TransactionType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SubstituteEmployeeId { get; set; }
        public List<string> Itinerary { get; set; }
        public int EmployeeId { get; set; }
    }
}
