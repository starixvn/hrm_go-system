CREATE TABLE [dbo].[HRM_Team] (
    [CompanyId]    SMALLINT       NOT NULL,
    [TeamId]       INT            NOT NULL,
    [Code]         NVARCHAR (50)  NULL,
    [Name]         NVARCHAR (200) NULL,
    [SectionId]    INT            NULL,
    [IsDeleted]    BIT            NULL,
    [ModifiedDate] DATETIME       NULL,
    [SyncDate]     DATETIME       NULL,
    CONSTRAINT [PK_HRM_Team] PRIMARY KEY CLUSTERED ([CompanyId] ASC, [TeamId] ASC)
);



