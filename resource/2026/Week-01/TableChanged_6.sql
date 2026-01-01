USE [AA4]

GO
 
-- =============================================

-- STEP 1: FIX TABLE SCHEMA (Add Missing Columns)

-- =============================================
 
-- 1.1 Add Auditing Columns if missing

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'CreatedAt' AND Object_ID = Object_ID(N'dbo.VendorPayment'))

BEGIN

    ALTER TABLE [dbo].[VendorPayment] ADD [CreatedAt] DATETIME NULL DEFAULT GETDATE();

END

GO
 
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'UpdatedBy' AND Object_ID = Object_ID(N'dbo.VendorPayment'))

BEGIN

    ALTER TABLE [dbo].[VendorPayment] ADD [UpdatedBy] NVARCHAR(100) NULL;

END

GO
 
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'UpdatedAt' AND Object_ID = Object_ID(N'dbo.VendorPayment'))

BEGIN

    ALTER TABLE [dbo].[VendorPayment] ADD [UpdatedAt] DATETIME NULL;

END

GO
 
-- 1.2 Add Relationship Columns (The "Edge Case" fix from previous step)

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'PoReceivedId' AND Object_ID = Object_ID(N'dbo.VendorPayment'))

BEGIN

    ALTER TABLE [dbo].[VendorPayment] ADD [PoReceivedId] INT NULL;

    ALTER TABLE [dbo].[VendorPayment] WITH CHECK ADD CONSTRAINT [FK_VendorPayment_PoReceived] FOREIGN KEY([PoReceivedId]) REFERENCES [dbo].[PoReceived] ([Id]);

END

GO
 
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'PoRequestedId' AND Object_ID = Object_ID(N'dbo.VendorPayment'))

BEGIN

    ALTER TABLE [dbo].[VendorPayment] ADD [PoRequestedId] INT NULL;

    ALTER TABLE [dbo].[VendorPayment] WITH CHECK ADD CONSTRAINT [FK_VendorPayment_PoRequested] FOREIGN KEY([PoRequestedId]) REFERENCES [dbo].[PoRequested] ([Id]);

END

GO
 
-- =============================================

-- 2. UPDATE PO RECEIVED TABLE (Atomic Checks)

-- =============================================
 
-- 2.1 Add TotalPaymentDue (Computed)

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'TotalPaymentDue' AND Object_ID = Object_ID(N'dbo.PoReceived'))

BEGIN

    ALTER TABLE [dbo].[PoReceived] 

    ADD [TotalPaymentDue] AS (ISNULL([ReceivedQuantity],0) * ISNULL([BuyingPrice],0)) PERSISTED;

END

GO
 
-- 2.2 Add TotalPaid

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'TotalPaid' AND Object_ID = Object_ID(N'dbo.PoReceived'))

BEGIN

    ALTER TABLE [dbo].[PoReceived] ADD [TotalPaid] DECIMAL(18, 2) NOT NULL DEFAULT 0;

END

GO
 
-- 2.3 Add PaymentStatus (The Fix: Separate check ensures this creates even if TotalPaid exists)

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'PaymentStatus' AND Object_ID = Object_ID(N'dbo.PoReceived'))

BEGIN

    ALTER TABLE [dbo].[PoReceived] ADD [PaymentStatus] NVARCHAR(20) NOT NULL DEFAULT 'Unpaid';

END

GO
 
-- 2.4 Add Constraint (Check if constraint exists, not just the column)

IF NOT EXISTS(SELECT 1 FROM sys.check_constraints WHERE name = 'CHK_PoReceived_PaymentStatus')

BEGIN

    ALTER TABLE [dbo].[PoReceived] ADD CONSTRAINT [CHK_PoReceived_PaymentStatus] 

    CHECK ([PaymentStatus] IN ('Unpaid', 'Partial', 'Paid', 'Overpaid'));

END

GO
 
-- =============================================

-- 3. PERFORMANCE INDEXES

-- =============================================

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_VendorPayment_PoReceived')

    CREATE NONCLUSTERED INDEX [IX_VendorPayment_PoReceived] ON [dbo].[VendorPayment] ([PoReceivedId]) INCLUDE ([Amount], [Status]);

GO
 
-- =============================================

-- 4. TRIGGER

-- =============================================

CREATE OR ALTER TRIGGER [dbo].[TR_VendorPayment_Sync_PoReceived]

ON [dbo].[VendorPayment]

AFTER INSERT, UPDATE, DELETE

AS

BEGIN

    SET NOCOUNT ON;
 
    -- 1. Identify which PoReceived records were affected

    DECLARE @AffectedIds TABLE (Id INT);
 
    INSERT INTO @AffectedIds

    SELECT DISTINCT PoReceivedId FROM Inserted WHERE PoReceivedId IS NOT NULL

    UNION

    SELECT DISTINCT PoReceivedId FROM Deleted WHERE PoReceivedId IS NOT NULL;
 
    -- 2. Recalculate TotalPaid for those records

    UPDATE pr

    SET 

        pr.TotalPaid = ISNULL(p.SumAmount, 0),

        pr.PaymentStatus = CASE 

            WHEN ISNULL(p.SumAmount, 0) >= pr.TotalPaymentDue AND pr.TotalPaymentDue > 0 THEN 'Paid'

            WHEN ISNULL(p.SumAmount, 0) > pr.TotalPaymentDue THEN 'Overpaid'

            WHEN ISNULL(p.SumAmount, 0) > 0 THEN 'Partial'

            ELSE 'Unpaid'

        END,

        pr.UpdatedAt = GETUTCDATE()

    FROM [dbo].[PoReceived] pr

    LEFT JOIN (

        SELECT PoReceivedId, SUM(Amount) as SumAmount

        FROM [dbo].[VendorPayment]

        WHERE Status = 'Completed' -- Only count completed payments

        GROUP BY PoReceivedId

    ) p ON pr.Id = p.PoReceivedId

    WHERE pr.Id IN (SELECT Id FROM @AffectedIds);

END;

GO
 
-- =============================================

-- 5. STORED PROCEDURE

-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[InsertVendorPayment]

    @VendorId INT,

    @PoReceivedId INT = NULL,   

    @PoRequestedId INT = NULL,  

    @PaymentMethodId INT,

    @Amount DECIMAL(18,2),

    @ReferenceNumber NVARCHAR(100) = NULL,

    @Notes NVARCHAR(500) = NULL,

    @CreatedBy NVARCHAR(100),

    @NewPaymentId INT OUTPUT

AS

BEGIN

    SET NOCOUNT ON;

    -- 1. Validation

    IF @Amount <= 0 

        THROW 50001, 'Payment amount must be greater than zero.', 1;
 
    IF @PoReceivedId IS NULL AND @PoRequestedId IS NULL

        THROW 50002, 'Payment must be linked to either a Received Order or a Purchase Request.', 1;
 
    BEGIN TRANSACTION;

    BEGIN TRY

        -- 2. Insert Payment

        INSERT INTO [dbo].[VendorPayment] (

            VendorId, 

            PaymentMethodId, 

            PoReceivedId,

            PoRequestedId,

            InventoryTransactionId, 

            ReferenceNumber,

            PaymentType,

            Amount,

            PaymentDate,

            Status,

            Notes,

            CreatedBy,

            CreatedAt

        )

        SELECT

            @VendorId,

            @PaymentMethodId,

            @PoReceivedId,

            @PoRequestedId,

            (SELECT TOP 1 Id FROM InventoryTransaction WHERE PoReceivedId = @PoReceivedId), 

            @ReferenceNumber,

            CASE WHEN @PoReceivedId IS NULL THEN 'Advance' ELSE 'Standard' END,

            @Amount,

            GETUTCDATE(),

            'Completed',

            @Notes,

            @CreatedBy,

            GETUTCDATE();

        SET @NewPaymentId = SCOPE_IDENTITY();

        COMMIT TRANSACTION;
 
    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        THROW;

    END CATCH

END;

GO
 