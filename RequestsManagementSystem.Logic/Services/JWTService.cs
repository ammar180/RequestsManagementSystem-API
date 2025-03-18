using Microsoft.IdentityModel.Tokens;
using RequestsManagementSystem.DTOs.api.EmployeeDtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace RequestsManagementSystem.Logic.Services
{
    public class JWTService : IJWTService
    {
        private readonly IConfiguration _configuration;
        public JWTService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public bool IsTokenExpired(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero // Optional: Set to zero to remove delay of token expiration
                }, out SecurityToken validatedToken);
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var expClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "exp");

                if (expClaim != null)
                {
                    var expDateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim.Value)).UtcDateTime;
                    if(expDateTime <= DateTime.UtcNow)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    throw new Exception("Expiration claim not found");
                }
            }
            catch (Exception ex)
            {
                return true;
            }
        }
        public string GenerateJwtToken(EmployeePayLoad employee, bool isRefreshToken = false)
        {
            var claims = new List<Claim>
            {
                new(nameof(EmployeePayLoad.EmployeeId), employee.EmployeeId.ToString()),
                new(nameof(EmployeePayLoad.EmployeeName), employee.EmployeeName),
                new(nameof(EmployeePayLoad.EmployeeRole), employee.EmployeeRole),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: isRefreshToken ? DateTime.Now.AddDays(double.Parse(_configuration["Jwt:refreshExpiresInDays"]!)) : DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpiresInMinutes"]!)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public EmployeePayLoad GetEmployeePayloadFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
                // Read the token
            var jwtToken = tokenHandler.ReadJwtToken(token);

                // Extract claims
            var claims = jwtToken.Claims;
            return new EmployeePayLoad
                {
                    EmployeeId = int.Parse(claims.First(x => x.Type == nameof(EmployeePayLoad.EmployeeId)).Value),
                    EmployeeName = claims.First(x => x.Type == nameof(EmployeePayLoad.EmployeeName)).Value,
                    EmployeeRole = claims.First(x => x.Type == nameof(EmployeePayLoad.EmployeeRole)).Value
                };
        }
    }
}
