using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Core.Enums;
using RequestsManagementSystem.Dtos.TransactionsDtos;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

public class LeaveRequestValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is CreateTransactionDto dto)
        {
            if (!Enum.TryParse(dto.Title, true, out TransactionTitle title))
                return new ValidationResult("لا يمكن تحديد عنوان المعاملة.");

            // Check if Type is a valid enum
            if (!Enum.TryParse(dto.Type, true, out TransactionType type))
                return new ValidationResult("لا يمكن تحديد نوع المعاملة.");

            if (dto.StartDate > dto.EndDate)
                return new ValidationResult("لا يمكن تسجيل بطلب تاريخ البداية قبل تاريخ النهاية.");

            if (title == TransactionTitle.Leave)
            {
                if (dto.StartDate.Date < DateTime.Now.Date)
                {
                    return new ValidationResult("برجاء إدخال تاريخ بداية الإجازة بشكل صحيح.");
                }

                var days = (dto.EndDate - dto.StartDate).Days;

                if (type == TransactionType.RegularLeave && dto.StartDate.Date < DateTime.Today.Date.AddDays(2))
                {
                    return new ValidationResult("يجب تقديم طلب الإجازة قبل يومين على الأقل من تاريخ الإجازة.");
                }

                if (type == TransactionType.CasualLeave && days > 2)
                {
                    return new ValidationResult("الإجازة العارضة لا تتجاوز يومين.");
                }

                if (type == TransactionType.RegularLeave && days > 16)
                {
                    return new ValidationResult("الإجازة الاعتيادية لا تتجاوز 16 يومًا.");
                }
            }

            return ValidationResult.Success!;
        }

        return new ValidationResult("القيمة المدخلة غير صالحة.");
    }
}
