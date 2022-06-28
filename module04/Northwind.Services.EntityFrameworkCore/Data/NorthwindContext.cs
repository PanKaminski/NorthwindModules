using Microsoft.EntityFrameworkCore;
using Northwind.Services.Employees;
using Northwind.Services.Products;

namespace Northwind.Services.EntityFrameworkCore.Data
{
    /// <summary>
    /// In-memory database access context.
    /// </summary>
    public class NorthwindContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NorthwindContext"/> class.
        /// </summary>
        /// <param name="options">Db context options.</param>
        public NorthwindContext(DbContextOptions<NorthwindContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets product categories.
        /// </summary>
        public DbSet<ProductCategory> ProductCategories { get; set; }

        /// <summary>
        /// Gets or sets products.
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// Gets or sets employees.
        /// </summary>
        public DbSet<Employee> Employees { get; set; }
    }
}
