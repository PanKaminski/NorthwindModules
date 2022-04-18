using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NorthwindApp.FrontEnd.Mvc.ViewModels.Employees
{
    public class EmployeeInputViewModel
    {
        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string Title { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string TitleOfCourtesy { get; set; }

        public DateTime BirthDate { get; set; }

        public DateTime HireDate { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Address { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string City { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string Region { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        [Phone]
        public string HomePhone { get; set; }

        [Required]
        public string Extension { get; set; }

        public IFormFile Photo { get; set; }

        [Required]
        public string Notes { get; set; }

        public int ReportsTo { get; set; }
    }
}
