
CREATE PROCEDURE [dbo].[Manpower_Flow_GetByUnit] @UnitName NVARCHAR(200)
AS
/*  
Created: 20230605@Tim.Trinh
Task: get flow by unit
DB:
Original:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT flow.UnitName
		,flow.ProcessId
		,c.CompanyId
		,c.[Name] AS CompanyName
		,emp.EmpId
		,emp.Code
		,emp.Nickname
		,emp.FullName
		,emp.Email
		,emp.IsActive
		,emp.IsDeleted
	FROM dbo.Manpower_Flow AS flow WITH (NOLOCK)
		INNER JOIN dbo.HRM_Employee AS emp WITH (NOLOCK) ON flow.CompanyId = emp.CompanyId
		AND flow.EmpId = emp.EmpId
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON flow.CompanyId = c.CompanyId
		AND c.IsActive = 1
	WHERE flow.UnitName = @UnitName
END