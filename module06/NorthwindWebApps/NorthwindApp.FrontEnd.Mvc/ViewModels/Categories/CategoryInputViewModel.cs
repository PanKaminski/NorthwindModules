﻿using Microsoft.AspNetCore.Http;

namespace NorthwindApp.FrontEnd.Mvc.ViewModels.Categories
{
    public class CategoryInputViewModel 
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public IFormFile Image { get; set; }
    }
}
