using System.ComponentModel;
namespace RequestsManagementSystem.Core.Enums
{
	public enum ETransactionType
	{
        // leave reques types
        [Description("عارضه")]
        CasualLeave = 1,
        [Description("اعتيادية")]
        RegularLeave,
        [Description("نصف يوم")]
        HalfDay,
        [Description("ربع يوم")]
        QuarterDay,
        [Description("رصيد اعتيادي إضافي")]
        AdditionalRegularLeave,
        [Description("رصيد عارضه إضافي")]
        AdditionalCasualLeave,
        [Description("غياب بأذن")]
        ExcusedAbsent,
        [Description("غياب بدون بأذن")]
        UnexcusedAbsent,
        // mission reques types
        [Description("يوم كامل")]
        FullDay,
        [Description("يوم جزئي")]
        PartialDay,

        Other = 0,
    }
}