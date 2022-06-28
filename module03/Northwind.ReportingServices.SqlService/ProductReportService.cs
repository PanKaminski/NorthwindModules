using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Northwind.CurrencyServices.Contract.CountryCurrency;
using Northwind.CurrencyServices.Contract.CurrencyExchange;
using Northwind.ReportingServices.Contract.ProductReports;

namespace Northwind.ReportingServices.SqlService
{
    /// <inheritdoc cref="IProductReportService"/>
    public class ProductReportService : IProductReportService
    {
        private readonly SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of <see cref="ProductReportService"/>.
        /// </summary>
        /// <param name="connectionString">Sql connection string.</param>
        public ProductReportService(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("DB connection string is null or whitespace.");
            }

            this.connection = new SqlConnection(connectionString);
        }

        /// <inheritdoc cref="IProductReportService"/>
        public async Task<ProductReport<ProductPrice>> GetCurrentProducts()
        {
            using (var sqlCommand = new SqlCommand("Products_In_Release", this.connection)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                return await ComposeProductReportWithCommonPrice(sqlCommand);
            }
        }

        /// <inheritdoc cref="IProductReportService"/>
        public async Task<ProductReport<ProductPrice>> GetMostExpensiveProductsReport(int count)
        {
            using (var sqlCommand = new SqlCommand("Most_Expensive_Products", this.connection)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                sqlCommand.Parameters.Add(new SqlParameter("@products_count", SqlDbType.Int));
                sqlCommand.Parameters["@products_count"].Value = count;

                return await ComposeProductReportWithCommonPrice(sqlCommand);
            }
        }

        /// <inheritdoc cref="IProductReportService"/>
        public async Task<ProductReport<ProductPrice>> GetProductsReportWithPriceLessThanSpecified(decimal price)
        {
            using (var sqlCommand = new SqlCommand("Products_With_Price_Less_Than", this.connection)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                sqlCommand.Parameters.Add(new SqlParameter("@price", SqlDbType.Money));
                sqlCommand.Parameters["@price"].Value = price;

                return await ComposeProductReportWithCommonPrice(sqlCommand);
            }
        }

        /// <inheritdoc cref="IProductReportService"/>
        public async Task<ProductReport<ProductPrice>> GetProductsReportWithPriceRestriction(decimal min, decimal max)
        {
            using (var sqlCommand = new SqlCommand("Products_With_Price_Restriction", this.connection)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                sqlCommand.Parameters.Add(new SqlParameter("@min_price", SqlDbType.Money));
                sqlCommand.Parameters["@min_price"].Value = min;
                sqlCommand.Parameters.Add(new SqlParameter("@max_price", SqlDbType.Money));
                sqlCommand.Parameters["@max_price"].Value = max;

                return await ComposeProductReportWithCommonPrice(sqlCommand);
            }
        }

        /// <inheritdoc cref="IProductReportService"/>
        public async Task<ProductReport<ProductPrice>> GetProductsReportWithPriceAboveThanAverage()
        {
            using (var sqlCommand = new SqlCommand("SELECT ProductName, UnitPrice FROM [Products Above Average Price]", this.connection)
            {
                CommandType = CommandType.Text
            })
            {
                return await ComposeProductReportWithCommonPrice(sqlCommand);
            }
        }
        /// <inheritdoc cref="IProductReportService"/>
        public async Task<ProductReport<ProductPrice>> GetProductsReportWithUnitsInStockDeficit()
        {
            using (var sqlCommand = new SqlCommand("Products_With_UnitsInStock_Deficit", this.connection)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                return await ComposeProductReportWithCommonPrice(sqlCommand);
            }
        }

        /// <inheritdoc cref="IProductReportService"/>
        public async Task<ProductReport<ProductLocalPrice>> GetCurrentProductsWithLocalCurrencyReport(ICurrencyExchangeService currencyExchangeService,
            ICountryCurrencyService countryCurrencyService)
        {
            using (var sqlCommand = new SqlCommand("Products_With_Local_Price", this.connection)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                await this.connection.OpenAsync();

                var reader = await sqlCommand.ExecuteReaderAsync();

                var products = new List<ProductLocalPrice>();

                while (await reader.ReadAsync())
                {
                    var product = await CreateProductWithLocalPrice(reader, currencyExchangeService, countryCurrencyService);

                    products.Add(product);
                }

                return new ProductReport<ProductLocalPrice>(products);
            }
        }

        private async Task<ProductReport<ProductPrice>> ComposeProductReportWithCommonPrice(SqlCommand sqlCommand)
        {
            await this.connection.OpenAsync();

            var reader = await sqlCommand.ExecuteReaderAsync();

            var products = new List<ProductPrice>();

            while (await reader.ReadAsync())
            {
                var product = CreateProduct(reader);

                products.Add(product);
            }

            reader.Close();

            return new ProductReport<ProductPrice>(products);
        }

        private static async Task<ProductLocalPrice> CreateProductWithLocalPrice(SqlDataReader reader, ICurrencyExchangeService currencyExchangeService,
            ICountryCurrencyService countryCurrencyService)
        {
            var name = (string)reader["ProductName"];
            var price = reader["UnitPrice"] != DBNull.Value ? (decimal)reader["UnitPrice"] : 0;
            var country = (string)reader["Country"];

            var countryInfo = await countryCurrencyService.GetLocalCurrencyByCountry(country);
            var currencyExchangeRate = await currencyExchangeService.GetCurrencyExchangeRate("USD", countryInfo.CurrencyCode);


            return new ProductLocalPrice
            {
                Name = name,
                Price = price,
                Country = countryInfo.CountryName,
                CurrencySymbol = countryInfo.CurrencySymbol,
                LocalPrice = price * currencyExchangeRate
            };
        }

        private static ProductPrice CreateProduct(SqlDataReader reader)
        {
            return new ProductPrice
            {
                Name = (string)reader["ProductName"],
                Price = reader["UnitPrice"] != DBNull.Value ? (decimal)reader["UnitPrice"] : 0,
            };
        }
    }
}
