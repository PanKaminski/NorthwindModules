using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;
using Northwind.Services.Customers;
using NorthwindApp.FrontEnd.Mvc.Services.Interfaces;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Customers;

namespace NorthwindApp.FrontEnd.Mvc.Services.Implementations
{
    public class CustomersHttpApiClient : ICustomersApiClient
    {
        private const string ApiPath = "customers";

        private readonly HttpClient httpClient;
        private readonly IMapper mapper;

        public CustomersHttpApiClient(HttpClient httpClient, IMapper mapper)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            this.httpClient.DefaultRequestHeaders.Accept.Clear();
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<(CustomerResponseViewModel, bool)> GetCustomerAsync(string customerId)
        {
            var response = await this.httpClient.GetAsync($"{ApiPath}/{customerId}");

            if (response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest)
            {
                return (new CustomerResponseViewModel(), false);
            }

            response.EnsureSuccessStatusCode();

            var jsonObject = await response.Content.ReadAsStringAsync();

            return (this.mapper
                .Map<CustomerResponseViewModel>(JsonConvert.DeserializeObject<Customer>(jsonObject)), true);
        }
    }
}