using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NorthwindApp.FrontEnd.Mvc.Services;
using NorthwindApp.FrontEnd.Mvc.ViewModels;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Products;

namespace NorthwindApp.FrontEnd.Mvc.Controllers
{
    public class ProductsController : Controller
    {
        private const int PageSize = 8;

        private readonly ILogger<ProductsController> logger;
        private readonly IProductsApiClient apiClient;

        public ProductsController(ILogger<ProductsController> logger, IProductsApiClient apiClient)
        {
            this.logger = logger;
            this.apiClient = apiClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ShowCategoryProductsAsync(int categoryId, int page = 1, int pageSize = PageSize)
        {
            this.logger.LogDebug($"Request to method {nameof(ProductsController)}/ShowCategoryProducts with categoryId = {categoryId}.");

            var result = await this.apiClient.GetProductsByCategoryAsync(categoryId, (page - 1) * pageSize, pageSize);

            return this.View(new ProductListViewModel
            {
                Products = result.Item2,
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
            return this.View(await this.apiClient.GetProduct(id));
        }

        
    }
}
