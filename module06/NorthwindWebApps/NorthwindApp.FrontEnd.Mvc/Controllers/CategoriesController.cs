using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NorthwindApp.FrontEnd.Mvc.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using NorthwindApp.FrontEnd.Mvc.Services;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Categories;

namespace NorthwindApp.FrontEnd.Mvc.Controllers
{
    public class CategoriesController : Controller
    {
        private const int PageSize = 10;
        private readonly ILogger<CategoriesController> logger;
        private readonly ICategoriesApiClient apiClient;

        public CategoriesController(ILogger<CategoriesController> logger, ICategoriesApiClient apiClient)
        {
            this.logger = logger;
            this.apiClient = apiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = PageSize)
        {
            this.logger.LogDebug($"Request to method {nameof(CategoriesController)}/Index with value {nameof(page)} = {page}.");

            IList<CategoryResponseViewModel> categories = new List<CategoryResponseViewModel>();

            await foreach (var category in this.apiClient.GetCategoriesAsync((page - 1) * pageSize, pageSize))
            {
                categories.Add(category);
            }
            
            return this.View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> GetCategory(int id)
        {
            this.logger.LogDebug($"Request to method {nameof(CategoriesController)}/GetCategory with value {nameof(id)} = {id}.");

            var category = await this.apiClient.GetCategoryAsync(id);

            return this.View(category);
        }

        [HttpGet]
        public IActionResult AddCategoryAsync() => this.View();

        [HttpPost]
        public async Task<IActionResult> AddCategoryAsync(CategoryInputViewModel category)
        {
            var categoryId = await this.apiClient.CreateCategoryAsync(category);
            return RedirectToAction("GetCategory", new { id = categoryId });
        }

        [HttpGet]
        public IActionResult UpdateCategoryAsync() => this.View();

        [HttpGet("{categoryId}/picture")]
        public async Task<ActionResult> GetPicture(int categoryId)
        {
            return this.File(await this.apiClient.UploadImage(categoryId), "image/bmp");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategoryAsync(int categoryId, CategoryInputViewModel category)
        {
            var isUpdated = await this.apiClient.UpdateCategoryAsync(categoryId, category);

            if (!isUpdated)
            {
                this.View();
            }

            return RedirectToAction("GetCategory", new { id = categoryId });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
