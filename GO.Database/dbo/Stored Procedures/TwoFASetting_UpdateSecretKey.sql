

CREATE PROCEDURE [dbo].[TwoFASetting_UpdateSecretKey] @UserId INT
	,@SiteId SMALLINT
	,@SecretKey VARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20240508@Frankie.Nguyen
Task: Update secret key OTP method
DB: bodb_IUC
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
		,@startRun DATETIME = GETUTCDATE()
		,@endRun DATETIME
		,@duration INT
		,@methodId SMALLINT = 1 --only otp method run this sp

	SET @xml = (
			SELECT UserId AS '@UserId'
				,SiteId AS '@SiteId'
				,MethodId AS '@MethodId'
				,[Enabled] AS '@Enabled'
				,SecretKey AS '@SecretKey'
				,IsRegistered AS '@IsRegistered'
			FROM [dbo].[TwoFASetting] WITH (NOLOCK)
			WHERE UserId = @UserId
				AND SiteId = @SiteId
				AND MethodId = @methodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	UPDATE [dbo].[TwoFASetting] WITH (ROWLOCK, UPDLOCK)
	SET SecretKey = @SecretKey
	WHERE UserId = @UserId
		AND SiteId = @SiteId
		AND MethodId = @methodId

	-- write log
	SET @xml = (
			SELECT UserId AS '@UserId'
				,SiteId AS '@SiteId'
				,MethodId AS '@MethodId'
				,[Enabled] AS '@Enabled'
				,SecretKey AS '@SecretKey'
				,IsRegistered AS '@IsRegistered'
			FROM [dbo].[TwoFASetting] WITH (NOLOCK)
			WHERE UserId = @UserId
				AND SiteId = @SiteId
				AND MethodId = @methodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(MILLISECOND, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @UserId
		,@SiteId = @SiteId
		,@SpName = 'TwoFASetting_UpdateSecretKey'
		,@ObjectId = NULL
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END