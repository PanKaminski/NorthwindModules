using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Northwind.Services.Entities
{
    public partial class SalesByCategory
    {
        [Column("CategoryID")]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(15)]
        public string CategoryName { get; set; }

        [Required]
        [StringLength(40)]
        public string ProductName { get; set; }

        [Column(TypeName = "money")]
        public decimal? ProductSales { get; set; }
    }
}
