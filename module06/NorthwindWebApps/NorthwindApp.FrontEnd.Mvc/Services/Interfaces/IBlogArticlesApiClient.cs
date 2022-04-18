using System.Collections.Generic;
using System.Threading.Tasks;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Blogging;

namespace NorthwindApp.FrontEnd.Mvc.Services.Interfaces
{
    public interface IBlogArticlesApiClient
    {
        Task<(int, IEnumerable<BlogArticleResponseViewModel>)> GetBlogArticles(int offset, int limit);

        Task<(BlogArticleInfoViewModel, bool)> GetBlogArticle(int articleId, int commentsOffset, int commentsLimit);

        Task<int> CreateBlogArticleAsync(BlogArticleInputViewModel articleModel, int employeeId);

        Task<int> CreateBlogCommentAsync(BlogCommentInputViewModel commentModel, int blogArticleId, string customerId);

        Task<(BlogCommentResponseViewModel, bool)> GetBlogCommentAsync(int commentId);

        Task<bool> DeleteBlogCommentAsync(int blogArticleId, int commentId);

        Task<bool> UpdateBlogArticleAsync(BlogArticleInputViewModel articleModel, int articleId);

        Task<bool> DeleteBlogArticleAsync(int blogArticleId);
    }
}
