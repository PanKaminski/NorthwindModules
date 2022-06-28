using System.Collections.Generic;
using System.Threading.Tasks;

namespace Northwind.Services.Employees
{
    /// <summary>
    /// Represents a management service for employees.
    /// </summary>
    public interface IEmployeeManagementService
    {
        /// <summary>
        /// Gets the collection of employees.
        /// </summary>
        /// <returns>All employees.</returns>
        IAsyncEnumerable<Employee> GetEmployeesAsync();

        /// <summary>
        /// Gets the collection of employees.
        /// </summary>
        /// <param name="offset">An offset of the first element to return.</param>
        /// <param name="limit">A limit of elements to return.</param>
        /// <returns>Collection of limit count of employees starting with offset position.</returns>
        IAsyncEnumerable<Employee> GetEmployeesAsync(int offset, int limit);

        /// <summary>
        /// Get employee by id.
        /// </summary>
        /// <param name="employeeId">Employee id.</param>
        /// <returns>True and found employee, if employee with such id exists, otherwise false with null.</returns>
        Task<(bool, Employee)> TryGetEmployeeAsync(int employeeId);

        /// <summary>
        /// Creates employee.
        /// </summary>
        /// <param name="employee">Employee to add into storage.</param>
        /// <returns>Id of the created employee.</returns>
        Task<int> CreateEmployeeAsync(Employee employee);

        /// <summary>
        /// Removes employee from storage.
        /// </summary>
        /// <param name="employeeId">Employee id.</param>
        /// <returns>True, if the employee was successfully removed, otherwise, false.</returns>
        Task<bool> DestroyEmployeeAsync(int employeeId);

        /// <summary>
        /// Updates employee.
        /// </summary>
        /// <param name="employeeId">Employee id.</param>
        /// <param name="employee">New values.</param>
        /// <returns>True, if the employee was successfully updated, otherwise, false.</returns>
        Task<bool> UpdateEmployeeAsync(int employeeId, Employee employee);
    }
}
