

CREATE PROCEDURE [dbo].[TwoFASetting_UpdateConversationId] @UserId INT
	,@ChannelId VARCHAR(50)
	,@ConversationId VARCHAR(255) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20240607@Frankie.Nguen
Task: Update conversation id
DB: bodb_HRP
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

	IF EXISTS (
			SELECT 1
			FROM [dbo].[Conversation] WITH (NOLOCK)
			WHERE UserId = @UserId
				AND ChannelId = @ChannelId
			)
	BEGIN
		SET @xml = (
				SELECT UserId AS '@UserId'
					,ChannelId AS '@ChannelId'
					,ConversationId AS '@ConversationId'
				FROM [dbo].[Conversation] WITH (NOLOCK)
				WHERE UserId = @UserId
					AND ChannelId = @ChannelId
				FOR XML PATH('r')
					,TYPE
				)
		SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

		UPDATE [dbo].[Conversation] WITH (ROWLOCK, UPDLOCK)
		SET ConversationId = @ConversationId
		WHERE UserId = @UserId
			AND ChannelId = @ChannelId
	END
	ELSE
	BEGIN
		INSERT INTO [dbo].[Conversation] (
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
			SELECT UserId AS '@UserId'
					,ChannelId AS '@ChannelId'
					,ConversationId AS '@ConversationId'
				FROM [dbo].[Conversation] WITH (NOLOCK)
				WHERE UserId = @UserId
					AND ChannelId = @ChannelId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(MILLISECOND, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @UserId
		,@SiteId = 13
		,@SpName = 'TwoFASetting_UpdateConversationId'
		,@ObjectId = NULL
		,@Data = @logData
		,@OldData = @logDataOld
		,@LogDate = @startRun
		,@Duration = @duration
END