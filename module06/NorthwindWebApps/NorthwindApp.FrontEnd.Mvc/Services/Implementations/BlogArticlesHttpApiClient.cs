using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;
using Northwind.Services.Blogging;
using Northwind.Services.Customers;
using NorthwindApp.FrontEnd.Mvc.Services.Interfaces;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Blogging;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Customers;

namespace NorthwindApp.FrontEnd.Mvc.Services.Implementations
{
    public class BlogArticlesHttpApiClient : IBlogArticlesApiClient
    {
        private const string BloggingApiPath = "blogarticles";
        private const string CustomersApiPath = "customers";

        private readonly HttpClient httpClient;
        private readonly IMapper mapper;

        public BlogArticlesHttpApiClient(HttpClient httpClient, IMapper mapper)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            this.httpClient.DefaultRequestHeaders.Accept.Clear();
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<(int, IEnumerable<BlogArticleResponseViewModel>)> GetBlogArticles(int offset, int limit)
        {
            var response = await this.httpClient.GetStringAsync($"{BloggingApiPath}/{offset}/{limit}");
            var result = JsonConvert.DeserializeObject<IEnumerable<BlogArticle>>(response);

            var countResponse = await this.httpClient.GetStringAsync($"{BloggingApiPath}/count");
            var count = JsonConvert.DeserializeObject<int>(countResponse);

            return (count, result?.Select(p => this.mapper.Map<BlogArticleResponseViewModel>(p)));
        }

        public async Task<(int, BlogArticleInfoViewModel)> GetBlogArticle(int articleId, int commentsOffset, int commentsLimit)
        {
            var articleResponse = await this.httpClient.GetAsync($"{BloggingApiPath}/{articleId}");

            if (articleResponse.StatusCode != HttpStatusCode.OK)
            {
                return ((int)articleResponse.StatusCode, new BlogArticleInfoViewModel());
            }

            articleResponse.EnsureSuccessStatusCode();

            var jsonObject = await articleResponse.Content.ReadAsStringAsync();
            var articleModel = this.mapper
                .Map<BlogArticleInfoViewModel>(JsonConvert.DeserializeObject<BlogArticle>(jsonObject));

            var commentsResponse = await this.httpClient.GetAsync($"{BloggingApiPath}/{articleId}/comments");

            var jsonString = await commentsResponse.Content.ReadAsStringAsync();
            var comments = JsonConvert.DeserializeObject<ICollection<BlogComment>>(jsonString);

            var commentsList = new List<BlogCommentResponseViewModel>();

            if (comments is not null)
            {
                foreach (var comment in comments)
                {
                    var commentModel = await this.MapComment(comment, comment.CustomerId);

                    commentsList.Add(commentModel);
                }
            }

            articleModel.Comments = commentsList;

            return (200, articleModel);
        }

        public async Task<int> CreateBlogArticleAsync(BlogArticleInputViewModel articleModel, int employeeId)
        {
            var entity = this.mapper.Map<BlogArticle>(articleModel);
            entity.AuthorId = employeeId;
            var response = await this.httpClient.PostAsJsonAsync(BloggingApiPath, entity);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return -1;
            }

            response.EnsureSuccessStatusCode();

            int id = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync()); 
            
            return id;
        }

        public async Task<int> CreateBlogCommentAsync(BlogCommentInputViewModel commentModel, int blogArticleId, string customerId)
        {
            var entity = this.mapper.Map<BlogComment>(commentModel);
            entity.CustomerId = customerId;

            var response = await this.httpClient.PostAsJsonAsync($"{BloggingApiPath}/{blogArticleId}/comments", entity);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return -1;
            }

            response.EnsureSuccessStatusCode();

            int id = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());

            return id;
        }

        public async Task<int> EditBlogCommentAsync(BlogCommentEditViewModel commentModel, int blogArticleId, string customerId)
        {
            var entity = this.mapper.Map<BlogComment>(commentModel);
            entity.CustomerId = customerId;

            var response = await this.httpClient.PutAsJsonAsync($"{BloggingApiPath}/{blogArticleId}/comments/{commentModel.CommentId}", entity);

            return (int)response.StatusCode;
        }

        public async Task<(int, BlogCommentResponseViewModel)> GetBlogCommentAsync(int commentId)
        {
            var articleResponse = await this.httpClient.GetAsync($"{BloggingApiPath}/comments/{commentId}");

            if (articleResponse.StatusCode != HttpStatusCode.OK)
            {
                return ((int)articleResponse.StatusCode, new BlogCommentResponseViewModel());
            }

            articleResponse.EnsureSuccessStatusCode();

            var jsonObject = await articleResponse.Content.ReadAsStringAsync();
            var comment = this.mapper
                .Map<BlogCommentResponseViewModel>(JsonConvert.DeserializeObject<BlogComment>(jsonObject));

            return (200, comment);
        }

        public async Task<int> DeleteBlogCommentAsync(int blogArticleId, int commentId)
        {
            var response = await this.httpClient.DeleteAsync($"{BloggingApiPath}/{blogArticleId}/comments/{commentId}");

            return (int)response.StatusCode;
        }

        public async Task<int> UpdateBlogArticleAsync(BlogArticleInputViewModel articleModel, int articleId)
        {
            var response = await this.httpClient.PutAsJsonAsync($"{BloggingApiPath}/{articleId}", this.mapper.Map<BlogArticle>(articleModel));

            return (int)response.StatusCode;
        }

        public async Task<int> DeleteBlogArticleAsync(int blogArticleId)
        {
            var response = await this.httpClient.DeleteAsync($"{BloggingApiPath}/{blogArticleId}");

            return (int)response.StatusCode;
        }

        private async Task<BlogCommentResponseViewModel> MapComment(BlogComment comment, string customerId)
        {
            var articleResponse = await this.httpClient.GetAsync($"{CustomersApiPath}/{customerId}");

            if (articleResponse.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest)
            {
                return null;
            }

            articleResponse.EnsureSuccessStatusCode();

            var jsonObject = await articleResponse.Content.ReadAsStringAsync();
            var customerModel = this.mapper
                .Map<CustomerResponseViewModel>(JsonConvert.DeserializeObject<Customer>(jsonObject));

            var commentModel = this.mapper.Map<BlogCommentResponseViewModel>(comment);
            commentModel.Author = customerModel;

            return commentModel;
        }
    }
}