
CREATE PROCEDURE [dbo].[AutoJob_Section_GetLastModifiedDateByCompanyId] @CompanyId SMALLINT
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
	FROM dbo.HRM_Section WITH (NOLOCK)
	WHERE CompanyId = @CompanyId
END