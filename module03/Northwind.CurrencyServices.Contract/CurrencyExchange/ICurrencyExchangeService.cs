// <copyright file="ICurrencyExchangeService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#pragma warning disable SA1200 // Using directive should appear within a namespace declaration

using System.Threading.Tasks;

namespace Northwind.CurrencyServices.Contract.CurrencyExchange
{
    /// <summary>
    /// Contains methods for calculating currency ratios.
    /// </summary>
    public interface ICurrencyExchangeService
    {
        /// <summary>
        /// Calculates currency conversion coefficient.
        /// </summary>
        /// <param name="baseCurrency">Currency from witch money should be converted.</param>
        /// <param name="exchangeCurrency">Currency to witch money should be converted.</param>
        /// <returns>Currency conversion coefficient.</returns>
        Task<decimal> GetCurrencyExchangeRate(string baseCurrency, string exchangeCurrency);
    }
}
