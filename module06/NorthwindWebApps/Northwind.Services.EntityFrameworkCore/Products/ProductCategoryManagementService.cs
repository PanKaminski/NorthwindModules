using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Entities;
using Northwind.Services.EntityFrameworkCore.Context;
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
        public async Task<int> CreateCategoryAsync(Category productCategory)
        {
            if (productCategory is null)
            {
                return -1;
            }

            if (await this.dbContext.Categories.ContainsAsync(productCategory))
            {
                return -1;
            }

            await this.dbContext.Categories.AddAsync(productCategory);
            await this.dbContext.SaveChangesAsync();

            return productCategory.Id;
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<Category> GetCategoriesByNameAsync(IList<string> names) =>
            this.dbContext.Categories
                .Where(c => names.Contains(c.Name))
                .AsAsyncEnumerable();

        /// <inheritdoc/>
        public IAsyncEnumerable<Category> GetCategoriesAsync(int offset, int limit)
        {
            return this.dbContext.Categories.Skip(offset).Take(limit).AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<Category> GetCategoriesAsync()
        {
            return this.dbContext.Categories.AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public async Task<(bool, Category)> TryGetCategoryAsync(int categoryId)
        {
            var productCategory = this.dbContext.Categories
                .SingleOrDefaultAsync(c => c.Id == categoryId);

            return (productCategory != null, await productCategory);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateCategoryAsync(int categoryId, Category productCategory)
        {
            if (productCategory is null)
            {
                return false;
            }

            var category = await this.dbContext.Categories.SingleAsync(c => c.Id == categoryId);

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
            var category = await this.dbContext.Categories.FindAsync(categoryId);

            if (category is null)
            {
                return false;
            }

            this.dbContext.Categories.Remove(category);
            await this.dbContext.SaveChangesAsync();

            return true;
        }
    }
}