
CREATE PROCEDURE dbo.TwoFASetting_UpdateChannelInfo @UserId INT
	,@SiteId SMALLINT
	,@ChannelId VARCHAR(255)
	,@ConversationId VARCHAR(255) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20240508@Frankie.Nguyen
Task: update channel info (for chatbot method)
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
		,@methodId SMALLINT = 3 --only chatbot method run this sp

	SET @xml = (
			SELECT s.UserId AS '@UserId'
				,s.SiteId AS '@SiteId'
				,s.MethodId AS '@MethodId'
				,s.ChannelId AS '@ChannelId'
				,c.ConversationId AS '@ConversationId'
			FROM dbo.TwoFASetting AS s WITH (NOLOCK)
				LEFT JOIN dbo.[Conversation] AS c WITH (NOLOCK) ON s.UserId = c.UserId
				AND s.ChannelId = c.ChannelId
			WHERE s.UserId = @UserId
				AND s.SiteId = @SiteId
				AND s.MethodId = @methodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	UPDATE dbo.TwoFASetting WITH (ROWLOCK, UPDLOCK)
	SET ChannelId = @ChannelId
	WHERE UserId = @UserId
		AND SiteId = @SiteId
		AND MethodId = @methodId

	IF EXISTS (
			SELECT 1
			FROM dbo.[Conversation] WITH (NOLOCK)
			WHERE UserId = @UserId
				AND ChannelId = @ChannelId
			)
	BEGIN
		UPDATE dbo.[Conversation] WITH (ROWLOCK, UPDLOCK)
		SET ConversationId = @ConversationId
		WHERE UserId = @UserId
			AND ChannelId = @ChannelId
	END
	ELSE
	BEGIN
		INSERT INTO dbo.[Conversation] (
			UserId
			,ChannelId
			,ConversationId
			)
		VALUES (
			@UserId
			,@ChannelId
			,@ConversationId
			)
	END

	-- write log
	SET @xml = (
			SELECT s.UserId AS '@UserId'
				,s.SiteId AS '@SiteId'
				,s.MethodId AS '@MethodId'
				,s.ChannelId AS '@ChannelId'
				,c.ConversationId AS '@ConversationId'
			FROM dbo.TwoFASetting AS s WITH (NOLOCK)
				LEFT JOIN dbo.[Conversation] AS c WITH (NOLOCK) ON s.UserId = c.UserId
				AND s.ChannelId = c.ChannelId
			WHERE s.UserId = @UserId
				AND s.SiteId = @SiteId
				AND s.MethodId = @methodId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(MILLISECOND, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @UserId
		,@SiteId = @SiteId
		,@SpName = 'TwoFASetting_UpdateChannelInfo'
		,@ObjectId = NULL
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END