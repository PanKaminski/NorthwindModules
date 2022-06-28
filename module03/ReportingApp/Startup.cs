using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.CurrencyServices.Contract.CountryCurrency;
using Northwind.CurrencyServices.Contract.CurrencyExchange;
using Northwind.CurrencyServices.CountryCurrency;
using Northwind.CurrencyServices.CurrencyExchange;
using Northwind.ReportingServices.Contract.ProductReports;

namespace ReportingApp
{
    /// <summary>
    /// Composition root.
    /// </summary>
    public sealed class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        public Startup()
        {
            this.AppConfigurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        /// <summary>
        /// Gets application configuration root.
        /// </summary>
        public IConfigurationRoot AppConfigurationRoot { get; }

        /// <summary>
        /// Resolves dependencies.
        /// </summary>
        /// <returns>Service provider.</returns>
        public IServiceProvider CreateServiceProvider()
        {
            var serviceCollection = new ServiceCollection()
                .AddTransient<ICountryCurrencyService, CountryCurrencyService>()
                .AddTransient<ICurrencyExchangeService>(p =>
                    new CurrencyExchangeService(this.AppConfigurationRoot["Key"]))
                .AddTransient<CurrentProductLocalPriceReport>();

            switch (this.AppConfigurationRoot["Storage"])
            {
                case "OData":
                    serviceCollection.AddSingleton<IProductReportService>(p =>
                        new Northwind.ReportingServices.OData.ProductReports.ProductReportService(this.AppConfigurationRoot["Connection"]));
                    break;
                case "SQL":
                    serviceCollection.AddSingleton<IProductReportService>(p =>
                        new Northwind.ReportingServices.SqlService.ProductReportService(this.AppConfigurationRoot["Connection"]));
                    break;
                default: throw new NotSupportedException();
            }

            return serviceCollection.BuildServiceProvider();
        }
    }
}
