CREATE TABLE [dbo].[UserMapping_New] (
    [UserId]       INT      NOT NULL,
    [LinkToUserId] INT      NOT NULL,
    [CreatedBy]    INT      NULL,
    [CreatedDate]  DATETIME NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL,
    CONSTRAINT [PK_UserMapping_New] PRIMARY KEY CLUSTERED ([UserId] ASC, [LinkToUserId] ASC)
);

