namespace Northwind.Services.Blogging
{
    public class BlogArticle
    {
        public int Id { get; set; }

        public int AuthorId { get; set; }

        public string AuthorName { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public string Posted { get; set; }
    }
}
