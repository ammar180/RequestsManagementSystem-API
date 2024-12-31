using System.ComponentModel;

namespace RequestsManagementSystem.Core.Enums
{
    public enum TransactionStatus
	{
		[Description("معلق")]
        Pending,
        [Description("مقبول")]
        Approved,
        [Description("مرفوض")]
        Rejected,
        [Description("معدل")]
        Edited,
	}
}