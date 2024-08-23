

CREATE PROCEDURE [dbo].[UserTwoFA_GetConversation] @UserId INT
	,@ChannelId varchar(50)
AS
/*
Created: 20240606@Frankie.Nguyen
Task: get conversation user 2fa
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT c.UserId
		,c.ChannelId
		,c.ConversationId
	FROM dbo.[Conversation] as c WITH (NOLOCK)
	WHERE c.UserId = @UserId
		AND c.ChannelId = @ChannelId
END