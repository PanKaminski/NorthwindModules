using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Northwind.Services.Employees;
using NorthwindApp.FrontEnd.Mvc.ViewModels;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Account;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Customers;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Employees;

namespace NorthwindApp.FrontEnd.Mvc.Services.Interfaces
{
    public interface IUserManagementService
    {
        Task<PageListViewModel<UserResponseViewModel>> GetUsers(int page, int pageSize);

        Task<IEnumerable<string>> GetEmployeeNames();

        Task<UserResponseViewModel> GetUserAsync(int userId);

        Task<bool> RegisterAsync(RegisterViewModel registerModel);

        Task<bool> VerifyCredentialsAsync(LoginViewModel loginModel);

        Task<ClaimsIdentity> GenerateClaimsAsync(string email);

        Task<int> CreateEmployeeAsync(int userId, EmployeeInputViewModel employeeModel);

        Task<int> GetNorthwindEmployeeIdAsync(string email);

        Task<(string, bool)> GetNorthwindCustomerIdAsync(string email);

        Task<(int, string)> CreateCustomerAsync(CustomerInputViewModel customerModel, string email);
    }
}
