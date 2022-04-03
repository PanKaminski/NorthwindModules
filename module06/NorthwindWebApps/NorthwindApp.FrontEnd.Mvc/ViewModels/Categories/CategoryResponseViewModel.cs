using System.IO;

namespace NorthwindApp.FrontEnd.Mvc.ViewModels.Categories
{
    public class CategoryResponseViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool HasImage { get; set; }
    }
}
