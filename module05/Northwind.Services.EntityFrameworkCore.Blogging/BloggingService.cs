using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Blogging;
using Northwind.Services.EntityFrameworkCore.Blogging.Context;
using Northwind.Services.EntityFrameworkCore.Blogging.Entities;

namespace Northwind.Services.EntityFrameworkCore.Blogging
{
    /// <summary>
    /// Service for maintaining Northwind blogging articles through EF.
    /// </summary>
    public class BloggingService : IBloggingService
    {
        private readonly BloggingContext dbContext;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="BloggingService"/> class.
        /// </summary>
        /// <param name="dbContext">Entity framework database context.</param>
        /// <param name="mapper">Mapper for mapping blog articles models.</param>
        public BloggingService(BloggingContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<BlogArticleClientModel> GetBlogArticlesAsync()
        {
            return this.dbContext.BlogArticles
                .Select(a => this.mapper.Map<BlogArticleClientModel>(a))
                .AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<BlogArticleClientModel> GetBlogArticlesAsync(int offset, int limit)
        {
            return this.dbContext.BlogArticles
                .Skip(offset)
                .Take(limit)
                .Select(a => this.mapper.Map<BlogArticleClientModel>(a))
                .AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public async Task<(bool, BlogArticleClientModel)> TryGetBlogArticleAsync(int articleId)
        {
            var article = await this.dbContext.BlogArticles.FindAsync(articleId);

            return article is null ? (false, null) : (true, this.mapper.Map<BlogArticleClientModel>(article));
        }

        /// <inheritdoc/>
        public async Task<int> CreateArticleAsync(BlogArticleClientModel articleDto)
        {
            var article = this.mapper.Map<BlogArticle>(articleDto);
            article.PublicationDate = DateTime.Now;

            await this.dbContext.BlogArticles.AddAsync(article);
            await this.dbContext.SaveChangesAsync();

            return (await this.dbContext.BlogArticles.OrderBy(a => a.Id)
                .LastAsync(a => a.Title == article.Title && a.Content == article.Content && a.EmployeeId == article.EmployeeId))
                .Id;
        }

        /// <inheritdoc/>
        public async Task<bool> DestroyBlogArticleAsync(int articleId)
        {
            var article = await this.dbContext.BlogArticles.FindAsync(articleId);

            if (article is null)
            {
                return false;
            }

            var deleted = this.dbContext.BlogArticles.Remove(article);
            await this.dbContext.SaveChangesAsync();

            return deleted.Entity.Id == articleId;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateBlogArticleAsync(int articleId, BlogArticleClientModel articleDto)
        {
            if (articleDto is null)
            {
                throw new ArgumentNullException(nameof(articleDto));
            }

            var article = await this.dbContext.BlogArticles.FindAsync(articleId);

            if (article is null)
            {
                return false;
            }

            article.Title = articleDto.Title;
            article.Content = articleDto.Text;

            return await this.dbContext.SaveChangesAsync() > 0;
        }
    }
}
