CREATE PROCEDURE [dbo].[Products_With_UnitsInStock_Deficit]
AS
	SELECT ProductName, UnitPrice
	FROM Products
	WHERE UnitsInStock < UnitsOnOrder