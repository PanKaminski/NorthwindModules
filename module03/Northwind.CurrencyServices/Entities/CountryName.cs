// <copyright file="CountryName.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#pragma warning disable SA1200 // Using directive should appear within a namespace declaration
#pragma warning disable CA2227 // Change 'FullName' to be read-only by removing the property setter
#pragma warning disable CA1812 // internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static members, make it static

using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Northwind.CurrencyServices.Entities
{
    /// <summary>
    /// Various types of country name.
    /// </summary>
    internal class CountryName
    {
        /// <summary>
        /// Gets or sets official country name.
        /// </summary>
        [JsonProperty("official")]
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets common name.
        /// </summary>
        [JsonProperty("common")]
        public string CommonName { get; set; }
    }
}
