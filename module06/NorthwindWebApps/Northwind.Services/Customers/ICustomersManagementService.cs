using System.Threading.Tasks;

namespace Northwind.Services.Customers
{
    /// <summary>
    /// Provides methods for managing Northwind customers.
    /// </summary>
    public interface ICustomersManagementService
    {
        /// <summary>
        /// Determines, if the customer with specified id exists.
        /// </summary>
        /// <param name="customerId">Customer id.</param>
        /// <returns>True, if the customer exists in storage, otherwise, false.</returns>
        Task<bool> DoesExist(string customerId);

        /// <summary>
        /// Receives customer by name.
        /// </summary>
        /// <param name="name">Customer name.</param>
        /// <returns>True with customer, if customer was found, otherwise false.</returns>
        Task<(bool, Customer)> TryGetCustomerByFullName(string name);
    }
}
