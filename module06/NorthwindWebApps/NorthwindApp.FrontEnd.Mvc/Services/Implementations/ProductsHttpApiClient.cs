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
using Northwind.Services.Products;
using NorthwindApp.FrontEnd.Mvc.Services.Interfaces;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Products;

namespace NorthwindApp.FrontEnd.Mvc.Services.Implementations
{
    public class ProductsHttpApiClient : IProductsApiClient
    {
        private const string ApiPath = "products";

        private readonly HttpClient httpClient;
        private readonly IMapper mapper;

        public ProductsHttpApiClient(HttpClient httpClient, IMapper mapper)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            this.httpClient.DefaultRequestHeaders.Accept.Clear();
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<(int, IEnumerable<ProductResponseViewModel>)> GetProductsByCategoryAsync(int categoryId, int offset, int limit)
        {
            var response = await this.httpClient.GetStringAsync($"{ApiPath}/category/{categoryId}/{offset}/{limit}");
            var result = JsonConvert.DeserializeObject<IEnumerable<Product>>(response);

            var countResponse = await this.httpClient.GetStringAsync($"{ApiPath}/category/{categoryId}/count");
            var count = JsonConvert.DeserializeObject<int>(countResponse);

            return (count, result?.Select(p => this.mapper.Map<ProductResponseViewModel>(p)));
        }

        public async Task<(int, IEnumerable<ProductResponseViewModel>)> GetProductsAsync(int offset, int limit)
        {
            var response = await this.httpClient.GetStringAsync($"{ApiPath}/{offset}/{limit}");
            var result = JsonConvert.DeserializeObject<IEnumerable<Product>>(response);

            var countResponse = await this.httpClient.GetStringAsync($"{ApiPath}/count");
            var count = JsonConvert.DeserializeObject<int>(countResponse);

            return (count, result?.Select(p => this.mapper.Map<ProductResponseViewModel>(p)));
        }

        public async Task<ProductResponseViewModel> GetProductAsync(int id)
        {
            var response = await this.httpClient.GetAsync($"{ApiPath}/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<Product>(await response.Content.ReadAsStringAsync());

            return this.mapper.Map<ProductResponseViewModel>(result);
        }

        public async Task<int> CreateProductAsync(ProductInputViewModel productModel, int? categoryId)
        {
            var entity = this.mapper.Map<Product>(productModel);
            entity.CategoryId = categoryId;

            var response = await this.httpClient.PostAsJsonAsync(ApiPath, entity);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return -1;
            }

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
        }

        public async Task<bool> UpdateProductAsync(int id, ProductInputViewModel productModel, int? categoryId)
        {
            var entity = this.mapper.Map<Product>(productModel);
            entity.CategoryId = categoryId;
            var response = await this.httpClient.PutAsJsonAsync($"{ApiPath}/{id}", entity);

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

        public async Task<bool> DeleteProductAsync(int id)
        {
            var response = await this.httpClient.DeleteAsync($"{ApiPath}/{id}");

            return response.StatusCode != HttpStatusCode.NotFound;
        }
    }
}