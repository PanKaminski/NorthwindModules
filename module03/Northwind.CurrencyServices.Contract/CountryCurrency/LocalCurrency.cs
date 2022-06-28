// <copyright file="LocalCurrency.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Northwind.CurrencyServices.Contract.CountryCurrency
{
    /// <summary>
    /// Country currency info.
    /// </summary>
    public class LocalCurrency
    {
        /// <summary>
        /// Gets or sets name of the country.
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Gets or sets currency code.
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets currency symbol.
        /// </summary>
        public string CurrencySymbol { get; set; }
    }
}
