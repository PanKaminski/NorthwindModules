using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFrameworkCore.Blogging.Entities
{
    public class BlogComment
    {
        [Key]
        [Column("blog_comment_id")]
        public int Id { get; set; }

        [Column("customer_id")]
        [StringLength(5)]
        public string CustomerId { get; set; }

        [Column("article_id")]
        public int ArticleId { get; set; }

        [Column("text")]
        [StringLength(512)]
        public string Text { get; set; }

        public BlogArticle BlogArticle { get; set; }
    }
}
