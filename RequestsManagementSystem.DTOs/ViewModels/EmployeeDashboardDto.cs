using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestsManagementSystem.DTOs.ViewModels
{
    public class EmployeeDashboardDto
    {
            public string Code { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public double RegularBalance { get; set; }
            public double CausalBalance { get; set; }

    }
}
