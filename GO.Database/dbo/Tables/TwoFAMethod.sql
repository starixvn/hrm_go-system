CREATE TABLE [dbo].[TwoFAMethod] (
    [MethodId]   SMALLINT     NOT NULL,
    [MethodCode] VARCHAR (50) NOT NULL,
    [IsActive]   BIT          NULL,
    CONSTRAINT [PK_TwoFAMethod] PRIMARY KEY CLUSTERED ([MethodId] ASC)
);

