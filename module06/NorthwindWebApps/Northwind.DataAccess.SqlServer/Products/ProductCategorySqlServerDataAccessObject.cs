using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Northwind.DataAccess.Products;

namespace Northwind.DataAccess.SqlServer.Products
{
    /// <summary>
    /// Represents a SQL Server-tailored DAO for Northwind product categories.
    /// </summary>
    public sealed class ProductCategorySqlServerDataAccessObject : IProductCategoryDataAccessObject
    {
        private readonly SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategorySqlServerDataAccessObject"/> class.
        /// </summary>
        /// <param name="connection">A <see cref="SqlConnection"/>.</param>
        public ProductCategorySqlServerDataAccessObject(SqlConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <inheritdoc/>
        public async Task<int> InsertProductCategoryAsync(ProductCategoryTransferObject productCategory)
        {
            if (productCategory is null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            await using var sqlCommand = new SqlCommand("Insert_Category", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(productCategory, sqlCommand);

            await this.OpenIfClosedAsync();
            return await sqlCommand.ExecuteNonQueryAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteProductCategoryAsync(int productCategoryId)
        {
            if (productCategoryId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productCategoryId));
            }

            await using var sqlCommand = new SqlCommand("Delete_Category", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string categoryId = "@categoryID";
            sqlCommand.Parameters.Add(categoryId, SqlDbType.Int);
            sqlCommand.Parameters[categoryId].Value = productCategoryId;

            await this.OpenIfClosedAsync();
            return await sqlCommand.ExecuteNonQueryAsync() > 0;
        }

        /// <inheritdoc/>
        public async Task<ProductCategoryTransferObject> FindProductCategoryAsync(int productCategoryId)
        {
            if (productCategoryId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(productCategoryId));
            }

            await using var sqlCommand = new SqlCommand("Get_Category_By_Id", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string categoryId = "@categoryId";
            sqlCommand.Parameters.Add(categoryId, SqlDbType.Int);
            sqlCommand.Parameters[categoryId].Value = productCategoryId;

            await this.OpenIfClosedAsync();

            var reader = await sqlCommand.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                throw new ProductCategoryNotFoundException(productCategoryId);
            }

            await reader.ReadAsync();

            return CreateProductCategory(reader);
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProductCategoryTransferObject> SelectProductCategoriesAsync(int offset, int limit)
        {
            if (offset < 0)
            {
                throw new ArgumentException("Offset must be greater than or equals zero.", nameof(offset));
            }

            if (limit < 0)
            {
                throw new ArgumentException("Limit must be greater than or equals zero.", nameof(limit));
            }

            await using var sqlCommand = new SqlCommand("Get_Categories_With_Limit", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            sqlCommand.Parameters.Add("@offset", SqlDbType.Int);
            sqlCommand.Parameters["@offset"].Value = offset;

            sqlCommand.Parameters.Add("@limit", SqlDbType.Int);
            sqlCommand.Parameters["@limit"].Value = limit;

            await this.OpenIfClosedAsync();

            var reader = await sqlCommand.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                yield return CreateProductCategory(reader);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProductCategoryTransferObject> SelectProductCategoriesAsync()
        {
            await using var sqlCommand = new SqlCommand("Get_Categories", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            await this.OpenIfClosedAsync();

            var reader = await sqlCommand.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                yield return CreateProductCategory(reader);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ProductCategoryTransferObject> SelectProductCategoriesByNameAsync(ICollection<string> productCategoryNames)
        {
            if (productCategoryNames == null)
            {
                throw new ArgumentNullException(nameof(productCategoryNames));
            }

            if (productCategoryNames.Count < 1)
            {
                throw new ArgumentException("Collection is empty.", nameof(productCategoryNames));
            }

            await using var sqlCommand = new SqlCommand("Get_Categories_By_Names", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            sqlCommand.Parameters.Add("@categoryNames", SqlDbType.NVarChar, 256);
            sqlCommand.Parameters["@categoryNames"].Value = string.Join(' ', productCategoryNames);

            await this.OpenIfClosedAsync();

            var reader = await sqlCommand.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                yield return CreateProductCategory(reader);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateProductCategoryAsync(ProductCategoryTransferObject productCategory)
        {
            if (productCategory == null)
            {
                throw new ArgumentNullException(nameof(productCategory));
            }

            await using var sqlCommand = new SqlCommand("Update_Category", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(productCategory, sqlCommand);

            const string categoryId = "@categoryId";
            sqlCommand.Parameters.Add(categoryId, SqlDbType.Int);
            sqlCommand.Parameters[categoryId].Value = productCategory.Id;

            await this.OpenIfClosedAsync();
            return await sqlCommand.ExecuteNonQueryAsync() > 0;
        }

        private static ProductCategoryTransferObject CreateProductCategory(SqlDataReader reader)
        {
            var id = (int)reader["CategoryID"];
            var name = (string)reader["CategoryName"];

            const string descriptionColumnName = "Description";
            string description = reader[descriptionColumnName] != DBNull.Value ? (string)reader["Description"] : null;

            const string pictureColumnName = "Picture";
            byte[] picture = reader[pictureColumnName] != DBNull.Value ? (byte[])reader["Picture"] : null;

            return new ProductCategoryTransferObject
            {
                Id = id,
                Name = name,
                Description = description,
                Picture = picture,
            };
        }

        private static void AddSqlParameters(ProductCategoryTransferObject productCategory, SqlCommand command)
        {
            const string categoryNameParameter = "@categoryName";
            command.Parameters.Add(categoryNameParameter, SqlDbType.NVarChar, 15);
            command.Parameters[categoryNameParameter].Value = productCategory.Name;

            const string descriptionParameter = "@description";
            command.Parameters.Add(descriptionParameter, SqlDbType.NText);
            command.Parameters[descriptionParameter].IsNullable = true;
            command.Parameters[descriptionParameter].Value = productCategory.Description is null ? DBNull.Value : productCategory.Description;

            const string pictureParameter = "@picture";
            command.Parameters.Add(pictureParameter, SqlDbType.Image);
            command.Parameters[pictureParameter].IsNullable = true;
            command.Parameters[pictureParameter].Value = productCategory.Picture is null ? DBNull.Value : productCategory.Picture;
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