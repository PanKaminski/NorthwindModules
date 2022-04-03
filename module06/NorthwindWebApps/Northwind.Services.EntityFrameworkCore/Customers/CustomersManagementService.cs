using System;
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
        public async Task<(bool, Customer)> TryGetCustomerByFullName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return (false, new Customer());
            }

            var employee = await this.dbContext.Customers.FirstOrDefaultAsync(e => name == e.ContactName);


            return (employee is not null, this.mapper.Map<Customer>(employee));
        }
    }
}
