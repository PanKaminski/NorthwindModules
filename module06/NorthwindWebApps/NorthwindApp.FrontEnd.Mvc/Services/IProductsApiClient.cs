using System.Collections.Generic;
using System.Threading.Tasks;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Products;

namespace NorthwindApp.FrontEnd.Mvc.Services
{
    public interface IProductsApiClient
    {
        Task<(int, IEnumerable<ProductResponseViewModel>)> GetProductsByCategoryAsync(int categoryId, int offset, int limit);

        IAsyncEnumerable<ProductResponseViewModel> GetProductsAsync(int offset, int limit);

        Task<ProductResponseViewModel> GetProduct(int id);

        Task<int> AddProduct();

        Task<int> UpdateProduct(int id);

        Task<bool> DeleteProduct(int id);
    }
}
