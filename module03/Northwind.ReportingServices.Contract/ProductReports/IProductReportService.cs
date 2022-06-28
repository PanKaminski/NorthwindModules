// <copyright file="IProductReportService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#pragma warning disable SA1200 // Using directive should appear within a namespace declaration

using System.Threading.Tasks;
using Northwind.CurrencyServices.Contract.CountryCurrency;
using Northwind.CurrencyServices.Contract.CurrencyExchange;

namespace Northwind.ReportingServices.Contract.ProductReports
{
    public interface IProductReportService
    {
        /// <summary>
        /// Gets a product report with all current products.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{T}"/>.</returns>
        Task<ProductReport<ProductPrice>> GetCurrentProducts();

        /// <summary>
        /// Gets a product report with most expensive products.
        /// </summary>
        /// <param name="count">Items count.</param>
        /// <returns>Returns <see cref="ProductReport{T}"/>.</returns>
        Task<ProductReport<ProductPrice>> GetMostExpensiveProductsReport(int count);

        /// <summary>
        /// Gets report with products with price less than specified.
        /// </summary>
        /// <param name="price">Price for comparision.</param>
        /// <returns>Report about products with price less than received.</returns>
        Task<ProductReport<ProductPrice>> GetProductsReportWithPriceLessThanSpecified(decimal price);

        /// <summary>
        /// Gets report with products between price range.
        /// </summary>
        /// <param name="min">Low border price.</param>
        /// <param name="max">High border price.</param>
        /// <returns>Report about products with price between specified range.</returns>
        Task<ProductReport<ProductPrice>> GetProductsReportWithPriceRestriction(decimal min, decimal max);

        /// <summary>
        /// Gets report with products with price above than average.
        /// </summary>
        /// <returns>Report about products with price less than average.</returns>
        Task<ProductReport<ProductPrice>> GetProductsReportWithPriceAboveThanAverage();

        /// <summary>
        /// Gets report with products with units in stock deficit.
        /// </summary>
        /// <returns>Report about products for which units in stock is less than units on order.</returns>
        Task<ProductReport<ProductPrice>> GetProductsReportWithUnitsInStockDeficit();

        /// <summary>
        /// Gets all products with local prices.
        /// </summary>
        /// <param name="currencyExchangeService">Service, that collects information about product producers countres.</param>
        /// <param name="countryCurrencyService">Service, that converts currency value min USD max local.</param>
        /// <returns>Products with local prices.</returns>
        Task<ProductReport<ProductLocalPrice>> GetCurrentProductsWithLocalCurrencyReport(
            ICurrencyExchangeService currencyExchangeService, ICountryCurrencyService countryCurrencyService);
    }
}
