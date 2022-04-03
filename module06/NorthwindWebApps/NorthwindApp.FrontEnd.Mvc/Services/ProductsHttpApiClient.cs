using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;
using Northwind.Services.Products;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Products;

namespace NorthwindApp.FrontEnd.Mvc.Services
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

            return (count, result.Select(p => this.mapper.Map<ProductResponseViewModel>(p)));
        }

        public IAsyncEnumerable<ProductResponseViewModel> GetProductsAsync(int offset, int limit)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ProductResponseViewModel> GetProduct(int id)
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

        public Task<int> AddProduct()
        {
            throw new System.NotImplementedException();
        }

        public Task<int> UpdateProduct(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteProduct(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}