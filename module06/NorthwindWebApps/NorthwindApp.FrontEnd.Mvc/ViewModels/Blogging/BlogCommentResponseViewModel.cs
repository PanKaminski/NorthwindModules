using NorthwindApp.FrontEnd.Mvc.ViewModels.Customers;

namespace NorthwindApp.FrontEnd.Mvc.ViewModels.Blogging
{
    public class BlogCommentResponseViewModel
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public string CustomerId { get; set; }

        public CustomerResponseViewModel Author { get; set; }
    }
}
