using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind.Services.Blogging;
using Northwind.Services.Employees;

namespace NorthwindApiApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BlogArticlesController : ControllerBase
    {
        private readonly IEmployeeManagementService employeeService;
        private readonly IBloggingService bloggingService;

        public BlogArticlesController(IEmployeeManagementService employeeService, IBloggingService bloggingService)
        {
            this.employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            this.bloggingService = bloggingService ?? throw new ArgumentNullException(nameof(bloggingService));
        }

        [HttpGet("{offset:int}/{limit:int}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async IAsyncEnumerable<BlogArticleClientModel> GetAsync(int offset, int limit)
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
        public async IAsyncEnumerable<BlogArticleClientModel> GetAsync()
        {
            await foreach (var article in this.bloggingService.GetBlogArticlesAsync())
            {
                var (_, employee) = await this.employeeService.TryGetEmployeeAsync(article.AuthorId);
                article.AuthorName = $"{employee.FirstName} {employee.LastName}, {employee.Title}";

                yield return article;
            }
        }

        [HttpGet("{articleId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BlogArticleClientModel>> GetAsync(int articleId)
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateAsync(BlogArticleClientModel article)
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

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateAsync(int id, BlogArticleClientModel blogArticle)
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
