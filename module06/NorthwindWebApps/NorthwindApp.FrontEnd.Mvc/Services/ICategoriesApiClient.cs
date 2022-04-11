using System.Collections.Generic;
using System.Threading.Tasks;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Categories;

namespace NorthwindApp.FrontEnd.Mvc.Services
{
    public interface ICategoriesApiClient
    {
        Task<CategoryResponseViewModel> GetCategoryAsync(int id);

        IAsyncEnumerable<CategoryResponseViewModel> GetCategoriesAsync(int offset, int limit);

        IAsyncEnumerable<CategoryResponseViewModel> GetCategoriesAsync();

        Task<CategoryResponseViewModel> GetCategoryByNameAsync(string name);

        Task<CategoryResponseViewModel> GetProductByProductAsync(int productId);

        Task<int> CreateCategoryAsync(CategoryInputViewModel category);

        Task<bool> UpdateCategoryAsync(int id, CategoryInputViewModel category);

        Task<byte[]> UploadImage(int categoryId);

        Task<bool> DeleteCategory(int categoryId);
    }
}
