
CREATE PROCEDURE [dbo].[API_User_Insert_V1] @UserId INT OUT
	,@SiteId SMALLINT
	,@Username VARCHAR(50) = NULL
	,@Code VARCHAR(20) = NULL
	,@Nickname NVARCHAR(50) = NULL
	,@FullName NVARCHAR(100) = NULL
	,@Password VARCHAR(100) = NULL
	,@BCryptPassword VARCHAR(100) = NULL
	,@Email VARCHAR(50) = NULL
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(1000) = '' OUTPUT
AS
/*
Created: 20211022@Tim.Trinh
Task: insert user
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
		,@xml XML

	INSERT INTO dbo.[User] (
		Username
		,Code
		,Nickname
		,FullName
		,[Password]
		,BCryptPassword
		,Email
		,CreatedBy
		,CreatedDate
		)
	VALUES (
		@Username
		,@Code
		,@Nickname
		,@FullName
		,@Password
		,@BCryptPassword
		,@Email
		,@CreatedBy
		,@startRun
		)

	SET @UserId = SCOPE_IDENTITY()

	INSERT INTO dbo.UserSite (
		UserId
		,SiteId
		,IsActive
		,CreatedBy
		,CreatedDate
		)
	VALUES (
		@UserId
		,@SiteId
		,1
		,@CreatedBy
		,@startRun
		)

	INSERT INTO dbo.UserRole (
		UserId
		,RoleId
		,SiteId
		,CreatedBy
		,CreatedDate
		)
	SELECT @UserId
		,RoleId
		,@SiteId
		,@CreatedBy
		,@startRun
	FROM dbo.[Role] WITH (NOLOCK)
	WHERE SiteId = @SiteId
		AND IsActive = 1
		AND IsDefault = 1

	-- write log
	SET @xml = (
			SELECT UserId AS '@UserId'
				,Username AS '@Username'
				,Code AS '@Code'
				,Nickname AS '@Nickname'
				,FullName AS '@FullName'
				,[Password] AS '@Password'
				,BCryptPassword AS '@BCryptPassword'
				,Email AS '@Email'
				,CreatedBy AS '@CreatedBy'
				,CreatedDate AS '@CreatedDate'
			FROM dbo.[User] WITH (NOLOCK)
			WHERE userid = @UserId
			FOR XML PATH('r')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')
	SET @endRun = GETUTCDATE()
	SET @duration = DATEDIFF(millisecond, @startRun, @endRun)

	EXEC dbo.Log_Detail_Insert @UserId = @CreatedBy
		,@SiteId = @SiteId
		,@SpName = 'API_User_Insert_New'
		,@ObjectId = @UserId
		,@Data = @logData
		,@OldData = NULL
		,@LogDate = @startRun
		,@Duration = @duration
END