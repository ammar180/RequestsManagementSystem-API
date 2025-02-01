namespace RequestsManagementSystem.Core.Entities
{
    public class EmployeeLevel
    {
        public int Id { get; set; }
        public string LevelName { get; set; }
        public string LevelDescription { get; set; }
        public float RegularLeaveperMonth { get; set; }
        public float CasualLeavePerMonth { get; set; }
        public int RegularLeaveperYear { get; set; }
        public int CasualLeavePerYear { get; set; }
        public int OrderId { get; set; }
    }
}
