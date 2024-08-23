CREATE TABLE [dbo].[Manpower_ExcludedEmployee] (
    [CompanyId]    SMALLINT NOT NULL,
    [EmpId]        INT      NOT NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL,
    CONSTRAINT [PK_Manpower_ExcludedEmployee] PRIMARY KEY CLUSTERED ([CompanyId] ASC, [EmpId] ASC)
);



