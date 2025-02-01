using RequestsManagementSystem.Dtos.EmployeeDtos;

namespace RequestsManagementSystem.Services
{
    public interface IJWTService
    {
        bool IsTokenExpired(string token);
        string GenerateJwtToken(EmployeePayLoad employee, bool isRefreshToken = false);
        EmployeePayLoad GetEmployeePayloadFromToken(string token);
    }
}
