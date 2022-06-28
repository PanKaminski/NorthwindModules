REATE PROCEDURE [dbo].[Products_With_Price_Restriction]
	@min_price money = 0,
	@max_price money = 0
AS
	SELECT ProductName, UnitPrice
	FROM Products
	WHERE UnitPrice < @max_price AND UnitPrice > @min_price