CREATE TABLE [dbo].[TwoFASetting] (
    [UserId]           INT           NOT NULL,
    [SiteId]           SMALLINT      NOT NULL,
    [MethodId]         SMALLINT      NOT NULL,
    [SecretKey]        VARCHAR (250) NULL,
    [VerificationCode] VARCHAR (6)   NULL,
    [ExpirationTime]   DATETIME      NULL,
    [Enabled]          BIT           NOT NULL,
    [IsRegistered]     BIT           NULL,
    [ChannelId]        VARCHAR (50)  NULL,
    CONSTRAINT [PK_TwoFASetting] PRIMARY KEY CLUSTERED ([UserId] ASC, [SiteId] ASC, [MethodId] ASC)
);

