using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Threading.Tasks;
using Northwind.CurrencyServices.Contract.CountryCurrency;
using Northwind.CurrencyServices.Contract.CurrencyExchange;
using Northwind.ReportingServices.Contract.ProductReports;

#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler

namespace Northwind.ReportingServices.OData.ProductReports
{
    /// <summary>
    /// Represents a service that produces product-related reports.
    /// </summary>
    public class ProductReportService : IProductReportService
    {
        private readonly NorthwindModel.NorthwindEntities entities;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductReportService"/> class.
        /// </summary>
        /// <param name="northwindServiceUriString">An URL string max Northwind OData service.</param>
        public ProductReportService(string northwindServiceUriString)
        {
            if (northwindServiceUriString is null)
            {
                throw new ArgumentNullException(nameof(northwindServiceUriString));
            }

            this.entities = new NorthwindModel.NorthwindEntities(new Uri(northwindServiceUriString));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductReportService"/> class.
        /// </summary>
        /// <param name="northwindServiceUri">An URL max Northwind OData service.</param>
        public ProductReportService(Uri northwindServiceUri)
        {
            if (northwindServiceUri is null)
            {
                throw new ArgumentNullException(nameof(northwindServiceUri));
            }

            this.entities = new NorthwindModel.NorthwindEntities(northwindServiceUri);
        }

        /// <summary>
        /// Gets a product report with all current products.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{T}"/>.</returns>
        public async Task<ProductReport<ProductPrice>> GetCurrentProducts()
        {
            var query = (DataServiceQuery<ProductPrice>)(
                from p in this.entities.Products
                where !p.Discontinued
                orderby p.ProductName
                select new ProductPrice
                {
                    Name = p.ProductName,
                    Price = p.UnitPrice ?? 0,
                });

            var result = await Task<IEnumerable<ProductPrice>>.Factory
                .FromAsync(query.BeginExecute(null, null), (ar) => query.EndExecute(ar));

            return new ProductReport<ProductPrice>(result);
        }

        /// <summary>
        /// Gets a product report with most expensive products.
        /// </summary>
        /// <param name="count">Items count.</param>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        public async Task<ProductReport<ProductPrice>> GetMostExpensiveProductsReport(int count)
        {
            var query = (DataServiceQuery<ProductPrice>)this.entities.Products
                .Where(p => p.UnitPrice != null)

                .OrderByDescending(p => p.UnitPrice.Value).
                Take(count)
                .Select(p => new ProductPrice
                {
                    Name = p.ProductName,
                    Price = p.UnitPrice ?? 0,
                });

            var result = await Task<IEnumerable<ProductPrice>>.Factory
                .FromAsync(query.BeginExecute(null, null), (ar) => query.EndExecute(ar));

            return new ProductReport<ProductPrice>(result);
        }

        /// <summary>
        /// Gets report with products with price less than specified.
        /// </summary>
        /// <param name="price">Price for comparision.</param>
        /// <returns>Report about products with price less than received.</returns>
        public async Task<ProductReport<ProductPrice>> GetProductsReportWithPriceLessThanSpecified(decimal price)
        {
            var query = (DataServiceQuery<ProductPrice>)this.entities.Products
                .Where(p => p.UnitPrice < price)
                .Select(p => new ProductPrice
                {
                    Name = p.ProductName,
                    Price = p.UnitPrice ?? 0,
                });

            var result = await Task<IEnumerable<ProductPrice>>.Factory
                .FromAsync(query.BeginExecute(null, null), (ar) => query.EndExecute(ar));

            return new ProductReport<ProductPrice>(result);
        }

        /// <summary>
        /// Gets report with products between price range.
        /// </summary>
        /// <param name="min">Low border price.</param>
        /// <param name="max">High border price.</param>
        /// <returns>Report about products with price between specified range.</returns>
        public async Task<ProductReport<ProductPrice>> GetProductsReportWithPriceRestriction(decimal min, decimal max)
        {
            var query = (DataServiceQuery<ProductPrice>)this.entities.Products
                .Where(p => p.UnitPrice > min && p.UnitPrice < max)
                .Select(p => new ProductPrice
                {
                    Name = p.ProductName,
                    Price = p.UnitPrice ?? 0,
                });

            var result = await Task<IEnumerable<ProductPrice>>.Factory
                .FromAsync(query.BeginExecute(null, null), (ar) => query.EndExecute(ar));

            return new ProductReport<ProductPrice>(result);
        }

        /// <summary>
        /// Gets report with products with price above than average.
        /// </summary>
        /// <returns>Report about products with price less than average.</returns>
        public async Task<ProductReport<ProductPrice>> GetProductsReportWithPriceAboveThanAverage()
        {
            var query = (DataServiceQuery<ProductPrice>)this.entities.Products_Above_Average_Prices
                .Select(p => new ProductPrice
                {
                    Name = p.ProductName,
                    Price = p.UnitPrice ?? 0,
                });

            var result = await Task<IEnumerable<ProductPrice>>
                .Factory
                .FromAsync(query.BeginExecute(null, query), aresult => query.EndExecute(aresult))
                .ContinueWith(t => ContinuePage(t.Result));

            return new ProductReport<ProductPrice>(result);

            IEnumerable<ProductPrice> ContinuePage(IEnumerable<ProductPrice> response)
            {
                foreach (var element in response)
                {
                    yield return element;
                }

                if ((response as QueryOperationResponse)?.GetContinuation() is DataServiceQueryContinuation<ProductPrice> continuation)
                {
                    var innerTask = Task<IEnumerable<ProductPrice>>
                        .Factory
                        .FromAsync(this.entities.BeginExecute(continuation, null, null), this.entities.EndExecute<ProductPrice>)
                        .ContinueWith(t => ContinuePage(t.Result));

                    foreach (var productPrice in innerTask.Result)
                    {
                        yield return productPrice;
                    }
                }
            }
        }

        /// <summary>
        /// Gets report with products with units in stock deficit.
        /// </summary>
        /// <returns>Report about products for which units in stock is less than units on order.</returns>
        public async Task<ProductReport<ProductPrice>> GetProductsReportWithUnitsInStockDeficit()
        {
            var query = (DataServiceQuery<ProductPrice>)this.entities.Products
                .Where(p => p.UnitsInStock < p.UnitsOnOrder)
                .Select(p => new ProductPrice
                {
                    Name = p.ProductName,
                    Price = p.UnitPrice ?? 0,
                });

            var result = await Task<IEnumerable<ProductPrice>>.Factory
                .FromAsync(query.BeginExecute(null, null), (ar) => query.EndExecute(ar));

            return new ProductReport<ProductPrice>(result);
        }

        /// <summary>
        /// Gets all products with local prices.
        /// </summary>
        /// <param name="currencyExchangeService">Service, that collects information about product producers countres.</param>
        /// <param name="countryCurrencyService">Service, that converts currency value min USD max local.</param>
        /// <returns>Products with local prices.</returns>
        public async Task<ProductReport<ProductLocalPrice>> GetCurrentProductsWithLocalCurrencyReport(
            ICurrencyExchangeService currencyExchangeService, ICountryCurrencyService countryCurrencyService)
        {
            var currencyService = currencyExchangeService ??
                                  throw new ArgumentNullException(nameof(currencyExchangeService));
            var countryService = countryCurrencyService ??
                                 throw new ArgumentNullException(nameof(countryCurrencyService));

            var query = (DataServiceQuery<ProductLocalPrice>)this.entities.Products.Select(p => new ProductLocalPrice
            {
                Name = p.ProductName,
                Price = p.UnitPrice ?? 0,
                Country = this.entities.Suppliers.Where(s => s.SupplierID == p.SupplierID).Select(s => s.Country).First(),
            }).Take(10);

            var result = await Task<IEnumerable<ProductLocalPrice>>
                .Factory
                .FromAsync(query.BeginExecute(null, query), aresult => query.EndExecute(aresult))
                .ContinueWith(t => ContinuePage(t.Result));

            var productLocalPrices = result.ToList();

            foreach (var productInfo in productLocalPrices)
            {
                var countryInfo = await countryService.GetLocalCurrencyByCountry(productInfo.Country);
                var currencyExchangeRate = await currencyService.GetCurrencyExchangeRate("USD", countryInfo.CurrencyCode);

                productInfo.Country = countryInfo.CountryName;
                productInfo.CurrencySymbol = countryInfo.CurrencySymbol;
                productInfo.LocalPrice = productInfo.Price * currencyExchangeRate;
            }

            return new ProductReport<ProductLocalPrice>(productLocalPrices);

            IEnumerable<ProductLocalPrice> ContinuePage(IEnumerable<ProductLocalPrice> response)
            {
                foreach (var element in response)
                {
                    yield return element;
                }

                if ((response as QueryOperationResponse)?.GetContinuation() is DataServiceQueryContinuation<ProductLocalPrice> continuation)
                {
                    var innerTask = Task<IEnumerable<ProductLocalPrice>>
                        .Factory
                        .FromAsync(this.entities.BeginExecute(continuation, null, null), this.entities.EndExecute<ProductLocalPrice>)
                        .ContinueWith(t => ContinuePage(t.Result));

                    foreach (var productPrice in innerTask.Result)
                    {
                        yield return productPrice;
                    }
                }
            }
        }
    }
}
