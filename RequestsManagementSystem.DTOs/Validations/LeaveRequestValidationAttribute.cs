using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.DTOs.api.TransactionsDtos;
using System.ComponentModel.DataAnnotations;

namespace RequestsManagementSystem.DTOs.Validations
{
    public class LeaveRequestValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is CreateTransactionDto dto)
            {
                if (dto.StartDate > dto.EndDate)
                    return new ValidationResult("لا يمكن تسجيل بطلب تاريخ البداية قبل تاريخ النهاية.");

                if (dto.Title == "Leave")
                {
                    if (dto.StartDate.Date < DateTime.Now.Date)
                    {
                        return new ValidationResult("برجاء إدخال تاريخ بداية الإجازة بشكل صحيح.");
                    }

                    var days = (dto.EndDate - dto.StartDate).Days;

                    if (dto.Type == "RegularLeave" && dto.StartDate.Date < DateTime.Today.Date.AddDays(2))
                    {
                        return new ValidationResult("يجب تقديم طلب الإجازة قبل يومين على الأقل من تاريخ الإجازة.");
                    }

                    if (dto.Type == "CasualLeave" && days > 2)
                    {
                        return new ValidationResult("الإجازة العارضه لا تتجاوز يومين.");
                    }

                    if (dto.Type == "RegularLeave" && days > 16)
                    {
                        return new ValidationResult("الإجازة الاعتيادية لا تتجاوز 16 يومًا.");
                    }
                }
                else
                {
                    return new ValidationResult("لا يمكن تحديد عنوان المعاملة.");
                }

                return ValidationResult.Success!;
            }

            return new ValidationResult("القيمة المدخلة غير صالحة.");
        }
    }
}