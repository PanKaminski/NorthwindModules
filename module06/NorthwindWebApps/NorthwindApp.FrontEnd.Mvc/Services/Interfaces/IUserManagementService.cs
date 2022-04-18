using System.Security.Claims;
using System.Threading.Tasks;
using NorthwindApp.FrontEnd.Mvc.ViewModels;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Account;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Employees;

namespace NorthwindApp.FrontEnd.Mvc.Services.Interfaces
{
    public interface IUserManagementService
    {
        Task<PageListViewModel<UserResponseViewModel>> GetUsers(int page, int pageSize);

        Task<UserResponseViewModel> Get(int userId);

        Task<bool> RegisterAsync(RegisterViewModel registerModel);

        Task<bool> VerifyCredentialsAsync(LoginViewModel loginModel);

        Task<ClaimsIdentity> GenerateClaims(string email);

        Task<int> CreateEmployee(int userId, EmployeeInputViewModel employeeModel);
    }
}
