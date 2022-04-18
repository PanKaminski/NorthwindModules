using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Northwind.Services.Employees;
using Northwind.Services.EntityFrameworkCore.Context;

namespace Northwind.Services.EntityFrameworkCore.Employees
{
    /// <summary>
    /// Manages employees photos.
    /// </summary>
    public class EmployeePicturesService : IEmployeePicturesService
    {
        private readonly NorthwindContext dbContext;

        /// <summary>
        /// Initializes new instance of <see cref="EmployeePicturesService"/>.
        /// </summary>
        /// <param name="dbContext">Northwind data base context.</param>
        public EmployeePicturesService(NorthwindContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <inheritdoc/>
        public async Task<(bool, byte[])> TryGetPhotoAsync(int employeeId)
        {
            var employee = await this.dbContext.Employees
                .SingleOrDefaultAsync(c => c.Id == employeeId);

            if (employee.Photo is null || employee.Photo.Length == 0)
            {
                return (false, null);
            }

            var result = new byte[employee.Photo.Length - 78];
            Array.Copy(employee.Photo, 78, result, 0, employee.Photo.Length - 78);

            return (true, result);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdatePhotoAsync(int employeeId, Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var employee = await this.dbContext.Employees.SingleOrDefaultAsync(c => c.Id == employeeId);

            if (employee is null)
            {
                return false;
            }

            var bytes = new byte[stream.Length];

            await using (var memoryStream = new MemoryStream(bytes))
            {
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(memoryStream);
                employee.Photo = memoryStream.ToArray();
                await this.dbContext.SaveChangesAsync();

                return true;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeletePhotoAsync(int employeeId)
        {
            var employee = await this.dbContext.Employees.SingleOrDefaultAsync(c => c.Id == employeeId);

            if (employee is null)
            {
                return false;
            }

            employee.Photo = null;
            await this.dbContext.SaveChangesAsync();

            return true;
        }
    }
}
