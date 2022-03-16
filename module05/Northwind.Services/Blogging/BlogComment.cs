namespace Northwind.Services.Blogging
{
    /// <summary>
    /// Customers comment for the article.
    /// </summary>
    public class BlogComment
    {
        /// <summary>
        /// Gets or sets comment id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets customer's id.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets text content.
        /// </summary>
        public string Text { get; set; }
    }
}
