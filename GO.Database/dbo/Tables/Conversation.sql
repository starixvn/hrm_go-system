CREATE TABLE [dbo].[Conversation] (
    [UserId]         INT           NOT NULL,
    [ChannelId]      VARCHAR (50)  NOT NULL,
    [ConversationId] VARCHAR (255) NULL,
    CONSTRAINT [PK_Conversation] PRIMARY KEY CLUSTERED ([UserId] ASC, [ChannelId] ASC)
);

