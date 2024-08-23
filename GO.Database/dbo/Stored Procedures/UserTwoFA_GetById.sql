
CREATE PROCEDURE [dbo].[UserTwoFA_GetById] @UserId INT
	,@SiteId SMALLINT
	,@MethodId SMALLINT = NULL
	,@Enabled BIT = NULL
AS
/*
Created: 20240508@Frankie.Nguyen
Task: get user has 2fa setting
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	MERGE dbo.TwoFASetting AS target
	USING dbo.TwoFAMethod AS source ON target.UserId = @UserId
			AND target.SiteId = @SiteId
			AND target.MethodId = source.MethodId
	WHEN NOT MATCHED BY TARGET THEN
			INSERT (
				UserId
				,SiteId
				,MethodId
				,[Enabled]
				)
			VALUES (
				@UserId
				,@SiteId
				,source.MethodId
				,0
				);

	SELECT s.UserId
		,s.SiteId
		,s.MethodId
		,m.MethodCode
		,s.ChannelId
		,c.ConversationId
		,s.SecretKey
		,s.[Enabled]
		,s.IsRegistered
		,s.VerificationCode
		,s.ExpirationTime
	FROM dbo.TwoFASetting AS s WITH (NOLOCK)
		INNER JOIN dbo.TwoFAMethod AS m WITH (NOLOCK) ON m.MethodId = s.MethodId
		LEFT JOIN dbo.[Conversation] AS c WITH (NOLOCK) ON c.UserId = s.UserId
		AND c.ChannelId = s.ChannelId
	WHERE s.UserId = @UserId
		AND s.SiteId = @SiteId
		AND (
			@MethodId IS NULL
			OR s.MethodId = @MethodId
			)
		AND m.IsActive = 1
		AND (
			@Enabled IS NULL
			OR s.[Enabled] = @Enabled
			)
END