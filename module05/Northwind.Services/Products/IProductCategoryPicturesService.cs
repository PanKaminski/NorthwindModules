using System.IO;
using System.Threading.Tasks;

namespace Northwind.Services.Products
{
    /// <summary>
    /// Provides methods for maintaining Northwind pictures.
    /// </summary>
    public interface IProductCategoryPicturesService
    {
        /// <summary>
        /// Try to show a product category picture.
        /// </summary>
        /// <param name="categoryId">A product category identifier.</param>
        /// <returns>True if a product category is exist; otherwise false.</returns>
        Task<(bool, byte[])> TryGetPictureAsync(int categoryId);

        /// <summary>
        /// Update a product category picture.
        /// </summary>
        /// <param name="categoryId">A product category identifier.</param>
        /// <param name="stream">A <see cref="Stream"/>.</param>
        /// <returns>True if a product category is exist; otherwise false.</returns>
        Task<bool> UpdatePictureAsync(int categoryId, Stream stream);

        /// <summary>
        /// Destroy a product category picture.
        /// </summary>
        /// <param name="categoryId">A product category identifier.</param>
        /// <returns>True if a product category is exist; otherwise false.</returns>
        Task<bool> DestroyPictureAsync(int categoryId);
    }
}
