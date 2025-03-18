using RequestsManagementSystem.DTOs.Validations;
using System.ComponentModel.DataAnnotations;

namespace RequestsManagementSystem.DTOs.api.TransactionsDtos
{
    [MissionRequestValidation]
    [LeaveRequestValidation]
    public class CreateTransactionDto
    {
        [Required(ErrorMessage = "The title field is requird")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "The type field is requird")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "The start date field is requird")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "The end date field is requird")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "The Substitute Employee Id field is requird")]
        public int SubstituteEmployeeId { get; set; }


        public List<string>? Itinerary { get; set; }
        public int EmployeeId { get; set; }
    }
}
