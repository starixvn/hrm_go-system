
CREATE PROCEDURE [dbo].[UserMapping_Delete] @UserId INT
	,@LinkToUserId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20240607@Tim.Trinh
Task: delete user mapping
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
		,@logDataOld NVARCHAR(MAX)
		,@xml XML

	-- write log
	SET @xml = (
			SELECT UserId AS '@UserId'
				,LinkToUserId AS '@LinkToUserId'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.UserMapping_New WITH (NOLOCK)
			WHERE UserId = @UserId
				AND LinkToUserId = @LinkToUserId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	-- delete
	DELETE
	FROM dbo.UserMapping_New WITH (ROWLOCK, UPDLOCK)
	WHERE UserId = @UserId
		AND LinkToUserId = @LinkToUserId

	-- write log
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
		,@SiteId = NULL
		,@SpName = 'UserMapping_Delete'
		,@ObjectId = @UserId
		,@Data = NULL
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END