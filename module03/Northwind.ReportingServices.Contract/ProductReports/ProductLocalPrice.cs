// <copyright file="ProductLocalPrice.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Northwind.ReportingServices.Contract.ProductReports
{
    /// <summary>
    /// Product info with local price.
    /// </summary>
    public class ProductLocalPrice
    {
        /// <summary>
        /// Gets or sets products name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets product price in USD.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets product country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets product local price.
        /// </summary>
        public decimal LocalPrice { get; set; }

        /// <summary>
        /// Gets or sets currency symbol.
        /// </summary>
        public string CurrencySymbol { get; set; }
    }
}
