using System.Collections.Generic;
using System.Threading.Tasks;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Categories;

namespace NorthwindApp.FrontEnd.Mvc.Services
{
    public interface ICategoriesApiClient
    {
        Task<CategoryResponseViewModel> GetCategoryAsync(int id);

        IAsyncEnumerable<CategoryResponseViewModel> GetCategoriesAsync(int offset, int limit);

        Task<int> CreateCategoryAsync(CategoryInputViewModel category);

        Task<bool> UpdateCategoryAsync(int id, CategoryInputViewModel category);
    }
}
