CREATE PROCEDURE [dbo].[Products_With_Price_Less_Than]
	@price money = 0
AS
	SELECT ProductName, UnitPrice
	FROM Products
	WHERE UnitPrice < @price