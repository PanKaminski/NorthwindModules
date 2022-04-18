using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using NorthwindApp.FrontEnd.Mvc.Services;
using NorthwindApp.FrontEnd.Mvc.Services.Interfaces;
using NorthwindApp.FrontEnd.Mvc.ViewModels;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Products;

namespace NorthwindApp.FrontEnd.Mvc.Controllers
{
    public class ProductsController : Controller
    {
        private const int PageSize = 8;

        private readonly ILogger<ProductsController> logger;
        private readonly IProductsApiClient apiClient;
        private readonly ICategoriesApiClient categoriesApiClient;

        public ProductsController(ILogger<ProductsController> logger, IProductsApiClient apiClient, ICategoriesApiClient categoriesApiClient)
        {
            this.logger = logger;
            this.apiClient = apiClient;
            this.categoriesApiClient = categoriesApiClient;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = PageSize)
        {
            this.logger.LogDebug($"Request to method {nameof(ProductsController)}/Index.");

            var result = await this.apiClient.GetProductsAsync((page - 1) * pageSize, pageSize);

            return this.View(new PageListViewModel<ProductResponseViewModel>()
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

        public async Task<IActionResult> ShowCategoryProductsAsync(int categoryId, int page = 1, int pageSize = PageSize)
        {
            this.logger.LogDebug($"Request to method {nameof(ProductsController)}/ShowCategoryProducts with productId = {categoryId}.");

            var result = await this.apiClient.GetProductsByCategoryAsync(categoryId, (page - 1) * pageSize, pageSize);

            return this.View(new CategoryProductListViewModel
            {
                Items = result.Item2,
                PageInfo = new PageInfo
                {
                    CountOfPages = (int)Math.Ceiling((decimal)result.Item1 / pageSize),
                    CurrentPage = page,
                    ItemsPerPage = pageSize
                },
                CategoryId = categoryId,
            });
        }

        public async Task<IActionResult> ShowProductAsync(int id)
        {
            return this.View(await this.apiClient.GetProductAsync(id));
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> AddProductAsync()
        {
            this.ViewBag.productStatus = new SelectList(new [] {"Discontinued", "In production"});

            IList<string> categoryNames = new List<string>();

            await foreach (var category in this.categoriesApiClient.GetCategoriesAsync())
            {
                categoryNames.Add(category.Name);
            }

            this.ViewBag.categoryNames = new SelectList(categoryNames);

            return this.View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> AddProductAsync(ProductInputViewModel productModel)
        {
            var category = await this.categoriesApiClient.GetCategoryByNameAsync(productModel.CategoryName);

            var productId = await this.apiClient.CreateProductAsync(productModel, category?.Id);

            if (productId < 1)
            {
                return this.View(productModel);
            }

            return this.RedirectToAction("ShowProduct", new { id = productId });
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> UpdateProductAsync(int productId)
        {
            this.ViewBag.productId = productId;
            var result = await this.apiClient.GetProductAsync(productId);

            this.ViewBag.productStatus = new SelectList(new[] { "Discontinued", "In production" });

            IList<string> categoryNames = new List<string>();

            await foreach (var category in this.categoriesApiClient.GetCategoriesAsync())
            {
                categoryNames.Add(category.Name);
            }

            this.ViewBag.categoryNames = new SelectList(categoryNames);

            return result is null ? this.View("Error") : this.View(new ProductInputViewModel
            {
                Name = result.Name,
                QuantityPerUnit = result.QuantityPerUnit,
                UnitPrice = result.UnitPrice ?? 0,
                UnitsInStock = result.UnitsInStock,
                ProductStatus = result.Discontinued ? "Discontinued" : "In production",
                CategoryName = (await this.categoriesApiClient.GetProductByProductAsync(result.Id))?.Name,
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> UpdateProductAsync(ProductInputViewModel productModel, int productId)
        {
            var category = await this.categoriesApiClient.GetCategoryByNameAsync(productModel.CategoryName);

            var isUpdated = await this.apiClient.UpdateProductAsync(productId, productModel, category?.Id);

            if (!isUpdated)
            {
                this.View(productModel);
            }

            return this.RedirectToAction("ShowProduct", new { id = productId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> DeleteProductAsync(int productId)
        {
            var result = await this.apiClient.DeleteProductAsync(productId);

            if (!result)
            {
                return this.View("Error");
            }

            return this.RedirectToAction("Index", "Products");
        }

    }
}
