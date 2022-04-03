using System.Collections.Generic;
using System.Threading.Tasks;

namespace Northwind.Services.Products
{
    /// <summary>
    /// Represents a management service for products.
    /// </summary>
    public interface IProductManagementService
    {
        /// <summary>
        /// Shows a list of products.
        /// </summary>
        /// <returns>A <see cref="IAsyncEnumerable{T}"/> of <see cref="Product"/>.</returns>
        IAsyncEnumerable<Product> GetProductsAsync();

        /// <summary>
        /// Shows a list of products using specified offset and limit for pagination.
        /// </summary>
        /// <param name="offset">An offset of the first element to return.</param>
        /// <param name="limit">A limit of elements to return.</param>
        /// <returns>A <see cref="IAsyncEnumerable{T}"/> of <see cref="Product"/>.</returns>
        IAsyncEnumerable<Product> GetProductsAsync(int offset, int limit);

        /// <summary>
        /// Try to show a product with specified identifier.
        /// </summary>
        /// <param name="productId">A product identifier.</param>
        /// <returns>Returns true with product if a product is returned, otherwise false with null.</returns>
        Task<(bool, Product)> TryGetProductAsync(int productId);

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="product">A <see cref="Product"/> to create.</param>
        /// <returns>An identifier of a created product.</returns>
        Task<int> CreateProductAsync(Product product);

        /// <summary>
        /// Destroys an existed product.
        /// </summary>
        /// <param name="productId">A product identifier.</param>
        /// <returns>True if a product is destroyed; otherwise false.</returns>
        Task<bool> DestroyProductAsync(int productId);

        /// <summary>
        /// Looks up for product with specified names.
        /// </summary>
        /// <param name="names">A list of product names.</param>
        /// <returns>A collection of products with specified names.</returns>
        IAsyncEnumerable<Product> GetProductsByNameAsync(IList<string> names);

        /// <summary>
        /// Updates a product.
        /// </summary>
        /// <param name="productId">A product identifier.</param>
        /// <param name="product">A <see cref="Product"/>.</param>
        /// <returns>True if a product is updated; otherwise false.</returns>
        Task<bool> UpdateProductAsync(int productId, Product product);

        /// <summary>
        /// Shows a list of products that belongs to a specified category.
        /// </summary>
        /// <param name="categoryId">A product category identifier.</param>
        /// <param name="offset">Count of products to skip.</param>
        /// <param name="limit">Count of products to take.</param>
        /// <returns>A <see cref="IAsyncEnumerable{T}"/> of <see cref="Product"/>.</returns>
        IAsyncEnumerable<Product> GetProductsForCategoryAsync(int categoryId, int offset, int limit);

        /// <summary>
        /// Gets count of products.
        /// </summary>
        /// <returns>Count of products.</returns>
        Task<int> GetProductsCountAsync();

        /// <summary>
        /// Gets count of products.
        /// </summary>
        /// <param name="categoryId">Id of the category.</param>
        /// <returns>Count of products.</returns>
        Task<int> GetProductsCountAsync(int categoryId);
    }
}