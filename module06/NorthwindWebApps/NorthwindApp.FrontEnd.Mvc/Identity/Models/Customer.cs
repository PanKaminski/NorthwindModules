using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NorthwindApp.FrontEnd.Mvc.Identity.Models
{
    public class Customer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string NorthwindId { get; set; }
    }
}
