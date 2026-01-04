USE [AA4]
 
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
 USE [AA4]
GO
/****** Object:  Table [dbo].[EmailTemplate]    Script Date: 30-Dec-25 12:59:47 PM ******/
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
SET IDENTITY_INSERT [dbo].[EmailTemplate] ON 

INSERT [dbo].[EmailTemplate] ([Id], [TemplateKey], [Name], [Description], [ToEmail], [CcEmail], [BccEmail], [FromEmail], [FromName], [ReplyEmail], [Subject], [BodyContent], [BodyFile], [IsActive], [LastUpdatedBy], [LastUpdatedDate]) VALUES (1, N'mailToOrderPlace', N'', N'', N'##ToEmail##', N'', N'', N'support@mdua.com', N'MDUA Support', N'', N'Order Confirmed! (##OrderId##)', N'<!DOCTYPE html>
<html>
<head>
    <style>
        body {
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }
        .header {
            background-color: #4CAF50;
            color: white;
            padding: 20px;
            text-align: center;
            border-radius: 5px 5px 0 0;
        }
        .content {
            background-color: #f9f9f9;
            padding: 20px;
            border: 1px solid #ddd;
            border-radius: 0 0 5px 5px;
        }
        .order-details {
            background-color: white;
            padding: 15px;
            margin: 15px 0;
            border-left: 4px solid #4CAF50;
        }
        .footer {
            text-align: center;
            margin-top: 20px;
            color: #777;
            font-size: 12px;
        }
        .amount {
            font-size: 24px;
            color: #4CAF50;
            font-weight: bold;
        }
    </style>
</head>
<body>
    <div class="header">
        <h1>Order Confirmed! </h1>
    </div>
    <div class="content">
        <p>Dear ##UserName##,</p>
        <p>Thank you for your order! Your order has been successfully confirmed.</p>
        
        <div class="order-details">
            <h3>Order Details</h3>
            <p><strong>Order Number:</strong> ##OrderId##</p>
            <p><strong>Quantity:</strong> ##OrderQty## item(s)</p>
            <p><strong>Total Amount:</strong> <span class="amount">##OrderTotal## Tk</span></p>
        </div>

        <p>We''ll notify you once your order is shipped. You can track your order status using the order number above.</p>
        
        <p>If you have any questions, feel free to contact our support team.</p>

        <p>Best regards,<br><strong>MDUA Team</strong></p>
    </div>
    <div class="footer">
        <p>This is an automated email. Please do not reply to this message.</p>
        <p>&copy; 2025 MDUA. All rights reserved.</p>
    </div>
</body>
</html>', N'', 0, N'admin', CAST(N'2025-12-30T06:52:33.763' AS DateTime))
INSERT [dbo].[EmailTemplate] ([Id], [TemplateKey], [Name], [Description], [ToEmail], [CcEmail], [BccEmail], [FromEmail], [FromName], [ReplyEmail], [Subject], [BodyContent], [BodyFile], [IsActive], [LastUpdatedBy], [LastUpdatedDate]) VALUES (2, N'ThanksForJoining', N'ThanksForJoining', N'System Generated Template', N'##ToEmail##', NULL, NULL, N'support@mdua.com', N'MDUA Support', NULL, N'Welcome to MDUA!', N'
<style>
    body { font-family: ''Helvetica Neue'', Helvetica, Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }
    .container { max-width: 600px; margin: 20px auto; background: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
    .header { background: #1a1a1a; padding: 20px; text-align: center; }
    .header h1 { color: #ffffff; margin: 0; font-size: 24px; letter-spacing: 1px; }
    .content { padding: 30px; color: #333333; line-height: 1.6; }
    .content h2 { color: #2c3e50; margin-top: 0; }
    .btn { display: inline-block; background: #27ae60; color: #ffffff; text-decoration: none; padding: 12px 25px; border-radius: 4px; font-weight: bold; margin-top: 20px; }
    .btn:hover { background: #219150; }
    .footer { background: #f9f9f9; padding: 15px; text-align: center; font-size: 12px; color: #999999; border-top: 1px solid #eeeeee; }
    .info-table { width: 100%; border-collapse: collapse; margin: 15px 0; }
    .info-table th { text-align: left; color: #777; font-weight: normal; border-bottom: 1px solid #eee; padding: 8px 0; }
    .info-table td { text-align: right; font-weight: bold; border-bottom: 1px solid #eee; padding: 8px 0; }
    .highlight { color: #27ae60; font-weight: bold; }
</style>
<div class="container">
    <div class="header"><h1>WELCOME!</h1></div>
    <div class="content">
        <h2>Hi ##UserName##,</h2>
        <p>Your account has been successfully created.</p>
        <center><a href="##LoginLink##" class="btn">Login Now</a></center>
    </div>
    <div class="footer">&copy; 2025 MDUA Shop.</div>
</div>', NULL, 1, N'SystemSeed', CAST(N'2025-12-30T12:05:44.347' AS DateTime))
INSERT [dbo].[EmailTemplate] ([Id], [TemplateKey], [Name], [Description], [ToEmail], [CcEmail], [BccEmail], [FromEmail], [FromName], [ReplyEmail], [Subject], [BodyContent], [BodyFile], [IsActive], [LastUpdatedBy], [LastUpdatedDate]) VALUES (3, N'ResetPassword', N'ResetPassword', N'System Generated Template', N'##ToEmail##', NULL, NULL, N'support@mdua.com', N'MDUA Support', NULL, N'Reset Password - OTP: ##OTPCode##', N'
<style>
    body { font-family: ''Helvetica Neue'', Helvetica, Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }
    .container { max-width: 600px; margin: 20px auto; background: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
    .header { background: #1a1a1a; padding: 20px; text-align: center; }
    .header h1 { color: #ffffff; margin: 0; font-size: 24px; letter-spacing: 1px; }
    .content { padding: 30px; color: #333333; line-height: 1.6; }
    .content h2 { color: #2c3e50; margin-top: 0; }
    .btn { display: inline-block; background: #27ae60; color: #ffffff; text-decoration: none; padding: 12px 25px; border-radius: 4px; font-weight: bold; margin-top: 20px; }
    .btn:hover { background: #219150; }
    .footer { background: #f9f9f9; padding: 15px; text-align: center; font-size: 12px; color: #999999; border-top: 1px solid #eeeeee; }
    .info-table { width: 100%; border-collapse: collapse; margin: 15px 0; }
    .info-table th { text-align: left; color: #777; font-weight: normal; border-bottom: 1px solid #eee; padding: 8px 0; }
    .info-table td { text-align: right; font-weight: bold; border-bottom: 1px solid #eee; padding: 8px 0; }
    .highlight { color: #27ae60; font-weight: bold; }
</style>
<div class="container">
    <div class="header"><h1>PASSWORD RESET</h1></div>
    <div class="content">
        <p>Use the OTP code below to verify your identity.</p>
        <div style="background:#f4f4f4; padding:15px; text-align:center; font-size:24px; font-weight:bold; margin:20px 0;">
            ##OTPCode##
        </div>
    </div>
    <div class="footer">Secure Notification System</div>
</div>', NULL, 1, N'SystemSeed', CAST(N'2025-12-30T12:05:44.347' AS DateTime))
INSERT [dbo].[EmailTemplate] ([Id], [TemplateKey], [Name], [Description], [ToEmail], [CcEmail], [BccEmail], [FromEmail], [FromName], [ReplyEmail], [Subject], [BodyContent], [BodyFile], [IsActive], [LastUpdatedBy], [LastUpdatedDate]) VALUES (4, N'OTPEmail', N'OTPEmail', N'System Generated Template', N'##ToEmail##', NULL, NULL, N'support@mdua.com', N'MDUA Support', NULL, N'Your Verification Code: ##OTPCode##', N'
<style>
    body { font-family: ''Helvetica Neue'', Helvetica, Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }
    .container { max-width: 600px; margin: 20px auto; background: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
    .header { background: #1a1a1a; padding: 20px; text-align: center; }
    .header h1 { color: #ffffff; margin: 0; font-size: 24px; letter-spacing: 1px; }
    .content { padding: 30px; color: #333333; line-height: 1.6; }
    .content h2 { color: #2c3e50; margin-top: 0; }
    .btn { display: inline-block; background: #27ae60; color: #ffffff; text-decoration: none; padding: 12px 25px; border-radius: 4px; font-weight: bold; margin-top: 20px; }
    .btn:hover { background: #219150; }
    .footer { background: #f9f9f9; padding: 15px; text-align: center; font-size: 12px; color: #999999; border-top: 1px solid #eeeeee; }
    .info-table { width: 100%; border-collapse: collapse; margin: 15px 0; }
    .info-table th { text-align: left; color: #777; font-weight: normal; border-bottom: 1px solid #eee; padding: 8px 0; }
    .info-table td { text-align: right; font-weight: bold; border-bottom: 1px solid #eee; padding: 8px 0; }
    .highlight { color: #27ae60; font-weight: bold; }
</style>
<div class="container">
    <div class="header"><h1>VERIFY EMAIL</h1></div>
    <div class="content">
        <p>Your verification code is:</p>
        <div style="text-align:center; font-size: 32px; font-weight: bold; margin: 30px 0;">##OTPCode##</div>
    </div>
    <div class="footer">&copy; 2025 MDUA Shop.</div>
</div>', NULL, 1, N'SystemSeed', CAST(N'2025-12-30T12:05:44.350' AS DateTime))
INSERT [dbo].[EmailTemplate] ([Id], [TemplateKey], [Name], [Description], [ToEmail], [CcEmail], [BccEmail], [FromEmail], [FromName], [ReplyEmail], [Subject], [BodyContent], [BodyFile], [IsActive], [LastUpdatedBy], [LastUpdatedDate]) VALUES (5, N'InvoiceEmail', N'InvoiceEmail', N'System Generated Template', N'##ToEmail##', NULL, NULL, N'support@mdua.com', N'MDUA Support', NULL, N'Invoice ##InvoiceNo##', N'
<style>
    body { font-family: ''Helvetica Neue'', Helvetica, Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }
    .container { max-width: 600px; margin: 20px auto; background: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
    .header { background: #1a1a1a; padding: 20px; text-align: center; }
    .header h1 { color: #ffffff; margin: 0; font-size: 24px; letter-spacing: 1px; }
    .content { padding: 30px; color: #333333; line-height: 1.6; }
    .content h2 { color: #2c3e50; margin-top: 0; }
    .btn { display: inline-block; background: #27ae60; color: #ffffff; text-decoration: none; padding: 12px 25px; border-radius: 4px; font-weight: bold; margin-top: 20px; }
    .btn:hover { background: #219150; }
    .footer { background: #f9f9f9; padding: 15px; text-align: center; font-size: 12px; color: #999999; border-top: 1px solid #eeeeee; }
    .info-table { width: 100%; border-collapse: collapse; margin: 15px 0; }
    .info-table th { text-align: left; color: #777; font-weight: normal; border-bottom: 1px solid #eee; padding: 8px 0; }
    .info-table td { text-align: right; font-weight: bold; border-bottom: 1px solid #eee; padding: 8px 0; }
    .highlight { color: #27ae60; font-weight: bold; }
</style>
<div class="container">
    <div class="header"><h1>INVOICE ##InvoiceNo##</h1></div>
    <div class="content">
        <h2>Dear ##UserName##,</h2>
        <table class="info-table">
            <tr><th>Invoice No</th><td>##InvoiceNo##</td></tr>
            <tr><th>Date</th><td>##InvoiceDate##</td></tr>
            <tr><th>Amount Due</th><td style="color:#e74c3c;">##AmountDue## Tk</td></tr>
        </table>
        <center><a href="##DownloadLink##" class="btn" style="background:#3498db;">Download PDF</a></center>
    </div>
    <div class="footer">Thank you for your business.</div>
</div>', NULL, 1, N'SystemSeed', CAST(N'2025-12-30T12:05:44.350' AS DateTime))
INSERT [dbo].[EmailTemplate] ([Id], [TemplateKey], [Name], [Description], [ToEmail], [CcEmail], [BccEmail], [FromEmail], [FromName], [ReplyEmail], [Subject], [BodyContent], [BodyFile], [IsActive], [LastUpdatedBy], [LastUpdatedDate]) VALUES (6, N'OrderStatusTemp', N'OrderStatusTemp', N'System Generated Template', N'##ToEmail##', NULL, NULL, N'support@mdua.com', N'MDUA Support', NULL, N'Order Update: ##NewStatus##', N'
<style>
    body { font-family: ''Helvetica Neue'', Helvetica, Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }
    .container { max-width: 600px; margin: 20px auto; background: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
    .header { background: #1a1a1a; padding: 20px; text-align: center; }
    .header h1 { color: #ffffff; margin: 0; font-size: 24px; letter-spacing: 1px; }
    .content { padding: 30px; color: #333333; line-height: 1.6; }
    .content h2 { color: #2c3e50; margin-top: 0; }
    .btn { display: inline-block; background: #27ae60; color: #ffffff; text-decoration: none; padding: 12px 25px; border-radius: 4px; font-weight: bold; margin-top: 20px; }
    .btn:hover { background: #219150; }
    .footer { background: #f9f9f9; padding: 15px; text-align: center; font-size: 12px; color: #999999; border-top: 1px solid #eeeeee; }
    .info-table { width: 100%; border-collapse: collapse; margin: 15px 0; }
    .info-table th { text-align: left; color: #777; font-weight: normal; border-bottom: 1px solid #eee; padding: 8px 0; }
    .info-table td { text-align: right; font-weight: bold; border-bottom: 1px solid #eee; padding: 8px 0; }
    .highlight { color: #27ae60; font-weight: bold; }
</style>
<div class="container">
    <div class="header"><h1>UPDATE: ##NewStatus##</h1></div>
    <div class="content">
        <h2>Hi ##UserName##,</h2>
        <p>The status of your order <b>##OrderId##</b> has changed.</p>
        <center><a href="##TrackLink##" class="btn">Track Package</a></center>
    </div>
    <div class="footer">&copy; 2025 MDUA Logistics.</div>
</div>', NULL, 1, N'SystemSeed', CAST(N'2025-12-30T12:05:44.350' AS DateTime))
INSERT [dbo].[EmailTemplate] ([Id], [TemplateKey], [Name], [Description], [ToEmail], [CcEmail], [BccEmail], [FromEmail], [FromName], [ReplyEmail], [Subject], [BodyContent], [BodyFile], [IsActive], [LastUpdatedBy], [LastUpdatedDate]) VALUES (7, N'ContactUs', N'ContactUs', N'System Generated Template', N'##ToEmail##', NULL, NULL, N'support@mdua.com', N'MDUA Support', NULL, N'Inquiry from ##UserName##', N'
<style>
    body { font-family: ''Helvetica Neue'', Helvetica, Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }
    .container { max-width: 600px; margin: 20px auto; background: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
    .header { background: #1a1a1a; padding: 20px; text-align: center; }
    .header h1 { color: #ffffff; margin: 0; font-size: 24px; letter-spacing: 1px; }
    .content { padding: 30px; color: #333333; line-height: 1.6; }
    .content h2 { color: #2c3e50; margin-top: 0; }
    .btn { display: inline-block; background: #27ae60; color: #ffffff; text-decoration: none; padding: 12px 25px; border-radius: 4px; font-weight: bold; margin-top: 20px; }
    .btn:hover { background: #219150; }
    .footer { background: #f9f9f9; padding: 15px; text-align: center; font-size: 12px; color: #999999; border-top: 1px solid #eeeeee; }
    .info-table { width: 100%; border-collapse: collapse; margin: 15px 0; }
    .info-table th { text-align: left; color: #777; font-weight: normal; border-bottom: 1px solid #eee; padding: 8px 0; }
    .info-table td { text-align: right; font-weight: bold; border-bottom: 1px solid #eee; padding: 8px 0; }
    .highlight { color: #27ae60; font-weight: bold; }
</style>
<div class="container">
    <div class="header"><h1>NOTIFICATION</h1></div>
    <div class="content">
        <h2>Hello ##UserName##,</h2>
        <div style="background:#fff; border-left: 4px solid #333; padding: 10px 15px; margin: 20px 0;">
            ##MessageContent##
        </div>
    </div>
    <div class="footer">&copy; 2025 MDUA Shop.</div>
</div>', NULL, 1, N'SystemSeed', CAST(N'2025-12-30T12:05:44.350' AS DateTime))
INSERT [dbo].[EmailTemplate] ([Id], [TemplateKey], [Name], [Description], [ToEmail], [CcEmail], [BccEmail], [FromEmail], [FromName], [ReplyEmail], [Subject], [BodyContent], [BodyFile], [IsActive], [LastUpdatedBy], [LastUpdatedDate]) VALUES (9, N'TicketNotificationmail', N'TicketNotificationmail', N'System Generated Template', N'##ToEmail##', NULL, NULL, N'support@mdua.com', N'MDUA Support', NULL, N'Ticket ##TicketId## Update', N'
<style>
    body { font-family: ''Helvetica Neue'', Helvetica, Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }
    .container { max-width: 600px; margin: 20px auto; background: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
    .header { background: #1a1a1a; padding: 20px; text-align: center; }
    .header h1 { color: #ffffff; margin: 0; font-size: 24px; letter-spacing: 1px; }
    .content { padding: 30px; color: #333333; line-height: 1.6; }
    .content h2 { color: #2c3e50; margin-top: 0; }
    .btn { display: inline-block; background: #27ae60; color: #ffffff; text-decoration: none; padding: 12px 25px; border-radius: 4px; font-weight: bold; margin-top: 20px; }
    .btn:hover { background: #219150; }
    .footer { background: #f9f9f9; padding: 15px; text-align: center; font-size: 12px; color: #999999; border-top: 1px solid #eeeeee; }
    .info-table { width: 100%; border-collapse: collapse; margin: 15px 0; }
    .info-table th { text-align: left; color: #777; font-weight: normal; border-bottom: 1px solid #eee; padding: 8px 0; }
    .info-table td { text-align: right; font-weight: bold; border-bottom: 1px solid #eee; padding: 8px 0; }
    .highlight { color: #27ae60; font-weight: bold; }
</style>
<div class="container">
    <div class="header"><h1>NOTIFICATION</h1></div>
    <div class="content">
        <h2>Hello ##UserName##,</h2>
        <div style="background:#fff; border-left: 4px solid #333; padding: 10px 15px; margin: 20px 0;">
            ##MessageContent##
        </div>
    </div>
    <div class="footer">&copy; 2025 MDUA Shop.</div>
</div>', NULL, 1, N'SystemSeed', CAST(N'2025-12-30T12:05:44.350' AS DateTime))
INSERT [dbo].[EmailTemplate] ([Id], [TemplateKey], [Name], [Description], [ToEmail], [CcEmail], [BccEmail], [FromEmail], [FromName], [ReplyEmail], [Subject], [BodyContent], [BodyFile], [IsActive], [LastUpdatedBy], [LastUpdatedDate]) VALUES (10, N'mailCreateWorkOrder', N'mailCreateWorkOrder', N'System Generated Template', N'##ToEmail##', NULL, NULL, N'support@mdua.com', N'MDUA Support', NULL, N'Work Order Created', N'
<style>
    body { font-family: ''Helvetica Neue'', Helvetica, Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }
    .container { max-width: 600px; margin: 20px auto; background: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
    .header { background: #1a1a1a; padding: 20px; text-align: center; }
    .header h1 { color: #ffffff; margin: 0; font-size: 24px; letter-spacing: 1px; }
    .content { padding: 30px; color: #333333; line-height: 1.6; }
    .content h2 { color: #2c3e50; margin-top: 0; }
    .btn { display: inline-block; background: #27ae60; color: #ffffff; text-decoration: none; padding: 12px 25px; border-radius: 4px; font-weight: bold; margin-top: 20px; }
    .btn:hover { background: #219150; }
    .footer { background: #f9f9f9; padding: 15px; text-align: center; font-size: 12px; color: #999999; border-top: 1px solid #eeeeee; }
    .info-table { width: 100%; border-collapse: collapse; margin: 15px 0; }
    .info-table th { text-align: left; color: #777; font-weight: normal; border-bottom: 1px solid #eee; padding: 8px 0; }
    .info-table td { text-align: right; font-weight: bold; border-bottom: 1px solid #eee; padding: 8px 0; }
    .highlight { color: #27ae60; font-weight: bold; }
</style>
<div class="container">
    <div class="header"><h1>NOTIFICATION</h1></div>
    <div class="content">
        <h2>Hello ##UserName##,</h2>
        <div style="background:#fff; border-left: 4px solid #333; padding: 10px 15px; margin: 20px 0;">
            ##MessageContent##
        </div>
    </div>
    <div class="footer">&copy; 2025 MDUA Shop.</div>
</div>', NULL, 1, N'SystemSeed', CAST(N'2025-12-30T12:05:44.350' AS DateTime))
INSERT [dbo].[EmailTemplate] ([Id], [TemplateKey], [Name], [Description], [ToEmail], [CcEmail], [BccEmail], [FromEmail], [FromName], [ReplyEmail], [Subject], [BodyContent], [BodyFile], [IsActive], [LastUpdatedBy], [LastUpdatedDate]) VALUES (11, N'SendTaxInvoice', N'SendTaxInvoice', N'System Generated Template', N'##ToEmail##', NULL, NULL, N'support@mdua.com', N'MDUA Support', NULL, N'Your Tax Invoice', N'
<style>
    body { font-family: ''Helvetica Neue'', Helvetica, Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }
    .container { max-width: 600px; margin: 20px auto; background: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
    .header { background: #1a1a1a; padding: 20px; text-align: center; }
    .header h1 { color: #ffffff; margin: 0; font-size: 24px; letter-spacing: 1px; }
    .content { padding: 30px; color: #333333; line-height: 1.6; }
    .content h2 { color: #2c3e50; margin-top: 0; }
    .btn { display: inline-block; background: #27ae60; color: #ffffff; text-decoration: none; padding: 12px 25px; border-radius: 4px; font-weight: bold; margin-top: 20px; }
    .btn:hover { background: #219150; }
    .footer { background: #f9f9f9; padding: 15px; text-align: center; font-size: 12px; color: #999999; border-top: 1px solid #eeeeee; }
    .info-table { width: 100%; border-collapse: collapse; margin: 15px 0; }
    .info-table th { text-align: left; color: #777; font-weight: normal; border-bottom: 1px solid #eee; padding: 8px 0; }
    .info-table td { text-align: right; font-weight: bold; border-bottom: 1px solid #eee; padding: 8px 0; }
    .highlight { color: #27ae60; font-weight: bold; }
</style>
<div class="container">
    <div class="header"><h1>NOTIFICATION</h1></div>
    <div class="content">
        <h2>Hello ##UserName##,</h2>
        <div style="background:#fff; border-left: 4px solid #333; padding: 10px 15px; margin: 20px 0;">
            ##MessageContent##
        </div>
    </div>
    <div class="footer">&copy; 2025 MDUA Shop.</div>
</div>', NULL, 1, N'SystemSeed', CAST(N'2025-12-30T12:05:44.350' AS DateTime))
INSERT [dbo].[EmailTemplate] ([Id], [TemplateKey], [Name], [Description], [ToEmail], [CcEmail], [BccEmail], [FromEmail], [FromName], [ReplyEmail], [Subject], [BodyContent], [BodyFile], [IsActive], [LastUpdatedBy], [LastUpdatedDate]) VALUES (12, N'BookingEmail', N'BookingEmail', N'System Generated Template', N'##ToEmail##', NULL, NULL, N'support@mdua.com', N'MDUA Support', NULL, N'Booking Confirmation', N'
<style>

    body { font-family: ''Helvetica Neue'', Helvetica, Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }

    .container { max-width: 600px; margin: 20px auto; background: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }

    .header { background: #1a1a1a; padding: 20px; text-align: center; }

    .header h1 { color: #ffffff; margin: 0; font-size: 24px; letter-spacing: 1px; }

    .content { padding: 30px; color: #333333; line-height: 1.6; }

    .content h2 { color: #2c3e50; margin-top: 0; }

    .btn { display: inline-block; background: #27ae60; color: #ffffff; text-decoration: none; padding: 12px 25px; border-radius: 4px; font-weight: bold; margin-top: 20px; }

    .btn:hover { background: #219150; }

    .footer { background: #f9f9f9; padding: 15px; text-align: center; font-size: 12px; color: #999999; border-top: 1px solid #eeeeee; }

    .info-table { width: 100%; border-collapse: collapse; margin: 15px 0; }

    .info-table th { text-align: left; color: #777; font-weight: normal; border-bottom: 1px solid #eee; padding: 8px 0; }

    .info-table td { text-align: right; font-weight: bold; border-bottom: 1px solid #eee; padding: 8px 0; }

    .highlight { color: #27ae60; font-weight: bold; }
</style>
<div class="container">
<div class="header"><h1>NOTIFICATION</h1></div>
<div class="content">
<h2>Hello ##UserName##,</h2>
<div style="background:#fff; border-left: 4px solid #333; padding: 10px 15px; margin: 20px 0;">

            ##MessageContent##
</div>
</div>
<div class="footer">&copy; 2025 MDUA Shop.</div>
</div>', NULL, 1, N'SystemSeed', CAST(N'2025-12-30T06:54:03.297' AS DateTime))
SET IDENTITY_INSERT [dbo].[EmailTemplate] OFF
GO
