CREATE PROCEDURE [dbo].[Products_With_Local_Price]
AS
	SELECT ProductName, UnitPrice, Country
	FROM Products prs INNER JOIN Suppliers sup ON prs.SupplierID = sup.SupplierID