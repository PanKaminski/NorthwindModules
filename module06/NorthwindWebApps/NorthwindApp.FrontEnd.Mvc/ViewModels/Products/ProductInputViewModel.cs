namespace NorthwindApp.FrontEnd.Mvc.ViewModels.Products
{
    public class ProductInputViewModel
    {
        public string Name { get; set; }

        public string QuantityPerUnit { get; set; }

        public decimal? UnitPrice { get; set; }

        public short? UnitsInStock { get; set; }

        public bool Discontinued { get; set; }
    }
}
