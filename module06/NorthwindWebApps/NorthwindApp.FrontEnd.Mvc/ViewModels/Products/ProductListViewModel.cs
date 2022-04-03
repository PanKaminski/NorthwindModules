using System.Collections.Generic;

namespace NorthwindApp.FrontEnd.Mvc.ViewModels.Products
{
    public class ProductListViewModel
    {
        public IEnumerable<ProductResponseViewModel> Products { get; set; }

        public PageInfo PageInfo { get; set; }

        public int CategoryId { get; set; }
    }
}
