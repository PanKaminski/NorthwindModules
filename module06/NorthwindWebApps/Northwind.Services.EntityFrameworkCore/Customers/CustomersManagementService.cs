using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Customers;
using Northwind.Services.EntityFrameworkCore.Context;

namespace Northwind.Services.EntityFrameworkCore.Customers
{
    /// <summary>
    /// Represents a customers management service.
    /// </summary>
    public class CustomersManagementService : ICustomersManagementService
    {
        private readonly NorthwindContext dbContext;

        private readonly IMapper mapper;

        /// <summary>
        /// Creates new instance of <see cref="CustomersManagementService"/>.
        /// </summary>
        /// <param name="dbContext">Database context.</param>
        /// <param name="mapper">Customers mapper.</param>
        public CustomersManagementService(NorthwindContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<bool> DoesExist(string customerId) => await this.dbContext.Customers.FindAsync(customerId) is not null;

        /// <inheritdoc/>
        public IAsyncEnumerable<Customer> GetCustomersAsync()
        {
            return this.dbContext.Customers.Select(e => this.mapper.Map<Customer>(e)).AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<Customer> GetCustomersAsync(int offset, int limit)
        {
            return this.dbContext.Customers.Skip(offset).Take(limit).Select(e => this.mapper.Map<Customer>(e)).AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public async Task<(bool, Customer)> TryGetCustomerAsync(string customerId)
        {
            var customer = await this.dbContext.Customers
                .SingleOrDefaultAsync(c => c.CustomerId == customerId);

            return (customer != null, this.mapper.Map<Customer>(customer));
        }

        /// <inheritdoc/>
        public async Task<string> CreateCustomerAsync(Customer employee)
        {
            if (employee is null)
            {
                return string.Empty;
            }

            var customerEntity = this.mapper.Map<Entities.Customer>(employee);

            if (await this.dbContext.Customers.ContainsAsync(customerEntity))
            {
                return string.Empty;
            }

            await this.dbContext.Customers.AddAsync(customerEntity);
            await this.dbContext.SaveChangesAsync();

            return customerEntity.CustomerId;
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyCustomerAsync(string customerId)
        {
            var customer = await this.dbContext.Customers.FindAsync(customerId);

            if (customer is null)
            {
                return false;
            }

            this.dbContext.Customers.Remove(customer);
            await this.dbContext.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateCustomerAsync(string customerId, Customer customer)
        {
            if (customer is null)
            {
                return false;
            }

            var customerToChange = await this.dbContext.Customers.SingleAsync(c => c.CustomerId == customerId);

            if (customerToChange is null)
            {
                return false;
            }

            Map(customerToChange, customer);

            await this.dbContext.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<(bool, Customer)> TryGetCustomerByFullName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return (false, new Customer());
            }

            var employee = await this.dbContext.Customers.FirstOrDefaultAsync(e => name == e.ContactName);


            return (employee is not null, this.mapper.Map<Customer>(employee));
        }

        private static void Map(Entities.Customer productToChange, Customer source)
        {
            productToChange.ContactName = source.ContactName;
            productToChange.Address = source.Address;
            productToChange.City = source.City;
            productToChange.Country = source.Country;
            productToChange.PostalCode = source.PostalCode;
            productToChange.Region = source.Region;
            productToChange.ContactTitle = source.ContactTitle;
            productToChange.CompanyName = source.CompanyName;
            productToChange.Fax = source.Fax;
            productToChange.Phone = source.Phone;

        }
    }
}
