// <copyright file="CurrencyDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#pragma warning disable SA1200 // Using directive should appear within a namespace declaration
#pragma warning disable CA2227 // Change 'Quotes' to be read-only by removing the property setter
#pragma warning disable CA1812 // internal class that is apparently never instantiated. If so, remove the code from the assembly. If this class is intended to contain only static members, make it static

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Northwind.CurrencyServices.Entities
{
    /// <summary>
    /// Currency layer dto.
    /// </summary>
    internal class CurrencyDto
    {
        /// <summary>
        /// Gets or sets binded service currency.
        /// </summary>
        [JsonProperty("source")]
        public string PrimaryCurrency { get; set; }

        /// <summary>
        /// Gets or sets currency quotes.
        /// </summary>
        [JsonProperty("quotes")]
        public Dictionary<string, decimal> Quotes { get; set; }
    }
}
