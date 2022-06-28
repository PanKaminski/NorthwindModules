// <copyright file="CurrencyExchangeService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#pragma warning disable CA1001 // Type 'CountryCurrencyService' owns disposable field(s) 'httpClient' but is not disposable
#pragma warning disable SA1200 // Using directive should appear within a namespace declaration

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Northwind.CurrencyServices.Contract.CurrencyExchange;
using Northwind.CurrencyServices.Entities;

namespace Northwind.CurrencyServices.CurrencyExchange
{
/// <summary>
/// Web currency exchange service.
/// </summary>
public class CurrencyExchangeService : ICurrencyExchangeService
{
    private readonly string accessKey;
    private readonly HttpClient httpClient = new HttpClient();

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrencyExchangeService"/> class.
    /// </summary>
    /// <param name="accessKey">Web service access key.</param>
    public CurrencyExchangeService(string accessKey)
    {
        this.accessKey = !string.IsNullOrWhiteSpace(accessKey)
            ? accessKey
            : throw new ArgumentException("Access key is invalid.", nameof(accessKey));
    }

    /// <inheritdoc cref="ICurrencyExchangeService"/>
    public async Task<decimal> GetCurrencyExchangeRate(string baseCurrency, string exchangeCurrency)
    {
        if (baseCurrency == exchangeCurrency)
        {
            return 1;
        }

        string url = $"http://api.currencylayer.com/live?access_key={this.accessKey}&source={baseCurrency}";

        this.httpClient.DefaultRequestHeaders.Accept.Clear();
        this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var jsonObject = await this.httpClient.GetStringAsync(url).ConfigureAwait(true);

        var currencyLayer = JsonConvert.DeserializeObject<CurrencyDto>(jsonObject);

        return currencyLayer.Quotes[baseCurrency + exchangeCurrency];
    }
}
}
