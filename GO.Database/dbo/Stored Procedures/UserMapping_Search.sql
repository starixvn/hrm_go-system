
CREATE PROCEDURE [dbo].[UserMapping_Search] @CompanyId SMALLINT = NULL
	,@Keyword NVARCHAR(50) = NULL
	,@ExcludeUserId INT = NULL
	,@Type TINYINT = 1
AS
/*
Created: 20240607@Tim.Trinh
Task: search user
DB:
Original:

Revisions:
Param's Explanation:
	@Type: 1 - main, 2 - sub
*/
BEGIN
	SET NOCOUNT ON;
	SET @Keyword = '%' + ISNULL(@Keyword, '') + '%'

	SELECT TOP 20 emp.UserId
		,c.CompanyId
		,c.[Name] AS CompanyName
		,emp.Username
		,emp.Code
		,emp.Nickname
		,emp.FullName
	FROM dbo.HRM_Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON emp.CompanyId = c.CompanyId
	WHERE emp.IsActive = 1
		AND emp.IsDeleted = 0
		AND (
			ISNULL(emp.Nickname, '') LIKE @Keyword
			OR ISNULL(emp.FullName, '') LIKE @Keyword
			)
		AND (
			@CompanyId IS NULL
			OR c.CompanyId = @CompanyId
			)
		AND (
			(
				@Type = 1
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.UserMapping_New AS um WITH (NOLOCK)
					WHERE emp.UserId = um.UserId
					)
				)
			OR (
				@Type = 2
				AND emp.UserId != ISNULL(@ExcludeUserId, 0)
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.UserMapping_New AS um WITH (NOLOCK)
					WHERE emp.UserId = um.LinkToUserId
					)
				)
			)
END