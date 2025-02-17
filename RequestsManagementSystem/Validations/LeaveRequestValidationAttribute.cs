using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Core.Enums;
using RequestsManagementSystem.Dtos.TransactionsDtos;
using System.ComponentModel.DataAnnotations;

public class LeaveRequestValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is CreateTransactionDto dto)
        {
            if (!Enum.TryParse(dto.Title, true, out TransactionTitle title))
                return new ValidationResult("لا يمكن تحديد عنوان المعاملة.");

            // Check if Type is a valid enum
            if (!Enum.TryParse(dto.Type, true, out ETransactionType type))
                type = ETransactionType.Other;

            if (dto.StartDate > dto.EndDate)
                return new ValidationResult("لا يمكن تسجيل بطلب تاريخ البداية قبل تاريخ النهاية.");

            if (title == TransactionTitle.Leave)
            {
                if (dto.StartDate.Date < DateTime.Now.Date)
                {
                    return new ValidationResult("برجاء إدخال تاريخ بداية الإجازة بشكل صحيح.");
                }

                var days = (dto.EndDate - dto.StartDate).Days;

                if (type == ETransactionType.RegularLeave && dto.StartDate.Date < DateTime.Today.Date.AddDays(2))
                {
                    return new ValidationResult("يجب تقديم طلب الإجازة قبل يومين على الأقل من تاريخ الإجازة.");
                }

                if (type == ETransactionType.CasualLeave && days > 2)
                {
                    return new ValidationResult("الإجازة العارضه لا تتجاوز يومين.");
                }

                if (type == ETransactionType.RegularLeave && days > 16)
                {
                    return new ValidationResult("الإجازة الاعتيادية لا تتجاوز 16 يومًا.");
                }
            }

            return ValidationResult.Success!;
        }

        return new ValidationResult("القيمة المدخلة غير صالحة.");
    }
}
