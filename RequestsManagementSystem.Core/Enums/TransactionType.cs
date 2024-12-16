using System.ComponentModel;

namespace RequestsManagementSystem.Core.Enums
{
	public enum TransactionType
	{
        // leave reques types
        [Description("عارضة")]
        CasualLeave,
        [Description("اعتيادية")]
        RegularLeave,
        [Description("غياب بأذن")]
        ExcusedAbsent,
        [Description("غياب بدون بأذن")]
        UnexcusedAbsent,
        [Description("نصف يوم")]
        HalfDay,
        [Description("ربع يوم")]
        QuarterDay,
        // mission reques types
        [Description("يوم كامل")]
        FullDay,
        [Description("يوم جزئي")]
        PartialDay,
	}
}