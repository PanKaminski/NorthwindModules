using System;
using System.IO;
using System.Threading.Tasks;
using Northwind.DataAccess;
using Northwind.DataAccess.Products;
using Northwind.Services.Products;

namespace Northwind.Services.DataAccess.Products
{
    /// <summary>
    /// Service for maintaining pictures through database.
    /// </summary>
    public class ProductCategoryPicturesManagementDataAccessService : IProductCategoryPicturesService
    {
        private readonly IProductCategoryDataAccessObject categoryDao;

        /// <summary>
        /// Initializes a new instance of <see cref="ProductCategoryPicturesManagementDataAccessService"/>.
        /// </summary>
        /// <param name="dataAccessFactory">Categories data access factory.</param>
        /// <exception cref="ArgumentNullException">Throws, if dataAccessFactory is null.</exception>
        public ProductCategoryPicturesManagementDataAccessService(NorthwindDataAccessFactory dataAccessFactory)
        {
            if (dataAccessFactory is null)
            {
                throw new ArgumentNullException(nameof(dataAccessFactory));
            }

            this.categoryDao = dataAccessFactory.GetProductCategoryDataAccessObject();
        }

        /// <inheritdoc/>
        public async Task<(bool, byte[])> TryGetPictureAsync(int categoryId)
        {
            try
            {
                var categoryDto = await this.categoryDao.FindProductCategoryAsync(categoryId);

                if (categoryDto.Picture is null || categoryDto.Picture.Length == 0)
                {
                    return (false, Array.Empty<byte>());
                }

                var result = new byte[categoryDto.Picture.Length - 78];
                Array.Copy(categoryDto.Picture, 78, result, 0, categoryDto.Picture.Length - 78);

                return (true, result);

            }
            catch (ProductCategoryNotFoundException)
            {
                return (false, Array.Empty<byte>());
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdatePictureAsync(int categoryId, Stream stream)
        {
            try
            {
                if (stream is null)
                {
                    throw new ArgumentNullException(nameof(stream));
                }

                var categoryDto = await this.categoryDao.FindProductCategoryAsync(categoryId);

                var bytes = new byte[stream.Length];
                await using (var memoryStream = new MemoryStream(bytes))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    await stream.CopyToAsync(memoryStream);
                    categoryDto.Picture = memoryStream.ToArray();
                }

                await this.categoryDao.UpdateProductCategoryAsync(categoryDto);
                return true;
            }
            catch (ProductCategoryNotFoundException)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyPictureAsync(int categoryId)
        {
            try
            {
                var categoryDto = await this.categoryDao.FindProductCategoryAsync(categoryId);
                categoryDto.Picture = null;
                await this.categoryDao.UpdateProductCategoryAsync(categoryDto);
                return true;
            }
            catch (ProductCategoryNotFoundException)
            {
                return false;
            }
        }
    }
}