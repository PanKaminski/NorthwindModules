USE [Northwind]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Delete_Category]
    @categoryId int
AS
DELETE FROM dbo.Categories WHERE CategoryID = @categoryID
SELECT @@ROWCOUNT
GO

CREATE PROCEDURE [dbo].[Delete_Employee]
@employeeId int
AS
DELETE FROM dbo.Employees WHERE EmployeeID = @employeeId
SELECT @@ROWCOUNT
GO

CREATE PROCEDURE [dbo].[Delete_Product]
	@productID int
AS
	DELETE FROM dbo.Products WHERE ProductID = @productID
	SELECT @@ROWCOUNT
GO

CREATE PROCEDURE [dbo].[Get_Categories]
AS
SELECT c.CategoryID, c.CategoryName, c.Description, c.Picture
FROM dbo.Categories AS c
ORDER BY c.CategoryID
GO

CREATE PROCEDURE [dbo].[Get_Categories_By_Names]
    @categoryNames nvarchar(256)
AS
SELECT c.CategoryID, c.CategoryName, c.Description, c.Picture FROM dbo.Categories as c
WHERE @categoryNames LIKE CONCAT('%', c.CategoryName, '%')
ORDER BY c.CategoryID
GO

CREATE PROCEDURE [dbo].[Get_Categories_With_Limit]
    @offset int,
    @limit int
AS
SELECT c.CategoryID, c.CategoryName, c.Description, c.Picture FROM dbo.Categories AS c
ORDER BY c.CategoryID
OFFSET @offset ROWS
FETCH FIRST @limit ROWS ONLY
GO

CREATE PROCEDURE [dbo].[Get_Category_By_Id]
    @categoryId int
AS
SELECT c.CategoryID, c.CategoryName, c.Description, c.Picture FROM dbo.Categories as c
WHERE c.CategoryID = @categoryId
GO

CREATE PROCEDURE [dbo].[Get_Employee_By_Id]
@employeeId int
AS
SELECT emp.EmployeeID,
       emp.LastName,
       emp.FirstName,
       emp.Title,
       emp.TitleOfCourtesy,
       emp.BirthDate,
       emp.HireDate,
       emp.Address,
       emp.City,
       emp.Region,
       emp.PostalCode,
       emp.Country,
       emp.HomePhone,
       emp.Extension,
       emp.PhotoPath,
       emp.Photo,
       emp.ReportsTo,
       emp.Notes
FROM dbo.Employees as emp
WHERE emp.EmployeeID = @employeeId
GO

CREATE PROCEDURE [dbo].[Get_Employees]
AS
SELECT emp.EmployeeID,
       emp.LastName,
       emp.FirstName,
       emp.Title,
       emp.TitleOfCourtesy,
       emp.BirthDate,
       emp.HireDate,
       emp.Address,
       emp.City,
       emp.Region,
       emp.PostalCode,
       emp.Country,
       emp.HomePhone,
       emp.Extension,
       emp.PhotoPath,
       emp.Photo,
       emp.ReportsTo,
       emp.Notes
FROM dbo.Employees as emp
ORDER BY emp.EmployeeID
GO

CREATE PROCEDURE [dbo].[Get_Employees_With_Limit]
    @offset int,
    @limit int
AS
SELECT emp.EmployeeID,
       emp.LastName,
       emp.FirstName,
       emp.Title,
       emp.TitleOfCourtesy,
       emp.BirthDate,
       emp.HireDate,
       emp.Address,
       emp.City,
       emp.Region,
       emp.PostalCode,
       emp.Country,
       emp.HomePhone,
       emp.Extension,
       emp.PhotoPath,
       emp.Photo,
       emp.ReportsTo,
       emp.Notes
FROM dbo.Employees as emp
ORDER BY emp.EmployeeID
OFFSET @offset ROWS
FETCH FIRST @limit ROWS ONLY
GO

CREATE PROCEDURE [dbo].[Get_Product_By_Id]
	@productId int
AS
	SELECT p.ProductID, p.ProductName, p.SupplierID, p.CategoryID, p.QuantityPerUnit, p.UnitPrice, p.UnitsInStock, p.UnitsOnOrder, p.ReorderLevel, p.Discontinued 
	FROM dbo.Products AS p
	WHERE p.ProductID = @productId
GO

CREATE PROCEDURE [dbo].[Get_Products]
AS
	SELECT p.ProductID, p.ProductName, p.SupplierID, p.CategoryID, p.QuantityPerUnit, p.UnitPrice, p.UnitsInStock, p.UnitsOnOrder, p.ReorderLevel, p.Discontinued FROM dbo.Products as p
	ORDER BY p.ProductID
GO

CREATE PROCEDURE [dbo].[Get_Products_By_Categories]
	@categoriesIds nvarchar(256)
	AS
	SELECT p.ProductID, p.ProductName, p.SupplierID, p.CategoryID, p.QuantityPerUnit, p.UnitPrice, p.UnitsInStock, p.UnitsOnOrder, p.ReorderLevel, p.Discontinued
	FROM dbo.Products AS p
	WHERE @categoriesIds LIKE CONCAT('%', p.CategoryID, '%')
GO

CREATE PROCEDURE [dbo].[Get_Products_By_Name]
	@names nvarchar(256)
	AS
	SELECT p.ProductID, p.ProductName, p.SupplierID, p.CategoryID, p.QuantityPerUnit, p.UnitPrice, p.UnitsInStock, p.UnitsOnOrder, p.ReorderLevel, p.Discontinued FROM dbo.Products as p
    WHERE @names LIKE CONCAT('%', p.ProductName, '%')
	ORDER BY p.ProductID
GO

CREATE PROCEDURE [dbo].[Get_Products_With_Limit]
	@rowCount int,
	@offset int
AS
	SELECT p.ProductID, p.ProductName, p.SupplierID, p.CategoryID, p.QuantityPerUnit, p.UnitPrice, p.UnitsInStock, p.UnitsOnOrder, p.ReorderLevel, p.Discontinued FROM dbo.Products as p
	ORDER BY p.ProductID
	OFFSET @offset ROWS
	FETCH FIRST @rowCount ROWS ONLY
GO

CREATE PROCEDURE [dbo].[Insert_Category]
    @categoryName nvarchar(15),
    @description ntext,
    @picture image
AS
INSERT INTO dbo.Categories (CategoryName, Description, Picture)
OUTPUT Inserted.CategoryID
VALUES (@categoryName, @description, @picture)
GO

CREATE PROCEDURE [dbo].[Insert_Employee]
    @lastName nvarchar(20),
    @firstName nvarchar(10),
    @title nvarchar(30),
    @titleOfCourtesy nvarchar(25),
    @birthDate datetime,
    @hireDate datetime,
    @address nvarchar(60),
    @city nvarchar(15),
    @region nvarchar(15),
    @postalCode nvarchar(10),
    @country nvarchar(15),
    @homePhone nvarchar(24),
    @extension nvarchar(4),
    @photoPath nvarchar(255),
    @photo image,
    @reportsTo int,
    @notes ntext
AS
INSERT INTO dbo.Employees (
                           LastName,
                           FirstName,
                           Title,
                           TitleOfCourtesy,
                           BirthDate,
                           HireDate,
                           Address,
                           City,
                           Region,
                           PostalCode,
                           Country,
                           HomePhone,
                           Extension,
                           PhotoPath,
                           Photo,
                           ReportsTo,
                           Notes)
OUTPUT Inserted.EmployeeID
VALUES (
        @lastName,
        @firstName,
        @title,
        @titleOfCourtesy,
        @birthDate,
        @hireDate,
        @address,
        @city,
        @region,
        @postalCode,
        @country,
        @homePhone,
        @extension,
        @photoPath,
        @photo,
        @reportsTo,
        @notes)
GO

CREATE PROCEDURE [dbo].[Insert_Product]
	@productName nvarchar(40),
	@supplierId int,
	@categoryId int,
	@quantityPerUnit nvarchar(20),
	@unitPrice money,
	@unitsInStock smallint,
	@unitsOnOrder smallint,
	@reorderLevel smallint,
	@discontinued bit
AS
	INSERT INTO dbo.Products (
		ProductName, 
		SupplierID, 
		CategoryID, 
		QuantityPerUnit, 
		UnitPrice, 
		UnitsInStock, 
		UnitsOnOrder, 
		ReorderLevel, 
		Discontinued) 
	OUTPUT Inserted.ProductID
    VALUES (
		@productName, 
		@supplierId, 
		@categoryId, 
		@quantityPerUnit, 
		@unitPrice, 
		@unitsInStock, 
		@unitsOnOrder, 
		@reorderLevel, 
		@discontinued)
GO

CREATE PROCEDURE [dbo].[Update_Category]
    @categoryId int,
    @categoryName nvarchar(15),
    @description ntext,
    @picture image
AS
UPDATE dbo.Categories
SET CategoryName = @categoryName, Description = @description, Picture = @picture
WHERE CategoryID = @categoryId
SELECT @@ROWCOUNT
GO

CREATE PROCEDURE [dbo].[Update_Employee]
    @employeeId int,
    @lastName nvarchar(20),
    @firstName nvarchar(10),
    @title nvarchar(30),
    @titleOfCourtesy nvarchar(25),
    @birthDate datetime,
    @hireDate datetime,
    @address nvarchar(60),
    @city nvarchar(15),
    @region nvarchar(15),
    @postalCode nvarchar(10),
    @country nvarchar(15),
    @homePhone nvarchar(24),
    @extension nvarchar(4),
    @photoPath nvarchar(255),
    @photo image,
    @reportsTo int,
    @notes ntext
AS
UPDATE dbo.Employees
SET LastName = @lastName,
    FirstName = @firstName,
    Title = @title,
    TitleOfCourtesy = @titleOfCourtesy,
    BirthDate = @birthDate,
    HireDate = @hireDate,
    Address = @address,
    City = @city,
    Region = @region,
    PostalCode = @postalCode,
    Country = @country,
    HomePhone = @homePhone,
    Extension = @extension,
    PhotoPath = @photoPath,
    Photo = @photo,
    ReportsTo = @reportsTo,
    Notes = @notes
WHERE EmployeeID = @employeeId
SELECT @@ROWCOUNT
GO

CREATE PROCEDURE [dbo].[Update_Product]
	@productId int,
	@productName nvarchar(40),
	@supplierId int,
	@categoryId int,
	@quantityPerUnit int,
	@unitPrice money,
	@unitsInStock smallint,
	@unitsOnOrder smallint,
	@reorderLevel smallint,
	@discontinued bit
	AS
	UPDATE dbo.Products
	SET ProductName = @productName,
		SupplierID = @supplierId, 
		CategoryID = @categoryId, 
		QuantityPerUnit = @quantityPerUnit, 
		UnitPrice = @unitPrice, 
		UnitsInStock = @unitsInStock, 
		UnitsOnOrder = @unitsOnOrder, 
		ReorderLevel = @reorderLevel, 
		Discontinued = @discontinued
	WHERE ProductID = @productId
	SELECT @@ROWCOUNT
GO