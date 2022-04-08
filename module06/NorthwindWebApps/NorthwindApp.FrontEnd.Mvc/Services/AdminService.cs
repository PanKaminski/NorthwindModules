using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NorthwindApp.FrontEnd.Mvc.Identity;

namespace NorthwindApp.FrontEnd.Mvc.Services
{
    public class AdminService : IAdminService
    {
        private const string RoleName = "Admin";

        private readonly IServiceProvider serviceProvider;

        private bool adminExists;

        public AdminService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task<bool> CheckAdminUserCreationAsync()
        {
            if (this.adminExists)
            {
                return false;
            }

            using var scope = this.serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

            if (await dbContext.Users.AnyAsync(user => user.Role.Name == RoleName))
            {
                this.adminExists = true;
                return false;
            }

            return true;
        }
    }
}