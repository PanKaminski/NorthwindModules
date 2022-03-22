using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Blogging;
using Northwind.Services.EntityFrameworkCore.Blogging.Context;

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
        public IAsyncEnumerable<BlogArticle> GetBlogArticlesAsync()
        {
            return this.dbContext.BlogArticles
                .Select(a => this.mapper.Map<BlogArticle>(a))
                .AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<BlogArticle> GetBlogArticlesAsync(int offset, int limit)
        {
            return this.dbContext.BlogArticles
                .Skip(offset)
                .Take(limit)
                .Select(a => this.mapper.Map<BlogArticle>(a))
                .AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<BlogComment> GetBlogArticleCommentsAsync(int articleId)
        {
            return this.dbContext.BlogArticleComments
                .Where(c => c.ArticleId == articleId)
                .Select(c => this.mapper.Map<BlogComment>(c))
                .AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<BlogComment> GetBlogArticleCommentsAsync(int articleId, int offset, int limit)
        {
            return this.dbContext.BlogArticleComments
                .Where(c => c.ArticleId == articleId)
                .Skip(offset)
                .Take(limit)
                .Select(c => this.mapper.Map<BlogComment>(c))
                .AsAsyncEnumerable();
        }

        /// <inheritdoc/>
        public async Task<(bool, BlogArticle)> TryGetBlogArticleAsync(int articleId)
        {
            var article = await this.dbContext.BlogArticles.FindAsync(articleId);

            return article is null ? (false, null) : (true, this.mapper.Map<BlogArticle>(article));
        }

        /// <inheritdoc/>
        public async Task<int> CreateArticleAsync(BlogArticle articleDto)
        {
            var article = this.mapper.Map<Entities.BlogArticle>(articleDto);
            article.PublicationDate = DateTime.Now;

            await this.dbContext.BlogArticles.AddAsync(article);
            await this.dbContext.SaveChangesAsync();

            return article.Id;
        }

        /// <inheritdoc/>
        public async Task<int> CreateBlogArticleCommentAsync(int articleId, BlogComment comment)
        {
            var article = await this.dbContext.BlogArticles.FindAsync(articleId);

            if (article is null)
            {
                return -1;
            }

            var blogCommentDto = this.mapper.Map<Entities.BlogComment>(comment);
            blogCommentDto.ArticleId = articleId;
            blogCommentDto.BlogArticle = article;

            await this.dbContext.BlogArticleComments.AddAsync(blogCommentDto);
            await this.dbContext.SaveChangesAsync();

            return blogCommentDto.Id;
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
        public async Task<bool> DestroyBlogCommentAsync(int articleId, int commentId)
        {
            var article = await this.dbContext.BlogArticles.FindAsync(articleId);

            if (article is null)
            {
                return false;
            }

            var comment = await this.dbContext.BlogArticleComments.FindAsync(commentId);

            if (comment is null)
            {
                return false;
            }

            var result = this.dbContext.BlogArticleComments.Remove(comment);
            await this.dbContext.SaveChangesAsync();

            return result.Entity.Id == commentId;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateBlogArticleAsync(int articleId, BlogArticle articleDto)
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

        /// <inheritdoc/>
        public async Task<bool> UpdateBlogCommentAsync(int articleId, int commentId, BlogComment comment)
        {
            var article = await this.dbContext.BlogArticles.FindAsync(articleId);

            if (article is null)
            {
                return false;
            }

            var oldComment = await this.dbContext.BlogArticleComments.FindAsync(commentId);

            if (comment is null)
            {
                return false;
            }

            oldComment.Text = comment.Text;
            return await this.dbContext.SaveChangesAsync() > 0;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<int> GetRelatedProducts(int articleId)
        {
            var articles = await this.dbContext.BlogArticles
                .Include(ba => ba.RelatedProducts)
                .ToListAsync();

            var article = articles.Find(a => a.Id == articleId);

            if (article is null)
            {
                yield break;
            }

            foreach (var productId in article.RelatedProducts.Select(rp => rp.ProductId))
            {
                yield return productId;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> CreateLinkToProduct(int articleId, int productId)
        {
            var article = await this.dbContext.BlogArticles.FindAsync(articleId);

            if (article is null)
            {
                return false;
            }

            var productLinks = await this.dbContext.RelatedProducts
                .Include(rp => rp.BlogArticles)
                .ToListAsync();

            var productLink = productLinks.FirstOrDefault(pl => pl.ProductId == productId);

            if (productLink is null)
            {
                productLink = new Entities.BlogArticleProduct { ProductId = productId, };

                await this.dbContext.RelatedProducts.AddAsync(productLink);
            }

            productLink.BlogArticles.Add(article);

            await this.dbContext.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> RemoveLinkToProduct(int articleId, int productId)
        {
            var articles = await this.dbContext.BlogArticles
                .Include(ba => ba.RelatedProducts)
                .ToListAsync();

            var article = articles.Find(a => a.Id == articleId);

            if (article is null)
            {
                return false;
            }

            var productLink = await this.dbContext.RelatedProducts.FindAsync(productId);

            if (productLink is null)
            {
                return false;
            }

            var result = article.RelatedProducts.Remove(productLink);
            await this.dbContext.SaveChangesAsync();

            return result;
        }
    }
}
