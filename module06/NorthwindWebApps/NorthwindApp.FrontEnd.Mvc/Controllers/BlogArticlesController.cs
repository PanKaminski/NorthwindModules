using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindApp.FrontEnd.Mvc.Models;
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
            var model = await this.bloggingApiClient.GetBlogArticle(articleId, (page - 1) * pageSize, pageSize);
            var isAuth = this.User?.Identity?.IsAuthenticated;

            if (isAuth.HasValue && isAuth.Value)
            {
                if (this.User.IsInRole("Employee"))
                {
                    this.ViewBag.northwindUserId =
                        await this.userManagementService.GetNorthwindEmployeeId(this.User.Identity.Name);
                }
                else if(this.User.IsInRole("Customer"))
                {
                    this.ViewBag.northwindUserId =
                        await this.userManagementService.GetNorthwindCustomerId(this.User.Identity.Name);
                }
            }

            if (!model.Item2)
            {
                return this.Error();
            }

            return this.View(model.Item1);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Employee")]
        public IActionResult AddArticleAsync()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> AddArticleAsync(BlogArticleInputViewModel articleModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(articleModel);
            }

            var employeeId = await this.userManagementService.GetNorthwindEmployeeId(this.User?.Identity?.Name ?? "");

            if (employeeId < 0)
            {
                return this.Error();
            }

            var newArticleId = await this.bloggingApiClient.CreateBlogArticleAsync(articleModel, employeeId);

            return this.RedirectToAction("ShowBlogArticle", new { articleId = newArticleId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Customer")]
        public async Task<IActionResult> AddCommentAsync(BlogCommentInputViewModel commentModel, int blogArticleId)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(commentModel);
            }

            var (customerId, exists) = await this.userManagementService.GetNorthwindCustomerId(this.User?.Identity?.Name ?? "");

            if (!exists)
            {
                return this.Error();
            }

            var newCommentId = await this.bloggingApiClient.CreateBlogCommentAsync(commentModel, blogArticleId, customerId);

            return this.RedirectToAction("ShowBlogArticle", new { articleId = newCommentId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Customer")]
        public async Task<IActionResult> DeleteCommentAsync(int commentId, int blogArticleId)
        {
            var comment = await this.bloggingApiClient.GetBlogCommentAsync(commentId);

            if (!comment.Item2)
            {
                return this.Error();
            }

            var (customerId, exists) = await this.userManagementService.GetNorthwindCustomerId(this.User?.Identity?.Name ?? "");

            if (!exists || comment.Item1.CustomerId != customerId)
            {
                return this.Error();
            }

            await this.bloggingApiClient.DeleteBlogCommentAsync(blogArticleId, commentId);

            return this.RedirectToAction("ShowBlogArticle", new { articleId = blogArticleId });
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> UpdateArticleAsync(int articleId)
        {
            this.ViewBag.articleId = articleId;
            var result = await this.bloggingApiClient.GetBlogArticle(articleId, 0, CommentsPageSize);

            var employeeId = await this.userManagementService.GetNorthwindEmployeeId(this.User?.Identity?.Name ?? "");

            return !result.Item2 || employeeId != result.Item1.AuthorId ? 
                this.View("Error") : this.View(new BlogArticleInputViewModel
            {
                Title = result.Item1.Title,
                Text = result.Item1.Text,
            });
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Employee")]
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
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> DeleteBlogArticleAsync(int blogArticleId)
        {
            var article = await this.bloggingApiClient.GetBlogArticle(blogArticleId, 0, CommentsPageSize);

            if (!article.Item2)
            {
                return this.RedirectToAction("Index", "Blogging");
            }

            var employeeId = await this.userManagementService.GetNorthwindEmployeeId(this.User?.Identity?.Name ?? "");

            if (article.Item1.AuthorId != employeeId)
            {
                return this.View("Error");
            }

            var result = await this.bloggingApiClient.DeleteBlogArticleAsync(blogArticleId);

            if (!result)
            {
                return this.View("Error");
            }

            return this.RedirectToAction("Index", "Blogging");
        }
    }
}
