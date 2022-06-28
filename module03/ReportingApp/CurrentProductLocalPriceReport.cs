using System;
using System.Threading.Tasks;
using Northwind.CurrencyServices.Contract.CountryCurrency;
using Northwind.CurrencyServices.Contract.CurrencyExchange;
using Northwind.ReportingServices.Contract.ProductReports;

namespace ReportingApp
{
    /// <summary>
    /// Contains methods for displaying current local report.
    /// </summary>
    public class CurrentProductLocalPriceReport
    {
        private readonly IProductReportService productReportService;
        private readonly ICurrencyExchangeService currencyExchangeService;
        private readonly ICountryCurrencyService countryCurrencyService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentProductLocalPriceReport"/> class.
        /// </summary>
        /// <param name="productReportService">Northwind report service.</param>
        /// <param name="currencyExchangeService">Currency exchange service.</param>
        /// <param name="countryCurrencyService">Country currency service.</param>
        public CurrentProductLocalPriceReport(IProductReportService productReportService, ICurrencyExchangeService currencyExchangeService, ICountryCurrencyService countryCurrencyService)
        {
            this.productReportService = productReportService ?? throw new ArgumentNullException(nameof(productReportService));
            this.currencyExchangeService = currencyExchangeService ?? throw new ArgumentNullException(nameof(currencyExchangeService));
            this.countryCurrencyService = countryCurrencyService ?? throw new ArgumentNullException(nameof(countryCurrencyService));
        }

        /// <summary>
        /// Prints report about products with local prices.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task PrintProductLocalReportAsync()
        {
            var report = await this.productReportService.GetCurrentProductsWithLocalCurrencyReport(
                    this.currencyExchangeService, this.countryCurrencyService);

            Console.WriteLine("Report - current-products-local-prices:");
            foreach (var reportLine in report.Products)
            {
                Console.WriteLine("{0}, {1}, {2}, {3}", reportLine.Name, reportLine.Price + "$", reportLine.Country, reportLine.LocalPrice + reportLine.CurrencySymbol);
            }
        }
    }
}
