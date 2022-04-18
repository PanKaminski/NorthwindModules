using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Services.Blogging;
using Northwind.Services.Customers;
using Northwind.Services.Employees;
using Northwind.Services.EntityFrameworkCore.Blogging;
using Northwind.Services.EntityFrameworkCore.Blogging.Context;
using Northwind.Services.EntityFrameworkCore.Context;
using Northwind.Services.EntityFrameworkCore.Employees;
using Northwind.Services.Products;
using NorthwindApiApp.Infrastructure;

namespace NorthwindApiApp.Platform
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNorthwindServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(MappingProfile));

            switch (configuration["DbService"])
            {
                case "EF":
                    services
                        .AddTransient<IProductManagementService,
                            Northwind.Services.EntityFrameworkCore.Products.ProductManagementService>()
                        .AddTransient<IProductCategoryManagementService, Northwind.Services.EntityFrameworkCore.Products
                            .ProductCategoryManagementService>()
                        .AddTransient<IProductCategoryPicturesService, Northwind.Services.EntityFrameworkCore.Products.
                            ProductCategoryPicturesService>()
                        .AddTransient<IEmployeePicturesService, EmployeePicturesService>()
                        .AddTransient<IEmployeeManagementService, EmployeeManagementService>()
                        .AddTransient<ICustomersManagementService,
                            Northwind.Services.EntityFrameworkCore.Customers.CustomersManagementService>()
                        .AddTransient<IBloggingService, BloggingService>()
                        .AddDbContext<NorthwindContext>(opt =>
                            opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
                        .AddDbContext<BloggingContext>(opt =>
                            opt.UseSqlServer(configuration.GetConnectionString("BloggingConnection")));

                    break;
                default:
                    throw new NotSupportedException();
            }

            return services;
        }
    }
}