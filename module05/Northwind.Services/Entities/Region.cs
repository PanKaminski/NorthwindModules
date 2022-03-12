using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Northwind.Services.Entities
{
    [Table("Region")]
    public partial class Region
    {
        public Region()
        {
            this.Territories = new HashSet<Territory>();
        }

        [Key]
        [Column("RegionID")]
        public int RegionId { get; set; }

        [Required]
        [StringLength(50)]
        public string RegionDescription { get; set; }

        [InverseProperty(nameof(Territory.Region))]
        public virtual ICollection<Territory> Territories { get; set; }
    }
}
