using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NorthwindApp.FrontEnd.Mvc.ViewModels.Employees
{
    public class EmployeeInputViewModel
    {
        [Required]
        [Display(Name = "Last name")]
        [StringLength(60, MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "First name")]
        [StringLength(60, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Display(Name = "Title")]
        [StringLength(60, MinimumLength = 2)]
        public string Title { get; set; }

        [Display(Name = "Title of courtesy")]
        [StringLength(60, MinimumLength = 2)]
        public string TitleOfCourtesy { get; set; }

        [Display(Name = "Birth date")]
        public string BirthDate { get; set; }

        [Display(Name = "Hire date")]
        public string HireDate { get; set; }

        [StringLength(100, MinimumLength = 2)]
        public string Address { get; set; }

        [StringLength(60, MinimumLength = 2)]
        public string City { get; set; }

        [StringLength(60, MinimumLength = 2)]
        public string Region { get; set; }

        [Display(Name = "Postal code")]
        public string PostalCode { get; set; }

        public string Country { get; set; }

        [Display(Name = "Home phone")]
        [Phone]
        public string HomePhone { get; set; }

        public string Extension { get; set; }

        public IFormFile Photo { get; set; }

        public string Notes { get; set; }

        [Display(Name = "Reports to")]
        public string ReportsTo { get; set; }
    }
}
