using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Northwind.DataAccess;
using Northwind.DataAccess.Products;
using Northwind.Services.Entities;
using Northwind.Services.Products;

namespace Northwind.Services.DataAccess.Products
{
    /// <summary>
    /// Service for maintaining product categories through database.
    /// </summary>
    public class ProductCategoriesManagementDataAccessService : IProductCategoryManagementService
    {
        private readonly IProductCategoryDataAccessObject categoryDao;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of <see cref="ProductCategoriesManagementDataAccessService"/>.
        /// </summary>
        /// <param name="dataAccessFactory">Categories data access factory.</param>
        /// <param name="mapper">Mapper for mapping category from dto.</param>
        /// <exception cref="ArgumentNullException">Throws, if dataAccessFactory is null or mapper is null.</exception>
        public ProductCategoriesManagementDataAccessService(NorthwindDataAccessFactory dataAccessFactory, IMapper mapper)
        {
            if (dataAccessFactory is null)
            {
                throw new ArgumentNullException(nameof(dataAccessFactory));
            }

            this.categoryDao = dataAccessFactory.GetProductCategoryDataAccessObject();
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<Category> GetCategoriesAsync(int offset, int limit)
        {
            try
            {
                return GetAsync(offset, limit);
            }
            catch (ArgumentException)
            {
                return GetAsync(0, 0);
            }

            async IAsyncEnumerable<Category> GetAsync(int skipped, int token)
            {
                await foreach (var category in this.categoryDao.SelectProductCategoriesAsync(skipped, token))
                {
                    yield return this.mapper.Map<Category>(category);
                }
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<Category> GetCategoriesAsync()
        {
            await foreach (var category in this.categoryDao.SelectProductCategoriesAsync())
            {
                yield return this.mapper.Map<Category>(category);
            }
        }

        /// <inheritdoc/>
        public async Task<(bool, Category)> TryGetCategoryAsync(int categoryId)
        {
            try
            {
                var categoryDto = await this.categoryDao.FindProductCategoryAsync(categoryId);

                return (true, this.mapper.Map<Category>(categoryDto));
            }
            catch (ProductCategoryNotFoundException)
            {
                return (false, null);
            }
        }

        /// <inheritdoc/>
        public async Task<int> CreateCategoryAsync(Category productCategory)
        {
            if (productCategory is null)
            {
                return -1;
            }

            if ((await this.TryGetCategoryAsync(productCategory.Id)).Item1)
            {
                throw new InvalidOperationException("Category with such id already exists.");
            }

            return await this.categoryDao.InsertProductCategoryAsync(
                this.mapper.Map<ProductCategoryTransferObject>(productCategory));
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyCategoryAsync(int categoryId) =>
            await this.categoryDao.DeleteProductCategoryAsync(categoryId);

        /// <inheritdoc/>
        public async IAsyncEnumerable<Category> GetCategoriesByNameAsync(IList<string> names)
        {
            if (names is null || names.Count == 0)
            {
                yield break;
            }

            await foreach (var category in this.categoryDao.SelectProductCategoriesByNameAsync(names))
            {
                yield return this.mapper.Map<Category>(category);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateCategoryAsync(int categoryId, Category productCategory) =>
            await this.categoryDao.UpdateProductCategoryAsync(this.mapper.Map<ProductCategoryTransferObject>(productCategory));
    }
}