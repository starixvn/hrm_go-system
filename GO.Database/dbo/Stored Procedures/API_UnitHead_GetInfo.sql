
CREATE PROCEDURE [dbo].[API_UnitHead_GetInfo]
AS
/*  
Created: 20230227@Tim.Trinh
Task: get unit head info
DB:
Original:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT c.CompanyId
		,c.[Name] AS CompanyName
		,uhinfo.EmpId
		,uhinfo.Code
		,uhinfo.Nickname
		,uhinfo.FullName
		,uhinfo.DepartmentCode
		,uhinfo.DepartmentName
		,uhinfo.IsActive
		,uhinfo.IsDeleted
	FROM dbo.UnitHeadInfo AS uhinfo WITH (NOLOCK)
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON uhinfo.CompanyId = c.CompanyId
	WHERE c.CompanyId IN (1, 3) -- nexcel, sms
END