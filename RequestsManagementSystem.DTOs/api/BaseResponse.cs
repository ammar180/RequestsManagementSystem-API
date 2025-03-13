namespace RequestsManagementSystem.DTOs.api
{
    public class BaseResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
