using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Northwind.DataAccess.Products;

namespace Northwind.DataAccess.SqlServer.Products
{
    /// <summary>
    /// Represents a SQL Server-tailored DAO for Northwind products.
    /// </summary>
    public sealed class ProductSqlServerDataAccessObject : IProductDataAccessObject
    {
        private readonly SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductSqlServerDataAccessObject"/> class.
        /// </summary>
        /// <param name="connection">A <see cref="SqlConnection"/>.</param>
        public ProductSqlServerDataAccessObject(SqlConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <inheritdoc/>
        public async Task<int> InsertProductAsync(ProductTransferObject product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            using var sqlCommand = new SqlCommand("Insert_Product", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(product, sqlCommand);

            await this.OpenIfClosedAsync();
            return await sqlCommand.ExecuteNonQueryAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteProductAsync(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productId));
            }

            using var sqlCommand = new SqlCommand("Delete_Product", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string productIdParameter = "@productID";
            sqlCommand.Parameters.Add(productIdParameter, SqlDbType.Int);
            sqlCommand.Parameters[productIdParameter].Value = productId;

            await this.OpenIfClosedAsync();
            return await sqlCommand.ExecuteNonQueryAsync() > 0;
        }

        /// <inheritdoc/>
        public async Task<ProductTransferObject> FindProductAsync(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productId));
            }

            using var sqlCommand = new SqlCommand("Get_Product_By_Id", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string productIdParameter = "@productId";
            sqlCommand.Parameters.Add(productIdParameter, SqlDbType.Int);
            sqlCommand.Parameters[productIdParameter].Value = productId;

            await this.OpenIfClosedAsync();

            var reader = await sqlCommand.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                throw new ProductNotFoundException(productId);
            }

            await reader.ReadAsync();

            return CreateProduct(reader);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ProductTransferObject> SelectProductsAsync(int offset, int limit)
        {
            if (offset < 0)
            {
                throw new ArgumentException("Must be greater than zero or equals zero.", nameof(offset));
            }

            if (limit < 1)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(limit));
            }

            using var sqlCommand = new SqlCommand("Get_Products_With_Limit", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            sqlCommand.Parameters.Add("@offset", SqlDbType.Int);
            sqlCommand.Parameters["@offset"].Value = offset;

            sqlCommand.Parameters.Add("@rowCount", SqlDbType.Int);
            sqlCommand.Parameters["@rowCount"].Value = limit;

            await this.OpenIfClosedAsync();

            var reader = await sqlCommand.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                yield return CreateProduct(reader);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProductTransferObject> SelectProductsAsync()
        {
            using var sqlCommand = new SqlCommand("Get_Products", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            await this.OpenIfClosedAsync();

            var reader = await sqlCommand.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                yield return CreateProduct(reader);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProductTransferObject> SelectProductsByNameAsync(ICollection<string> productNames)
        {
            if (productNames == null)
            {
                throw new ArgumentNullException(nameof(productNames));
            }

            if (productNames.Count < 1)
            {
                throw new ArgumentException("Collection is empty.", nameof(productNames));
            }

            using var sqlCommand = new SqlCommand("Get_Products_By_Name", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            sqlCommand.Parameters.Add("@names", SqlDbType.NVarChar, 256);
            sqlCommand.Parameters["@names"].Value = string.Join(' ', productNames);

            await this.OpenIfClosedAsync();

            var reader = await sqlCommand.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                yield return CreateProduct(reader);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateProductAsync(ProductTransferObject product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            using var sqlCommand = new SqlCommand("Update_Product", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(product, sqlCommand);

            const string productId = "@productId";
            sqlCommand.Parameters.Add(productId, SqlDbType.Int);
            sqlCommand.Parameters[productId].Value = product.Id;

            await this.OpenIfClosedAsync();
            return await sqlCommand.ExecuteNonQueryAsync() > 0;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProductTransferObject> SelectProductByCategoryAsync(ICollection<int> collectionOfCategoryId)
        {
            if (collectionOfCategoryId == null)
            {
                throw new ArgumentNullException(nameof(collectionOfCategoryId));
            }

            using var sqlCommand = new SqlCommand("Get_Products_By_Categories", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            sqlCommand.Parameters.Add("@categoriesIds", SqlDbType.NVarChar, 256);
            var whereInClause = string.Join(' ', collectionOfCategoryId);
            sqlCommand.Parameters["@categoriesIds"].Value = whereInClause;

            await this.OpenIfClosedAsync();

            var reader = await sqlCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                yield return CreateProduct(reader);
            }
        }

        /// <inheritdoc/>
        public async Task<int> GetProductsCountAsync()
        {
            using var sqlCommand = new SqlCommand("Get_Count_OfProducts", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            await this.OpenIfClosedAsync();

            return (int)((await sqlCommand.ExecuteScalarAsync()) ?? 0);
        }

        /// <inheritdoc/>
        public async Task<int> GetProductsCountAsync(int categoryId)
        {
            using var sqlCommand = new SqlCommand("Get_Count_Of_Products_By_Category", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string categoryIdParameter = "@categoryId";
            sqlCommand.Parameters.Add(categoryIdParameter, SqlDbType.Int);
            sqlCommand.Parameters[categoryId].Value = categoryId;

            await this.OpenIfClosedAsync();

            return (int)((await sqlCommand.ExecuteScalarAsync()) ?? 0);
        }

        /// <inheritdoc/>
        private static ProductTransferObject CreateProduct(SqlDataReader reader)
        {
            var id = (int)reader["ProductID"];
            var name = (string)reader["ProductName"];

            int? supplierId = reader["SupplierID"] != DBNull.Value ? (int)reader["SupplierID"] : null;
            int? categoryId = reader["CategoryID"] != DBNull.Value ? (int)reader["CategoryID"] : null;
            string quantityPerUnit = reader["QuantityPerUnit"] != DBNull.Value ? (string)reader["QuantityPerUnit"] : null;
            decimal? unitPrice = reader["UnitPrice"] != DBNull.Value ? (decimal)reader["UnitPrice"] : null;
            short? unitsInStock = reader["UnitsInStock"] != DBNull.Value ? (short)reader["UnitsInStock"] : null;
            short? unitsOnOrder = reader["UnitsOnOrder"] != DBNull.Value ? (short)reader["UnitsOnOrder"] : null;
            short? reorderLevel = reader["ReorderLevel"] != DBNull.Value ? (short)reader["ReorderLevel"] : null;
            bool discontinued = (bool)reader["Discontinued"];

            return new ProductTransferObject
            {
                Id = id,
                Name = name,
                SupplierId = supplierId,
                CategoryId = categoryId,
                QuantityPerUnit = quantityPerUnit,
                UnitPrice = unitPrice,
                UnitsInStock = unitsInStock,
                UnitsOnOrder = unitsOnOrder,
                ReorderLevel = reorderLevel,
                Discontinued = discontinued,
            };
        }

        private static void AddSqlParameters(ProductTransferObject product, SqlCommand command)
        {
            const string productNameParameter = "@productName";
            command.Parameters.Add(productNameParameter, SqlDbType.NVarChar, 40);
            command.Parameters[productNameParameter].Value = product.Name;

            const string supplierIdParameter = "@supplierId";
            command.Parameters.Add(supplierIdParameter, SqlDbType.Int);
            command.Parameters[supplierIdParameter].IsNullable = true;
            command.Parameters[supplierIdParameter].Value = product.SupplierId is null ? DBNull.Value : product.SupplierId;

            const string categoryIdParameter = "@categoryId";
            command.Parameters.Add(categoryIdParameter, SqlDbType.Int);
            command.Parameters[categoryIdParameter].IsNullable = true;
            command.Parameters[categoryIdParameter].Value = product.CategoryId is null ? DBNull.Value : product.CategoryId;

            const string quantityPerUnitParameter = "@quantityPerUnit";
            command.Parameters.Add(quantityPerUnitParameter, SqlDbType.NVarChar, 20);
            command.Parameters[quantityPerUnitParameter].IsNullable = true;
            command.Parameters[quantityPerUnitParameter].Value = product.QuantityPerUnit is null ? DBNull.Value : product.QuantityPerUnit;

            const string unitPriceParameter = "@unitPrice";
            command.Parameters.Add(unitPriceParameter, SqlDbType.Money);
            command.Parameters[unitPriceParameter].IsNullable = true;
            command.Parameters[unitPriceParameter].Value = product.UnitPrice is null ? DBNull.Value : product.UnitPrice;

            const string unitsInStockParameter = "@unitsInStock";
            command.Parameters.Add(unitsInStockParameter, SqlDbType.SmallInt);
            command.Parameters[unitsInStockParameter].IsNullable = true;
            command.Parameters[unitsInStockParameter].Value = product.UnitsInStock is null ? DBNull.Value : product.UnitsInStock;

            const string unitsOnOrderParameter = "@unitsOnOrder";
            command.Parameters.Add(unitsOnOrderParameter, SqlDbType.SmallInt);
            command.Parameters[unitsOnOrderParameter].IsNullable = true;
            command.Parameters[unitsOnOrderParameter].Value = product.UnitsOnOrder is null ? DBNull.Value : product.UnitsOnOrder;

            const string reorderLevelParameter = "@reorderLevel";
            command.Parameters.Add(reorderLevelParameter, SqlDbType.SmallInt);
            command.Parameters[reorderLevelParameter].IsNullable = true;
            command.Parameters[reorderLevelParameter].Value = product.ReorderLevel is null ? DBNull.Value : product.ReorderLevel;

            const string discontinuedParameter = "@discontinued";
            command.Parameters.Add(discontinuedParameter, SqlDbType.Bit);
            command.Parameters[discontinuedParameter].Value = product.Discontinued;
        }

        private async Task OpenIfClosedAsync()
        {
            if (this.connection.State == ConnectionState.Open)
            {
                await this.connection.CloseAsync();
            }

            await this.connection.OpenAsync();
        }
    }
}