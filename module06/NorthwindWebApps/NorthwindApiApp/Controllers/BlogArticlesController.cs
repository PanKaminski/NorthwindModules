using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Blogging;
using Northwind.Services.Customers;
using Northwind.Services.Employees;
using Northwind.Services.Products;

namespace NorthwindApiApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BlogArticlesController : ControllerBase
    {
        private readonly IEmployeeManagementService employeeService;
        private readonly IProductManagementService productService;
        private readonly ICustomersManagementService customerService;
        private readonly IBloggingService bloggingService;

        public BlogArticlesController(IEmployeeManagementService employeeService, IBloggingService bloggingService, 
            IProductManagementService productService, ICustomersManagementService customerService)
        {
            this.employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            this.bloggingService = bloggingService ?? throw new ArgumentNullException(nameof(bloggingService));
            this.productService = productService ?? throw new ArgumentNullException(nameof(productService));
            this.customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        }

        [HttpGet("{offset:int}/{limit:int}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async IAsyncEnumerable<BlogArticle> GetAsync(int offset, int limit)
        {
            await foreach (var article in this.bloggingService.GetBlogArticlesAsync(offset, limit))
            {
                var (_, employee) = await this.employeeService.TryGetEmployeeAsync(article.AuthorId);
                article.AuthorName = $"{employee.FirstName} {employee.LastName}, {employee.Title}";

                yield return article;
            }
        }

        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async IAsyncEnumerable<BlogArticle> GetAsync()
        {
            await foreach (var article in this.bloggingService.GetBlogArticlesAsync())
            {
                var (_, employee) = await this.employeeService.TryGetEmployeeAsync(article.AuthorId);
                article.AuthorName = $"{employee.FirstName} {employee.LastName}, {employee.Title}";

                yield return article;
            }
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> GetCountAsync() => await this.bloggingService.GetBlogArticlesCountAsync();

        [HttpGet("{articleId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BlogArticle>> GetAsync(int articleId)
        {
            if (articleId < 1)
            {
                return this.BadRequest();
            }

            var (retrievingArticleResult, article) = await this.bloggingService.TryGetBlogArticleAsync(articleId);

            if (!retrievingArticleResult)
            {
                return this.NotFound();
            }

            var(_, employee) = await this.employeeService.TryGetEmployeeAsync(article.AuthorId);
            article.AuthorName = $"{employee.FirstName} {employee.LastName}, {employee.Title}";

            return article;
        }
        
        [HttpGet("{articleId}/products")]
        public async IAsyncEnumerable<Product> GetRelatedProductsAsync(int articleId)
        {
            await foreach (var productLink in this.bloggingService.GetRelatedProducts(articleId))
            {
                var (retrievingResult, product) = await this.productService.TryGetProductAsync(productLink);

                if (retrievingResult)
                {
                    yield return product;
                }
            }
        }

        [HttpGet("{articleId}/comments")]
        public IAsyncEnumerable<BlogComment> GetCommentsAsync(int articleId)
        {
            return this.bloggingService.GetBlogArticleCommentsAsync(articleId);
        }

        [HttpGet("comments/{commentId}")]
        public async Task<ActionResult<BlogComment>> GetCommentAsync(int commentId)
        {
            if (commentId < 1)
            {
                return this.BadRequest();
            }

            var (retrievingArticleResult, comment) = await this.bloggingService.TryGetBlogCommentAsync(commentId);

            if (!retrievingArticleResult)
            {
                return this.NotFound();
            }

            return comment;
        }


        [HttpGet("{articleId}/comments/{offset:int}/{limit:int}")]
        public IAsyncEnumerable<BlogComment> GetCommentsAsync(int articleId, int offset, int limit)
        {
            return this.bloggingService.GetBlogArticleCommentsAsync(articleId, offset, limit);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateAsync(BlogArticle article)
        {
            var (retrievingEmployeeResult, _) = await this.employeeService.TryGetEmployeeAsync(article.AuthorId);

            if (!retrievingEmployeeResult)
            {
                return this.BadRequest();
            }

            var articleId = await this.bloggingService.CreateArticleAsync(article);

            if (articleId < 1)
            {
                return this.BadRequest();
            }

            return this.Ok(articleId);
        }

        [HttpPost("{articleId}/products/{productId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateProductLinkAsync(int articleId, int productId)
        {
            if (articleId < 1 || productId < 1)
            {
                return this.BadRequest();
            }

            if (!(await this.bloggingService.CreateLinkToProduct(articleId, productId)))
            {
                return this.BadRequest();
            }

            return this.Ok();
        }

        [HttpPost("{articleId}/comments")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateCommentAsync(int articleId, BlogComment comment)
        {
            if (articleId < 1 || comment is null)
            {
                return this.BadRequest();
            }

            if (!(await this.customerService.DoesExist(comment.CustomerId)))
            {
                return this.BadRequest();
            }

            var id = await this.bloggingService.CreateBlogArticleCommentAsync(articleId, comment);

            return this.Ok(id);
        }


        [HttpDelete("{articleId}/products/{productId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RemoveProductLinkAsync(int articleId, int productId)
        {
            if (articleId < 1 || productId < 1)
            {
                return this.BadRequest();
            }

            if (!(await this.bloggingService.RemoveLinkToProduct(articleId, productId)))
            {
                return this.BadRequest();
            }

            return this.Ok();
        }

        [HttpDelete("{articleId}/comments/{commentId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RemoveCommentAsync(int articleId, int commentId)
        {
            if (articleId < 1 || commentId < 1)
            {
                return this.BadRequest();
            }

            if (!(await this.bloggingService.DestroyBlogCommentAsync(articleId, commentId)))
            {
                return this.BadRequest();
            }

            return this.Ok();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateAsync(int id, BlogArticle blogArticle)
        {
            if (blogArticle is null)
            {
                return this.BadRequest();
            }

            if (await this.bloggingService.UpdateBlogArticleAsync(id, blogArticle))
            {
                return this.NoContent();
            }

            return this.NotFound();
        }

        [HttpPut("{articleId}/comments/{commentId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateCommentAsync(int articleId, int commentId, BlogComment comment)
        {
            if (comment is null)
            {
                return this.BadRequest();
            }

            if (await this.bloggingService.UpdateBlogCommentAsync(articleId, commentId, comment))
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
            if (await this.bloggingService.DestroyBlogArticleAsync(id))
            {
                return this.NoContent();
            }

            return this.NotFound();
        }
    }
}
