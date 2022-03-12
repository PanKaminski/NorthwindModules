using System.Collections.Generic;
using System.Threading.Tasks;

namespace Northwind.DataAccess.Products
{
    /// <summary>
    /// Represents a DAO for Northwind products.
    /// </summary>
    public interface IProductDataAccessObject
    {
        /// <summary>
        /// Inserts a new Northwind product to a data storage.
        /// </summary>
        /// <param name="product">A <see cref="ProductTransferObject"/>.</param>
        /// <returns>A data storage identifier of a new product.</returns>
        Task<int> InsertProductAsync(ProductTransferObject product);

        /// <summary>
        /// Deletes a Northwind product from a data storage.
        /// </summary>
        /// <param name="productId">An product identifier.</param>
        /// <returns>True if a product is deleted; otherwise false.</returns>
        Task<bool> DeleteProductAsync(int productId);

        /// <summary>
        /// Updates a Northwind product in a data storage.
        /// </summary>
        /// <param name="product">A <see cref="ProductTransferObject"/>.</param>
        /// <returns>True if a product is updated; otherwise false.</returns>
        Task<bool> UpdateProductAsync(ProductTransferObject product);

        /// <summary>
        /// Finds a Northwind product using a specified identifier.
        /// </summary>
        /// <param name="productId">A data storage identifier of an existed product.</param>
        /// <returns>A <see cref="ProductTransferObject"/> with specified identifier.</returns>
        Task<ProductTransferObject> FindProductAsync(int productId);

        /// <summary>
        /// Selects products using specified offset and limit.
        /// </summary>
        /// <param name="offset">An offset of the first object.</param>
        /// <param name="limit">A limit of returned objects.</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="ProductTransferObject"/>.</returns>
        IAsyncEnumerable<ProductTransferObject> SelectProductsAsync(int offset, int limit);

        /// <summary>
        /// Selects products.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of <see cref="ProductTransferObject"/>.</returns>
        IAsyncEnumerable<ProductTransferObject> SelectProductsAsync();

        /// <summary>
        /// Selects all Northwind products with specified names.
        /// </summary>
        /// <param name="productNames">A <see cref="IEnumerable{T}"/> of product names.</param>
        /// <returns>A <see cref="IAsyncEnumerable{T}"/> of <see cref="ProductTransferObject"/>.</returns>
        IAsyncEnumerable<ProductTransferObject> SelectProductsByNameAsync(ICollection<string> productNames);

        /// <summary>
        /// Selects all Northwind products that belongs to specified categories.
        /// </summary>
        /// <param name="collectionOfCategoryId">A <see cref="ICollection{T}"/> of category id.</param>
        /// <returns>A <see cref="IAsyncEnumerable{T}"/> of <see cref="ProductTransferObject"/>.</returns>
        IAsyncEnumerable<ProductTransferObject> SelectProductByCategoryAsync(ICollection<int> collectionOfCategoryId);
    }
}