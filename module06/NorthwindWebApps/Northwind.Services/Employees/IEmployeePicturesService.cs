using System.IO;
using System.Threading.Tasks;

namespace Northwind.Services.Employees
{
    /// <summary>
    /// Provides methods for managing employees photos.
    /// </summary>
    public interface IEmployeePicturesService
    {
        /// <summary>
        /// Try to show an employee's picture.
        /// </summary>
        /// <param name="employeeId">An employee identifier.</param>
        /// <returns>True if an employee exists; otherwise false.</returns>
        Task<(bool, byte[])> TryGetPhotoAsync(int employeeId);

        /// <summary>
        /// Update an employee's picture.
        /// </summary>
        /// <param name="employeeId">An employee identifier.</param>
        /// <param name="stream">A <see cref="Stream"/>.</param>
        /// <returns>True if an employee photo was updated; otherwise false.</returns>
        Task<bool> UpdatePhotoAsync(int employeeId, Stream stream);

        /// <summary>
        /// Destroy an employee's picture.
        /// </summary>
        /// <param name="employeeId">An employee identifier.</param>
        /// <returns>True if an employee photo was deleted; otherwise false.</returns>
        Task<bool> DeletePhotoAsync(int employeeId);

    }
}
