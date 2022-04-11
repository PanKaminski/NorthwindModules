using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NorthwindApp.FrontEnd.Mvc.ViewModels.Products
{
    public class ProductInputViewModel
    {
        [Required]
        public string Name { get; set; }

        [Display(Name = "Quantity per unit")]
        public string QuantityPerUnit { get; set; }

        [Display(Name = "Unit price")]
        [Range(0, 10_000, ErrorMessage = "Specify unit price from 0 to 10 000")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Units in stock")]
        [Range(1, 200, ErrorMessage = "Invalid count og units in stock")]
        public short? UnitsInStock { get; set; }

        [Display(Name = "Product status")]
        public string ProductStatus { get; set; }

        [Display(Name = "Category name")]
        public string CategoryName { get; set; }
    }
}
