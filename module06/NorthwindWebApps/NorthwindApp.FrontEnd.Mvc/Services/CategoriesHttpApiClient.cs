using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;
using Northwind.Services.Entities;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Categories;

namespace NorthwindApp.FrontEnd.Mvc.Services
{
    public class CategoriesHttpApiClient : ICategoriesApiClient
    {
        private const string ApiPath = "/api/productcategories";

        private readonly HttpClient httpClient;
        private readonly IMapper mapper;

        public CategoriesHttpApiClient(HttpClient httpClient, IMapper mapper)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
            var category = JsonConvert.DeserializeObject<Category>(jsonObject);

            return this.mapper.Map<CategoryResponseViewModel>(category);
        }

        public async IAsyncEnumerable<CategoryResponseViewModel> GetCategoriesAsync(int offset, int limit)
        {
            var response = await this.httpClient.GetStringAsync($"{ApiPath}/{offset}/{limit}");

            var categories = JsonConvert.DeserializeObject<IAsyncEnumerable<Category>>(response);

            if (categories is null)
            {
                yield break;
            }

            await foreach (var category in categories)
            {
                yield return this.mapper.Map<CategoryResponseViewModel>(category);
            }
        }

        public async Task<int> CreateCategoryAsync(CategoryInputViewModel category)
        {
            var response = await this.httpClient.PostAsJsonAsync(ApiPath, this.mapper.Map<Category>(category));

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return -1;
            }

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
        }

        public async Task<bool> UpdateCategoryAsync(int id, CategoryInputViewModel category)
        {
            var response = await this.httpClient.PutAsJsonAsync($"{ApiPath}/{id}", this.mapper.Map<Category>(category));

            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.NotFound:
                    return false;
                default:
                    response.EnsureSuccessStatusCode();
                    return true;
            }
        }
    }
}