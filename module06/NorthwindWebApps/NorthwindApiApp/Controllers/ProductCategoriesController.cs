using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Entities;
using Northwind.Services.Products;

namespace NorthwindApiApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductCategoriesController : ControllerBase
    {
        private readonly IProductCategoryManagementService productCategoryService;
        private readonly IProductCategoryPicturesService picturesService;

        public ProductCategoriesController(IProductCategoryManagementService productCategoryService, IProductCategoryPicturesService picturesService)
        {
            this.productCategoryService = productCategoryService ?? throw new ArgumentNullException(nameof(productCategoryService));
            this.picturesService = picturesService ?? throw new ArgumentNullException(nameof(picturesService));
        }

        [HttpGet("{offset:int}/{limit:int}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public IAsyncEnumerable<Category> GetAsync(int offset, int limit) =>
            this.productCategoryService.GetCategoriesAsync(offset, limit);

        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public IAsyncEnumerable<Category> GetAsync() => this.productCategoryService.GetCategoriesAsync();

        [HttpGet("names")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public IAsyncEnumerable<Category> GetAsync(IList<string> names) =>
            this.productCategoryService.GetCategoriesByNameAsync(names);

        [HttpGet ("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Category>> GetAsync(int id)
        {
            if (id < 1)
            {
                return this.BadRequest();
            }

            var category = await this.productCategoryService.TryGetCategoryAsync(id);

            if (category.Item1)
            {
                return category.Item2;
            }

            return this.NotFound();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateAsync(Category productCategory)
        {
            try
            {
                var categoryId = await this.productCategoryService.CreateCategoryAsync(productCategory);

                if (categoryId < 1)
                {
                    return this.BadRequest();
                }

                return this.Ok(categoryId);

            }
            catch (InvalidOperationException e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateAsync(int id, Category productCategory)
        {
            if (productCategory is null)
            {
                return this.BadRequest();
            }

            if (id != productCategory.Id)
            {
                return this.BadRequest();
            }

            if (await this.productCategoryService.UpdateCategoryAsync(id, productCategory))
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
            if (await this.productCategoryService.DestroyCategoryAsync(id))
            {
                return this.NoContent();
            }

            return this.NotFound();
        }

        [HttpPut("{categoryId}/picture")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdatePictureAsync(int categoryId, IFormFile formFile)
        {
            if (categoryId < 1)
            {
                return this.BadRequest();
            }

            if (formFile is null)
            {
                return this.BadRequest();
            }

            if (!await this.picturesService.UpdatePictureAsync(categoryId, formFile.OpenReadStream()))
            {
                return this.NotFound();
            }

            return this.NoContent();
        }

        [HttpGet("{categoryId}/picture")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetPictureAsync(int categoryId)
        {
            var picture = await this.picturesService.TryGetPictureAsync(categoryId);
            if (picture.Item1)
            {
                return this.File(picture.Item2, "image/bmp");
            }

            return this.NotFound();
        }

        [HttpDelete("{categoryId}/picture")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeletePicture(int categoryId)
        {
            if (await this.picturesService.DestroyPictureAsync(categoryId))
            {
                return this.NoContent();
            }

            return this.NotFound();
        }
    }
}