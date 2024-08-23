
CREATE PROCEDURE [dbo].[AutoJob_UnitHead_UpdateSchedule] @Value NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*  
Created: 20230227@Tim.Trinh
Task: update unit head schedule
DB:
Original:
Param's Explanation:
	• @Value:
		<Root>
			<Row CompanyId="1" EmpId="1" IsWorkDay="true" IsOffDay="false" IsHoliday="false" IsLeaveDay="false" LeaveTypeCode="" LeaveTypeName="" />
		</Root>
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @startRun DATETIME = GETUTCDATE()

	CREATE TABLE #tempUnitHeadSchedule (
		CompanyId SMALLINT
		,EmpId INT
		,WorkDate DATE
		,IsWorkDay BIT DEFAULT(0)
		,IsOffDay BIT DEFAULT(0)
		,IsHoliday BIT DEFAULT(0)
		,IsLeaveDay BIT DEFAULT(0)
		,LeaveTypeCode NVARCHAR(50)
		,LeaveTypeName NVARCHAR(200)
		)

	DECLARE @DocHandler INT

	EXEC sp_xml_preparedocument @DocHandler OUTPUT
		,@Value

	INSERT INTO #tempUnitHeadSchedule (
		CompanyId
		,EmpId
		,WorkDate
		,IsWorkDay
		,IsOffDay
		,IsHoliday
		,IsLeaveDay
		,LeaveTypeCode
		,LeaveTypeName
		)
	SELECT CompanyId
		,EmpId
		,WorkDate
		,IsWorkDay
		,IsOffDay
		,IsHoliday
		,IsLeaveDay
		,LeaveTypeCode
		,LeaveTypeName
	FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (CompanyId SMALLINT, EmpId INT, WorkDate DATE, IsWorkDay BIT, IsOffDay BIT, IsHoliday BIT, IsLeaveDay BIT, LeaveTypeCode NVARCHAR(50), LeaveTypeName NVARCHAR(200)) AS Data

	EXEC sp_xml_removedocument @DocHandler

	-- update
	UPDATE uhs
	SET uhs.IsWorkDay = tmp.IsWorkDay
		,uhs.IsOffDay = tmp.IsOffDay
		,uhs.IsHoliday = tmp.IsHoliday
		,uhs.IsLeaveDay = tmp.IsLeaveDay
		,uhs.LeaveTypeCode = tmp.LeaveTypeCode
		,uhs.LeaveTypeName = tmp.LeaveTypeName
		,uhs.SyncDate = @startRun
	FROM dbo.UnitHeadSchedule AS uhs WITH (NOLOCK)
		INNER JOIN #tempUnitHeadSchedule AS tmp WITH (NOLOCK) ON uhs.CompanyId = tmp.CompanyId
		AND uhs.EmpId = tmp.EmpId
		AND uhs.WorkDate = tmp.WorkDate
	WHERE uhs.IsWorkDay != tmp.IsWorkDay
		OR uhs.IsOffDay != tmp.IsOffDay
		OR uhs.IsHoliday != tmp.IsHoliday
		OR uhs.IsLeaveDay != tmp.IsLeaveDay
		OR uhs.LeaveTypeCode != tmp.LeaveTypeCode
		OR uhs.LeaveTypeName != tmp.LeaveTypeName

	INSERT INTO dbo.UnitHeadSchedule (
		CompanyId
		,EmpId
		,WorkDate
		,IsWorkDay
		,IsOffDay
		,IsHoliday
		,IsLeaveDay
		,LeaveTypeCode
		,LeaveTypeName
		,SyncDate
		)
	SELECT tmp.CompanyId
		,tmp.EmpId
		,tmp.WorkDate
		,IsWorkDay
		,IsOffDay
		,IsHoliday
		,IsLeaveDay
		,LeaveTypeCode
		,LeaveTypeName
		,@startRun
	FROM #tempUnitHeadSchedule AS tmp WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.UnitHeadSchedule AS uhs WITH (NOLOCK)
			WHERE uhs.CompanyId = tmp.CompanyId
				AND uhs.EmpId = tmp.EmpId
				AND uhs.WorkDate = tmp.WorkDate
			)

	DROP TABLE #tempUnitHeadSchedule
END