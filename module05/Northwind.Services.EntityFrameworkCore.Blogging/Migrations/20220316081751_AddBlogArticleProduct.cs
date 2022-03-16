using Microsoft.EntityFrameworkCore.Migrations;

namespace Northwind.Services.EntityFrameworkCore.Blogging.Migrations
{
    public partial class AddBlogArticleProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RelatedProducts",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatedProducts", x => x.product_id);
                });

            migrationBuilder.CreateTable(
                name: "BlogArticleBlogArticleProduct",
                columns: table => new
                {
                    BlogArticlesId = table.Column<int>(type: "int", nullable: false),
                    RelatedProductsProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogArticleBlogArticleProduct", x => new { x.BlogArticlesId, x.RelatedProductsProductId });
                    table.ForeignKey(
                        name: "FK_BlogArticleBlogArticleProduct_BlogArticles_BlogArticlesId",
                        column: x => x.BlogArticlesId,
                        principalTable: "BlogArticles",
                        principalColumn: "blog_article_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogArticleBlogArticleProduct_RelatedProducts_RelatedProductsProductId",
                        column: x => x.RelatedProductsProductId,
                        principalTable: "RelatedProducts",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogArticleBlogArticleProduct_RelatedProductsProductId",
                table: "BlogArticleBlogArticleProduct",
                column: "RelatedProductsProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogArticleBlogArticleProduct");

            migrationBuilder.DropTable(
                name: "RelatedProducts");
        }
    }
}
