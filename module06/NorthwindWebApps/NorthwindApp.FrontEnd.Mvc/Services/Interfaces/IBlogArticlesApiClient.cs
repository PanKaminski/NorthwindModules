using System.Collections.Generic;
using System.Threading.Tasks;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Blogging;

namespace NorthwindApp.FrontEnd.Mvc.Services.Interfaces
{
    public interface IBlogArticlesApiClient
    {
        Task<(int, IEnumerable<BlogArticleResponseViewModel>)> GetBlogArticles(int offset, int limit);

        Task<(int, BlogArticleInfoViewModel)> GetBlogArticle(int articleId, int commentsOffset, int commentsLimit);

        Task<int> CreateBlogArticleAsync(BlogArticleInputViewModel articleModel, int employeeId);

        Task<int> CreateBlogCommentAsync(BlogCommentInputViewModel commentModel, int blogArticleId, string customerId);

        Task<(int, BlogCommentResponseViewModel)> GetBlogCommentAsync(int commentId);

        Task<int> DeleteBlogCommentAsync(int blogArticleId, int commentId);

        Task<int> UpdateBlogArticleAsync(BlogArticleInputViewModel articleModel, int articleId);

        Task<int> DeleteBlogArticleAsync(int blogArticleId);
    }
}
