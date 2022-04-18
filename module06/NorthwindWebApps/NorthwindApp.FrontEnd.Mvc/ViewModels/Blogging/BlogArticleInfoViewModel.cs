using System.Collections.Generic;

namespace NorthwindApp.FrontEnd.Mvc.ViewModels.Blogging
{
    public class BlogArticleInfoViewModel : BlogArticleResponseViewModel
    {
        public ICollection<BlogCommentResponseViewModel> Comments { get; set; } =
            new List<BlogCommentResponseViewModel>();
    }
}
