using System.Collections.Generic;
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
        /// Gets the collection of customers.
        /// </summary>
        /// <returns>All customers.</returns>
        IAsyncEnumerable<Customer> GetCustomersAsync();

        /// <summary>
        /// Gets the collection of customers.
        /// </summary>
        /// <param name="offset">An offset of the first element to return.</param>
        /// <param name="limit">A limit of elements to return.</param>
        /// <returns>Collection of limit count of customers starting with offset position.</returns>
        IAsyncEnumerable<Customer> GetCustomersAsync(int offset, int limit);

        /// <summary>
        /// Get customer by id.
        /// </summary>
        /// <param name="customerId">Customer id.</param>
        /// <returns>True and found customer, if customer with such id exists, otherwise false with null.</returns>
        Task<(bool, Customer)> TryGetCustomerAsync(string customerId);

        /// <summary>
        /// Creates customer.
        /// </summary>
        /// <param name="employee">Customer to add into storage.</param>
        /// <returns>Id of the created customer.</returns>
        Task<string> CreateCustomerAsync(Customer employee);

        /// <summary>
        /// Removes customer from storage.
        /// </summary>
        /// <param name="customerId">Customer id.</param>
        /// <returns>True, if the customer was successfully removed, otherwise, false.</returns>
        Task<bool> DestroyCustomerAsync(string customerId);

        /// <summary>
        /// Updates customer.
        /// </summary>
        /// <param name="customerId">Customer id.</param>
        /// <param name="customer">New values.</param>
        /// <returns>True, if the customer was successfully updated, otherwise, false.</returns>
        Task<bool> UpdateCustomerAsync(string customerId, Customer customer);

        /// <summary>
        /// Receives customer by name.
        /// </summary>
        /// <param name="name">Customer name.</param>
        /// <returns>True with customer, if customer was found, otherwise false.</returns>
        Task<(bool, Customer)> TryGetCustomerByFullName(string name);
    }
}
