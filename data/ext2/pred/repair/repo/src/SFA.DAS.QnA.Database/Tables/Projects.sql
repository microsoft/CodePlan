CREATE TABLE [Projects]
(
    [Id]                    [uniqueidentifier] NOT NULL,
    [Name]                  [nvarchar](250)    NOT NULL,
    [Description]           [nvarchar](max)    NOT NULL,
    [ApplicationDataSchema] [nvarchar](max)    NOT NULL,
    [CreatedAt]             [datetime2](7)     NOT NULL,
    [CreatedBy]             [nvarchar](250)    NOT NULL,
    CONSTRAINT [PK_Projects] PRIMARY KEY ( [Id] )
) 
GO

ALTER TABLE [Projects]
    ADD CONSTRAINT [DF_Projects_Id] DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [Projects]
    ADD CONSTRAINT [DF_Projects_Description] DEFAULT ('') FOR [Description]
GO

ALTER TABLE [Projects]
    ADD CONSTRAINT [DF_Projects_CreatedAt] DEFAULT (getutcdate()) FOR [CreatedAt]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Projects_Name] ON [Projects]   (  [Name]  )
GO