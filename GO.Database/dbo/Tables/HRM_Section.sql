CREATE TABLE [dbo].[HRM_Section] (
    [CompanyId]    SMALLINT       NOT NULL,
    [SectionId]    INT            NOT NULL,
    [Code]         NVARCHAR (50)  NULL,
    [Name]         NVARCHAR (200) NULL,
    [DepartmentId] INT            NULL,
    [IsActive]     BIT            NULL,
    [IsDeleted]    BIT            NULL,
    [ModifiedDate] DATETIME       NULL,
    [SyncDate]     DATETIME       NULL,
    CONSTRAINT [PK_HRM_Section] PRIMARY KEY CLUSTERED ([CompanyId] ASC, [SectionId] ASC)
);



