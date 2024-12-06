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
    public async Task<Employee?> GetEmployeeById(int id)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(e => e.EmployeeId == id);
    }
    public async Task<Employee?> GetEmployeeByIdWithTransaction(int id)
    {
        return await _context.Employees
            .Include(e => e.Transactions)
            .FirstOrDefaultAsync(e => e.EmployeeId == id);
    }

    // Get all employees
    public async Task<IEnumerable<Employee>> GetEmployes()
    {
        return await _context.Employees
            .Include(e => e.Manager) // Include manager information
            .ToListAsync();
    }

    // Update an employee
    public async Task<bool> UpdateAsync(Employee employee)
    {
        try
        {
            var existingEmployee = await _context.Employees.FindAsync(employee.EmployeeId);
            if (existingEmployee == null) return false;

            // Update fields
            existingEmployee.Name = employee.Name;
            existingEmployee.Password = employee.Password;
            existingEmployee.VacationsBalance = employee.VacationsBalance;
            existingEmployee.DateOfEmployment = employee.DateOfEmployment;
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
