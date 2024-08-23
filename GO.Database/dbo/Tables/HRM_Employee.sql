CREATE TABLE [dbo].[HRM_Employee] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [CompanyId]        SMALLINT       NOT NULL,
    [EmpId]            INT            NOT NULL,
    [UserId]           INT            NULL,
    [DomainUsername]   NVARCHAR (50)  NULL,
    [Username]         NVARCHAR (50)  NULL,
    [Code]             NVARCHAR (20)  NULL,
    [Nickname]         NVARCHAR (50)  NULL,
    [FullName]         NVARCHAR (100) NULL,
    [TeamId]           INT            NULL,
    [JobTitleId]       SMALLINT       NULL,
    [JobLevelId]       SMALLINT       NULL,
    [Email]            NVARCHAR (100) NULL,
    [Password]         NVARCHAR (100) NULL,
    [IsLocal]          BIT            NULL,
    [IsActive]         BIT            NULL,
    [IsSpecialUser]    BIT            NULL,
    [IsDeleted]        BIT            NULL,
    [GroupDate]        DATE           NULL,
    [JoinedDate]       DATE           NULL,
    [ConfirmedDate]    DATE           NULL,
    [ResignedDate]     DATE           NULL,
    [MilitaryFromDate] DATE           NULL,
    [MilitaryToDate]   DATE           NULL,
    [ModifiedDate]     DATETIME       NULL,
    [SyncDate]         DATETIME       NULL,
    CONSTRAINT [PK_HRM_Employee] PRIMARY KEY CLUSTERED ([CompanyId] ASC, [EmpId] ASC)
);





