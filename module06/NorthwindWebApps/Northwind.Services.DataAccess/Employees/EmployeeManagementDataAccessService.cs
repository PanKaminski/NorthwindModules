using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Northwind.DataAccess;
using Northwind.DataAccess.Employees;
using Northwind.Services.Employees;
using Northwind.Services.Entities;

namespace Northwind.Services.DataAccess.Employees
{
    /// <summary>
    /// Service for maintaining employees through database.
    /// </summary>
    public class EmployeeManagementDataAccessService : IEmployeeManagementService
    {
        private readonly IEmployeeDataAccessObject employeeDao;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of <see cref="EmployeeManagementDataAccessService"/>.
        /// </summary>
        /// <param name="dataAccessFactory">Employees data access factory.</param>
        /// <param name="mapper">Mapper for mapping employees from dto.</param>
        /// <exception cref="ArgumentNullException">Throws, if dataAccessFactory is null or mapper is null.</exception>
        public EmployeeManagementDataAccessService(NorthwindDataAccessFactory dataAccessFactory, IMapper mapper)
        {
            if (dataAccessFactory is null)
            {
                throw new ArgumentNullException(nameof(dataAccessFactory));
            }

            this.employeeDao = dataAccessFactory.GetEmployeeDataAccessObject();
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<Employee> GetEmployeesAsync()
        {
            await foreach (var employeeDto in this.employeeDao.SelectEmployeesAsync())
            {
                yield return this.mapper.Map<Employee>(employeeDto);
            }
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<Employee> GetEmployeesAsync(int offset, int limit)
        {
            try
            {
                return GetAsync(offset, limit);
            }
            catch (ArgumentException)
            {
                return GetAsync(0, 0);
            }

            async IAsyncEnumerable<Employee> GetAsync(int skipped, int token)
            {
                await foreach (var employeeDto in this.employeeDao.SelectEmployeesAsync(skipped, token))
                {
                    yield return this.mapper.Map<Employee>(employeeDto);
                }
            }
        }

        /// <inheritdoc/>
        public async Task<(bool, Employee)> TryGetEmployeeAsync(int employeeId)
        {
            try
            {
                var employeeDto = await this.employeeDao.FindEmployeeAsync(employeeId);

                return (true, this.mapper.Map<Employee>(employeeDto));
            }
            catch (EmployeeNotFoundException)
            {
                return (false, null);
            }
        }

        /// <inheritdoc/>
        public async Task<int> CreateEmployeeAsync(Employee employee)
        {
            if (employee is null)
            {
                return -1;
            }

            if ((await this.TryGetEmployeeAsync(employee.Id)).Item1)
            {
                throw new InvalidOperationException("Employee with such id already exists.");
            }

            return await this.employeeDao.InsertEmployeeAsync(
                this.mapper.Map<EmployeeTransferObject>(employee));
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyEmployeeAsync(int employeeId)=>
            await this.employeeDao.DeleteEmployeeAsync(employeeId);

        /// <inheritdoc/>
        public async Task<bool> UpdateEmployeeAsync(int employeeId, Employee employee) =>
            await this.employeeDao.UpdateEmployeeAsync(this.mapper.Map<EmployeeTransferObject>(employee));
    }
}