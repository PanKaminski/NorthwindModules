using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Services.EntityFrameworkCore.Blogging.Entities
{
    public class BlogArticle
    {
        [Column("blog_article_id")]
        public int Id { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("content")]
        public string Content { get; set; }

        [Column("publication_date")]
        public DateTime PublicationDate { get; set; }

        [Column("employee_id")]
        public int EmployeeId { get; set; }

        public ICollection<BlogArticleProduct> RelatedProducts { get; set; } = new List<BlogArticleProduct>();

        public ICollection<BlogComment> BlogArticleComments { get; set; } = new List<BlogComment>();
    }
}