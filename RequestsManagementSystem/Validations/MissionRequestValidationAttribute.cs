using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Dtos.TransactionsDtos;
using System.ComponentModel.DataAnnotations;

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
            }
        }
        return ValidationResult.Success!;
    }
}
