

CREATE PROCEDURE [dbo].[TwoFASetting_UpdateStatus] @UserId INT
	,@SiteId SMALLINT
	,@MethodId SMALLINT
	,@IsRegistered BIT
	,@Enabled BIT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
 Created: 20240508@Frankie.Nguyen
 Task : Update status 2fa setting
 DB: 
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

	SET @xml = (
			SELECT UserId AS '@UserId'
				,SiteId AS '@SiteId'
				,MethodId as '@MethodId'
				,[Enabled] AS '@Enabled'
				,SecretKey AS '@SecretKey'
				,IsRegistered AS '@IsRegistered'
			FROM [dbo].[TwoFASetting] WITH (NOLOCK)
			WHERE UserId = @UserId
				AND SiteId = @SiteId
				AND MethodId = @MethodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	UPDATE dbo.[TwoFASetting] WITH (ROWLOCK, UPDLOCK)
	SET IsRegistered = @IsRegistered
		,[Enabled] = @Enabled
	WHERE UserId = @UserId
		AND SiteId = @SiteId
		AND MethodId = @MethodId

	-- write log
	SET @xml = (
			SELECT UserId AS '@UserId'
				,SiteId AS '@SiteId'
				,MethodId as '@MethodId'
				,[Enabled] AS '@Enabled'
				,SecretKey AS '@SecretKey'
				,IsRegistered AS '@IsRegistered'
			FROM [dbo].[TwoFASetting] WITH (NOLOCK)
			WHERE UserId = @UserId
				AND SiteId = @SiteId
				AND MethodId = @MethodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(MILLISECOND, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @UserId
		,@SiteId = @SiteId
		,@SpName = 'TwoFASetting_UpdateStatus'
		,@ObjectId = NULL
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END