using System.Threading.Tasks;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Customers;

namespace NorthwindApp.FrontEnd.Mvc.Services.Interfaces
{
    public interface ICustomersApiClient
    {
        Task<(CustomerResponseViewModel, bool)> GetCustomerAsync(string customerId);
    }
}
