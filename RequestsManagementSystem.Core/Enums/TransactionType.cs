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
        // mission reques types
        [Description("يوم كامل")]
        FullDay,
        [Description("يوم جزئي")]
        PartialDay,
	}
}