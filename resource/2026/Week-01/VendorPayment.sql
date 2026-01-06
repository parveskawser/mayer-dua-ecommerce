--2026-01-01 11:22AM--

ALTER TABLE [dbo].[VendorPayment]
ADD [PoReceivedId] [int] NULL;

ALTER TABLE [dbo].[VendorPayment]  
WITH CHECK ADD CONSTRAINT [FK_VendorPayment_PoReceived] 
FOREIGN KEY([PoReceivedId])
REFERENCES [dbo].[PoReceived] ([Id]);


ADD [TotalPaymentDue] AS ([ReceivedQuantity] * [BuyingPrice]) PERSISTED,
    [TotalPaid] [decimal](18, 2) DEFAULT 0 NOT NULL,
    [PaymentStatus] [nvarchar](20) DEFAULT 'Unpaid' NOT NULL;

ALTER TABLE [dbo].[PoReceived]  
ADD CONSTRAINT [CHK_PoReceived_PaymentStatus] 
CHECK ([PaymentStatus] IN ('Unpaid', 'Partial', 'Paid', 'Overpaid'));

go
CREATE NONCLUSTERED INDEX [IX_VendorPayment_PoReceived] 
ON [dbo].[VendorPayment] ([PoReceivedId]) 
INCLUDE ([Amount], [Status], [PaymentDate]);

CREATE NONCLUSTERED INDEX [IX_PoReceived_PaymentStatus] 
ON [dbo].[PoReceived] ([PaymentStatus]) 
INCLUDE ([TotalPaid], [TotalPaymentDue]);


GO
CREATE OR ALTER PROCEDURE [dbo].[InsertVendorPayment]
    @VendorId INT,
    @PoReceivedId INT,
    @PaymentMethodId INT,
    @Amount DECIMAL(18,2),
    @ReferenceNumber NVARCHAR(100) = NULL,
    @Notes NVARCHAR(500) = NULL,
    @CreatedBy NVARCHAR(100),
    @NewPaymentId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @InventoryTransactionId INT;
    DECLARE @TotalDue DECIMAL(18,2);
    DECLARE @CurrentPaid DECIMAL(18,2);
    DECLARE @NewTotal DECIMAL(18,2);
    DECLARE @NewStatus NVARCHAR(20);
    
    BEGIN TRANSACTION;
    BEGIN TRY
        
        -- Get the linked InventoryTransaction and payment info
        SELECT 
            @InventoryTransactionId = it.Id,
            @TotalDue = pr.ReceivedQuantity * pr.BuyingPrice,
            @CurrentPaid = ISNULL(pr.TotalPaid, 0)
        FROM PoReceived pr
        LEFT JOIN InventoryTransaction it ON it.PoReceivedId = pr.Id
        WHERE pr.Id = @PoReceivedId;
        
        IF @InventoryTransactionId IS NULL
            THROW 50001, 'No inventory transaction found for this receipt', 1;
        
        -- Calculate new payment status
        SET @NewTotal = @CurrentPaid + @Amount;
        
        IF @NewTotal >= @TotalDue
            SET @NewStatus = 'Paid';
        ELSE IF @NewTotal > 0
            SET @NewStatus = 'Partial';
        ELSE
            SET @NewStatus = 'Unpaid';
        
        -- Insert payment record
        INSERT INTO VendorPayment (
            VendorId, 
            PaymentMethodId, 
            PoReceivedId,
            InventoryTransactionId,
            ReferenceNumber,
            PaymentType,
            Amount,
            PaymentDate,
            Status,
            Notes,
            CreatedBy,
            CreatedAt,
            UpdatedAt
        )
        VALUES (
            @VendorId,
            @PaymentMethodId,
            @PoReceivedId,
            @InventoryTransactionId,
            @ReferenceNumber,
            'Purchase',
            @Amount,
            GETUTCDATE(),
            'Completed',
            @Notes,
            @CreatedBy,
            GETUTCDATE(),
            GETUTCDATE()
        );
        
        SET @NewPaymentId = SCOPE_IDENTITY();
        
        -- Update PoReceived payment tracking
        UPDATE PoReceived
        SET 
            TotalPaid = @NewTotal,
            PaymentStatus = @NewStatus,
            UpdatedAt = GETUTCDATE(),
            UpdatedBy = @CreatedBy
        WHERE Id = @PoReceivedId;
        
        COMMIT TRANSACTION;
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO
