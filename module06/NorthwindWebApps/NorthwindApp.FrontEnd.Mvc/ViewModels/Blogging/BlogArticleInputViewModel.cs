using System.ComponentModel.DataAnnotations;

namespace NorthwindApp.FrontEnd.Mvc.ViewModels.Blogging
{
    public class BlogArticleInputViewModel
    {
        public int AuthorId { get; set; }

        [Required]
        [StringLength(60)]
        public string Title { get; set; }

        [Required]
        public string Text { get; set; }
    }
}
