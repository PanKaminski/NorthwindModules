using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Northwind.Services.Entities
{
    public partial class CategorySalesFor1997
    {
        [Required]
        [StringLength(15)]
        public string CategoryName { get; set; }

        [Column(TypeName = "money")]
        public decimal? CategorySales { get; set; }
    }
}
