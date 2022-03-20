using System;
using System.Threading.Tasks;
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

        /// <summary>
        /// Creates new instance of <see cref="CustomersManagementService"/>.
        /// </summary>
        /// <param name="dbContext">Database context.</param>
        public CustomersManagementService(NorthwindContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <inheritdoc/>
        public async Task<bool> DoesExist(string customerId) => await this.dbContext.Customers.FindAsync(customerId) is not null;
    }
}
