using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NorthwindApp.FrontEnd.Mvc.Identity;
using NorthwindApp.FrontEnd.Mvc.Identity.Models;
using NorthwindApp.FrontEnd.Mvc.Services.Interfaces;
using NorthwindApp.FrontEnd.Mvc.ViewModels;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Account;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Employees;

namespace NorthwindApp.FrontEnd.Mvc.Services.Implementations
{
    public class UserManagementService : IUserManagementService
    {
        private const string EmployeeApiPath = "employees";
        private const string CustomersApiPath = "customers";

        private readonly HttpClient httpClient;
        private readonly IdentityDbContext context;
        private readonly IMapper mapper;
        private readonly IAdminService adminService;

        public UserManagementService(IdentityDbContext context, IMapper mapper, IAdminService adminService, HttpClient httpClient)
        {
            this.context = context;
            this.mapper = mapper;
            this.adminService = adminService;
            this.httpClient = httpClient;
        }

        public async Task<PageListViewModel<UserResponseViewModel>> GetUsers(int page, int pageSize)
        {
            var users = this.context.Users
                .Include(u => u.Role)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsEnumerable();

            return new PageListViewModel<UserResponseViewModel>
            {
                PageInfo = new PageInfo
                {
                    CountOfPages = (int)Math.Ceiling((decimal)await this.context.Users.CountAsync() / pageSize),
                    CurrentPage = page,
                    ItemsPerPage = 0
                },
                Items = this.mapper.Map<IEnumerable<User>, IEnumerable<UserResponseViewModel>>(users),
            };
        }

        public async Task<IEnumerable<string>> GetEmployeeNames()
        {
            var response = await this.httpClient.GetAsync($"{EmployeeApiPath}");
            var jsonString = await response.Content.ReadAsStringAsync();
            var employees = JsonConvert.DeserializeObject<IEnumerable<Northwind.Services.Employees.Employee>>(jsonString);

            return employees?.Select(e => e.LastName + " " + e.FirstName);
        }

        public async Task<UserResponseViewModel> Get(int userId)
        {
            var user = await this.context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            return this.mapper.Map<UserResponseViewModel>(user);
        }

        public async Task<bool> RegisterAsync(RegisterViewModel registerModel)
        {
            User user = await this.context.Users.FirstOrDefaultAsync(u => u.Email == registerModel.Email);

            if (user is null)
            {

                user = new User
                {
                    Email = registerModel.Email, Password = BCrypt.Net.BCrypt.HashPassword(registerModel.Password),
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

                return true;
            }

            return false;
        }

        public async Task<bool> VerifyCredentialsAsync(LoginViewModel loginModel)
        {
            User user = await this.context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == loginModel.Email);

            if (user is not null && BCrypt.Net.BCrypt.Verify(loginModel.Password, user.Password))
            {
                return true;
            }

            return false;
        }

        public async Task<ClaimsIdentity> GenerateClaims(string email)
        {
            var user = await this.context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
                
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name),
            };

            return new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }

        public async Task<int> CreateEmployee(int userId, EmployeeInputViewModel employeeModel)
        {
            var employeeEntity = this.mapper.Map<Northwind.Services.Employees.Employee>(employeeModel);

            if (!string.IsNullOrWhiteSpace(employeeModel.ReportsTo))
            {
                var fullName = employeeModel.ReportsTo.Split();
                var chief = await this.GetIdByFullName(fullName[0], fullName[1]);

                employeeEntity.ReportsTo = chief > 0 ? chief : null;
            }

            var response = await this.httpClient.PostAsJsonAsync(EmployeeApiPath, employeeEntity);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return -1;
            }

            response.EnsureSuccessStatusCode();

            int id = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());

            var user = await this.context.Users.SingleOrDefaultAsync(u => u.Id == userId);
            user.Role = await this.context.Roles.FirstOrDefaultAsync(r => r.Name == "Employee");
            await this.context.EmployeesTransfer.AddAsync(new Employee { UserId = userId, NorthwindId = id });
            await this.context.SaveChangesAsync();

            if (employeeModel.Photo is not null)
            {
                await this.httpClient.PutAsJsonAsync($"{EmployeeApiPath}/{id}/photo", employeeModel.Photo);
            }

            return id;
        }

        public async Task<int> GetNorthwindEmployeeId(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return -1;
            }

            var employee = await this.context.Users
                .SingleOrDefaultAsync(u => u.Email == email);

            var connectionEntity = await this.context.EmployeesTransfer
                .SingleOrDefaultAsync(e => e.UserId == employee.Id);

            return connectionEntity?.NorthwindId ?? -1;
        }

        public async Task<(string, bool)> GetNorthwindCustomerId(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return ("", false);
            }

            var customer = await this.context.Users
                .SingleOrDefaultAsync(u => u.Email == email);
            var identityId = customer?.Id ?? 0;

            var connectionEntity = await this.context.CustomersTransfer
                .SingleOrDefaultAsync(e => e.UserId == identityId);

            var result = connectionEntity?.NorthwindId is not null;

            return (result ? connectionEntity.NorthwindId : "", result);
        }

        private async Task<int> GetIdByFullName(string firstName, string lastName)
        {
            var response = await this.httpClient.GetAsync($"{EmployeeApiPath}/{firstName}/{lastName}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return -1;
            }

            response.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<Northwind.Services.Employees.Employee>(await response.Content.ReadAsStringAsync());

            return result?.Id ?? -1;
        }
    }
}