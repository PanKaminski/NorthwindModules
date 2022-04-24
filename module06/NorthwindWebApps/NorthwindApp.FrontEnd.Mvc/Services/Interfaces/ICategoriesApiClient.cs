using System.Collections.Generic;
using System.Threading.Tasks;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Categories;

namespace NorthwindApp.FrontEnd.Mvc.Services.Interfaces
{
    public interface ICategoriesApiClient
    {
        Task<(int, CategoryResponseViewModel)> GetCategoryAsync(int id);

        Task<(int, IEnumerable<CategoryResponseViewModel>)> GetCategoriesAsync(int offset, int limit);

        IAsyncEnumerable<CategoryResponseViewModel> GetCategoriesAsync();

        Task<(int, CategoryResponseViewModel)> GetCategoryByNameAsync(string name);

        Task<CategoryResponseViewModel> GetCategoryByProductAsync(int productId);

        Task<int> CreateCategoryAsync(CategoryInputViewModel category);

        Task<int> UpdateCategoryAsync(int id, CategoryInputViewModel category);

        Task<byte[]> UploadImageAsync(int categoryId);

        Task<int> DeleteCategoryAsync(int categoryId);
    }
}
