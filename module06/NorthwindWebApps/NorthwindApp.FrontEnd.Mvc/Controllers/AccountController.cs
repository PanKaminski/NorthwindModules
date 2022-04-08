using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindApp.FrontEnd.Mvc.Identity;
using NorthwindApp.FrontEnd.Mvc.Identity.Models;
using NorthwindApp.FrontEnd.Mvc.Services;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Account;

namespace NorthwindApp.FrontEnd.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IdentityDbContext context;
        private readonly IAdminService adminService;

        public AccountController(IdentityDbContext context, IAdminService adminService)
        {
            this.context = context;
            this.adminService = adminService;
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

            User user = await this.context.Users.FirstOrDefaultAsync(u => u.Email == registerModel.Email);

            if (user is null)
            {
                user = new User
                {
                    Email = registerModel.Email,
                    
                    Password = BCrypt.Net.BCrypt.HashPassword(registerModel.Password),
                };

                var isAdminCreationAllowed = await this.adminService.CheckAdminUserCreationAsync();
                if (isAdminCreationAllowed)
                {
                    user.Role = await this.context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                }
                else
                {
                    user.Role = await this.context.Roles.FirstOrDefaultAsync(r => r.Name == "Customer");
                }

                this.context.Users.Add(user);

                await this.context.SaveChangesAsync();

                await this.Authenticate(user);

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

            User user = await this.context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == loginModel.Email);

            if (user is not null && BCrypt.Net.BCrypt.Verify(loginModel.Password, user.Password))
            {
                await this.Authenticate(user);

                return !string.IsNullOrEmpty(loginModel.ReturnUrl) && this.Url.IsLocalUrl(loginModel.ReturnUrl)
                    ? this.Redirect(loginModel.ReturnUrl) : this.RedirectToAction("Index", "Categories");
            }

            this.ModelState.AddModelError("", "Invalid login or password.");

            return this.View(loginModel);
        }

        public async Task<IActionResult> LogoutAsync()
        {
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return this.RedirectToAction("Index", "Categories");
        }

        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name),
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
