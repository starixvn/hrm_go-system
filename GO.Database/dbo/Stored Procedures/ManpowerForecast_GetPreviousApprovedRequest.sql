
CREATE PROCEDURE [dbo].[ManpowerForecast_GetPreviousApprovedRequest] @PeriodId SMALLINT
AS
/*  
Created: 20230622@Tim.Trinh
Task: get previous annual manpower forecast request
DB:
Original:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ApprovedStatusId TINYINT = 3
		,@Year INT
		,@PrevPeriodId SMALLINT = NULL
		,@EndDateOfYear DATE
		,@PrevEndDateOfYear DATE

	CREATE TABLE #tmpHeadcount (
		CompanyId SMALLINT
		,UnitId INT
		,UnitName NVARCHAR(200)
		,DepartmentId INT
		,DepartmentName NVARCHAR(200)
		,SectionId INT
		,SectionName NVARCHAR(200)
		,JobLevel NVARCHAR(50)
		,PrevLocalCurrentHeadcount INT
		,PrevExpatCurrentHeadcount INT
		,LocalCurrentHeadcount INT
		,ExpatCurrentHeadcount INT
		,LocalAddition SMALLINT
		,ExpatAddition SMALLINT
		)

	SELECT @Year = p.[Year]
	FROM dbo.Manpower_Period AS p WITH (NOLOCK)
	WHERE p.PeriodId = @PeriodId

	SELECT TOP 1 @PrevPeriodId = PeriodId
	FROM dbo.Manpower_Period WITH (NOLOCK)
	WHERE [Year] = @Year - 1
		AND IsActive = 1

	IF ISNULL(@PrevPeriodId, 0) > 0
	BEGIN
		SET @EndDateOfYear = DATEADD(DAY, - 1, CAST(@Year AS NVARCHAR(10)) + '-01-01')
		SET @EndDateOfYear = DATEADD(DAY, - 1, CAST(YEAR(@EndDateOfYear) AS NVARCHAR(10)) + '-09-01')

		IF EXISTS (
				SELECT 1
				FROM dbo.Manpower_Period WITH (NOLOCK)
				WHERE [Year] = @Year - 2
					AND IsActive = 1
				)
		BEGIN
			SET @PrevEndDateOfYear = DATEADD(DAY, - 1, CAST((@Year - 1) AS NVARCHAR(10)) + '-01-01')
			SET @PrevEndDateOfYear = DATEADD(DAY, - 1, CAST(YEAR(@PrevEndDateOfYear) AS NVARCHAR(10)) + '-09-01')
		END
		ELSE
		BEGIN
			SET @PrevEndDateOfYear = DATEADD(DAY, - 1, CAST((@Year - 1) AS NVARCHAR(10)) + '-01-01')
		END
	END
	ELSE
	BEGIN
		SET @PrevEndDateOfYear = DATEADD(DAY, - 1, CAST(@Year AS NVARCHAR(10)) + '-01-01')
		SET @EndDateOfYear = @PrevEndDateOfYear
	END

	SELECT c.CompanyId
		,c.[Name] AS CompanyName
		,unit.UnitId
		,unit.[Name] AS UnitName
		,dept.DepartmentId
		,dept.[Name] AS DepartmentName
		,sec.SectionId
		,sec.[Name] AS SectionName
		,rqdt.JobLevel
		,rqdt.LocalAddition
		,rqdt.ExpatAddition
	INTO #tmpAnnualRequest
	FROM dbo.Manpower_AnnualRequestForm AS rq WITH (NOLOCK)
		INNER JOIN dbo.Manpower_AnnualRequestFormDetail AS rqdt WITH (NOLOCK) ON rq.RequestId = rqdt.RequestId
		INNER JOIN dbo.HRM_Section AS sec WITH (NOLOCK) ON sec.CompanyId = rqdt.CompanyId
		AND sec.SectionId = rqdt.SectionId
		AND sec.IsActive = 1
		AND sec.IsDeleted = 0
		INNER JOIN dbo.HRM_Department AS dept WITH (NOLOCK) ON dept.CompanyId = sec.CompanyId
		AND dept.DepartmentId = sec.DepartmentId
		AND dept.IsActive = 1
		AND dept.IsDeleted = 0
		INNER JOIN dbo.HRM_Unit AS unit WITH (NOLOCK) ON unit.CompanyId = dept.CompanyId
		AND unit.UnitId = dept.UnitId
		AND unit.IsActive = 1
		AND unit.IsDeleted = 0
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON unit.CompanyId = c.CompanyId
		AND c.IsActive = 1
	WHERE rq.PeriodId = @PrevPeriodId
		AND rq.StatusId = @ApprovedStatusId

	INSERT INTO #tmpHeadcount (
		CompanyId
		,UnitId
		,UnitName
		,DepartmentId
		,DepartmentName
		,SectionId
		,SectionName
		,JobLevel
		)
	SELECT DISTINCT c.CompanyId
		,unit.UnitId
		,unit.[Name] AS UnitName
		,dept.DepartmentId
		,dept.[Name] AS DepartmentName
		,section.SectionId
		,section.[Name] AS SectionName
		,jlm.Parent AS JobLevel
	FROM dbo.HRM_Section AS section WITH (NOLOCK)
		INNER JOIN dbo.HRM_Department AS dept WITH (NOLOCK) ON section.CompanyId = dept.CompanyId
		AND section.DepartmentId = dept.DepartmentId
		AND dept.IsActive = 1
		AND dept.IsDeleted = 0
		INNER JOIN dbo.HRM_Unit AS unit WITH (NOLOCK) ON dept.CompanyId = unit.CompanyId
		AND dept.UnitId = unit.UnitId
		AND unit.IsActive = 1
		AND unit.IsDeleted = 0
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON unit.CompanyId = c.CompanyId
		AND c.IsActive = 1
		CROSS APPLY dbo.Manpower_JobLevelMapping AS jlm WITH (NOLOCK)
	WHERE section.IsActive = 1
		AND section.IsDeleted = 0
	ORDER BY c.CompanyId
		,unit.[Name]
		,dept.[Name]
		,section.SectionId
		,jlm.Parent

	SELECT emp.CompanyId
		,unit.UnitId
		,unit.[Name] AS UnitName
		,dept.DepartmentId
		,dept.[Name] AS DepartmentName
		,section.SectionId
		,section.[Name] AS SectionName
		,jlm.Parent AS JobLevel
		,SUM(CASE 
				WHEN emp.IsLocal = 1 THEN 1
				ELSE 0
				END) AS LocalCurrentHeadcount
		,SUM(CASE 
				WHEN emp.IsLocal = 0 THEN 1
				ELSE 0
				END) AS ExpatCurrentHeadcount
	INTO #tmpPrevCurrentHeadcount
	FROM dbo.HRM_Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.HRM_Team AS team WITH (NOLOCK) ON emp.CompanyId = team.CompanyId
		AND emp.TeamId = team.TeamId
		AND team.IsDeleted = 0
		INNER JOIN dbo.HRM_Section AS section WITH (NOLOCK) ON team.CompanyId = section.CompanyId
		AND team.SectionId = section.SectionId
		AND section.IsActive = 1
		AND section.IsDeleted = 0
		INNER JOIN dbo.HRM_Department AS dept WITH (NOLOCK) ON section.CompanyId = dept.CompanyId
		AND section.DepartmentId = dept.DepartmentId
		AND dept.IsActive = 1
		AND dept.IsDeleted = 0
		INNER JOIN dbo.HRM_Unit AS unit WITH (NOLOCK) ON dept.CompanyId = unit.CompanyId
		AND dept.UnitId = unit.UnitId
		AND unit.IsActive = 1
		AND unit.IsDeleted = 0
		INNER JOIN dbo.HRM_JobLevel AS jl WITH (NOLOCK) ON emp.CompanyId = jl.CompanyId
		AND emp.JobLevelId = jl.JobLevelId
		AND jl.IsDeleted = 0
		INNER JOIN dbo.Manpower_JobLevelMapping AS jlm WITH (NOLOCK) ON jl.[Name] = jlm.Child
	WHERE emp.JoinedDate <= @PrevEndDateOfYear
		AND (
			emp.IsActive = 1
			OR (
				emp.IsActive = 0
				AND emp.ResignedDate > @PrevEndDateOfYear
				)
			)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.Manpower_ExcludedEmployee AS exclude WITH (NOLOCK)
			WHERE emp.CompanyId = exclude.CompanyId
				AND emp.EmpId = exclude.EmpId
			)
	GROUP BY emp.CompanyId
		,unit.UnitId
		,unit.[Name]
		,dept.DepartmentId
		,dept.[Name]
		,section.SectionId
		,section.[Name]
		,jlm.Parent
	ORDER BY emp.CompanyId
		,unit.[Name]
		,dept.[Name]
		,section.SectionId
		,jlm.Parent

	SELECT emp.CompanyId
		,unit.UnitId
		,unit.[Name] AS UnitName
		,dept.DepartmentId
		,dept.[Name] AS DepartmentName
		,section.SectionId
		,section.[Name] AS SectionName
		,jlm.Parent AS JobLevel
		,SUM(CASE 
				WHEN emp.IsLocal = 1 THEN 1
				ELSE 0
				END) AS LocalCurrentHeadcount
		,SUM(CASE 
				WHEN emp.IsLocal = 0 THEN 1
				ELSE 0
				END) AS ExpatCurrentHeadcount
	INTO #tmpCurrentHeadcount
	FROM dbo.HRM_Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.HRM_Team AS team WITH (NOLOCK) ON emp.CompanyId = team.CompanyId
		AND emp.TeamId = team.TeamId
		AND team.IsDeleted = 0
		INNER JOIN dbo.HRM_Section AS section WITH (NOLOCK) ON team.CompanyId = section.CompanyId
		AND team.SectionId = section.SectionId
		AND section.IsActive = 1
		AND section.IsDeleted = 0
		INNER JOIN dbo.HRM_Department AS dept WITH (NOLOCK) ON section.CompanyId = dept.CompanyId
		AND section.DepartmentId = dept.DepartmentId
		AND dept.IsActive = 1
		AND dept.IsDeleted = 0
		INNER JOIN dbo.HRM_Unit AS unit WITH (NOLOCK) ON dept.CompanyId = unit.CompanyId
		AND dept.UnitId = unit.UnitId
		AND unit.IsActive = 1
		AND unit.IsDeleted = 0
		INNER JOIN dbo.HRM_JobLevel AS jl WITH (NOLOCK) ON emp.CompanyId = jl.CompanyId
		AND emp.JobLevelId = jl.JobLevelId
		AND jl.IsDeleted = 0
		INNER JOIN dbo.Manpower_JobLevelMapping AS jlm WITH (NOLOCK) ON jl.[Name] = jlm.Child
	WHERE emp.JoinedDate <= @EndDateOfYear
		AND (
			emp.IsActive = 1
			OR (
				emp.IsActive = 0
				AND emp.ResignedDate > @EndDateOfYear
				)
			)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.Manpower_ExcludedEmployee AS exclude WITH (NOLOCK)
			WHERE emp.CompanyId = exclude.CompanyId
				AND emp.EmpId = exclude.EmpId
			)
	GROUP BY emp.CompanyId
		,unit.UnitId
		,unit.[Name]
		,dept.DepartmentId
		,dept.[Name]
		,section.SectionId
		,section.[Name]
		,jlm.Parent
	ORDER BY emp.CompanyId
		,unit.[Name]
		,dept.[Name]
		,section.SectionId
		,jlm.Parent

	UPDATE tmp
	SET tmp.LocalAddition = rq.LocalAddition
		,tmp.ExpatAddition = rq.ExpatAddition
	FROM #tmpHeadcount AS tmp WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpAnnualRequest AS rq WITH (NOLOCK) ON tmp.CompanyId = rq.CompanyId
		AND tmp.UnitId = rq.UnitId
		AND tmp.UnitName = rq.UnitName
		AND tmp.DepartmentId = rq.DepartmentId
		AND tmp.DepartmentName = rq.DepartmentName
		AND tmp.SectionId = rq.SectionId
		AND tmp.SectionName = rq.SectionName
		AND tmp.JobLevel = rq.JobLevel

	UPDATE tmp
	SET tmp.PrevLocalCurrentHeadcount = rq.LocalCurrentHeadcount
		,tmp.PrevExpatCurrentHeadcount = rq.ExpatCurrentHeadcount
	FROM #tmpHeadcount AS tmp WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpPrevCurrentHeadcount AS rq WITH (NOLOCK) ON tmp.CompanyId = rq.CompanyId
		AND tmp.UnitId = rq.UnitId
		AND tmp.UnitName = rq.UnitName
		AND tmp.DepartmentId = rq.DepartmentId
		AND tmp.DepartmentName = rq.DepartmentName
		AND tmp.SectionId = rq.SectionId
		AND tmp.SectionName = rq.SectionName
		AND tmp.JobLevel = rq.JobLevel

	UPDATE tmp
	SET tmp.LocalCurrentHeadcount = rq.LocalCurrentHeadcount
		,tmp.ExpatCurrentHeadcount = rq.ExpatCurrentHeadcount
	FROM #tmpHeadcount AS tmp WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tmpCurrentHeadcount AS rq WITH (NOLOCK) ON tmp.CompanyId = rq.CompanyId
		AND tmp.UnitId = rq.UnitId
		AND tmp.UnitName = rq.UnitName
		AND tmp.DepartmentId = rq.DepartmentId
		AND tmp.DepartmentName = rq.DepartmentName
		AND tmp.SectionId = rq.SectionId
		AND tmp.SectionName = rq.SectionName
		AND tmp.JobLevel = rq.JobLevel

	SELECT CompanyId
		,UnitId
		,UnitName
		,DepartmentId
		,DepartmentName
		,SectionId
		,SectionName
		,JobLevel
		,PrevLocalCurrentHeadcount
		,PrevExpatCurrentHeadcount
		,LocalCurrentHeadcount
		,ExpatCurrentHeadcount
		,LocalAddition
		,ExpatAddition
	FROM #tmpHeadcount WITH (NOLOCK)

	DROP TABLE #tmpAnnualRequest

	DROP TABLE #tmpPrevCurrentHeadcount

	DROP TABLE #tmpCurrentHeadcount

	DROP TABLE #tmpHeadcount
END