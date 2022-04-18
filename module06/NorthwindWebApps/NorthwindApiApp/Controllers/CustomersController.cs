using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Customers;

namespace NorthwindApiApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomersManagementService customersService;

        public CustomersController(ICustomersManagementService customersService)
        {
            this.customersService = customersService ?? throw new ArgumentNullException(nameof(customersService));
        }

        [HttpGet("name/{name}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Customer>> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return this.BadRequest();
            }

            var customer = await this.customersService.TryGetCustomerByFullName(name);

            if (customer.Item1)
            {
                return customer.Item2;
            }

            return this.NotFound();
        }

        [HttpGet("{offset:int}/{limit:int}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public IAsyncEnumerable<Customer> GetAsync(int offset, int limit) =>
            this.customersService.GetCustomersAsync(offset, limit);

        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public IAsyncEnumerable<Customer> GetAsync() => this.customersService.GetCustomersAsync();

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Customer>> GetAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return this.BadRequest();
            }

            var customer = await this.customersService.TryGetCustomerAsync(id);

            if (customer.Item1)
            {
                return customer.Item2;
            }

            return this.NotFound();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateAsync(Customer employee)
        {
            try
            {
                var customerId = await this.customersService.CreateCustomerAsync(employee);

                if (string.IsNullOrEmpty(customerId))
                {
                    return this.BadRequest();
                }

                return this.Ok(customerId);

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
        public async Task<ActionResult> UpdateAsync(string id, Customer customer)
        {
            if (customer is null || string.IsNullOrWhiteSpace(id))
            {
                return this.BadRequest();
            }

            if (await this.customersService.UpdateCustomerAsync(id, customer))
            {
                return this.NoContent();
            }

            return this.NotFound();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return this.BadRequest();
            }

            if (await this.customersService.DestroyCustomerAsync(id))
            {
                return this.NoContent();
            }

            return this.NotFound();
        }

    }
}
