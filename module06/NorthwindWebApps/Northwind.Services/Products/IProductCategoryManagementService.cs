using System.Collections.Generic;
using System.Threading.Tasks;

namespace Northwind.Services.Products
{
    /// <summary>
    /// Provides methods for maintaining categories.
    /// </summary>
    public interface IProductCategoryManagementService
    {
        /// <summary>
        /// Shows a list of product categories using specified offset and limit for pagination.
        /// </summary>
        /// <param name="offset">An offset of the first element to return.</param>
        /// <param name="limit">A limit of elements to return.</param>
        /// <returns>A <see cref="IAsyncEnumerable{T}"/> of <see cref="ProductCategory"/>.</returns>
        IAsyncEnumerable<ProductCategory> GetCategoriesAsync(int offset, int limit);

        /// <summary>
        /// Shows a list of product categories.
        /// </summary>
        /// <returns>A <see cref="IAsyncEnumerable{T}"/> of <see cref="ProductCategory"/>.</returns>
        IAsyncEnumerable<ProductCategory> GetCategoriesAsync();

        /// <summary>
        /// Try to show a product category with specified identifier.
        /// </summary>
        /// <param name="categoryId">A product category identifier.</param>
        /// <returns>Returns true if a product category is returned; otherwise false.</returns>
        Task<(bool, ProductCategory)> TryGetCategoryAsync(int categoryId);

        /// <summary>
        /// Try to show a product category with specified identifier.
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        /// <returns>Returns true if a product category is returned; otherwise false.</returns>
        Task<(bool, ProductCategory)> TryGetCategoryByProductAsync(int productId);

        /// <summary>
        /// Creates a new product category.
        /// </summary>
        /// <param name="productCategory">A <see cref="ProductCategory"/> to create.</param>
        /// <returns>An identifier of a created product category.</returns>
        Task<int> CreateCategoryAsync(ProductCategory productCategory);

        /// <summary>
        /// Destroys an existed product category.
        /// </summary>
        /// <param name="categoryId">A product category identifier.</param>
        /// <returns>True if a product category is destroyed; otherwise false.</returns>
        Task<bool> DestroyCategoryAsync(int categoryId);

        /// <summary>
        /// Looks up for product categories with specified names.
        /// </summary>
        /// <param name="name">Category name.</param>
        /// <returns>A product category with specified name.</returns>
        Task<(bool, ProductCategory)> TryGetCategoryByNameAsync(string name);

        /// <summary>
        /// Updates a product category.
        /// </summary>
        /// <param name="categoryId">A product category identifier.</param>
        /// <param name="productCategory">A <see cref="ProductCategory"/>.</param>
        /// <returns>True if a product category is updated; otherwise false.</returns>
        Task<bool> UpdateCategoryAsync(int categoryId, ProductCategory productCategory);

        /// <summary>
        /// Gets count of categories.
        /// </summary>
        /// <returns>Count of categories.</returns>
        Task<int> GetCategoriesCountAsync();
    }
}