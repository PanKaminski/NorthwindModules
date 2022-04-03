using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Northwind.Services.EntityFrameworkCore.Entities
{
    public partial class Category
    {
        public Category()
        {
            this.Products = new HashSet<Product>();
        }

        [Key]
        [Column("CategoryID")]
        public int Id { get; set; }

        [Required]
        [StringLength(15)]
        [Column("CategoryName")]
        public string Name { get; set; }

        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        [Column(TypeName = "image")]
        public byte[] Picture { get; set; }

        [InverseProperty(nameof(Product.Category))]
        public virtual ICollection<Product> Products { get; set; }
    }
}
