
CREATE PROCEDURE [dbo].[UserMapping_Insert] @UserId INT
	,@LinkToUserId INT
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20240607@Tim.Trinh
Task: insert user mapping
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @startRun DATETIME = GETUTCDATE()
		,@endRun DATETIME
		,@duration INT
		,@logData NVARCHAR(MAX)
		,@xml XML

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.UserMapping_New WITH (NOLOCK)
			WHERE UserId = @UserId
				AND LinkToUserId = @LinkToUserId
			)
	BEGIN
		INSERT INTO dbo.UserMapping_New (
			UserId
			,LinkToUserId
			,CreatedBy
			,CreatedDate
			)
		VALUES (
			@UserId
			,@LinkToUserId
			,@CreatedBy
			,@startRun
			)

		-- write log
		SET @xml = (
			SELECT UserId AS '@UserId'
				,LinkToUserId AS '@LinkToUserId'
				,CreatedBy AS '@CreatedBy'
				,CreatedDate AS '@CreatedDate'
			FROM dbo.UserMapping_New WITH (NOLOCK)
			WHERE UserId = @UserId
				AND LinkToUserId = @LinkToUserId
			FOR XML PATH('r')
				,TYPE
			)
		SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
		SET @endRun = GETUTCDATE()
		SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

		EXEC dbo.Log_Detail_Insert @UserId = @CreatedBy
			,@SiteId = NULL
			,@SpName = 'UserMapping_Insert'
			,@ObjectId = @UserId
			,@Data = @logData
			,@OldData = NULL
			,@LogDate = @startRun
			,@Duration = @duration
	END
END