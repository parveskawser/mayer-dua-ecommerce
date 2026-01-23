USE [amaderprangondb]
GO

CREATE OR ALTER PROCEDURE [dbo].[ReceiveBulkOrderItems]
    @JsonItems NVARCHAR(MAX),
    @InvoiceNo NVARCHAR(100),
    @Remarks NVARCHAR(500),
    @TotalPaidAmount DECIMAL(18,2),
    @PaymentMethodId INT = NULL,
    @VendorId INT,
    @CreatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Parse JSON into Table Variable (Safe & Clean)
        DECLARE @ItemsToReceive TABLE (
            PoRequestId INT,
            ProductVariantId INT,
            Quantity INT,
            Price DECIMAL(18,2)
        );

        INSERT INTO @ItemsToReceive (PoRequestId, ProductVariantId, Quantity, Price)
        SELECT PoRequestId, ProductVariantId, Quantity, Price
        FROM OPENJSON(@JsonItems)
        WITH (
            PoRequestId INT '$.PoRequestId',
            ProductVariantId INT '$.ProductVariantId',
            Quantity INT '$.Quantity',
            Price DECIMAL(18,2) '$.Price'
        );

        -- 2. Loop Execution
        DECLARE @Cur_PoReqId INT, @Cur_VarId INT, @Cur_Qty INT, @Cur_Price DECIMAL(18,2);
        DECLARE @NewPoReceivedId INT;

        DECLARE ItemCursor CURSOR FOR 
        SELECT PoRequestId, ProductVariantId, Quantity, Price FROM @ItemsToReceive;

        OPEN ItemCursor;
        FETCH NEXT FROM ItemCursor INTO @Cur_PoReqId, @Cur_VarId, @Cur_Qty, @Cur_Price;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- 🛡️ GUARDRAIL: Prevent Double-Receive
            -- If this specific PO Request was already processed in a previous transaction, STOP.
            IF EXISTS (SELECT 1 FROM dbo.PoReceived WHERE PoRequestedId = @Cur_PoReqId)
            BEGIN
                -- Using THROW causes the transaction to Rollback immediately in the CATCH block
                -- Error 50001 is a custom error number (must be > 50000)
                THROW 50001, 'Error: One or more items in this list have already been received. Refresh the page.', 1;
            END

            -- A. Insert Receipt
            INSERT INTO [dbo].[PoReceived]
            (PoRequestedId, ReceivedQuantity, BuyingPrice, ReceivedDate, CreatedBy, CreatedAt, Remarks, InvoiceNo, TotalPaid, PaymentStatus, VendorId)
            VALUES
            (@Cur_PoReqId, @Cur_Qty, @Cur_Price, GETUTCDATE(), @CreatedBy, GETUTCDATE(), @Remarks, @InvoiceNo, 0, 'Unpaid', @VendorId);

            SET @NewPoReceivedId = SCOPE_IDENTITY();

            -- B. Update Physical Stock
            UPDATE [dbo].[VariantPriceStock] SET StockQty = StockQty + @Cur_Qty WHERE Id = @Cur_VarId;

            -- C. Log Inventory Transaction
            INSERT INTO [dbo].[InventoryTransaction]
            (PoReceivedId, ProductVariantId, InOut, Date, Price, Quantity, CreatedBy, CreatedAt, Remarks)
            VALUES
            (@NewPoReceivedId, @Cur_VarId, 'IN', GETUTCDATE(), @Cur_Price, @Cur_Qty, @CreatedBy, GETUTCDATE(), @Remarks);

            -- D. Update Request Status
            UPDATE [dbo].[PoRequested] SET Status = 'Received', UpdatedAt = GETUTCDATE() WHERE Id = @Cur_PoReqId;

            FETCH NEXT FROM ItemCursor INTO @Cur_PoReqId, @Cur_VarId, @Cur_Qty, @Cur_Price;
        END

        CLOSE ItemCursor;
        DEALLOCATE ItemCursor;

        -- 3. Invoice-Level Payment Logic
        -- We do not split payment across rows; we link it via InvoiceNo/VendorId
        IF @TotalPaidAmount > 0 AND @PaymentMethodId IS NOT NULL
        BEGIN
            INSERT INTO [dbo].[VendorPayment]
            (
                VendorId, PaymentMethodId, PaymentType, Amount, PaymentDate, 
                Status, CreatedBy, CreatedAt, ReferenceNumber, Notes, PoReceivedId
            )
            VALUES
            (
                @VendorId, @PaymentMethodId, 'Purchase', @TotalPaidAmount, GETUTCDATE(), 
                'Completed', @CreatedBy, GETUTCDATE(), @InvoiceNo, 'Bulk Order Payment', NULL
            );
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        
        -- Re-throw the error (e.g. "Item already received") so the C# / User knows why it failed
        THROW;
    END CATCH
END
GO

GO

-- This drops the Unique Index that is blocking your Bulk Order
DROP INDEX [UX_PoReceived_Vendor_Invoice] ON [dbo].[PoReceived];
GO