using System.Collections.Generic;
using System.Threading.Tasks;

namespace Northwind.Services.Blogging
{
    /// <summary>
    /// Provides methods for maintaining Northwind blogging articles.
    /// </summary>
    public interface IBloggingService
    {
        /// <summary>
        /// Gets the collection of articles.
        /// </summary>
        /// <returns>All articles.</returns>
        IAsyncEnumerable<BlogArticle> GetBlogArticlesAsync();

        /// <summary>
        /// Gets the collection of articles.
        /// </summary>
        /// <param name="offset">An offset of the first articleDto to return.</param>
        /// <param name="limit">A limit of articles to return.</param>
        /// <returns>Collection of limit count of articles starting with offset position.</returns>
        IAsyncEnumerable<BlogArticle> GetBlogArticlesAsync(int offset, int limit);

        /// <summary>
        /// Gets the collection of articles.
        /// </summary>
        /// <param name="articleId">Blog article id.</param>
        /// <returns>All blog article comments.</returns>
        IAsyncEnumerable<BlogComment> GetBlogArticleCommentsAsync(int articleId);

        /// <summary>
        /// Gets the collection of blog article comments.
        /// </summary>
        /// <param name="articleId">Blog article id.</param>
        /// <param name="offset">An offset of the first articleDto to return.</param>
        /// <param name="limit">A limit of articles to return.</param>
        /// <returns>Collection of limit count of comments starting with offset position.</returns>
        IAsyncEnumerable<BlogComment> GetBlogArticleCommentsAsync(int articleId, int offset, int limit);

        /// <summary>
        /// Get article by id.
        /// </summary>
        /// <param name="articleId">Article id.</param>
        /// <returns>True and found article, if article with such id exists, otherwise false with null.</returns>
        Task<(bool, BlogArticle)> TryGetBlogArticleAsync(int articleId);

        /// <summary>
        /// Get comment by id.
        /// </summary>
        /// <param name="commentId">Blog comment id.</param>
        /// <returns>True and found comment, if comment with such id exists, otherwise false with null.</returns>
        Task<(bool, BlogComment)> TryGetBlogCommentAsync(int commentId);

        /// <summary>
        /// Creates new article.
        /// </summary>
        /// <param name="articleDto">Blog articleDto to add into storage.</param>
        /// <returns>Id of the created article.</returns>
        Task<int> CreateArticleAsync(BlogArticle articleDto);

        /// <summary>
        /// Creates new comment.
        /// </summary>
        /// <param name="articleId">Article id.</param>
        /// <param name="comment">Blog comment to add into storage.</param>
        /// <returns>Id of the created comment.</returns>
        Task<int> CreateBlogArticleCommentAsync(int articleId, BlogComment comment);

        /// <summary>
        /// Removes blog article from storage.
        /// </summary>
        /// <param name="articleId">Blog article id.</param>
        /// <returns>True, if the article was successfully removed, otherwise, false.</returns>
        Task<bool> DestroyBlogArticleAsync(int articleId);

        /// <summary>
        /// Removes blog comment from storage.
        /// </summary>
        /// <param name="articleId">Article id.</param>
        /// <param name="commentId">Id of the blog comment to remove from storage.</param>
        /// <returns>True, if the article comment was successfully removed, otherwise, false.</returns>
        Task<bool> DestroyBlogCommentAsync(int articleId, int commentId);

        /// <summary>
        /// Updates article.
        /// </summary>
        /// <param name="articleId">Blog article id.</param>
        /// <param name="articleDto">New values.</param>
        /// <returns>True, if the article was successfully updated, otherwise, false.</returns>
        Task<bool> UpdateBlogArticleAsync(int articleId, BlogArticle articleDto);

        /// <summary>
        /// Updates article comment.
        /// </summary>
        /// <param name="articleId">Article id.</param>
        /// <param name="commentId">Id of the blog comment to remove from storage.</param>
        /// <param name="comment">Blog comment to update.</param>
        /// <returns>True, if the article comment was successfully updated, otherwise, false.</returns>
        Task<bool> UpdateBlogCommentAsync(int articleId, int commentId, BlogComment comment);

        /// <summary>
        /// Gets links to related products by article id.
        /// </summary>
        /// <param name="articleId">Blog article id.</param>
        /// <returns>Ids of related products.</returns>
        IAsyncEnumerable<int> GetRelatedProducts(int articleId);

        /// <summary>
        /// Adds link to product for blog article.
        /// </summary>
        /// <param name="articleId">Blog article.</param>
        /// <param name="productsId">Product id.</param>
        /// <returns>True, if link to product was added, otherwise, false.</returns>
        Task<bool> CreateLinkToProduct(int articleId, int productsId);

        /// <summary>
        /// Deletes link to product from blog article.
        /// </summary>
        /// <param name="articleId">Blog article.</param>
        /// <param name="productsId">Product id.</param>
        /// <returns>True, if link to product was deleted, otherwise, false.</returns>
        Task<bool> RemoveLinkToProduct(int articleId, int productsId);

        /// <summary>
        /// Gets count of blog articles.
        /// </summary>
        /// <returns>Count of blog articles.</returns>
        Task<int> GetBlogArticlesCountAsync();
    }
}
