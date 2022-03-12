using System.Collections.Generic;
using System.Threading.Tasks;
using Northwind.Services.Entities;

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
        /// <returns>A <see cref="IAsyncEnumerable{T}"/> of <see cref="Category"/>.</returns>
        IAsyncEnumerable<Category> GetCategoriesAsync(int offset, int limit);

        /// <summary>
        /// Shows a list of product categories.
        /// </summary>
        /// <returns>A <see cref="IAsyncEnumerable{T}"/> of <see cref="Category"/>.</returns>
        IAsyncEnumerable<Category> GetCategoriesAsync();

        /// <summary>
        /// Try to show a product category with specified identifier.
        /// </summary>
        /// <param name="categoryId">A product category identifier.</param>
        /// <returns>Returns true if a product category is returned; otherwise false.</returns>
        Task<(bool, Category)> TryGetCategoryAsync(int categoryId);

        /// <summary>
        /// Creates a new product category.
        /// </summary>
        /// <param name="productCategory">A <see cref="Category"/> to create.</param>
        /// <returns>An identifier of a created product category.</returns>
        Task<int> CreateCategoryAsync(Category productCategory);

        /// <summary>
        /// Destroys an existed product category.
        /// </summary>
        /// <param name="categoryId">A product category identifier.</param>
        /// <returns>True if a product category is destroyed; otherwise false.</returns>
        Task<bool> DestroyCategoryAsync(int categoryId);

        /// <summary>
        /// Looks up for product categories with specified names.
        /// </summary>
        /// <param name="names">A list of product category names.</param>
        /// <returns>A collection of product categories with specified names.</returns>
        IAsyncEnumerable<Category> GetCategoriesByNameAsync(IList<string> names);

        /// <summary>
        /// Updates a product category.
        /// </summary>
        /// <param name="categoryId">A product category identifier.</param>
        /// <param name="productCategory">A <see cref="ProductCategory"/>.</param>
        /// <returns>True if a product category is updated; otherwise false.</returns>
        Task<bool> UpdateCategoryAsync(int categoryId, Category productCategory);
    }
}