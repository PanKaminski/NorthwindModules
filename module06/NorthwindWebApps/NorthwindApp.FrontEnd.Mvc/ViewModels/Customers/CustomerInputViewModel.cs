using System.ComponentModel.DataAnnotations;

namespace NorthwindApp.FrontEnd.Mvc.ViewModels.Customers
{
    public class CustomerInputViewModel
    {
        [Required]
        [StringLength(40, MinimumLength = 2)]
        public string CompanyName { get; set; }

        [StringLength(30, MinimumLength = 2)]
        public string ContactName { get; set; }

        [StringLength(30, MinimumLength = 2)]
        public string ContactTitle { get; set; }

        [StringLength(60, MinimumLength = 2)]
        public string Address { get; set; }

        [StringLength(15, MinimumLength = 2)]
        public string City { get; set; }

        [StringLength(15, MinimumLength = 2)]
        public string Region { get; set; }

        [StringLength(10, MinimumLength = 2)]
        public string PostalCode { get; set; }

        [StringLength(15, MinimumLength = 2)]
        public string Country { get; set; }

        [Phone]
        [StringLength(24)]
        public string Phone { get; set; }

        [Phone]
        [StringLength(24)]
        public string Fax { get; set; }

        public string ReturnUrl { get; set; }
    }
}
