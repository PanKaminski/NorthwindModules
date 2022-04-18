namespace NorthwindApp.FrontEnd.Mvc.ViewModels.Products
{
    public class CategoryProductListViewModel : PageListViewModel<ProductResponseViewModel>
    {
        public int CategoryId { get; set; }
    }
}
