-- 1. Create Master Carrier Table

CREATE TABLE [dbo].[Carrier](

    [Id] [int] IDENTITY(1,1) NOT NULL,

    [CarrierName] [nvarchar](100) NOT NULL,

    [ApiEndpoint] [nvarchar](500) NULL,

    [RequiresApi] [bit] NOT NULL DEFAULT 0,

    [IsActive] [bit] NOT NULL DEFAULT 1,

    CONSTRAINT [PK_Carrier] PRIMARY KEY CLUSTERED ([Id] ASC)

);
 
-- 2. Seed Data

INSERT INTO [dbo].[Carrier] ([CarrierName], [RequiresApi], [ApiEndpoint])

VALUES 

    ('RedX', 1, 'https://api.redx.com.bd/v1/parcel'),

    ('Pathao', 1, 'https://api.pathao.com/v1/parcel'),

    ('Steadfast', 1, 'https://api.steadfast.com.bd/v1/parcel'),

    ('Own Delivery', 0, NULL);
 
-- 3. Create Company-Specific Settings

CREATE TABLE [dbo].[CompanyCarrier](

    [Id] [int] IDENTITY(1,1) NOT NULL,

    [CompanyId] [int] NOT NULL,

    [CarrierId] [int] NOT NULL,

    [ApiKey] [nvarchar](500) NULL,

    [ApiSecret] [nvarchar](500) NULL,

    [IsActive] [bit] NOT NULL DEFAULT 1,

    CONSTRAINT [PK_CompanyCarrier] PRIMARY KEY CLUSTERED ([Id] ASC),

    CONSTRAINT [FK_CompanyCarrier_Carrier] FOREIGN KEY([CarrierId]) REFERENCES [dbo].[Carrier] ([Id]),

    -- Add constraint for CompanyId if table exists:

    
   CONSTRAINT [FK_CompanyCarrier_Company] FOREIGN KEY([CompanyId]) REFERENCES [dbo].[Company] ([Id])

);
 
-- 4. Update Delivery Table

ALTER TABLE [dbo].[Delivery] ADD



    [CarrierCharge] DECIMAL(18, 2) NULL,

    [PackageWeightGrams] INT NULL,

    [CarrierResponse] NVARCHAR(MAX) NULL,

    [ConsignmentId] NVARCHAR(100) NULL,

    [CarrierId] INT NULL;
 
ALTER TABLE [dbo].[Delivery] 

WITH CHECK ADD CONSTRAINT [FK_Delivery_Carrier] 

FOREIGN KEY([CarrierId]) REFERENCES [dbo].[Carrier] ([Id]);

GO
 


ALTER TABLE [dbo].[Delivery] DROP COLUMN [CarrierName];

GO


IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Delivery_Carrier]') AND parent_object_id = OBJECT_ID(N'[dbo].[Delivery]'))
BEGIN
    ALTER TABLE [dbo].[Delivery] DROP CONSTRAINT [FK_Delivery_Carrier];
END
GO


ALTER TABLE [dbo].[Delivery] 
WITH CHECK ADD CONSTRAINT [FK_Delivery_CompanyCarrier] 
FOREIGN KEY([CarrierId])
REFERENCES [dbo].[CompanyCarrier] ([Id]);
GO

-- 3. Check/Enable the constraint
ALTER TABLE [dbo].[Delivery] CHECK CONSTRAINT [FK_Delivery_CompanyCarrier];
GO