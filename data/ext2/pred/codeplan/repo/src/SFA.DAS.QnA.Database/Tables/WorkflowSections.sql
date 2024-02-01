CREATE TABLE [WorkflowSections]
(
	[Id] [uniqueidentifier] NOT NULL,
	[ProjectId] [uniqueidentifier] NOT NULL,
	[QnAData] [nvarchar](max) NOT NULL,
	[Title] [nvarchar](250) NOT NULL,
	[LinkTitle] [nvarchar](250) NOT NULL,
	[DisplayType] [nvarchar](50) NOT NULL,
CONSTRAINT [PK_WorkflowSections] PRIMARY KEY ([Id])
) 
GO

ALTER TABLE [WorkflowSections] ADD  CONSTRAINT [DF_WorkflowSections_Id]  DEFAULT (newid()) FOR [Id]
GO


