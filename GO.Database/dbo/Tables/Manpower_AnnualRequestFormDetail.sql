CREATE TABLE [dbo].[Manpower_AnnualRequestFormDetail] (
    [RequestId]     INT           NOT NULL,
    [CompanyId]     SMALLINT      NOT NULL,
    [SectionId]     INT           NOT NULL,
    [JobLevel]      NVARCHAR (50) NOT NULL,
    [SectionName]   NVARCHAR (50) NULL,
    [LocalAddition] SMALLINT      NOT NULL,
    [ExpatAddition] SMALLINT      NOT NULL,
    CONSTRAINT [PK_Manpower_AnnualRequestFormDetail] PRIMARY KEY CLUSTERED ([RequestId] ASC, [CompanyId] ASC, [SectionId] ASC, [JobLevel] ASC)
);



