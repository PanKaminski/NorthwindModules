using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using NorthwindApp.FrontEnd.Mvc.Services.Interfaces;
using NorthwindApp.FrontEnd.Mvc.ViewModels;
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

            var result = await  this.apiClient.GetCategoriesAsync((page - 1) * pageSize, pageSize);

            return this.View(new PageListViewModel<CategoryResponseViewModel>()
            {
                Items = result.Item2,
                PageInfo = new PageInfo
                {
                    CountOfPages = (int)Math.Ceiling((decimal)result.Item1 / pageSize),
                    CurrentPage = page,
                    ItemsPerPage = pageSize
                },
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetCategory(int id)
        {
            this.logger.LogDebug($"Request to method {nameof(CategoriesController)}/GetCategory with value {nameof(id)} = {id}.");

            var (statusCode, category) = await this.apiClient.GetCategoryAsync(id);

            return statusCode != 200 ? this.View("Error", this.CreateErrorModel(statusCode)) : this.View(category);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Employee")]
        public IActionResult AddCategoryAsync() => this.View();

        [HttpPost]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> AddCategoryAsync(CategoryInputViewModel category)
        {
            var categoryId = await this.apiClient.CreateCategoryAsync(category);

            if (categoryId < 1)
            {
                return this.View(category);
            }

            return this.RedirectToAction("GetCategory", new { id = categoryId });
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> UpdateCategoryAsync(int categoryId)
        {
            this.ViewBag.categoryId = categoryId;
            var (statusCode, category) = await this.apiClient.GetCategoryAsync(categoryId);

            return statusCode != 200 ? this.View("Error", this.CreateErrorModel(statusCode)) :
                this.View(new CategoryInputViewModel
            {
                Name = category.Name,
                Description = category.Description,
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> UpdateCategoryAsync(CategoryInputViewModel category, int categoryId)
        {
            var statusCode = await this.apiClient.UpdateCategoryAsync(categoryId, category);

            if (statusCode != 204)
            {
                this.View("Error", this.CreateErrorModel(statusCode));
            }

            return this.RedirectToAction("GetCategory", new { id = categoryId });
        }

        [HttpGet("{categoryId}/picture")]
        public async Task<ActionResult> GetPicture(int categoryId)
        {
            return this.File(await this.apiClient.UploadImageAsync(categoryId), "image/bmp");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> DeleteCategoryAsync(int categoryId)
        {
            var statusCode = await this.apiClient.DeleteCategoryAsync(categoryId);

            if (statusCode != 204)
            {
                this.View("Error", this.CreateErrorModel(statusCode));
            }

            return this.RedirectToAction("Index", "Categories");
        }

        private ErrorViewModel CreateErrorModel(int statusCode)
        {
            var message = statusCode switch
            {
                404 => "Category not fount.",
                400 => "Sorry for inconvenience. Try to send request again.",
                _ => "Problem occurred during request. Sorry for inconvenience."
            };

            return new ErrorViewModel { ErrorMessage = message };
        }
    }
}
