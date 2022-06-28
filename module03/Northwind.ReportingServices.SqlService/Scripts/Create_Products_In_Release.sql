CREATE PROCEDURE [dbo].[Products_In_Release]
AS
	SELECT ProductName, UnitPrice
	FROM Products AS Product_List
	WHERE (((Product_List.Discontinued)=0))