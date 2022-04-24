using System.Collections.Generic;
using System.Threading.Tasks;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Products;

namespace NorthwindApp.FrontEnd.Mvc.Services.Interfaces
{
    public interface IProductsApiClient
    {
        Task<(int, IEnumerable<ProductResponseViewModel>)> GetProductsByCategoryAsync(int categoryId, int offset, int limit);

        Task<(int, IEnumerable<ProductResponseViewModel>)> GetProductsAsync(int offset, int limit);

        Task<(int,ProductResponseViewModel)> GetProductAsync(int id);

        Task<int> CreateProductAsync(ProductInputViewModel productModel, int? categoryId);

        Task<int> UpdateProductAsync(int id, ProductInputViewModel productModel, int? categoryId);

        Task<int> DeleteProductAsync(int id);
    }
}
