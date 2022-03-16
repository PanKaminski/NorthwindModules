using System;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.DataAccess.SqlServer;
using Northwind.Services.Blogging;
using Northwind.Services.Customers;
using Northwind.Services.Employees;
using Northwind.Services.EntityFrameworkCore.Blogging;
using Northwind.Services.EntityFrameworkCore.Blogging.Context;
using Northwind.Services.EntityFrameworkCore.Context;
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
                case "ADO":
                    services
                        .AddTransient<IProductManagementService, Northwind.Services.DataAccess.Products.ProductManagementDataAccessService>()
                        .AddTransient<IProductCategoryManagementService, Northwind.Services.DataAccess.Products.ProductCategoriesManagementDataAccessService>()
                        .AddTransient<IProductCategoryPicturesService, Northwind.Services.DataAccess.Products.ProductCategoryPicturesManagementDataAccessService>()
                        .AddTransient<IEmployeeManagementService, Northwind.Services.DataAccess.Employees.EmployeeManagementDataAccessService>()
                        .AddTransient<Northwind.DataAccess.NorthwindDataAccessFactory, SqlServerDataAccessFactory>()
                        .AddScoped(_ => new SqlConnection(configuration.GetConnectionString("DefaultConnection")));
                    break;
                case "EF":
                    services
                        .AddTransient<IProductManagementService,
                            Northwind.Services.EntityFrameworkCore.Products.ProductManagementService>()
                        .AddTransient<IProductCategoryManagementService, Northwind.Services.EntityFrameworkCore.Products
                            .ProductCategoryManagementService>()
                        .AddTransient<IProductCategoryPicturesService, Northwind.Services.EntityFrameworkCore.Products.
                            ProductCategoryPicturesService>()
                        .AddTransient<IEmployeeManagementService,
                            Northwind.Services.EntityFrameworkCore.Employees.EmployeeManagementService>()
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