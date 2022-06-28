// <copyright file="ICountryCurrencyService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#pragma warning disable SA1200 // Using directive should appear within a namespace declaration

using System.Threading.Tasks;

namespace Northwind.CurrencyServices.Contract.CountryCurrency
{
    /// <summary>
    /// Provides methods for searching country currency.
    /// </summary>
    public interface ICountryCurrencyService
    {
        /// <summary>
        /// Searches country currency.
        /// </summary>
        /// <param name="countryName">Country name.</param>
        /// <returns><see cref="LocalCurrency"/> of the country.</returns>
        Task<LocalCurrency> GetLocalCurrencyByCountry(string countryName);
    }
}
