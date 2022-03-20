using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.EntityFrameworkCore.Context;
using Northwind.Services.Products;

namespace Northwind.Services.EntityFrameworkCore.Products
{
    /// <summary>
    /// Represents picture management service.
    /// </summary>
    public class ProductCategoryPicturesService : IProductCategoryPicturesService
    {
        private readonly NorthwindContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoryPicturesService"/> class.
        /// </summary>
        /// <param name="dbContext">Database access layer.</param>
        public ProductCategoryPicturesService(NorthwindContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <inheritdoc/>
        public async Task<bool> UpdatePictureAsync(int categoryId, Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var category = await this.dbContext.Categories.SingleOrDefaultAsync(c => c.Id == categoryId);

            if (category is null)
            {
                return false;
            }

            var bytes = new byte[stream.Length];

            using (var memoryStream = new MemoryStream(bytes))
            {
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(memoryStream);
                category.Picture = memoryStream.ToArray();
                await this.dbContext.SaveChangesAsync();

                return true;
            }
        }

        /// <inheritdoc/>
        public async Task<(bool, byte[])> TryGetPictureAsync(int categoryId)
        {
            var productCategory = await this.dbContext.Categories
                .SingleOrDefaultAsync(c => c.Id == categoryId);

            if (productCategory.Picture is null || productCategory.Picture.Length == 0)
            {
                return (false, null);
            }

            var result = new byte[productCategory.Picture.Length - 78];
            Array.Copy(productCategory.Picture, 78, result, 0, productCategory.Picture.Length - 78);

            return (true, result);
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyPictureAsync(int categoryId)
        {
            var category = await this.dbContext.Categories.SingleOrDefaultAsync(c => c.Id == categoryId);

            if (category is null)
            {
                return false;
            }

            category.Picture = null;
            await this.dbContext.SaveChangesAsync();

            return true;
        }
    }
}
