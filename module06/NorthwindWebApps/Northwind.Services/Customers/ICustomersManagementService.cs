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
    }
}
