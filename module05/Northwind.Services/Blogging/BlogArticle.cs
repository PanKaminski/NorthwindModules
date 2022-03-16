namespace Northwind.Services.Blogging
{
    /// <summary>
    /// Northwind blog article.
    /// </summary>
    public class BlogArticle
    {
        /// <summary>
        /// Gets or sets article id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets article author's id.
        /// </summary>
        public int AuthorId { get; set; }

        /// <summary>
        /// Gets or sets article author's name.
        /// </summary>
        public string AuthorName { get; set; }

        /// <summary>
        /// Gets or sets article's title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets article's test.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the date of the post.
        /// </summary>
        public string Posted { get; set; }
    }
}
