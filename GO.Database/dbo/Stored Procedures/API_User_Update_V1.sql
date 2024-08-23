
CREATE PROCEDURE [dbo].[API_User_Update_V1] @UserId INT
	,@SiteId SMALLINT
	,@Code VARCHAR(20) = NULL
	,@Username VARCHAR(50) = NULL
	,@Nickname NVARCHAR(50) = NULL
	,@FullName NVARCHAR(100) = NULL
	,@Email VARCHAR(50) = NULL
	,@IsActive BIT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20211108@Tim.Trinh
Task: update user info
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
				,Code AS '@Code'
				,Username AS '@Username'
				,Nickname AS '@Nickname'
				,FullName AS '@FullName'
				,Email AS '@Email'
				,IsActive AS '@IsActive'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.[User] WITH (NOLOCK)
			WHERE UserId = @UserId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logDataOld = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	-- update
	UPDATE us
	SET us.IsActive = CASE
			WHEN u.IsActive = 1 AND @IsActive = 0 THEN 0
			WHEN u.IsActive = 0 AND @IsActive = 1 THEN 1
			ELSE us.IsActive END
		,us.ModifiedBy = @ModifiedBy
		,us.ModifiedDate = @startRun
	FROM dbo.[UserSite] AS us WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.[User] AS u WITH (NOLOCK) ON us.UserId = u.UserId
	WHERE u.UserId = @UserId
		AND (
			(u.IsActive = 1 AND @IsActive = 0)
			OR (u.IsActive = 0 AND @IsActive = 1)
			)
	
	UPDATE dbo.[User] WITH (ROWLOCK, UPDLOCK)
	SET Code = @Code
		,Username = CASE WHEN ISNULL(@Username, '') != '' THEN @Username ELSE Username END
		,Nickname = @Nickname
		,FullName = @FullName
		,Email = @Email
		,IsActive = @IsActive
		,IsDeleted = CASE WHEN @IsActive = 1 AND IsDeleted = 1 THEN 0 ELSE IsDeleted END
		,ModifiedBy = @ModifiedBy
		,ModifiedDate = @startRun
	WHERE UserId = @UserId

	-- write log
	SET @xml = (
			SELECT UserId AS '@UserId'
				,Code AS '@Code'
				,Username AS '@Username'
				,Nickname AS '@Nickname'
				,FullName AS '@FullName'
				,Email AS '@Email'
				,IsActive AS '@IsActive'
				,ModifiedBy AS '@ModifiedBy'
				,ModifiedDate AS '@ModifiedDate'
			FROM dbo.[User] WITH (NOLOCK)
			WHERE UserId = @UserId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	IF @ModifiedBy != 1
	BEGIN
		EXEC dbo.Log_Detail_Insert @UserId = @ModifiedBy
			,@SiteId = @SiteId
			,@SpName = 'API_User_Update_New'
			,@ObjectId = @UserId
			,@Data = @logData
			,@OldData = @logDataOld
			,@LogDate = @startRun
			,@Duration = @duration
	END
END