using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NorthwindApp.FrontEnd.Mvc.Identity.Models
{
    public class Employee
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NorthwindId { get; set; }
    }
}
