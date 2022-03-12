using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Northwind.DataAccess.Employees;

namespace Northwind.DataAccess.SqlServer.Employees
{
    /// <summary>
    /// Represents a SQL Server-tailored DAO for Northwind products.
    /// </summary>
    public sealed class EmployeeSqlServerDataAccessObject : IEmployeeDataAccessObject
    {
        private readonly SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeSqlServerDataAccessObject"/> class.
        /// </summary>
        /// <param name="connection">A <see cref="SqlConnection"/>.</param>
        public EmployeeSqlServerDataAccessObject(SqlConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <inheritdoc/>
        public async Task<int> InsertEmployeeAsync(EmployeeTransferObject employee)
        {
            if (employee is null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            await using var sqlCommand = new SqlCommand("Insert_Employee", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(employee, sqlCommand);

            await this.OpenIfClosedAsync();
            return await sqlCommand.ExecuteNonQueryAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            if (employeeId <= 0)
            {
                throw new ArgumentException("Must be greater than zero.", nameof(employeeId));
            }

            await using var sqlCommand = new SqlCommand("Delete_Employee", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string employeeIdParameter = "@employeeId";
            sqlCommand.Parameters.Add(employeeIdParameter, SqlDbType.Int);
            sqlCommand.Parameters[employeeIdParameter].Value = employeeId;

            await this.OpenIfClosedAsync();
            return await sqlCommand.ExecuteNonQueryAsync() > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateEmployeeAsync(EmployeeTransferObject employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            await using var sqlCommand = new SqlCommand("Update_Employee", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            AddSqlParameters(employee, sqlCommand);

            const string employeeId = "@employeeId";
            sqlCommand.Parameters.Add(employeeId, SqlDbType.Int);
            sqlCommand.Parameters[employeeId].Value = employee.Id;

            await this.OpenIfClosedAsync();
            return await sqlCommand.ExecuteNonQueryAsync() > 0;
        }

        /// <inheritdoc/>
        public async Task<EmployeeTransferObject> FindEmployeeAsync(int employeeId)
        {
            if (employeeId <= 0)
            {
                throw new ArgumentException("Employee id must be greater than zero.", nameof(employeeId));
            }

            await using var sqlCommand = new SqlCommand("Get_Employee_By_Id", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            const string employeeIdParameter = "@employeeId";
            sqlCommand.Parameters.Add(employeeIdParameter, SqlDbType.Int);
            sqlCommand.Parameters[employeeIdParameter].Value = employeeId;

            await this.OpenIfClosedAsync();

            var reader = await sqlCommand.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                throw new EmployeeNotFoundException(employeeId);
            }

            await reader.ReadAsync();

            return CreateEmployee(reader);
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<EmployeeTransferObject> SelectEmployeesAsync(int offset, int limit)
        {
            if (offset < 0)
            {
                throw new ArgumentException("Offset must be greater than or equals zero.", nameof(offset));
            }

            if (limit < 0)
            {
                throw new ArgumentException("Limit must be greater than or equals zero.", nameof(limit));
            }

            await using var sqlCommand = new SqlCommand("Get_Employees_With_Limit", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            sqlCommand.Parameters.Add("@offset", SqlDbType.Int);
            sqlCommand.Parameters["@offset"].Value = offset;

            sqlCommand.Parameters.Add("@limit", SqlDbType.Int);
            sqlCommand.Parameters["@limit"].Value = limit;

            await this.OpenIfClosedAsync();

            var reader = await sqlCommand.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                yield return CreateEmployee(reader);
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<EmployeeTransferObject> SelectEmployeesAsync()
        {
            await using var sqlCommand = new SqlCommand("Get_Employees", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            await this.OpenIfClosedAsync();

            var reader = await sqlCommand.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                yield return CreateEmployee(reader);
            }
        }

        private static EmployeeTransferObject CreateEmployee(SqlDataReader reader)
        {
            var id = (int)reader["EmployeeID"];
            var lastName = (string)reader["LastName"];
            var firstName = (string)reader["FirstName"];

            string title = reader["Title"] != DBNull.Value ? (string)reader["Title"] : null;
            string titleOfCourtesy = reader["TitleOfCourtesy"] != DBNull.Value ? (string)reader["TitleOfCourtesy"] : null;

            DateTime? birthDate = reader["BirthDate"] != DBNull.Value ? (DateTime?)reader["BirthDate"] : null;
            DateTime? hireDate = reader["HireDate"] != DBNull.Value ? (DateTime?)reader["HireDate"] : null;

            string address = reader["Address"] != DBNull.Value ? (string)reader["Address"] : null;
            string city = reader["City"] != DBNull.Value ? (string)reader["City"] : null;
            string region = reader["Region"] != DBNull.Value ? (string)reader["Region"] : null;

            string postalCode = reader["PostalCode"] != DBNull.Value ? (string)reader["PostalCode"] : null;
            string country = reader["Country"] != DBNull.Value ? (string)reader["Country"] : null;
            string homePhone = reader["HomePhone"] != DBNull.Value ? (string)reader["HomePhone"] : null;
            string extension = reader["Extension"] != DBNull.Value ? (string)reader["Extension"] : null;

            string photoPath = reader["PhotoPath"] != DBNull.Value ? (string)reader["PhotoPath"] : null;
            byte[] photo = reader["Photo"] != DBNull.Value ? (byte[])reader["Photo"] : null;
            int? reportsTo = reader["ReportsTo"] != DBNull.Value ? (int?)reader["ReportsTo"] : null;
            string notes = reader["Notes"] != DBNull.Value ? (string)reader["Notes"] : null;

            return new EmployeeTransferObject
            {
                Id = id,
                LastName = lastName,
                FirstName = firstName,
                Title = title,
                TitleOfCourtesy = titleOfCourtesy,
                BirthDate = birthDate,
                HireDate = hireDate,
                Address = address,
                City = city,
                Region = region,
                PostalCode = postalCode,
                Country = country,
                HomePhone = homePhone,
                Extension = extension,
                Photo = photo,
                Notes = notes,
                ReportsTo = reportsTo,
                PhotoPath = photoPath
            };
        }


        private static void AddSqlParameters(EmployeeTransferObject employee, SqlCommand command)
        {
            const string lastName = "@lastName";
            command.Parameters.Add(lastName, SqlDbType.NVarChar, 20);
            command.Parameters[lastName].Value = employee.LastName;

            const string firstName = "@firstName";
            command.Parameters.Add(firstName, SqlDbType.NVarChar, 10);
            command.Parameters[firstName].Value = employee.FirstName;

            const string title = "@title";
            command.Parameters.Add(title, SqlDbType.NVarChar, 30);
            command.Parameters[title].IsNullable = true;
            command.Parameters[title].Value = employee.Title is null ? DBNull.Value : employee.Title;

            const string titleOfCourtesy = "@titleOfCourtesy";
            command.Parameters.Add(titleOfCourtesy, SqlDbType.NVarChar, 25);
            command.Parameters[titleOfCourtesy].IsNullable = true;
            command.Parameters[titleOfCourtesy].Value = employee.TitleOfCourtesy is null ? DBNull.Value : employee.TitleOfCourtesy;


            const string birthDate = "@birthDate";
            command.Parameters.Add(birthDate, SqlDbType.DateTime);
            command.Parameters[birthDate].IsNullable = true;
            command.Parameters[birthDate].Value = employee.BirthDate is null ? DBNull.Value : employee.BirthDate;

            const string hireDate = "@hireDate";
            command.Parameters.Add(hireDate, SqlDbType.DateTime);
            command.Parameters[hireDate].IsNullable = true;
            command.Parameters[hireDate].Value = employee.HireDate is null ? DBNull.Value : employee.HireDate;

            const string address = "@address";
            command.Parameters.Add(address, SqlDbType.NVarChar, 60);
            command.Parameters[address].IsNullable = true;
            command.Parameters[address].Value = employee.Address is null ? DBNull.Value : employee.Address;

            const string city = "@city";
            command.Parameters.Add(city, SqlDbType.NVarChar, 15);
            command.Parameters[city].IsNullable = true;
            command.Parameters[city].Value = employee.City is null ? DBNull.Value : employee.City;

            const string region = "@region";
            command.Parameters.Add(region, SqlDbType.NVarChar, 15);
            command.Parameters[region].IsNullable = true;
            command.Parameters[region].Value = employee.Region is null ? DBNull.Value : employee.Region;

            const string postalCode = "@postalCode";
            command.Parameters.Add(postalCode, SqlDbType.NVarChar, 10);
            command.Parameters[postalCode].IsNullable = true;
            command.Parameters[postalCode].Value = employee.PostalCode is null ? DBNull.Value : employee.PostalCode;

            const string country = "@country";
            command.Parameters.Add(country, SqlDbType.NVarChar, 15);
            command.Parameters[country].IsNullable = true;
            command.Parameters[country].Value = employee.Country is null ? DBNull.Value : employee.Country;

            const string homePhone = "@homePhone";
            command.Parameters.Add(homePhone, SqlDbType.NVarChar, 24);
            command.Parameters[homePhone].IsNullable = true;
            command.Parameters[homePhone].Value = employee.HomePhone is null ? DBNull.Value : employee.HomePhone;

            const string extension = "@extension";
            command.Parameters.Add(extension, SqlDbType.NVarChar, 4);
            command.Parameters[extension].IsNullable = true;
            command.Parameters[extension].Value = employee.Extension is null ? DBNull.Value : employee.Extension;

            const string photo = "@photo";
            command.Parameters.Add(photo, SqlDbType.Image);
            command.Parameters[photo].IsNullable = true;
            command.Parameters[photo].Value = employee.Photo is null ? DBNull.Value : employee.Photo;

            const string notes = "@notes";
            command.Parameters.Add(notes, SqlDbType.NText);
            command.Parameters[notes].IsNullable = true;
            command.Parameters[notes].Value = employee.Notes is null ? DBNull.Value : employee.Notes;

            const string reportsTo = "@reportsTo";
            command.Parameters.Add(reportsTo, SqlDbType.Int);
            command.Parameters[reportsTo].IsNullable = true;
            command.Parameters[reportsTo].Value = employee.ReportsTo is null ? DBNull.Value : employee.ReportsTo;

            const string photoPath = "@photoPath";
            command.Parameters.Add(photoPath, SqlDbType.NVarChar, 255);
            command.Parameters[photoPath].IsNullable = true;
            command.Parameters[photoPath].Value = employee.PhotoPath is null ? DBNull.Value : employee.PhotoPath;
        }

        private async Task OpenIfClosedAsync()
        {
            if (this.connection.State == ConnectionState.Open)
            {
                await this.connection.CloseAsync();
            }

            await this.connection.OpenAsync();
        }
    }
}