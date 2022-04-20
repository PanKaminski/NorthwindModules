using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NorthwindApp.FrontEnd.Mvc.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using NorthwindApp.FrontEnd.Mvc.Services;
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

            var category = await this.apiClient.GetCategoryAsync(id);

            return category is null ? this.View("Error") : this.View(category);
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
            var result = await this.apiClient.GetCategoryAsync(categoryId);

            return result is null ? this.View("Error") : this.View(new CategoryInputViewModel
            {
                Name = result.Name,
                Description = result.Description,
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> UpdateCategoryAsync(CategoryInputViewModel category, int categoryId)
        {
            var isUpdated = await this.apiClient.UpdateCategoryAsync(categoryId, category);

            if (!isUpdated)
            {
                this.View(category);
            }

            return this.RedirectToAction("GetCategory", new { id = categoryId });
        }

        [HttpGet("{categoryId}/picture")]
        public async Task<ActionResult> GetPicture(int categoryId)
        {
            return this.File(await this.apiClient.UploadImage(categoryId), "image/bmp");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> DeleteCategoryAsync(int categoryId)
        {
            var result = await this.apiClient.DeleteCategory(categoryId);

            if (!result)
            {
                return this.View("Error");
            }

            return this.RedirectToAction("Index", "Categories");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
