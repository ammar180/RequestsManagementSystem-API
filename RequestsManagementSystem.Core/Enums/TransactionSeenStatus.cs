namespace RequestsManagementSystem.Core.Enums
{
	[Flags]
	public enum TransactionSeenStatus
	{
		None,
		EmployeeSeen,
		ManagerSeen,
		BothSeen = EmployeeSeen | ManagerSeen
	}

}
