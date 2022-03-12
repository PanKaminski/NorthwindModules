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
    /// Service for maintaining products through database.
    /// </summary>
    public class ProductManagementDataAccessService : IProductManagementService
    {
        private readonly IProductDataAccessObject productDao;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of <see cref="ProductManagementDataAccessService"/>.
        /// </summary>
        /// <param name="dataAccessFactory">Categories data access factory.</param>
        /// <param name="mapper">Mapper for mapping category from dto.</param>
        /// <exception cref="ArgumentNullException">Throws, if dataAccessFactory is null or mapper is null.</exception>
        public ProductManagementDataAccessService(NorthwindDataAccessFactory dataAccessFactory, IMapper mapper)
        {
            if (dataAccessFactory is null)
            {
                throw new ArgumentNullException(nameof(dataAccessFactory));
            }

            this.productDao = dataAccessFactory.GetProductDataAccessObject();
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<Product> GetProductsAsync()
        {
            await foreach (var productDto in this.productDao.SelectProductsAsync())
            {
                yield return this.mapper.Map<Product>(productDto);
            }
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<Product> GetProductsAsync(int offset, int limit)
        {
            try
            {
                return GetAsync(offset, limit);
            }
            catch (ArgumentException)
            {
                return GetAsync(0, 0);
            }

            async IAsyncEnumerable<Product> GetAsync(int skipped, int token)
            {
                await foreach (var productDto in this.productDao.SelectProductsAsync(skipped,token))
                {
                    yield return this.mapper.Map<Product>(productDto);
                }
            }
        }

        /// <inheritdoc/>
        public async Task<(bool, Product)> TryGetProductAsync(int productId)
        {
            try
            {
                var result = await this.productDao.FindProductAsync(productId);

                return (true, this.mapper.Map<Product>(result));
            }
            catch (ProductNotFoundException)
            {
                return (false, null);
            }
        }

        /// <inheritdoc/>
        public async Task<int> CreateProductAsync(Product product)
        {
            if (product is null)
            {
                return -1;
            }

            var contains = await this.TryGetProductAsync(product.Id);
            if (contains.Item1)
            {
                throw new InvalidOperationException("Product with such id already exists.");
            }

            return await this.productDao.InsertProductAsync(this.mapper.Map<ProductTransferObject>(product));
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyProductAsync(int productId) => await this.productDao.DeleteProductAsync(productId);

        public async IAsyncEnumerable<Product> GetProductsByNameAsync(IList<string> names)
        {
            await foreach (var product in this.productDao.SelectProductsByNameAsync(names))
            {
                yield return this.mapper.Map<Product>(product);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateProductAsync(int productId, Product product) =>
            await this.productDao.UpdateProductAsync(this.mapper.Map<ProductTransferObject>(product));

        /// <inheritdoc/>
        public async IAsyncEnumerable<Product> GetProductsForCategoryAsync(int categoryId)
        {
            if (categoryId < 1)
            {
                yield break;
            }

            await foreach (var product in this.productDao.SelectProductByCategoryAsync(new[] { categoryId }))
            {
                yield return this.mapper.Map<Product>(product);
            }
        }
    }
}