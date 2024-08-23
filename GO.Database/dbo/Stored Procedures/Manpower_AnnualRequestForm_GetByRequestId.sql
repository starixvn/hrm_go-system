
CREATE PROCEDURE [dbo].[Manpower_AnnualRequestForm_GetByRequestId] @RequestId INT
AS
/*  
Created: 20230616@Tim.Trinh
Task: get annual request by id
DB:
Original:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @PeriodId SMALLINT
		,@Year INT
		,@UserId INT
		,@VerifierId INT
		,@ApproverId INT
		,@UnitName NVARCHAR(50)
		,@VerifyProcessId TINYINT = 2
		,@ApproveProcessId TINYINT = 3
		,@CurrentProcessId TINYINT = NULL
		,@PendingStatusId TINYINT = 1
		,@VerifiedStatusId TINYINT = 2
		,@ApprovedStatusId TINYINT = 3
		,@Today DATE = GETDATE()

	CREATE TABLE #tmpJobLevel (JobLevel NVARCHAR(50))

	SELECT @UserId = p.CreatedBy
		,@Year = p.[Year]
		,@UnitName = UnitName
		,@PeriodId = p.PeriodId
	FROM dbo.Manpower_AnnualRequestForm AS rq WITH (NOLOCK)
		INNER JOIN dbo.Manpower_Period AS p WITH (NOLOCK) ON rq.PeriodId = p.PeriodId
	WHERE rq.RequestId = @RequestId

	SELECT @VerifierId = emp.UserId
	FROM dbo.Manpower_Flow AS flow WITH (NOLOCK)
		INNER JOIN dbo.HRM_Employee AS emp WITH (NOLOCK) ON emp.CompanyId = flow.CompanyId
		AND emp.EmpId = flow.EmpId
	WHERE flow.UnitName = @UnitName
		AND flow.ProcessId = @VerifyProcessId

	SELECT @ApproverId = emp.UserId
	FROM dbo.Manpower_Flow AS flow WITH (NOLOCK)
		INNER JOIN dbo.HRM_Employee AS emp WITH (NOLOCK) ON emp.CompanyId = flow.CompanyId
		AND emp.EmpId = flow.EmpId
	WHERE flow.UnitName = @UnitName
		AND flow.ProcessId = @ApproveProcessId

	SELECT @CurrentProcessId = ProcessId
	FROM dbo.Manpower_PeriodProcessSetting WITH (NOLOCK)
	WHERE PeriodId = @PeriodId
		AND @Today BETWEEN FromDate
			AND ToDate

	INSERT INTO #tmpJobLevel (JobLevel)
	SELECT DISTINCT Parent
	FROM dbo.Manpower_JobLevelMapping WITH (NOLOCK)

	SELECT DISTINCT rq.RequestId
		,rq.UnitName
		,rq.StatusId
		,@CurrentProcessId AS CurrentProcessId
		,rq.PeriodId
		,p.[Name]
		,p.[Year]
		,creator.UserId AS CreatorId
		,creator.Code
		,creator.Nickname
		,creator.FullName
		,verifier.UserId AS VerifierId
		,verifier.Code
		,verifier.Nickname
		,verifier.FullName
		,approver.UserId AS ApproverId
		,approver.Code
		,approver.Nickname
		,approver.FullName
	FROM dbo.Manpower_AnnualRequestForm AS rq WITH (NOLOCK)
		INNER JOIN dbo.Manpower_Period AS p WITH (NOLOCK) ON rq.PeriodId = p.PeriodId
		INNER JOIN dbo.[User] AS creator WITH (NOLOCK) ON rq.CreatedBy = creator.UserId
		INNER JOIN dbo.[User] AS verifier WITH (NOLOCK) ON (
			rq.StatusId IN (@VerifiedStatusId, @ApprovedStatusId)
			AND rq.VerifiedBy = verifier.UserId
			)
		OR (
			rq.StatusId = @PendingStatusId
			AND verifier.UserId = @VerifierId
			)
		INNER JOIN dbo.[User] AS approver WITH (NOLOCK) ON (
			rq.StatusId = @ApprovedStatusId
			AND rq.ApprovedBy = approver.UserId
			)
		OR (
			rq.StatusId IN (@PendingStatusId, @VerifiedStatusId)
			AND approver.UserId = @ApproverId
			)
	WHERE rq.RequestId = @RequestId

	SELECT c.CompanyId
		,c.[Name] AS CompanyName
		,unit.UnitId
		,unit.[Name] AS UnitName
		,dept.DepartmentId
		,dept.[Name] AS DepartmentName
		,sec.SectionId
		,sec.[Name] AS SectionName
		,tmpjl.JobLevel
		,rqdt.LocalAddition
		,rqdt.ExpatAddition
	FROM dbo.Manpower_AnnualRequestForm AS rq WITH (NOLOCK)
		INNER JOIN dbo.HRM_Unit AS unit WITH (NOLOCK) ON rq.UnitName = unit.[Name]
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON unit.CompanyId = c.CompanyId
		AND c.IsActive = 1
		INNER JOIN dbo.HRM_Department AS dept WITH (NOLOCK) ON unit.CompanyId = dept.CompanyId
		AND unit.UnitId = dept.UnitId
		AND dept.IsActive = 1
		AND dept.IsDeleted = 0
		INNER JOIN dbo.HRM_Section AS sec WITH (NOLOCK) ON dept.CompanyId = sec.CompanyId
		AND dept.DepartmentId = sec.DepartmentId
		AND sec.IsActive = 1
		AND sec.IsDeleted = 0
		CROSS APPLY #tmpJobLevel AS tmpjl WITH (NOLOCK)
		LEFT JOIN dbo.Manpower_AnnualRequestFormDetail AS rqdt WITH (NOLOCK) ON rq.RequestId = rqdt.RequestId
		AND sec.CompanyId = rqdt.CompanyId
		AND sec.SectionId = rqdt.SectionId
		AND rqdt.JobLevel = tmpjl.JobLevel
	WHERE rq.RequestId = @RequestId
	ORDER BY c.[Name] ASC
		,unit.[Name] ASC
		,dept.[Name] ASC
		,sec.[Name] ASC
		,tmpjl.JobLevel DESC

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
		,rqdt.ModifiedDate
		,rqdt.ModifiedBy
		,modifier.Code
		,modifier.Nickname
		,modifier.FullName
	FROM dbo.Manpower_AnnualRequestForm AS rq WITH (NOLOCK)
		INNER JOIN dbo.HRM_Unit AS unit WITH (NOLOCK) ON rq.UnitName = unit.[Name]
		INNER JOIN dbo.Company AS c WITH (NOLOCK) ON unit.CompanyId = c.CompanyId
		AND c.IsActive = 1
		INNER JOIN dbo.HRM_Department AS dept WITH (NOLOCK) ON unit.CompanyId = dept.CompanyId
		AND unit.UnitId = dept.UnitId
		AND dept.IsActive = 1
		AND dept.IsDeleted = 0
		INNER JOIN dbo.HRM_Section AS sec WITH (NOLOCK) ON dept.CompanyId = sec.CompanyId
		AND dept.DepartmentId = sec.DepartmentId
		AND sec.IsActive = 1
		AND sec.IsDeleted = 0
		INNER JOIN dbo.Manpower_AnnualRequestFormDetail_Log AS rqdt WITH (NOLOCK) ON rq.RequestId = rqdt.RequestId
		AND sec.CompanyId = rqdt.CompanyId
		AND sec.SectionId = rqdt.SectionId
		INNER JOIN dbo.[User] AS modifier WITH (NOLOCK) ON rqdt.ModifiedBy = modifier.UserId
	WHERE rq.RequestId = @RequestId
	ORDER BY c.[Name] ASC
		,unit.[Name] ASC
		,dept.[Name] ASC
		,sec.[Name] ASC
		,rqdt.JobLevel DESC
		,rqdt.ModifiedDate DESC

	DROP TABLE #tmpJobLevel
END