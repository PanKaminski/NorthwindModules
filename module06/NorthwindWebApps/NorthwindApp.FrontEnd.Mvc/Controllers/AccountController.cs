using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NorthwindApp.FrontEnd.Mvc.Services.Interfaces;
using NorthwindApp.FrontEnd.Mvc.ViewModels;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Account;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Customers;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Employees;

namespace NorthwindApp.FrontEnd.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private const int PageSize = 15;
        private readonly IUserManagementService userManagementService;

        public AccountController(IUserManagementService userManagementService)
        {
            this.userManagementService = userManagementService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            var isAuthenticated = this.User?.Identity?.IsAuthenticated;

            if (isAuthenticated.HasValue && isAuthenticated.Value)
            {
                return this.RedirectToAction("Index", "Categories");
            }

            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAsync(RegisterViewModel registerModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(registerModel);
            }

            var isCreated = await this.userManagementService.RegisterAsync(registerModel);

            if (isCreated)
            {
                var id = await this.userManagementService.GenerateClaimsAsync(registerModel.Email);

                await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(id));

                return this.RedirectToAction("Index", "Categories");
            }

            this.ModelState.AddModelError("", "Invalid login or password.");

            return this.View(registerModel);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return this.View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(LoginViewModel loginModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(loginModel);
            }

            if (await this.userManagementService.VerifyCredentialsAsync(loginModel))
            {
                var id = await this.userManagementService.GenerateClaimsAsync(loginModel.Email);

                await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(id));

                return !string.IsNullOrEmpty(loginModel.ReturnUrl) && this.Url.IsLocalUrl(loginModel.ReturnUrl)
                    ? this.Redirect(loginModel.ReturnUrl) : this.RedirectToAction("Index", "Categories");
            }

            this.ModelState.AddModelError("", "Invalid login or password.");

            return this.View(loginModel);
        }

        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return this.RedirectToAction("Index", "Categories");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ViewRolesAsync(int page = 1, int pageSize = PageSize)
        {
            return this.View(await this.userManagementService.GetUsers(page, pageSize));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MakeEmployee(int userId)
        {
            this.ViewBag.userId = userId;
            var names = await this.userManagementService.GetEmployeeNames();
            this.ViewBag.employees = new SelectList(names);
            return this.View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MakeEmployee(int userId, EmployeeInputViewModel employeeModel)
        {
            if (!this.ModelState.IsValid)
            {
                this.ViewBag.userId = userId;
                var names = await this.userManagementService.GetEmployeeNames();
                this.ViewBag.employees = new SelectList(names);

                return this.View(employeeModel);
            }

            await this.userManagementService.CreateEmployeeAsync(userId, employeeModel);
            return this.RedirectToAction("ViewRoles");
        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Welcome(string returnUrl = null)
        {
            var creationCheck = await this.userManagementService.GetNorthwindCustomerIdAsync(this.User?.Identity?.Name ?? string.Empty);

            if (creationCheck.Item2)
            {
                return this.RedirectToAction("Index", "Categories");
            }

            return this.View(new CustomerInputViewModel
            {
                ReturnUrl = returnUrl,
            });
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Welcome(CustomerInputViewModel customerModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(customerModel);
            }

            var (statusCode, id) = await this.userManagementService
                .CreateCustomerAsync(customerModel, this.User?.Identity?.Name ?? string.Empty);

            if (statusCode != 200)
            {
                return this.View("Error", this.CreateErrorModel(statusCode));
            }

            return !string.IsNullOrEmpty(customerModel.ReturnUrl) && this.Url.IsLocalUrl(customerModel.ReturnUrl)
                ? this.Redirect(customerModel.ReturnUrl) : this.RedirectToAction("Index", "Categories");
        }

        private ErrorViewModel CreateErrorModel(int statusCode)
        {
            var message = statusCode switch
            {
                404 => "User not fount.",
                400 => "Sorry for inconvenience. Try to send request again.",
                _ => "Problem occurred during request. Sorry for inconvenience."
            };

            return new ErrorViewModel { ErrorMessage = message };
        }

    }
}
