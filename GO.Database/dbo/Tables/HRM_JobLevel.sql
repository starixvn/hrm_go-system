CREATE TABLE [dbo].[HRM_JobLevel] (
    [CompanyId]    SMALLINT       NOT NULL,
    [JobLevelId]   INT            NOT NULL,
    [Name]         NVARCHAR (150) NULL,
    [IsDeleted]    BIT            NULL,
    [ModifiedDate] DATETIME       NULL,
    [SyncDate]     DATETIME       NULL,
    CONSTRAINT [PK_HRM_JobLevel] PRIMARY KEY CLUSTERED ([CompanyId] ASC, [JobLevelId] ASC)
);



