using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.EntityFrameworkCore.Context;
using Northwind.Services.Products;

namespace Northwind.Services.EntityFrameworkCore.Products
{
    /// <summary>
    /// Represents a stub for a product management service.
    /// </summary>
    public sealed class ProductManagementService : IProductManagementService
    {
        private readonly NorthwindContext dbContext;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductManagementService"/> class.
        /// </summary>
        /// <param name="dbContext">Database access layer.</param>
        public ProductManagementService(NorthwindContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<int> CreateProductAsync(Product product)
        {
            if (product is null)
            {
                return -1;
            }

            var productEntity = this.mapper.Map<Entities.Product>(product);

            if (await this.dbContext.Products.ContainsAsync(productEntity))
            {
                return -1;
            }

            await this.dbContext.Products.AddAsync(productEntity);
            await this.dbContext.SaveChangesAsync();

            return productEntity.Id;
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
            return this.dbContext.Products
                .Where(p => names.Contains(p.Name))
                .Select(p => this.mapper.Map<Product>(p))
                .AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<Product> GetProductsAsync()
        {
            return this.dbContext.Products.Select(p => this.mapper.Map<Product>(p)).AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<Product> GetProductsAsync(int offset, int limit)
        {
            return this.dbContext.Products.Skip(offset).Take(limit).Select(p => this.mapper.Map<Product>(p)).AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<Product> GetProductsForCategoryAsync(int categoryId)
        {
            return this.dbContext.Products
                .Where(p => p.CategoryId == categoryId)
                .Select(p => this.mapper.Map<Product>(p))
                .AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public async Task<(bool, Product)> TryGetProductAsync(int productId)
        {
            var product = await this.dbContext.Products
                .SingleOrDefaultAsync(c => c.Id == productId);

            return (product != null, this.mapper.Map<Product>(product));
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

        private static void Map(Entities.Product productToChange, Product source)
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