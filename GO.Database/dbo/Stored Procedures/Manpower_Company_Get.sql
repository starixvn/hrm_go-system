
CREATE PROCEDURE [dbo].[Manpower_Company_Get]
AS
/*  
Created: 20230628@Tim.Trinh
Task: get company
DB:
Original:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT c.CompanyId
		,c.[Name]
	FROM dbo.Company AS c WITH (NOLOCK)
	WHERE c.IsActive = 1
		AND EXISTS (
			SELECT 1
			FROM dbo.HRM_Employee AS emp WITH (NOLOCK)
			WHERE c.CompanyId = emp.CompanyId
			)
END