CREATE PROCEDURE [dbo].[Most_Expensive_Products]
	@products_count int = 0 AS
	SELECT TOP(@products_count) ProductName, UnitPrice
	FROM [dbo].[Products]
	ORDER BY UnitPrice DESC