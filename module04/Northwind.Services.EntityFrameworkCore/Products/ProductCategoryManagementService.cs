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
    /// Represents category management service.
    /// </summary>
    public class ProductCategoryManagementService : IProductCategoryManagementService
    {
        private readonly NorthwindContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoryManagementService"/> class.
        /// </summary>
        /// <param name="dbContext">Database access layer.</param>
        public ProductCategoryManagementService(NorthwindContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <inheritdoc/>
        public async Task<int> CreateCategoryAsync(ProductCategory productCategory)
        {
            if (productCategory is null)
            {
                return -1;
            }

            if (await this.dbContext.ProductCategories.ContainsAsync(productCategory))
            {
                return -1;
            }

            await this.dbContext.ProductCategories.AddAsync(productCategory);
            await this.dbContext.SaveChangesAsync();

            return productCategory.Id;
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<ProductCategory> GetCategoriesByNameAsync(IList<string> names) =>
            this.dbContext.ProductCategories
                .Where(c => names.Contains(c.Name))
                .AsAsyncEnumerable();

        /// <inheritdoc/>
        public IAsyncEnumerable<ProductCategory> GetCategoriesAsync(int offset, int limit)
        {
            return this.dbContext.ProductCategories.Skip(offset).Take(limit).AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<ProductCategory> GetCategoriesAsync()
        {
            return this.dbContext.ProductCategories.AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public async Task<(bool, ProductCategory)> TryGetCategoryAsync(int categoryId)
        {
            var productCategory = this.dbContext.ProductCategories
                .SingleOrDefaultAsync(c => c.Id == categoryId);

            return (productCategory != null, await productCategory);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateCategoryAsync(int categoryId, ProductCategory productCategory)
        {
            if (productCategory is null)
            {
                return false;
            }

            var category = await this.dbContext.ProductCategories.SingleAsync(c => c.Id == categoryId);

            if (category is null)
            {
                return false;
            }

            category.Name = productCategory.Name;
            category.Description = productCategory.Description;
            category.Picture = productCategory.Picture;
            await this.dbContext.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyCategoryAsync(int categoryId)
        {
            var category = await this.dbContext.ProductCategories.FindAsync(categoryId);

            if (category is null)
            {
                return false;
            }

            this.dbContext.ProductCategories.Remove(category);
            await this.dbContext.SaveChangesAsync();

            return true;
        }
    }
}