using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.EntityFrameworkCore.Context;
using Northwind.Services.EntityFrameworkCore.Entities;
using Northwind.Services.Products;

namespace Northwind.Services.EntityFrameworkCore.Products
{
    /// <summary>
    /// Represents category management service.
    /// </summary>
    public class ProductCategoryManagementService : IProductCategoryManagementService
    {
        private readonly NorthwindContext dbContext;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoryManagementService"/> class.
        /// </summary>
        /// <param name="dbContext">Database access layer.</param>
        public ProductCategoryManagementService(NorthwindContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc/>
        public async Task<int> CreateCategoryAsync(ProductCategory productCategory)
        {
            if (productCategory is null)
            {
                return -1;
            }

            var categoryEntity = this.mapper.Map<Category>(productCategory);

            if (await this.dbContext.Categories.ContainsAsync(categoryEntity))
            {
                return -1;
            }

            await this.dbContext.Categories.AddAsync(categoryEntity);
            await this.dbContext.SaveChangesAsync();

            return categoryEntity.Id;
        }

        /// <inheritdoc/>
        public async Task<(bool, ProductCategory)> TryGetCategoryByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return (false, new ProductCategory());
            }

            var category = await this.dbContext.Categories.FirstOrDefaultAsync(c => c.Name == name);

            return (category is not null, this.mapper.Map<ProductCategory>(category));
        }

        public async Task<(bool, ProductCategory)> TryGetCategoryByProductAsync(int productId)
        {
            var product = await this.dbContext.Products
                .Include(p => p.Category)
                .SingleOrDefaultAsync(p => p.Id == productId);

            if (product?.Category is null)
            {
                return (false, new ProductCategory());
            }

            return (true, this.mapper.Map<ProductCategory>(product.Category));
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<ProductCategory> GetCategoriesAsync(int offset, int limit)
        {
            return this.dbContext.Categories
                .Skip(offset)
                .Take(limit)
                .Select(c => this.mapper.Map<ProductCategory>(c))
                .AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<ProductCategory> GetCategoriesAsync()
        {
            return this.dbContext.Categories.Select(c => this.mapper.Map<ProductCategory>(c)).AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public async Task<(bool, ProductCategory)> TryGetCategoryAsync(int categoryId)
        {
            var productCategory = await this.dbContext.Categories
                .SingleOrDefaultAsync(c => c.Id == categoryId);

            return (productCategory != null, this.mapper.Map<ProductCategory>(productCategory));
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateCategoryAsync(int categoryId, ProductCategory productCategory)
        {
            if (productCategory is null)
            {
                return false;
            }

            var categoryEntity = this.mapper.Map<Category>(productCategory);

            var category = await this.dbContext.Categories.SingleAsync(c => c.Id == categoryId);

            if (category is null)
            {
                return false;
            }

            category.Name = categoryEntity.Name;
            category.Description = categoryEntity.Description;
            category.Picture = categoryEntity.Picture;

            await this.dbContext.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<int> GetCategoriesCountAsync() => await this.dbContext.Categories.CountAsync();

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