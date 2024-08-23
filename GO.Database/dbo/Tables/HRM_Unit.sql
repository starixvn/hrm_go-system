CREATE TABLE [dbo].[HRM_Unit] (
    [CompanyId]    SMALLINT       NOT NULL,
    [UnitId]       INT            NOT NULL,
    [Code]         NVARCHAR (50)  NULL,
    [Name]         NVARCHAR (200) NULL,
    [IsActive]     BIT            NULL,
    [IsDeleted]    BIT            NULL,
    [ModifiedDate] DATETIME       NULL,
    [SyncDate]     DATETIME       NULL,
    CONSTRAINT [PK_HRM_Unit] PRIMARY KEY CLUSTERED ([CompanyId] ASC, [UnitId] ASC)
);



