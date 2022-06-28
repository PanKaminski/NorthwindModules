// <copyright file="CountryCurrencyService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#pragma warning disable SA1200 // Using directive should appear within a namespace declaration
#pragma warning disable CA1001 // Type 'CountryCurrencyService' owns disposable field(s) 'httpClient' but is not disposable

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Northwind.CurrencyServices.Contract.CountryCurrency;
using Northwind.CurrencyServices.Entities;

namespace Northwind.CurrencyServices.CountryCurrency
{
    /// <summary>
    /// Web country currency service.
    /// </summary>
    public class CountryCurrencyService : ICountryCurrencyService
    {
        private readonly HttpClient httpClient = new HttpClient();

        /// <inheritdoc cref="ICountryCurrencyService"/>
        public async Task<LocalCurrency> GetLocalCurrencyByCountry(string countryName)
        {
            string url = $"https://restcountries.com/v3.1/name/{countryName}";

            var jsonObject = await this.httpClient.GetStringAsync(url).ConfigureAwait(true);

            var countryLayer = JsonConvert.DeserializeObject<List<CountryDto>>(jsonObject);

            var dto = countryLayer.First(c => c.ShortNames.Contains(countryName) || c.Name.CommonName == countryName || c.Name.FullName == countryName);
            var currency = dto.Currencies.First();

            return new LocalCurrency { CountryName = dto.Name.FullName, CurrencyCode = currency.Key, CurrencySymbol = currency.Value.Symbol };
        }
    }
}
