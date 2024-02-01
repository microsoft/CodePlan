CREATE TABLE [Workflows]
(
	[Id] [uniqueidentifier] NOT NULL,
	[ProjectId] [uniqueidentifier] NOT NULL,
	[Description] [nvarchar](200) NOT NULL,
	[Version] [nvarchar](10) NOT NULL,
	[Type] [nvarchar](10) NOT NULL,
	[Status] [nvarchar](20) NOT NULL DEFAULT 'Live',
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](256) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nvarchar](256) NULL,
	[DeletedAt] [datetime2](7) NULL,
	[DeletedBy] [nvarchar](256) NULL, 
    [ApplicationDataSchema] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_Workflows] PRIMARY KEY (	[Id] )
) 
GO

ALTER TABLE [Workflows] ADD  DEFAULT (newid()) FOR [Id]
GO

CREATE INDEX [IX_Workflows_TypeStatus] ON [Workflows]  ( [Type], [Status] )
GO