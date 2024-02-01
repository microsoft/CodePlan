-- Loads projects as defined in @ProjectList

-- uses parameters
-- If using BLOB storage then the Database will need an External Data Source 'BlobStorage' needs to exist
/*    if working locally you will need a storage account and set the values in the Publish profilecreate
		create using:
		-- Create a database master key if one does not already exist, using your own password. This key is used to encrypt the credential secret in next step.
		CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'MaryHadALittleLAmb';
		
		-- Create a database scoped credential with Azure storage account key as the secret.
		CREATE DATABASE SCOPED CREDENTIAL BlobCredential WITH IDENTITY = 'SHARED ACCESS SIGNATURE', SECRET = 'shared access secret';

		-- Create an external data source with CREDENTIAL option.
		CREATE EXTERNAL DATA SOURCE BlobStorage WITH (LOCATION = '<ProjectPath>',CREDENTIAL = BlobCredential,TYPE = BLOB_STORAGE);
*/
-- $(ProjectPath) = Directory Path for local or BLOB

DECLARE @ProjectList VARCHAR(100) = '{"projects":["epaoall","roatp"]}';

DECLARE @ProjectLocation VARCHAR(100) = '$(ProjectPath)';

DECLARE @ProjectDef VARCHAR(100),
         @ProjectIndex INT,
         @ProjectExists INT,
         @ProjectName VARCHAR(100),
         @ProjectDesc VARCHAR(100),
         @ProjectId UNIQUEIDENTIFIER,
         @ProjectWorkflows VARCHAR(MAX),
         @ApplicationDataSchema VARCHAR(MAX),
         @JSON VARCHAR(MAX),
         @QJSON VARCHAR(MAX);

DECLARE @WorkflowIndex INT,
         @Workflows VARCHAR(MAX),
         @WorkflowExists INT,
         @WorkflowUpdate INT,
         @WorkflowId UNIQUEIDENTIFIER,
         @WorkFlowDescription VARCHAR(100),
         @WorkFlowVersion VARCHAR(100),
         @WorkFlowType VARCHAR(100);

DECLARE @sectionIndex INT,
        @sectionNo INT,
        @sequenceNo INT,
        @sequenceExists INT,
        @sectionId UNIQUEIDENTIFIER,
        @sections VARCHAR(MAX),
        @sectionFileId VARCHAR(100),
        @sectionIsActive BIT;

DECLARE @SectionTitle VARCHAR(250),
        @SectionLinkTitle VARCHAR(250),
        @SectionDisplayType VARCHAR(200);

DECLARE @LoadBLOB BIT = 0;  -- assume local - set to 1 if $(ProjectPath) starts with http

DECLARE @SQLString NVARCHAR(4000);  
DECLARE @ParmDefinition NVARCHAR(500);

DECLARE @AllSections NVARCHAR(MAX);
DECLARE @ObsoleteSections TABLE 	
(Id UNIQUEIDENTIFIER, SectionId UNIQUEIDENTIFIER, SequenceNo INT, SectionNo INT);
DECLARE @ObsoleteSectionsCount INT;

BEGIN
-- loop through the projects
	SET @ProjectIndex = 0;

	WHILE @ProjectIndex >= 0
	BEGIN
		SELECT @ProjectDef = JSON_VALUE(@ProjectList,'$.projects['+RTRIM(convert(char,@ProjectIndex))+']');
		PRINT 'Get project '+@ProjectDef;

		IF @ProjectDef IS NULL
			BREAK;

		-- START
		SET @ProjectLocation = '$(ProjectPath)';

		IF SUBSTRING(@ProjectLocation,1,4) = 'http'
		BEGIN
			SET @LoadBLOB = 1;
			SET @ProjectLocation = 'projects/' + @ProjectDef +'/';
			PRINT 'Loading from BLOB Storage '+@ProjectLocation;
		END
		ELSE
		BEGIN
			SET @ProjectLocation = @ProjectLocation + 'projects\' + @ProjectDef +'\';
			PRINT 'Loading from File Storage: '+@ProjectLocation;
		END
	
		-- inject project

		-- get project file
		IF @LoadBLOB = 1
			SET @SQLString = 'SELECT @project = BulkColumn
			FROM OPENROWSET
			(BULK '''+@ProjectLocation+'project.json'', DATA_SOURCE = ''BlobStorage'', SINGLE_CLOB) 
			AS project';
		ELSE
			SET @SQLString = 'SELECT @project = BulkColumn
			FROM OPENROWSET
			(BULK '''+@ProjectLocation+'project.json'', SINGLE_CLOB) 
			AS project';
		
		SET @ParmDefinition = '@project VARCHAR(MAX) OUTPUT';
		EXECUTE sp_executesql @SQLString, @ParmDefinition, @project = @JSON OUTPUT;

		-- extract project and workflow  
        SELECT @ProjectName = JSON_VALUE(@JSON,'$.Name'),  @ProjectDesc = JSON_VALUE(@JSON,'$.Description');
            
        -- load the Workflows
        SET @WorkflowIndex = 0;
        -- loop  through the workflows
        WHILE @WorkflowIndex >= 0
        BEGIN
            -- get the first/next workflow from project.
            PRINT 'Getting Workflow at index '+RTRIM(CONVERT(char,@WorkflowIndex))+' for project '+@ProjectName
            SELECT @Workflows = JSON_QUERY(@JSON,'$.Workflows['+RTRIM(convert(char,@WorkflowIndex))+']');

            IF @Workflows IS NULL
                BEGIN
					PRINT 'Nothing found'
                    BREAK;
				END
                                
            -- extract workflow(s)
            SELECT @WorkflowDescription = JSON_VALUE(@Workflows,'$.Description'), @WorkflowVersion = JSON_VALUE(@Workflows,'$.Version'), @WorkflowType = JSON_VALUE(@Workflows,'$.Type');
			
            PRINT 'Configuring Workflow '+RTRIM(CONVERT(char,@WorkflowDescription))+' '+RTRIM(CONVERT(char,@WorkflowVersion))

            -- check if project exists
            SELECT @ProjectExists = COUNT(*) FROM projects WHERE Name = @ProjectName

            -- Get the ApplicationDataSchema
            IF @LoadBLOB = 1
                SET @SQLString = 'SELECT @ad = BulkColumn
                FROM OPENROWSET
                (BULK '''+@ProjectLocation+'ApplicationDataSchema.json'', DATA_SOURCE = ''BlobStorage'', SINGLE_CLOB) 
                AS ad';
            ELSE
                SET @SQLString = 'SELECT @ad = BulkColumn
                FROM OPENROWSET
                (BULK '''+@ProjectLocation+'ApplicationDataSchema.json'', SINGLE_CLOB) 
                AS ad';
                    
            SET @ParmDefinition = '@ad VARCHAR(MAX) OUTPUT';
            EXECUTE sp_executesql @SQLString, @ParmDefinition, @ad = @ApplicationDataSchema OUTPUT;
                
            IF @ProjectExists = 0 
            BEGIN
            -- Need to create the "Project"
            
                INSERT INTO projects (Name, Description, ApplicationDataSchema, CreatedAt, CreatedBy)
                    VALUES (@ProjectName, @ProjectDesc, @ApplicationDataSchema, GETUTCDATE(), 'Deployment');
            END
            ELSE
            BEGIN
            -- Update the the "Project"
                UPDATE projects SET ApplicationDataSchema = @ApplicationDataSchema
                WHERE Name = @ProjectName;
            END
            -- get project id (back)
            SELECT @ProjectId = Id FROM projects WHERE Name = @ProjectName


            SELECT @WorkflowExists = COUNT(*) 
            FROM [Workflows]
            WHERE [ProjectId] = @ProjectId 
              AND [Description] = @WorkFlowDescription
              AND [Version] = @WorkFlowVersion;
          
            IF @WorkflowExists = 0 
            BEGIN	  
            -- Need to create the "Workflow"
            
                INSERT INTO [Workflows] ([ProjectId], [Description], [Version], [Type], [Status], [CreatedAt] ,[CreatedBy], [ApplicationDataSchema])
                SELECT p1.Id ProjectId, @WorkFlowDescription, @WorkFlowVersion, @WorkFlowType, 'Draft', [CreatedAt] ,[CreatedBy], [ApplicationDataSchema]
                FROM projects p1
                WHERE Name = @ProjectName;

            END
            BEGIN
            -- Update the "Workflow" - if needed
                SELECT @WorkflowUpdate = COUNT(*) 
                FROM [Workflows]
                WHERE [ProjectId] = @ProjectId 
                  AND [Description] = @WorkFlowDescription
                  AND [Version] = @WorkFlowVersion
                  AND [ApplicationDataSchema] = @ApplicationDataSchema;
                  
                IF @WorkflowUpdate = 0
                BEGIN
                    UPDATE [Workflows] 
                    SET [ApplicationDataSchema] = @ApplicationDataSchema, UpdatedAt = GETUTCDATE(), UpdatedBy = 'Deployment'
                    WHERE ProjectId = @ProjectId AND [Description] = @WorkFlowDescription AND [Version] = @WorkFlowVersion;
                END
            END
        
            -- get workflow id (back)
            SELECT @WorkflowId = Id 
             FROM [Workflows]
            WHERE ProjectId = @ProjectId 
              AND [Description] = @WorkFlowDescription
              AND [Version] = @WorkFlowVersion

            -- load the Sequences and Sections
            SET @sectionIndex = 0;
            -- loop  thorugh the sections
            WHILE @sectionIndex >= 0
            BEGIN
            -- get the first/next section from workflow.
                SELECT @sections = JSON_QUERY(@Workflows,'$.section['+RTRIM(convert(char,@sectionIndex))+']');
            
                IF @sections IS NULL
                    BREAK;
                
                SELECT @sectionNo = JSON_VALUE(@sections ,'$.SectionNo'), @sequenceNo = JSON_VALUE(@sections ,'$.SequenceNo'), @sectionFileId = JSON_VALUE(@sections ,'$.id')+'.json';
                -- Is section (de)Active - default Active = true
                SELECT @sectionIsActive = ISNULL(JSON_VALUE(@sections, '$.IsActive'),1);

                PRINT 'Configure Sequence '+RTRIM(CONVERT(char,@sequenceNo))+' Section '+RTRIM(CONVERT(char,@sectionNo));

                SELECT @sequenceExists = COUNT(*) 
                FROM [WorkflowSequences]
                WHERE [WorkflowId] = @WorkflowId AND [SequenceNo] = @sequenceNo AND [SectionNo] = @sectionNo;

                IF @sequenceExists = 0
                BEGIN
                    PRINT 'Insert Workflow for Sequence '+RTRIM(CONVERT(char,@sequenceNo))+' Section '+RTRIM(CONVERT(char,@sectionNo));
                    INSERT INTO [WorkflowSequences] (Workflowid, SequenceNo, SectionNo, SectionId, IsActive)
                    VALUES ( @WorkflowId, @sequenceNo, @sectionNo, NEWID(), @sectionIsActive);
                END
                ELSE
                BEGIN
                    UPDATE [WorkflowSequences] 
                    SET IsActive = @sectionIsActive
                    WHERE [WorkflowId] = @WorkflowId AND [SequenceNo] = @sequenceNo AND [SectionNo] = @sectionNo;
                END

                -- get section id 
                SELECT @sectionId = [SectionId] 
                FROM [WorkflowSequences]
                WHERE [WorkflowId] = @WorkflowId AND [SequenceNo] = @sequenceNo AND [SectionNo] = @sectionNo;

                PRINT 'Load Sequence '+RTRIM(CONVERT(char,@sequenceNo))+' Section '+RTRIM(CONVERT(char,@sectionNo));
                IF @LoadBLOB = 1
                    SET @SQLString = 'SELECT @qnaData = BulkColumn
                    FROM OPENROWSET
                    (BULK '''+@ProjectLocation+'sections/'+@sectionFileId+''', DATA_SOURCE = ''BlobStorage'', SINGLE_CLOB) 
                    AS qnaData';
                ELSE
                    SET @SQLString = 'SELECT @qnaData = BulkColumn
                    FROM OPENROWSET
                    (BULK '''+@ProjectLocation+'sections\'+@sectionFileId+''', SINGLE_CLOB) 
                    AS qnaData';

                SET @ParmDefinition = '@qnaData VARCHAR(MAX) OUTPUT';
                EXECUTE sp_executesql @SQLString, @ParmDefinition, @qnaData = @QJSON OUTPUT;
            
                -- get the Section details
                SELECT @SectionTitle = JSON_VALUE(@QJSON,'$.Title'),  @SectionLinkTitle = JSON_VALUE(@QJSON,'$.LinkTitle'), @SectionDisplayType = JSON_VALUE(@QJSON,'$.DisplayType')
            
                MERGE INTO [Workflowsections] ws1
                USING (SELECT @sectionId sectionId) upd
                ON (ws1.Id = upd.sectionId)
                WHEN MATCHED THEN 
                    UPDATE SET QnAData = @QJSON, [Title] = @SectionTitle, [LinkTitle] = @SectionLinkTitle, [DisplayType] = @SectionDisplayType
                WHEN NOT MATCHED THEN
                    INSERT (Id, ProjectId, QnAData, Title, LinkTitle, DisplayType)
                    VALUES (@sectionId, @ProjectId, @QJSON, @SectionTitle, @SectionLinkTitle, @SectionDisplayType);

                SET @sectionIndex = @sectionIndex + 1;
            END

            -- tidyup
            IF @WorkflowExists = 0 
            -- have created a new workflow
            BEGIN
                UPDATE Workflows SET Status = 'Dead' WHERE [Id] != @WorkflowId AND [Type] = @WorkflowType AND [ProjectId] = @ProjectId
                UPDATE Workflows SET Status = 'Live' WHERE [Id] = @WorkflowId
            END
            ELSE 
            BEGIN
                -- Tidy up any sections that exist in the database but not in projects.json
                
                SELECT @AllSections = section FROM OPENJSON(@Workflows)
                WITH (
                        [section] NVARCHAR(MAX) AS JSON
                    )

                INSERT INTO @ObsoleteSections
                SELECT Id, SectionId, ws.SequenceNo, ws.SectionNo FROM dbo.WorkflowSequences ws 
                LEFT JOIN OPENJSON(@AllSections) 
                WITH
                    (
                        [SequenceNo] INT,
                        [SectionNo] INT
                    )
                jws
                ON jws.SequenceNo = ws.SequenceNo
                AND jws.SectionNo = ws.SectionNo
                WHERE WorkflowId = @WorkflowId
                AND jws.SequenceNo IS NULL
                AND jws.SectionNo IS NULL
                
                SELECT @ObsoleteSectionsCount = COUNT(*) FROM @ObsoleteSections
                PRINT 'Obsolete sections to be removed : ' + CONVERT(char,@ObsoleteSectionsCount)
                            
                DELETE FROM dbo.WorkflowSections WHERE Id IN (SELECT SectionId FROM @ObsoleteSections)

                DELETE FROM dbo.WorkflowSequences WHERE WorkflowId = @WorkflowId AND Id IN (SELECT Id FROM @ObsoleteSections)
            END
            SET @WorkflowIndex = @WorkflowIndex + 1;
        END

		SET @ProjectIndex = @ProjectIndex + 1;
	END

END

GO

