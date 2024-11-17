using RequestsManagementSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestsManagementSystem.Core.Interfaces
{
    public interface IEmployeeService
    {
        Task<string> LoginAsync(LoginEmployeeDto loginEmployeeDto);
    }
}
