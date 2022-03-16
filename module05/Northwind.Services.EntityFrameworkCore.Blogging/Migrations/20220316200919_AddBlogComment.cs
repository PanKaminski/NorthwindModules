using Microsoft.EntityFrameworkCore.Migrations;

namespace Northwind.Services.EntityFrameworkCore.Blogging.Migrations
{
    public partial class AddBlogComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlogArticleComments",
                columns: table => new
                {
                    blog_comment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_id = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    article_id = table.Column<int>(type: "int", nullable: false),
                    text = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    BlogArticleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogArticleComments", x => x.blog_comment_id);
                    table.ForeignKey(
                        name: "FK_BlogArticleComments_BlogArticles_BlogArticleId",
                        column: x => x.BlogArticleId,
                        principalTable: "BlogArticles",
                        principalColumn: "blog_article_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogArticleComments_BlogArticleId",
                table: "BlogArticleComments",
                column: "BlogArticleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogArticleComments");
        }
    }
}
