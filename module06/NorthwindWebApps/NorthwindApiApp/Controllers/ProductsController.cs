using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Products;

namespace NorthwindApiApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class ProductsController : ControllerBase
    {
        private readonly IProductManagementService productService;

        public ProductsController(IProductManagementService productService)
        {
            this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public IAsyncEnumerable<Product> GetAsync() => this.productService.GetProductsAsync();

        [HttpGet("{offset:int}/{limit:int}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public IAsyncEnumerable<Product> GetAsync(int offset, int limit) =>
            this.productService.GetProductsAsync(offset, limit);

        [HttpGet("category/{categoryId:int}/{offset:int}/{limit:int}")]
        public IAsyncEnumerable<Product> GetByCategoryAsync(int categoryId, int offset, int limit) => 
            this.productService.GetProductsForCategoryAsync(categoryId, offset, limit);

        [HttpGet("names")]
        public IAsyncEnumerable<Product> GetAsync(IList<string> names) =>
            this.productService.GetProductsByNameAsync(names);

        [HttpGet("category/{categoryId:int}/count")]
        public async Task<ActionResult<int>> GetCountAsync(int categoryId) => await this.productService.GetProductsCountAsync(categoryId);

        [HttpGet("count")]
        public async Task<ActionResult<int>> GetCountAsync() => await this.productService.GetProductsCountAsync();

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Product>> GetAsync(int id)
        {
            if (id < 1)
            {
                return this.BadRequest();
            }

            var product = await this.productService.TryGetProductAsync(id);

            if (product.Item1)
            {
                return product.Item2;
            }

            return this.NotFound();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateAsync(Product product)
        {
            try
            {
                var productId = await this.productService.CreateProductAsync(product);

                if (productId < 0)
                {
                    return this.BadRequest();
                }

                return this.Ok(productId);
            }
            catch (InvalidOperationException e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateAsync(int id, Product product)
        {
            if (product is null)
            {
                return this.BadRequest();
            }

            if (await this.productService.UpdateProductAsync(id, product))
            {
                return this.NoContent();
            }

            return this.NotFound();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            if (await this.productService.DestroyProductAsync(id))
            {
                return this.NoContent();
            }

            return this.NotFound();
        }
    }
}