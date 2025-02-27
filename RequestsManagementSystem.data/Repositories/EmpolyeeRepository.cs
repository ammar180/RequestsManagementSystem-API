using Microsoft.EntityFrameworkCore;
using RequestsManagementSystem.Core.Entities;
using RequestsManagementSystem.Core.Interfaces;
using RequestsManagementSystem.Data;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Add a new employee
    public async Task<bool> AddAsync(Employee employee)
    {
        try
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Delete an employee by ID
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return false;

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Get an employee by ID
    public async Task<Employee?> GetEmployeeByCode(string code, string[]? includes = null)
    {
        IQueryable<Employee> query = _context.Employees;

        if (includes != null)
        {
            foreach (var navigation in includes)
            {
                query = query.Include(navigation);
            }
        }

        return await query.FirstOrDefaultAsync(e => e.Code == code);
    }
    // Get an employee by Id
    public async Task<Employee?> GetEmployeeById(int id, string[]? includes = null)
    {
        IQueryable<Employee> query = _context.Employees;

        if (includes != null)
        {
            foreach (var navigation in includes)
            {
                query = query.Include(navigation);
            }
        }

        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }
    //Get Employee by Department name
    public async Task<IEnumerable<Employee>> GetEmployesByDepartment(string Department)
    {
       return await _context.Employees
            .Where(x=>x.DepartmentName == Department).ToListAsync();
    }

    public async Task<IEnumerable<Employee>> GetEmployesIncludeTransactionAsync()
    {
        return await _context.Employees.Include(x=>x.Transactions).Include(x=>x.EmployeeLevel).ToListAsync();
    }

    // Update an employee
    public async Task<bool> UpdateAsync(Employee employee)
    {
        try
        {
            var existingEmployee = await _context.Employees.FindAsync(employee.Id);
            if (existingEmployee == null) return false;

            // Update fields
            existingEmployee.Name = employee.Name;
            existingEmployee.Password = employee.Password;
            existingEmployee.DateOfEmployment = employee.DateOfEmployment;
            existingEmployee.EmployeeRole = employee.EmployeeRole;
            existingEmployee.DepartmentName = employee.DepartmentName;
            existingEmployee.ManagerId = employee.ManagerId;

            _context.Employees.Update(existingEmployee);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
