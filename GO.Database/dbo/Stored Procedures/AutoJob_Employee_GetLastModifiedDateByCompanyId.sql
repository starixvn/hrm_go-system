
CREATE PROCEDURE [dbo].[AutoJob_Employee_GetLastModifiedDateByCompanyId] @CompanyId SMALLINT
AS
/*
Created: 20230316@Tim.Trinh
Task: get last modified date
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT MAX(ModifiedDate)
	FROM dbo.HRM_Employee WITH (NOLOCK)
	WHERE CompanyId = @CompanyId
END