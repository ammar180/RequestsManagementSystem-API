using RequestsManagementSystem.DTOs.api.EmployeeDtos;

namespace RequestsManagementSystem.Core.Interfaces.IServices
{
    public interface IJWTService
    {
        bool IsTokenExpired(string token);
        string GenerateJwtToken(EmployeePayLoad employee, bool isRefreshToken = false);
        EmployeePayLoad GetEmployeePayloadFromToken(string token);
    }
}
