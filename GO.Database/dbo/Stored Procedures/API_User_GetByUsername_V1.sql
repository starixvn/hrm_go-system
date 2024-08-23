
CREATE PROCEDURE [dbo].[API_User_GetByUsername_V1] @Username NVARCHAR(50)
AS
/*
Created: 20211014@Tim.Trinh
Task: get user by username
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT u.UserId
		,emp.DomainUsername
		,u.Username
		,u.Nickname
		,u.FullName
		,u.Code
		,u.BCryptPassword
		,u.OldPassword AS [Password]
		,u.Email
		,u.IsActive
		,u.IsDeleted
		,emp.EmpId
		,c.CompanyId
		,c.[Name] AS CompanyName
	INTO #tmpUser
	FROM dbo.[User] AS u WITH (NOLOCK)
		LEFT JOIN dbo.HRM_Employee AS emp WITH (NOLOCK) ON u.UserId = emp.UserId
		LEFT JOIN dbo.Company AS c WITH (NOLOCK) ON emp.CompanyId = c.CompanyId
	WHERE (
			u.Username = @Username
			OR u.Code = @Username
			OR SUBSTRING(ISNULL(emp.DomainUsername, ''), CHARINDEX('\', ISNULL(emp.DomainUsername, '')) + 1, LEN(ISNULL(emp.DomainUsername, ''))) = @Username
			)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.UserMapping_New AS um WITH (NOLOCK)
			WHERE u.UserId = um.UserId
			)

	SELECT UserId
		,DomainUsername
		,Username
		,Nickname
		,FullName
		,Code
		,BCryptPassword
		,[Password]
		,Email
		,IsActive
		,IsDeleted
		,NULL AS LinkToUserId
		,EmpId
		,CompanyId
		,CompanyName
	FROM #tmpUser WITH (NOLOCK)
	
	UNION
	SELECT u.UserId
		,emp.DomainUsername
		,u.Username
		,u.Nickname
		,u.FullName
		,u.Code
		,u.BCryptPassword
		,u.OldPassword AS [Password]
		,u.Email
		,u.IsActive
		,u.IsDeleted
		,um.LinkToUserId
		,emp.EmpId
		,c.CompanyId
		,c.[Name] AS CompanyName
	FROM dbo.[User] AS u WITH (NOLOCK)
		INNER JOIN dbo.UserMapping_New AS um WITH (NOLOCK) ON u.UserId = um.UserId
		LEFT JOIN dbo.HRM_Employee AS emp WITH (NOLOCK) ON u.UserId = emp.UserId
		LEFT JOIN dbo.Company AS c WITH (NOLOCK) ON emp.CompanyId = c.CompanyId
	WHERE EXISTS (
			SELECT 1
			FROM #tmpUser AS tmp WITH (NOLOCK)
			WHERE um.LinkToUserId = tmp.UserId
			)

	DROP TABLE #tmpUser
END