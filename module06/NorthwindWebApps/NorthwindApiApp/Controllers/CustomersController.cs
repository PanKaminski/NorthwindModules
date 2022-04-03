﻿using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpGet("{name}")]
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

    }
}
