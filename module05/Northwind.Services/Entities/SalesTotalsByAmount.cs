using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Northwind.Services.Entities
{
    public partial class SalesTotalsByAmount
    {
        [Column(TypeName = "money")]
        public decimal? SaleAmount { get; set; }

        [Column("OrderID")]
        public int OrderId { get; set; }

        [Required]
        [StringLength(40)]
        public string CompanyName { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ShippedDate { get; set; }
    }
}
