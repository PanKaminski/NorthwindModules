using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Northwind.ReportingServices.Contract.ProductReports;

namespace ReportingApp
{
    /// <summary>
    /// Program class.
    /// </summary>
    public static class Program
    {
        private const string CurrentProductsReport = "current-products";
        private const string MostExpensiveProductsReport = "most-expensive-products";
        private const string PriceLessThanProducts = "price-less-then-products";
        private const string PriceAboveAverageProducts = "price-above-average-products";
        private const string UnitsInStockDeficit = "units-in-stock-deficit";
        private const string CurrentProductsLocalPrices = "current-products-local-prices";

        private static IServiceProvider serviceResolver = new Startup().CreateServiceProvider();

        /// <summary>
        /// A program entry point.
        /// </summary>
        /// <param name="args">Program arguments.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Main(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                ShowHelp();
                return;
            }

            var reportName = args[0];

            if (string.Equals(reportName, CurrentProductsReport, StringComparison.InvariantCultureIgnoreCase))
            {
                await ShowCurrentProducts();
            }
            else if (string.Equals(reportName, MostExpensiveProductsReport, StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length > 1 && int.TryParse(args[1], out int count))
                {
                    await ShowMostExpensiveProducts(count);
                }
            }
            else if (string.Equals(reportName, PriceLessThanProducts, StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length > 1 && decimal.TryParse(args[1], out decimal price))
                {
                    await ShowProductsWithPriceLessThanSpecified(price);
                }
            }
            else if (string.Equals(reportName, PriceAboveAverageProducts, StringComparison.InvariantCultureIgnoreCase))
            {
                await ShowProductsWithPriceAboveAverage();
            }
            else if (string.Equals(reportName, CurrentProductsLocalPrices, StringComparison.InvariantCultureIgnoreCase))
            {
                await ShowCurrentProductsLocalPrices();
            }
            else if (string.Equals(reportName, UnitsInStockDeficit, StringComparison.InvariantCultureIgnoreCase))
            {
                await ShowProductsWithUnitsInStockDeficit();
            }
            else
            {
                ShowHelp();
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("\tReportingApp.exe <report> <report-argument1> <report-argument2> ...");
            Console.WriteLine();
            Console.WriteLine("Reports:");
            Console.WriteLine($"\t{CurrentProductsReport}\t\tShows current products.");
            Console.WriteLine($"\t{MostExpensiveProductsReport}\t\tShows specified number of the most expensive products.");
            Console.WriteLine($"\t{PriceLessThanProducts}\t\tShows products with price less than.");
            Console.WriteLine($"\t{PriceAboveAverageProducts}\t\tShows products with price above than average.");
            Console.WriteLine($"\t{UnitsInStockDeficit}\t\tShows products with units in stock deficit.");
            Console.WriteLine($"\t{CurrentProductsLocalPrices}\t\tShows products with local price.");
        }

        private static async Task ShowCurrentProducts()
        {
            var service = serviceResolver.GetService<IProductReportService>();
            var report = await service.GetCurrentProducts();
            PrintProductReport("current products:", report);
        }

        private static async Task ShowMostExpensiveProducts(int count)
        {
            var service = serviceResolver.GetService<IProductReportService>();
            var report = await service.GetMostExpensiveProductsReport(count);
            PrintProductReport($"{count} most expensive products:", report);
        }

        private static async Task ShowProductsWithPriceLessThanSpecified(decimal price)
        {
            var service = serviceResolver.GetService<IProductReportService>();
            var report = await service.GetProductsReportWithPriceLessThanSpecified(price);
            PrintProductReport($"products with price less than {price}:", report);
        }

        private static async Task ShowProductsWithPriceAboveAverage()
        {
            var service = serviceResolver.GetService<IProductReportService>();
            var report = await service.GetProductsReportWithPriceAboveThanAverage();
            PrintProductReport("products with price above average:", report);
        }

        private static async Task ShowProductsWithUnitsInStockDeficit()
        {
            var service = serviceResolver.GetService<IProductReportService>();
            var report = await service.GetProductsReportWithUnitsInStockDeficit();
            PrintProductReport("products with units in stock deficit:", report);
        }

        private static async Task ShowCurrentProductsLocalPrices()
        {
            var localReportPrinter = serviceResolver.GetService<CurrentProductLocalPriceReport>();
            await localReportPrinter?.PrintProductLocalReportAsync();
        }

        private static void PrintProductReport(string header, ProductReport<ProductPrice> productReport)
        {
            Console.WriteLine($"Report - {header}");
            foreach (var reportLine in productReport.Products)
            {
                Console.WriteLine("{0}, {1}", reportLine.Name, reportLine.Price);
            }
        }
    }
}
