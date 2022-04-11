using System.Collections.Generic;
using System.Threading.Tasks;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Products;

namespace NorthwindApp.FrontEnd.Mvc.Services
{
    public interface IProductsApiClient
    {
        Task<(int, IEnumerable<ProductResponseViewModel>)> GetProductsByCategoryAsync(int categoryId, int offset, int limit);

        Task<(int, IEnumerable<ProductResponseViewModel>)> GetProductsAsync(int offset, int limit);

        Task<ProductResponseViewModel> GetProductAsync(int id);

        Task<int> CreateProductAsync(ProductInputViewModel productModel, int? categoryId);

        Task<bool> UpdateProductAsync(int id, ProductInputViewModel productModel, int? categoryId);

        Task<bool> DeleteProductAsync(int id);
    }
}
