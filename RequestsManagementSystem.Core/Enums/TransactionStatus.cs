using System.ComponentModel;

namespace RequestsManagementSystem.Core.Enums
{
    public enum TransactionStatus
	{
		[Description("قيد الانتظار")]
        Pending,
        [Description("مقبول")]
        Approved,
        [Description("مرفوض")]
        Rejected
	}
}