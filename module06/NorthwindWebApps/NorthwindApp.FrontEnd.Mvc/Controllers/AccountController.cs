using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindApp.FrontEnd.Mvc.Services.Interfaces;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Account;
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
                var id = await this.userManagementService.GenerateClaims(registerModel.Email);

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
                var id = await this.userManagementService.GenerateClaims(loginModel.Email);

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
        public IActionResult MakeEmployee(int userId)
        {
            this.ViewBag.userId = userId;
            return this.View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MakeEmployee(int userId, EmployeeInputViewModel employeeModel)
        {
            if (!this.ModelState.IsValid)
            {
                this.ViewBag.userId = userId;
                return this.View(employeeModel);
            }

            await this.userManagementService.CreateEmployee(userId, employeeModel);
            return this.RedirectToAction("ViewRoles");
        }
    }
}
