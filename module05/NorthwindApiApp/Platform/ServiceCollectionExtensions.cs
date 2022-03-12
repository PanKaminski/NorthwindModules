using System;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.DataAccess.SqlServer;
using Northwind.Services.DataAccess.Infrastructure;
using Northwind.Services.Employees;
using Northwind.Services.EntityFrameworkCore.Context;
using Northwind.Services.Products;

namespace NorthwindApiApp.Platform
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNorthwindServices(this IServiceCollection services, IConfiguration configuration)
        {
            switch (configuration["Storage"])
            {
                case "DB":
                    services
                        .AddTransient<IProductManagementService, Northwind.Services.DataAccess.Products.ProductManagementDataAccessService>()
                        .AddTransient<IProductCategoryManagementService, Northwind.Services.DataAccess.Products.ProductCategoriesManagementDataAccessService>()
                        .AddTransient<IProductCategoryPicturesService, Northwind.Services.DataAccess.Products.ProductCategoryPicturesManagementDataAccessService>()
                        .AddTransient<IEmployeeManagementService, Northwind.Services.DataAccess.Employees.EmployeeManagementDataAccessService>()
                        .AddAutoMapper(typeof(MappingProfile))
                        .AddTransient<Northwind.DataAccess.NorthwindDataAccessFactory, SqlServerDataAccessFactory>()
                        .AddScoped(_ => new SqlConnection(configuration.GetConnectionString("DefaultConnection")));
                    break;
                case "Memory":
                    services
                        .AddTransient<IProductManagementService, Northwind.Services.EntityFrameworkCore.Products.ProductManagementService>()
                        .AddTransient<IProductCategoryManagementService, Northwind.Services.EntityFrameworkCore.Products.ProductCategoryManagementService>()
                        .AddTransient<IProductCategoryPicturesService, Northwind.Services.EntityFrameworkCore.Products.ProductCategoryPicturesService>()
                        .AddTransient<IEmployeeManagementService, Northwind.Services.EntityFrameworkCore.Employees.EmployeeManagementService>()
                        .AddDbContext<NorthwindContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
                    break;
                default:
                    throw new NotSupportedException();
            }

            return services;
        }
    }
}