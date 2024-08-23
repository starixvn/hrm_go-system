
CREATE PROCEDURE [dbo].[Manpower_Flow_Update] @UnitName NVARCHAR(200)
	,@FlowXml NVARCHAR(MAX)
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20230606@Tim.Trinh
Task: update flow
DB:
Original:

Revisions:

Param's Explanation:
	@FlowXml:
		N'<Root>
		   <Row ProcessId="1" CompanyId="1" EmpId="1" />
		</Root>'
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @idoc INT
		,@startRun DATETIME = GETDATE()
		,@endRun DATETIME
		,@duration INT
		,@xml XML
		,@logData NVARCHAR(MAX)
		,@logOldData NVARCHAR(MAX)

	CREATE TABLE #tmpFlow (
		ProcessId TINYINT
		,CompanyId SMALLINT
		,EmpId INT
		)

	IF ISNULL(@FlowXml, '') != ''
	BEGIN
		EXEC sp_xml_preparedocument @idoc OUTPUT
			,@FlowXml

		INSERT INTO #tmpFlow (
			ProcessId
			,CompanyId
			,EmpId
			)
		SELECT ProcessId
			,CompanyId
			,EmpId
		FROM OPENXML(@idoc, '/Root/Row', 1) WITH (ProcessId TINYINT, CompanyId SMALLINT, EmpId INT) AS data

		EXEC sp_xml_removedocument @idoc
	END

	-- write log
	SET @xml = (
			SELECT flow.UnitName AS '@UnitName'
				,flow.ProcessId AS '@ProcessId'
				,flow.CompanyId AS '@CompanyId'
				,flow.EmpId AS '@EmpId'
			FROM dbo.Manpower_Flow AS flow WITH (NOLOCK)
			WHERE flow.UnitName = @UnitName
			FOR XML PATH('Row')
				,TYPE
			)
	SET @logOldData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	-- update
	DELETE flow
	FROM dbo.Manpower_Flow AS flow WITH (ROWLOCK, UPDLOCK)
	WHERE flow.UnitName = @UnitName

	INSERT INTO dbo.Manpower_Flow (
		UnitName
		,ProcessId
		,CompanyId
		,EmpId
		)
	SELECT @UnitName
		,ProcessId
		,CompanyId
		,EmpId
	FROM #tmpFlow AS tbl WITH (NOLOCK)

	-- write log
	SET @xml = (
			SELECT flow.UnitName AS '@UnitName'
				,flow.ProcessId AS '@ProcessId'
				,flow.CompanyId AS '@CompanyId'
				,flow.EmpId AS '@EmpId'
			FROM dbo.Manpower_Flow AS flow WITH (NOLOCK)
			WHERE flow.UnitName = @UnitName
			FOR XML PATH('Row')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETDATE()
	SET @duration = DATEDIFF(MILLISECOND, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
		,@SpName = 'Manpower_Flow_Update'
		,@ObjectId = NULL
		,@Data = @logData
		,@OldData = @logOldData
		,@LogDate = @startRun
		,@Duration = @duration

	DROP TABLE #tmpFlow
END