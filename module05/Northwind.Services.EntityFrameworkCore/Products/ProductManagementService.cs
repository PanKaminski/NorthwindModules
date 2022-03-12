using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.EntityFrameworkCore.Data;
using Northwind.Services.Products;

namespace Northwind.Services.EntityFrameworkCore.Products
{
    /// <summary>
    /// Represents a stub for a product management service.
    /// </summary>
    public sealed class ProductManagementService : IProductManagementService
    {
        private readonly NorthwindContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductManagementService"/> class.
        /// </summary>
        /// <param name="dbContext">Database access layer.</param>
        public ProductManagementService(NorthwindContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <inheritdoc/>
        public async Task<int> CreateProductAsync(Product product)
        {
            if (product is null)
            {
                return -1;
            }

            if (await this.dbContext.Products.ContainsAsync(product))
            {
                return -1;
            }

            await this.dbContext.Products.AddAsync(product);
            await this.dbContext.SaveChangesAsync();

            return product.Id;
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyProductAsync(int productId)
        {
            var product = await this.dbContext.Products.FindAsync(productId);

            if (product is null)
            {
                return false;
            }

            this.dbContext.Products.Remove(product);
            await this.dbContext.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<Product> GetProductsByNameAsync(IList<string> names)
        {
            return this.dbContext.Products.Where(p => names.Contains(p.Name)).AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<Product> GetProductsAsync()
        {
            return this.dbContext.Products.AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<Product> GetProductsAsync(int offset, int limit)
        {
            return this.dbContext.Products.Skip(offset).Take(limit).AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<Product> GetProductsForCategoryAsync(int categoryId)
        {
            return this.dbContext.Products.Where(p => p.CategoryId == categoryId).AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public async Task<(bool, Product)> TryGetProductAsync(int productId)
        {
            var product = this.dbContext.Products
                .SingleOrDefaultAsync(c => c.Id == productId);

            return (product != null, await product);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateProductAsync(int productId, Product product)
        {
            if (product is null)
            {
                return false;
            }

            var productToChange = await this.dbContext.Products.SingleAsync(c => c.Id == productId);

            if (productToChange is null)
            {
                return false;
            }

            Map(productToChange, product);

            await this.dbContext.SaveChangesAsync();

            return true;
        }

        private static void Map(Product productToChange, Product source)
        {
            productToChange.CategoryId = source.CategoryId;
            productToChange.Discontinued = source.Discontinued;
            productToChange.Name = source.Name;
            productToChange.QuantityPerUnit = source.QuantityPerUnit;
            productToChange.ReorderLevel = source.ReorderLevel;
            productToChange.SupplierId = source.SupplierId;
            productToChange.UnitPrice = source.UnitPrice;
            productToChange.UnitsInStock = source.UnitsInStock;
            productToChange.UnitsOnOrder = source.UnitsOnOrder;
        }
    }
}