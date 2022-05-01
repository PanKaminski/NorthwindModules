using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindApp.FrontEnd.Mvc.Services.Interfaces;
using NorthwindApp.FrontEnd.Mvc.ViewModels;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Blogging;

namespace NorthwindApp.FrontEnd.Mvc.Controllers
{
    public class BloggingController : Controller
    {
        private const int ArticlesPageSize = 10;
        private const int CommentsPageSize = 5;
        private readonly IBlogArticlesApiClient bloggingApiClient;
        private readonly IUserManagementService userManagementService;

        public BloggingController(IBlogArticlesApiClient bloggingApiClient, IUserManagementService userManagementService/*, IdentityDbContext context*/)
        {
            this.bloggingApiClient = bloggingApiClient ?? throw new ArgumentNullException(nameof(bloggingApiClient));
            this.userManagementService =
                userManagementService ?? throw new ArgumentNullException(nameof(userManagementService));
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = ArticlesPageSize)
        {
            var articles = await this.bloggingApiClient.GetBlogArticles((page - 1) * pageSize, pageSize);

            return this.View(new PageListViewModel<BlogArticleResponseViewModel>
            {
                Items = articles.Item2,
                PageInfo = new PageInfo
                {
                    CountOfPages = (int)Math.Ceiling((decimal)articles.Item1 / pageSize),
                    CurrentPage = page,
                    ItemsPerPage = pageSize
                },
            });
        }

        public async Task<IActionResult> ShowBlogArticle(int articleId, int page = 1, int pageSize = CommentsPageSize)
        {
            var (statusCode, article) = await this.bloggingApiClient.GetBlogArticle(articleId, (page - 1) * pageSize, pageSize);
            var isAuth = this.User?.Identity?.IsAuthenticated;

            if (isAuth.HasValue && isAuth.Value)
            {
                if (this.User.IsInRole("Employee"))
                {
                    this.ViewBag.northwindUserId =
                        await this.userManagementService.GetNorthwindEmployeeIdAsync(this.User.Identity.Name);
                }
                else if(this.User.IsInRole("Customer"))
                {
                    this.ViewBag.northwindUserId =
                        (await this.userManagementService.GetNorthwindCustomerIdAsync(this.User.Identity.Name)).Item1;
                }
            }
            return statusCode != 200 ? this.View("Error", this.CreateErrorModel(statusCode)) : this.View(article);
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public IActionResult AddArticleAsync()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> AddArticleAsync(BlogArticleInputViewModel articleModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(articleModel);
            }

            var employeeId = await this.userManagementService.GetNorthwindEmployeeIdAsync(this.User?.Identity?.Name ?? "");

            if (employeeId < 0)
            {
                return this.View("Error");
            }

            var newArticleId = await this.bloggingApiClient.CreateBlogArticleAsync(articleModel, employeeId);

            return this.RedirectToAction("ShowBlogArticle", new { articleId = newArticleId });
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddCommentAsync(BlogCommentInputViewModel commentModel, int blogArticleId)
        {
            var (customerId, exists) = await this.userManagementService.GetNorthwindCustomerIdAsync(this.User?.Identity?.Name ?? "");

            if (!exists)
            {
                return this.Unauthorized();
            }

            var newCommentId = await this.bloggingApiClient.CreateBlogCommentAsync(commentModel, blogArticleId, customerId);
            JsonResult response = new JsonResult(true);
            return response;
        }

        public async Task<IActionResult> EditCommentAsync(BlogCommentEditViewModel commentModel, int blogArticleId)
        {
            var (customerId, exists) = await this.userManagementService.GetNorthwindCustomerIdAsync(this.User?.Identity?.Name ?? "");

            if (!exists)
            {
                return this.Unauthorized();
            }

            var resultStatusCode = await this.bloggingApiClient.EditBlogCommentAsync(commentModel, blogArticleId, customerId);

            if (resultStatusCode == 204)
            {
                JsonResult response = new JsonResult(true);
                return response;
            }
            else
            {
                return this.View("Error", this.CreateErrorModel(resultStatusCode));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DeleteCommentAsync(int commentId, int blogArticleId)
        {
            var (statusCode, comment) = await this.bloggingApiClient.GetBlogCommentAsync(commentId);

            if (statusCode != 200)
            {
                return this.View("Error", this.CreateErrorModel(statusCode));
            }

            var (customerId, exists) = await this.userManagementService.GetNorthwindCustomerIdAsync(this.User?.Identity?.Name ?? "");

            if (!exists || comment.CustomerId != customerId)
            {
                return this.View("Error", this.CreateErrorModel(441));
            }

            await this.bloggingApiClient.DeleteBlogCommentAsync(blogArticleId, commentId);

            return this.RedirectToAction("ShowBlogArticle", new { articleId = blogArticleId });
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> UpdateArticleAsync(int articleId)
        {
            this.ViewBag.articleId = articleId;
            var (statusCode, article) = await this.bloggingApiClient.GetBlogArticle(articleId, 0, CommentsPageSize);

            var employeeId = await this.userManagementService.GetNorthwindEmployeeIdAsync(this.User?.Identity?.Name ?? "");

            return statusCode != 204 || employeeId != article.AuthorId ? 
                this.View("Error", this.CreateErrorModel(statusCode)) : this.View(new BlogArticleInputViewModel
            {
                Title = article.Title,
                Text = article.Text,
            });
        }


        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> UpdateArticleAsync(BlogArticleInputViewModel articleModel, int articleId)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(articleModel);
            }

            await this.bloggingApiClient.UpdateBlogArticleAsync(articleModel, articleId);

            return this.RedirectToAction("ShowBlogArticle", new { articleId });
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> DeleteBlogArticleAsync(int blogArticleId)
        {
            var (statusCode, article) = await this.bloggingApiClient.GetBlogArticle(blogArticleId, 0, CommentsPageSize);

            if (statusCode != 200)
            {
                return this.RedirectToAction("Index", "Blogging");
            }

            var employeeId = await this.userManagementService.GetNorthwindEmployeeIdAsync(this.User?.Identity?.Name ?? "");

            if (article.AuthorId != employeeId)
            {
                return this.View("Error");
            }

            var result = await this.bloggingApiClient.DeleteBlogArticleAsync(blogArticleId);

            if (result != 204)
            {
                return this.View("Error");
            }

            return this.RedirectToAction("Index", "Blogging");
        }

        private ErrorViewModel CreateErrorModel(int statusCode)
        {
            var message = statusCode switch
            {
                404 => "Blog article wasn't fount.",
                400 => "Sorry for inconvenience. Try to send request again.",
                441 => "Such customer haven't registered.",
                _ => "Problem occurred during request. Sorry for inconvenience."
            };

            return new ErrorViewModel { ErrorMessage = message };
        }
    }
}
