
CREATE PROCEDURE [dbo].[UserMapping_Get] @Keyword NVARCHAR(100) = NULL
AS
/*
Created: 20240606@Tim.Trinh
Task: get user mapping
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET @Keyword = '%' + ISNULL(@Keyword, '') + '%'

	SELECT emp.UserId
		,c.CompanyId
		,c.[Name] AS CompanyName
		,emp.EmpId
		,emp.Code
		,emp.Nickname
		,emp.FullName
		,dept.[Name] AS DepartmentName
		,s.[Name] AS SectionName
		,t.[Name] AS TeamName
		,um.LinkToUserId
	INTO #tmpSubUser
	FROM dbo.HRM_Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.UserMapping_New AS um WITH (NOLOCK) ON emp.UserId = um.UserId
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON emp.CompanyId = c.CompanyId
		AND c.IsActive = 1
		INNER JOIN dbo.HRM_Team AS t WITH (NOLOCK) ON emp.CompanyId = t.CompanyId
		AND emp.TeamId = t.TeamId
		AND t.IsDeleted = 0
		LEFT JOIN dbo.HRM_Section AS s WITH (NOLOCK) ON t.CompanyId = s.CompanyId
		AND t.SectionId = s.SectionId
		AND s.IsActive = 1
		AND s.IsDeleted = 0
		LEFT JOIN dbo.HRM_Department AS dept WITH (NOLOCK) ON s.CompanyId = dept.CompanyId
		AND s.DepartmentId = dept.DepartmentId
		AND dept.IsActive = 1
		AND dept.IsDeleted = 0
	WHERE emp.IsActive = 1
		AND emp.IsDeleted = 0

	SELECT DISTINCT emp.UserId
		,c.CompanyId
		,c.[Name] AS CompanyName
		,emp.EmpId
		,emp.Code
		,emp.Nickname
		,emp.FullName
		,dept.[Name] AS DepartmentName
		,s.[Name] AS SectionName
		,t.[Name] AS TeamName
	INTO #tmpMainUser
	FROM dbo.HRM_Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON emp.CompanyId = c.CompanyId
		AND c.IsActive = 1
		INNER JOIN dbo.HRM_Team AS t WITH (NOLOCK) ON emp.CompanyId = t.CompanyId
		AND emp.TeamId = t.TeamId
		AND t.IsDeleted = 0
		LEFT JOIN dbo.HRM_Section AS s WITH (NOLOCK) ON t.CompanyId = s.CompanyId
		AND t.SectionId = s.SectionId
		AND s.IsActive = 1
		AND s.IsDeleted = 0
		LEFT JOIN dbo.HRM_Department AS dept WITH (NOLOCK) ON s.CompanyId = dept.CompanyId
		AND s.DepartmentId = dept.DepartmentId
		AND dept.IsActive = 1
		AND dept.IsDeleted = 0
	WHERE emp.IsActive = 1
		AND emp.IsDeleted = 0
		AND (
			emp.Nickname LIKE @Keyword
			OR emp.FullName LIKE @Keyword
			)
		AND EXISTS (
			SELECT 1
			FROM #tmpSubUser AS su WITH (NOLOCK)
			WHERE emp.UserId = su.LinkToUserId
		)
	ORDER BY c.[Name]

	SELECT UserId
		,CompanyId
		,CompanyName
		,EmpId
		,Code
		,Nickname
		,FullName
		,DepartmentName
		,SectionName
		,TeamName
	FROM #tmpMainUser WITH (NOLOCK)

	SELECT UserId
		,CompanyId
		,CompanyName
		,EmpId
		,Code
		,Nickname
		,FullName
		,DepartmentName
		,SectionName
		,TeamName
		,LinkToUserId
	FROM #tmpSubUser AS su WITH (NOLOCK)
	WHERE EXISTS (
			SELECT 1
			FROM #tmpMainUser AS mu WITH (NOLOCK)
			WHERE su.LinkToUserId = mu.UserId
			)
	ORDER BY CompanyName

	DROP TABLE #tmpMainUser

	DROP TABLE #tmpSubUser
END