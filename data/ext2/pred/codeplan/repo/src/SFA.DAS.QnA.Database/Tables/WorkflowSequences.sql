CREATE TABLE [WorkflowSequences]
(
	[Id] [uniqueidentifier] NOT NULL,
	[WorkflowId] [uniqueidentifier] NOT NULL,
	[SequenceNo] [int] NOT NULL,
	[SectionNo] [int] NOT NULL,
	[SectionId] [uniqueidentifier] NOT NULL,
	[IsActive] [bit] NOT NULL DEFAULT 0,
CONSTRAINT [PK_WorkflowSequences] PRIMARY KEY ( [Id]  )
)
GO

ALTER TABLE [WorkflowSequences] ADD  CONSTRAINT [DF_WorkflowSequences_Id]  DEFAULT (newid()) FOR [Id]
GO

CREATE INDEX [IX_WorkflowSequences_WorkflowId] ON [WorkflowSequences]  ( [WorkflowId] )
GO
