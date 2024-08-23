

CREATE PROCEDURE [dbo].[TwoFASetting_SaveChannelId] @UserId INT
	,@SiteId SMALLINT
	,@ChannelId VARCHAR(255)
	,@ConversationId VARCHAR(255) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20240508@Frankie.Nguyen
Task: Save Channel Id login 2fa(for chatbot method)
DB: bodb_IUC
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	EXEC [dbo].[TwoFASetting_UpdateConversationId] @UserId = @UserId
	,@ChannelId = @ChannelId
	,@ConversationId  = @ConversationId

	DECLARE @logData NVARCHAR(MAX)
		,@logDataOld NVARCHAR(MAX)
		,@xml XML
		,@startRun DATETIME = GETUTCDATE()
		,@endRun DATETIME
		,@duration INT
		,@methodId SMALLINT = 3 --only chatbot method run this sp

	SET @xml = (
			SELECT UserId AS '@UserId'
				,SiteId AS '@SiteId'
				,MethodId AS '@MethodId'
				,ChannelId AS '@ChannelId'
			FROM [dbo].[TwoFASetting] WITH (NOLOCK)
			WHERE UserId = @UserId
				AND SiteId = @SiteId
				AND MethodId = @methodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	UPDATE [dbo].[TwoFASetting] WITH (ROWLOCK, UPDLOCK)
	SET ChannelId = @ChannelId
	WHERE UserId = @UserId
		AND SiteId = @SiteId
		AND MethodId = @methodId

	-- write log
	SET @xml = (
			SELECT UserId AS '@UserId'
				,SiteId AS '@SiteId'
				,MethodId AS '@MethodId'
				,ChannelId AS '@ChannelId'
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
		,@SpName = 'TwoFASetting_SaveChannelId'
		,@ObjectId = NULL
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END