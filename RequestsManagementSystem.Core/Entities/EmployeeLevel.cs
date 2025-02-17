namespace RequestsManagementSystem.Core.Entities
{
    public class EmployeeLevel
    {
        public int Id { get; set; }
        public string LevelName { get; set; } = string.Empty;
        public string LevelDescription { get; set; } = string.Empty;
        public int RegularLeaveperYear { get; set; }
        public double RegularLeaveperMonth { get => RegularLeaveperYear / 12.0; }
        public int CasualLeavePerYear { get; set; }
        public double CasualLeavePerMonth { get => CasualLeavePerYear / 12.0; }
        public int OrderId { get; set; }
    }
}
