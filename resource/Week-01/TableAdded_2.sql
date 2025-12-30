USE [AA4]
 
GO
 
/****** Object:  Table [dbo].[EmailTemplate]    Script Date: 12/29/2025 12:00:01 PM ******/
 
SET ANSI_NULLS ON
 
GO
 
SET QUOTED_IDENTIFIER ON
 
GO
 
CREATE TABLE [dbo].[EmailTemplate](
 
	[Id] [int] IDENTITY(1,1) NOT NULL,
 
	[TemplateKey] [nvarchar](250) NULL,
 
	[Name] [nvarchar](250) NULL,
 
	[Description] [nvarchar](2050) NULL,
 
	[ToEmail] [nvarchar](250) NULL,
 
	[CcEmail] [nvarchar](250) NULL,
 
	[BccEmail] [nvarchar](250) NULL,
 
	[FromEmail] [nvarchar](250) NULL,
 
	[FromName] [nvarchar](250) NULL,
 
	[ReplyEmail] [nvarchar](250) NULL,
 
	[Subject] [nvarchar](250) NULL,
 
	[BodyContent] [nvarchar](max) NULL,
 
	[BodyFile] [nvarchar](250) NULL,
 
	[IsActive] [bit] NULL,
 
	[LastUpdatedBy] [nvarchar](250) NULL,
 
	[LastUpdatedDate] [datetime] NULL,
 
CONSTRAINT [PK_EmailTemplate] PRIMARY KEY CLUSTERED
 
(
 
	[Id] ASC
 
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
 
 
GO
 
/****** Object:  Table [dbo].[EmailHistory]    Script Date: 12/29/2025 12:00:05 PM ******/
 
SET ANSI_NULLS ON
 
GO
 
SET QUOTED_IDENTIFIER ON
 
GO
 
CREATE TABLE [dbo].[EmailHistory](
 
	[Id] [int] IDENTITY(1,1) NOT NULL,
 
	[TemplateKey] [nvarchar](250) NULL,
 
	[ToEmail] [nvarchar](250) NULL,
 
	[CcEmail] [nvarchar](250) NULL,
 
	[BccEmail] [nvarchar](250) NULL,
 
	[FromEmail] [nvarchar](250) NULL,
 
	[FromName] [nvarchar](250) NULL,
 
	[EmailSubject] [nvarchar](250) NULL,
 
	[EmailBodyContent] [nvarchar](max) NULL,
 
	[EmailSentDate] [datetime] NULL,
 
	[IsSystemAutoSent] [bit] NULL,
 
	[IsRead] [bit] NULL,
 
	[ReadCount] [int] NULL,
 
	[LastUpdatedDate] [datetime] NULL,
 
CONSTRAINT [PK_EmailHistory] PRIMARY KEY CLUSTERED
 
(
 
	[Id] ASC
 
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
 
GO
 
 
-- Step 1: Drop all dependent objects

DECLARE @SQL NVARCHAR(MAX) = '';
 
-- Drop Indexes

SELECT @SQL = @SQL + 'DROP INDEX [' + i.name + '] ON [dbo].[DeliveryStatusLog];' + CHAR(13)

FROM sys.indexes i

WHERE i.object_id = OBJECT_ID('[dbo].[DeliveryStatusLog]')

  AND i.name IS NOT NULL

  AND i.is_primary_key = 0;
 
-- Drop Check Constraints

SELECT @SQL = @SQL + 'ALTER TABLE [dbo].[DeliveryStatusLog] DROP CONSTRAINT [' + cc.name + '];' + CHAR(13)

FROM sys.check_constraints cc

WHERE cc.parent_object_id = OBJECT_ID('[dbo].[DeliveryStatusLog]');
 
-- Drop Foreign Keys

SELECT @SQL = @SQL + 'ALTER TABLE [dbo].[DeliveryStatusLog] DROP CONSTRAINT [' + fk.name + '];' + CHAR(13)

FROM sys.foreign_keys fk

WHERE fk.parent_object_id = OBJECT_ID('[dbo].[DeliveryStatusLog]');
 
-- Drop Default Constraints

SELECT @SQL = @SQL + 'ALTER TABLE [dbo].[DeliveryStatusLog] DROP CONSTRAINT [' + dc.name + '];' + CHAR(13)

FROM sys.default_constraints dc

WHERE dc.parent_object_id = OBJECT_ID('[dbo].[DeliveryStatusLog]');
 
-- Drop Table

IF OBJECT_ID('[dbo].[DeliveryStatusLog]', 'U') IS NOT NULL

    SET @SQL = @SQL + 'DROP TABLE [dbo].[DeliveryStatusLog];' + CHAR(13);
 
-- Execute the dynamic SQL

IF LEN(@SQL) > 0

    EXEC sp_executesql @SQL;
 
-- Step 2: Create Table

CREATE TABLE [dbo].[DeliveryStatusLog](

	[Id] [int] IDENTITY(1,1) NOT NULL,

	-- Reference to what was changed

	[EntityType] [nvarchar](50) NOT NULL,

	[EntityId] [int] NOT NULL,

	[SalesOrderId] [int] NULL,

	-- Status Change Details

	[OldStatus] [nvarchar](50) NULL,

	[NewStatus] [nvarchar](50) NOT NULL,

	[ChangeReason] [nvarchar](500) NULL,

	-- Additional Context

	[OldConfirmed] [bit] NULL,

	[NewConfirmed] [bit] NULL,

	[TrackingNumber] [nvarchar](100) NULL,

	-- Audit Fields

	[ChangedBy] [nvarchar](100) NOT NULL,

	[ChangedAt] [datetime] NOT NULL CONSTRAINT [DF_DeliveryStatusLog_ChangedAt] DEFAULT (GETDATE()),

	[IPAddress] [varchar](45) NULL,

	[UserAgent] [nvarchar](500) NULL,

	-- System Metadata

	[IsSystemGenerated] [bit] NOT NULL CONSTRAINT [DF_DeliveryStatusLog_IsSystemGenerated] DEFAULT (0),

	[SourceAction] [nvarchar](100) NULL,

	CONSTRAINT [PK_DeliveryStatusLog] PRIMARY KEY CLUSTERED ([Id] ASC)

);
 
-- Step 3: Add Foreign Key

ALTER TABLE [dbo].[DeliveryStatusLog] WITH CHECK

ADD CONSTRAINT [FK_DeliveryStatusLog_SalesOrderHeader]

FOREIGN KEY ([SalesOrderId])

REFERENCES [dbo].[SalesOrderHeader] ([Id]);
 
-- Step 4: Add Check Constraint

ALTER TABLE [dbo].[DeliveryStatusLog] WITH CHECK

ADD CONSTRAINT [CHK_EntityType]

CHECK ([EntityType] IN ('Delivery', 'SalesOrderHeader'));
 
-- Step 5: Create Indexes

CREATE NONCLUSTERED INDEX [IX_DeliveryStatusLog_Entity]

ON [dbo].[DeliveryStatusLog] ([EntityType], [EntityId])

INCLUDE ([ChangedAt], [NewStatus]);
 
CREATE NONCLUSTERED INDEX [IX_DeliveryStatusLog_SalesOrderId]

ON [dbo].[DeliveryStatusLog] ([SalesOrderId])

INCLUDE ([EntityType], [ChangedAt], [NewStatus]);
 
CREATE NONCLUSTERED INDEX [IX_DeliveryStatusLog_ChangedAt]

ON [dbo].[DeliveryStatusLog] ([ChangedAt] DESC)

INCLUDE ([EntityType], [EntityId], [NewStatus]);
 
CREATE NONCLUSTERED INDEX [IX_DeliveryStatusLog_ChangedBy]

ON [dbo].[DeliveryStatusLog] ([ChangedBy])

INCLUDE ([ChangedAt], [EntityType], [NewStatus]);
 


GO
 
-- 1. Insert New Permissions (if they don't exist)

IF NOT EXISTS (SELECT 1 FROM [Permission] WHERE [Name] = 'Report.View')

BEGIN

    INSERT INTO [Permission] ([Name]) VALUES ('Report.View');

END
 
IF NOT EXISTS (SELECT 1 FROM [Permission] WHERE [Name] = 'Report.DeliveryLog')

BEGIN

    INSERT INTO [Permission] ([Name]) VALUES ('Report.DeliveryLog');

END
 
-- 2. Variables to hold IDs (Dynamic Mapping)

DECLARE @Perm_ReportView INT = (SELECT Id FROM [Permission] WHERE [Name] = 'Report.View');

DECLARE @Perm_DeliveryLog INT = (SELECT Id FROM [Permission] WHERE [Name] = 'Report.DeliveryLog');
 
DECLARE @Group_Admin INT = 1;         -- Admin

DECLARE @Group_OrderManager INT = 3;  -- Order Manager
 
-- 3. Map to Admin (Group 1) - They get everything

IF NOT EXISTS (SELECT 1 FROM [PermissionGroupMap] WHERE PermissionId = @Perm_ReportView AND PermissionGroupId = @Group_Admin)

BEGIN

    INSERT INTO [PermissionGroupMap] (PermissionId, PermissionGroupId) VALUES (@Perm_ReportView, @Group_Admin);

END
 
IF NOT EXISTS (SELECT 1 FROM [PermissionGroupMap] WHERE PermissionId = @Perm_DeliveryLog AND PermissionGroupId = @Group_Admin)

BEGIN

    INSERT INTO [PermissionGroupMap] (PermissionId, PermissionGroupId) VALUES (@Perm_DeliveryLog, @Group_Admin);

END
 
-- 4. Map to Order Manager (Group 3) - They need to see logs for debugging

IF NOT EXISTS (SELECT 1 FROM [PermissionGroupMap] WHERE PermissionId = @Perm_ReportView AND PermissionGroupId = @Group_OrderManager)

BEGIN

    INSERT INTO [PermissionGroupMap] (PermissionId, PermissionGroupId) VALUES (@Perm_ReportView, @Group_OrderManager);

END
 
IF NOT EXISTS (SELECT 1 FROM [PermissionGroupMap] WHERE PermissionId = @Perm_DeliveryLog AND PermissionGroupId = @Group_OrderManager)

BEGIN

    INSERT INTO [PermissionGroupMap] (PermissionId, PermissionGroupId) VALUES (@Perm_DeliveryLog, @Group_OrderManager);

END
 
PRINT 'Permissions for Reports seeded successfully.';

GO
 