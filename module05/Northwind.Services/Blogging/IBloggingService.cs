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
        IAsyncEnumerable<BlogArticleClientModel> GetBlogArticlesAsync();

        /// <summary>
        /// Gets the collection of articles.
        /// </summary>
        /// <param name="offset">An offset of the first articleDto to return.</param>
        /// <param name="limit">A limit of articles to return.</param>
        /// <returns>Collection of limit count of articles starting with offset position.</returns>
        IAsyncEnumerable<BlogArticleClientModel> GetBlogArticlesAsync(int offset, int limit);

        /// <summary>
        /// Get articleDto by id.
        /// </summary>
        /// <param name="articleId">Article id.</param>
        /// <returns>True and found articleDto, if articleDto with such id exists, otherwise false with null.</returns>
        Task<(bool, BlogArticleClientModel)> TryGetBlogArticleAsync(int articleId);

        /// <summary>
        /// Creates articleDto.
        /// </summary>
        /// <param name="articleDto">Blog articleDto to add into storage.</param>
        /// <returns>Id of the created articleDto.</returns>
        Task<int> CreateArticleAsync(BlogArticleClientModel articleDto);

        /// <summary>
        /// Removes blog articleDto from storage.
        /// </summary>
        /// <param name="articleId">Blog articleDto id.</param>
        /// <returns>True, if the articleDto was successfully removed, otherwise, false.</returns>
        Task<bool> DestroyBlogArticleAsync(int articleId);

        /// <summary>
        /// Updates articleDto.
        /// </summary>
        /// <param name="articleId">Blog articleDto id.</param>
        /// <param name="articleDto">New values.</param>
        /// <returns>True, if the articleDto was successfully updated, otherwise, false.</returns>
        Task<bool> UpdateBlogArticleAsync(int articleId, BlogArticleClientModel articleDto);
    }
}
