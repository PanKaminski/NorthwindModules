// <copyright file="CountryDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#pragma warning disable SA1200 // Using directive should appear within a namespace declaration
#pragma warning disable CA2227 // Change 'Currencies' to be read-only by removing the property setter
#pragma warning disable CA1812 // internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static members, make it static

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Northwind.CurrencyServices.Entities
{
    /// <summary>
    /// Country dto.
    /// </summary>
    internal class CountryDto
    {
        /// <summary>
        /// Gets or sets country name.
        /// </summary>
        [JsonProperty("name")]
        public CountryName Name { get; set; }

        /// <summary>
        /// Gets or sets country currencies.
        /// </summary>
        [JsonProperty("currencies")]
        public Dictionary<string, CurrencyDesignation> Currencies { get; set; }

        /// <summary>
        /// Gets or sets short abbreviations.
        /// </summary>
        [JsonProperty("altSpellings")]
        public List<string> ShortNames { get; set; }
    }
}
