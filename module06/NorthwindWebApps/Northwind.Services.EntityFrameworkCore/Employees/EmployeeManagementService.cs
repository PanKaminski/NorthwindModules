using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Employees;
using Northwind.Services.EntityFrameworkCore.Context;

namespace Northwind.Services.EntityFrameworkCore.Employees
{
    /// <summary>
    /// Represents a management service for employees.
    /// </summary>
    public class EmployeeManagementService : IEmployeeManagementService
    {
        private readonly NorthwindContext dbContext;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeManagementService"/> class.
        /// </summary>
        /// <param name="dbContext">Database context.</param>
        public EmployeeManagementService(NorthwindContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<Employee> GetEmployeesAsync()
        {
            return this.dbContext.Employees.Select(e => this.mapper.Map<Employee>(e)).AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<Employee> GetEmployeesAsync(int offset, int limit)
        {
            return this.dbContext.Employees.Skip(offset).Take(limit).Select(e => this.mapper.Map<Employee>(e)).AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public async Task<(bool, Employee)> TryGetEmployeeAsync(int employeeId)
        {
            var employee = await this.dbContext.Employees
                .SingleOrDefaultAsync(c => c.Id == employeeId);

            return (employee != null, this.mapper.Map<Employee>(employee));
        }

        /// <inheritdoc/>
        public async Task<int> CreateEmployeeAsync(Employee employee)
        {
            if (employee is null)
            {
                return -1;
            }

            var employeeEntity = this.mapper.Map<Entities.Employee>(employee);

            if (await this.dbContext.Employees.ContainsAsync(employeeEntity))
            {
                return -1;
            }

            await this.dbContext.Employees.AddAsync(employeeEntity);
            await this.dbContext.SaveChangesAsync();

            return employeeEntity.Id;
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyEmployeeAsync(int employeeId)
        {
            var employee = await this.dbContext.Employees.FindAsync(employeeId);

            if (employee is null)
            {
                return false;
            }

            this.dbContext.Employees.Remove(employee);
            await this.dbContext.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateEmployeeAsync(int employeeId, Employee employee)
        {
            if (employee is null)
            {
                return false;
            }

            var employeeToChange = await this.dbContext.Employees.SingleAsync(c => c.Id == employeeId);

            if (employeeToChange is null)
            {
                return false;
            }

            Map(employeeToChange, employee);

            await this.dbContext.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<(bool, Employee)> TryGetEmployeeByFullName(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                return (false, new Employee());
            }

            var employee = await this.dbContext.Employees.FirstOrDefaultAsync(e => firstName == e.FirstName && lastName == e.LastName);


            return (employee is not null, this.mapper.Map<Employee>(employee));
        }

        private static void Map(Entities.Employee productToChange, Employee source)
        {
            productToChange.FirstName = source.FirstName;
            productToChange.LastName = source.LastName;
            productToChange.Address = source.Address;
            productToChange.BirthDate = source.BirthDate;
            productToChange.HireDate = source.HireDate;
            productToChange.City = source.City;
            productToChange.Country = source.Country;
            productToChange.PostalCode = source.PostalCode;
            productToChange.Extension = source.Extension;
            productToChange.HomePhone = source.HomePhone;
            productToChange.Notes = source.Notes;
            productToChange.Region = source.Region;
            productToChange.ReportsTo = source.ReportsTo;
            productToChange.Photo = source.Photo;
            productToChange.PhotoPath = source.PhotoPath;
            productToChange.Title = source.Title;
            productToChange.TitleOfCourtesy = source.TitleOfCourtesy;
        }
    }
}