using System;
using System.Globalization;
using System.Runtime.Serialization;

#pragma warning disable CA1032 // Implement standard exception constructors

namespace Northwind.DataAccess.Employees
{
    /// <summary>
    /// The exception that is thrown when an employee is not found in a data storage.
    /// </summary>
    [Serializable]
    public class EmployeeNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeNotFoundException"/> class with specified identifier and object type.
        /// </summary>
        /// <param name="id">A requested identifier.</param>
        public EmployeeNotFoundException(int id)
            : base(string.Format(CultureInfo.InvariantCulture, $"A product category with identifier = {id}."))
        {
            this.EmployeeId = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeNotFoundException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">An object that describes the source or destination of the serialized data.</param>
        protected EmployeeNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets an identifier of a product category that is missed in a data storage.
        /// </summary>
        public int EmployeeId { get; }
    }
}