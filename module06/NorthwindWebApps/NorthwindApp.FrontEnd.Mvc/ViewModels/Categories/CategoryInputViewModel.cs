using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NorthwindApp.FrontEnd.Mvc.ViewModels.Categories
{
    public class CategoryInputViewModel 
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public IFormFile Image { get; set; }
    }
}
