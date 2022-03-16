using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFrameworkCore.Blogging.Entities
{
    public class BlogArticleProduct
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("product_id")]
        public int ProductId { get; set; }

        public ICollection<BlogArticle> BlogArticles { get; set; } = new List<BlogArticle>();
    }
}
