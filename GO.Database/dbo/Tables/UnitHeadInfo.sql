CREATE TABLE [dbo].[UnitHeadInfo] (
    [CompanyId]      SMALLINT       NOT NULL,
    [EmpId]          INT            NOT NULL,
    [Code]           NVARCHAR (20)  NULL,
    [Nickname]       NVARCHAR (50)  NULL,
    [FullName]       NVARCHAR (100) NULL,
    [DepartmentCode] NVARCHAR (50)  NULL,
    [DepartmentName] NVARCHAR (200) NULL,
    [IsActive]       BIT            NULL,
    [IsDeleted]      BIT            NULL,
    [SyncDate]       DATETIME       NULL,
    CONSTRAINT [PK_UnitHeadInfo] PRIMARY KEY CLUSTERED ([CompanyId] ASC, [EmpId] ASC)
);



