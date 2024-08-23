
CREATE PROCEDURE [dbo].[API_User_UpdatePassword_V1] @UserId INT
	,@SiteId SMALLINT
	,@BCryptPassword NVARCHAR(100)
	,@IsResetPassword BIT = 0
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20211015@Tim.Trinh
Task: update user password
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
		,@logDataOld NVARCHAR(MAX)
		,@xml XML

	-- write log
	SET @xml = (
			SELECT UserId AS '@UserId'
				,BCryptPassword AS '@BCryptPassword'
				,IsResetPassword AS '@IsResetPassword'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.[User] WITH (NOLOCK)
			WHERE UserId = @UserId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	-- update password
	UPDATE dbo.[User] WITH (ROWLOCK, UPDLOCK)
	SET BCryptPassword = @BCryptPassword
		,IsResetPassword = @IsResetPassword
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = @startRun
	WHERE UserId = @UserId

	-- write log
	SET @xml = (
			SELECT UserId AS '@UserId'
				,BCryptPassword AS '@BCryptPassword'
				,IsResetPassword AS '@IsResetPassword'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.[User] WITH (NOLOCK)
			WHERE UserId = @UserId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(MILLISECOND, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
		,@SiteId = @SiteId
		,@SpName = 'API_User_UpdatePassword_New'
		,@ObjectId = @UserId
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END