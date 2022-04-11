using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;
using Northwind.Services.Products;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Categories;

namespace NorthwindApp.FrontEnd.Mvc.Services
{
    public class CategoriesHttpApiClient : ICategoriesApiClient
    {
        private const string ApiPath = "productcategories";

        private readonly HttpClient httpClient;
        private readonly IMapper mapper;

        public CategoriesHttpApiClient(HttpClient httpClient, IMapper mapper)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            this.httpClient.DefaultRequestHeaders.Accept.Clear();
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<CategoryResponseViewModel> GetCategoryAsync(int id)
        {
            var response = await this.httpClient.GetAsync($"{ApiPath}/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var jsonObject = await response.Content.ReadAsStringAsync();
            var category = JsonConvert.DeserializeObject<ProductCategory>(jsonObject);

            return this.mapper.Map<CategoryResponseViewModel>(category);
        }

        public async IAsyncEnumerable<CategoryResponseViewModel> GetCategoriesAsync(int offset, int limit)
        {
            var response = await this.httpClient.GetAsync($"{ApiPath}/{offset}/{limit}");
            var jsonString = await response.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<IEnumerable<ProductCategory>>(jsonString);

            if (categories is null)
            {
                yield break;
            }

            foreach (var category in categories)
            {
                var viewModel = this.mapper.Map<CategoryResponseViewModel>(category);

                yield return viewModel;
            }
        }

        public async IAsyncEnumerable<CategoryResponseViewModel> GetCategoriesAsync()
        {
            var response = await this.httpClient.GetAsync(ApiPath);
            var jsonString = await response.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<IEnumerable<ProductCategory>>(jsonString);

            if (categories is null)
            {
                yield break;
            }

            foreach (var category in categories)
            {
                var viewModel = this.mapper.Map<CategoryResponseViewModel>(category);

                yield return viewModel;
            }
        }

        public async Task<CategoryResponseViewModel> GetCategoryByNameAsync(string name)
        {
            var response = await this.httpClient.GetAsync($"{ApiPath}/name/{name}");

            if (response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var jsonObject = await response.Content.ReadAsStringAsync();
            var category = JsonConvert.DeserializeObject<ProductCategory>(jsonObject);

            return this.mapper.Map<CategoryResponseViewModel>(category);
        }

        public async Task<CategoryResponseViewModel> GetProductByProductAsync(int productId)
        {
            var response = await this.httpClient.GetAsync($"{ApiPath}/product/{productId}");

            if (response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var jsonObject = await response.Content.ReadAsStringAsync();
            var category = JsonConvert.DeserializeObject<ProductCategory>(jsonObject);

            return this.mapper.Map<CategoryResponseViewModel>(category);
        }

        public async Task<int> CreateCategoryAsync(CategoryInputViewModel category)
        {
            var response = await this.httpClient.PostAsJsonAsync(ApiPath, this.mapper.Map<ProductCategory>(category));

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return -1;
            }

            response.EnsureSuccessStatusCode();

            int id = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());

            if (category.Image is not null)
            {
                await this.httpClient.PutAsJsonAsync($"{ApiPath}/{id}/ picture", category.Image);
            }

            return id;
        }

        public async Task<bool> UpdateCategoryAsync(int id, CategoryInputViewModel category)
        {
            var response = await this.httpClient.PutAsJsonAsync($"{ApiPath}/{id}", this.mapper.Map<ProductCategory>(category));

            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.NotFound:
                    return false;
                default:
                    response.EnsureSuccessStatusCode();

                    if (category.Image is not null)
                    {
                        await this.httpClient.PutAsJsonAsync($"{ApiPath}/{id}/ picture", category.Image);
                    }

                    return true;
            }
        }

        public async Task<byte[]> UploadImage(int categoryId)
        {
            var response = await this.httpClient.GetAsync($"{ApiPath}/{categoryId}/picture");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return Array.Empty<byte>();
            }

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<bool> DeleteCategory(int categoryId)
        {
            var response = await this.httpClient.DeleteAsync($"{ApiPath}/{categoryId}");

            return response.StatusCode != HttpStatusCode.NotFound;
        }
    }
}