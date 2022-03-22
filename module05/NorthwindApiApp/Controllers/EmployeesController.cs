using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Employees;

namespace NorthwindApiApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeManagementService employeeService;

        public EmployeesController(IEmployeeManagementService productCategoryService)
        {
            this.employeeService = productCategoryService ?? throw new ArgumentNullException(nameof(productCategoryService));
        }

        [HttpGet("{offset:int}/{limit:int}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public IAsyncEnumerable<Employee> GetAsync(int offset, int limit) =>
            this.employeeService.GetEmployeesAsync(offset, limit);

        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public IAsyncEnumerable<Employee> GetAsync() => this.employeeService.GetEmployeesAsync();

        [HttpGet ("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Employee>> GetAsync(int id)
        {
            if (id < 1)
            {
                return this.BadRequest();
            }

            var employee = await this.employeeService.TryGetEmployeeAsync(id);

            if (employee.Item1)
            {
                return employee.Item2;
            }

            return this.NotFound();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateAsync(Employee employee)
        {
            try
            {
                var employeeId = await this.employeeService.CreateEmployeeAsync(employee);

                if (employeeId < 1)
                {
                    return this.BadRequest();
                }

                return this.Ok(employeeId);

            }
            catch (InvalidOperationException e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateAsync(int id, Employee employee)
        {
            if (employee is null)
            {
                return this.BadRequest();
            }

            if (await this.employeeService.UpdateEmployeeAsync(id, employee))
            {
                return this.NoContent();
            }

            return this.NotFound();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            if (await this.employeeService.DestroyEmployeeAsync(id))
            {
                return this.NoContent();
            }

            return this.NotFound();
        }
    }
}