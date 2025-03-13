using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.DTOs.api.TransactionsDtos;
using System.ComponentModel.DataAnnotations;

namespace RequestsManagementSystem.DTOs.Validations
{
    public class MissionRequestValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is CreateTransactionDto dto && Enum.TryParse(dto.Title, true, out TransactionTitle title))
            {
                if (title == TransactionTitle.Mission)
                {
                    var today = DateTime.Today;

                    // Mission date constraints
                    if (dto.StartDate.Date < today.AddDays(-2))
                    {
                        return new ValidationResult("يجب تقديم طلب المهمة خلال يومين قبل أو بعد تاريخ بدء المهمة.");
                    }

                    if (dto.StartDate.Day >= 20 || dto.EndDate.Day >= 20)
                    {
                        return new ValidationResult("لا يمكن تقديم طلب المهمة بعد يوم 20 لمهام بدأت قبل يوم 20.");
                    }
                    if (dto.Itinerary == null || dto.Itinerary.Count == 0)
                    {
                        return new ValidationResult("برجاء تقديم خط السير، وجهه واحدة على الاقل.");
                    }
                }
            }
            return ValidationResult.Success!;
        }
    }
}