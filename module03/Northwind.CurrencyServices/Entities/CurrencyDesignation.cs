// <copyright file="CurrencyDesignation.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#pragma warning disable SA1200 // Using directive should appear within a namespace declaration

using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Northwind.CurrencyServices.Entities
{
    /// <summary>
    /// Country currency info.
    /// </summary>
    public class CurrencyDesignation
    {
        /// <summary>
        /// Gets or sets currency name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets currency symbol.
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
    }
}
