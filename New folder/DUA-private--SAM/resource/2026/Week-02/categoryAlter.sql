ALTER TABLE [dbo].[ProductCategory]
ADD [IsActive] BIT NOT NULL
CONSTRAINT DF_ProductCategory_IsActive DEFAULT (1);


GO

CREATE OR ALTER PROCEDURE [dbo].[InsertVendorPayment]
(
    @Id int OUTPUT,
    @VendorId int,
    @PaymentMethodId int,
    @InventoryTransactionId int = NULL,
    @ReferenceNumber nvarchar(100) = NULL,
    @PaymentType nvarchar(20), -- 'Purchase', 'Advance'
    @Amount decimal(18, 2),
    @PaymentDate datetime,
    @Status nvarchar(20),
    @Notes nvarchar(500) = NULL,
    @CreatedBy nvarchar(100),
    @CreatedAt datetime,
    @PoReceivedId int = NULL, -- If NULL, we trigger Auto-Distribution
    @PoRequestedId int = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    -- 0. Ensure Vendor is Linked
    DECLARE @CompanyId int = 1; 
    DECLARE @RealFKId int;
    SELECT @RealFKId = Id FROM [dbo].[CompanyVendor] WHERE VendorId = @VendorId AND CompanyId = @CompanyId;
    
    IF @RealFKId IS NULL
    BEGIN
        INSERT INTO [dbo].[CompanyVendor] (CompanyId, VendorId) VALUES (@CompanyId, @VendorId);
        SET @RealFKId = SCOPE_IDENTITY();
    END

    -- Variable to track money left to distribute
    DECLARE @RemainingAmount DECIMAL(18,2) = @Amount;

    -- ==========================================================================================
    -- CASE A: SPECIFIC BILL SELECTED (Manual Mode)
    -- ==========================================================================================
    IF @PoReceivedId IS NOT NULL
    BEGIN
        -- Reuse your existing logic for specific bills:
        DECLARE @BillBalance decimal(18,2) = 0;
        SELECT @BillBalance = (TotalPaymentDue - TotalPaid) FROM PoReceived WHERE Id = @PoReceivedId;

        DECLARE @PayToBill decimal(18,2) = 0;
        DECLARE @Excess decimal(18,2) = 0;

        IF @RemainingAmount <= @BillBalance
            SET @PayToBill = @RemainingAmount;
        ELSE
        BEGIN
            SET @PayToBill = @BillBalance;
            SET @Excess = @RemainingAmount - @BillBalance;
        END

        -- 1. Record Bill Payment
        IF @PayToBill > 0
        BEGIN
            INSERT INTO [dbo].[VendorPayment]
            (VendorId, PaymentMethodId, PaymentType, Amount, PaymentDate, Status, Notes, CreatedBy, CreatedAt, PoReceivedId, PoRequestedId)
            VALUES
            (@RealFKId, @PaymentMethodId, 'Purchase', @PayToBill, @PaymentDate, @Status, @Notes, @CreatedBy, @CreatedAt, @PoReceivedId, NULL);
            
            SET @Id = CAST(SCOPE_IDENTITY() AS INT);

            -- Update Bill
            UPDATE PoReceived 
            SET TotalPaid = TotalPaid + @PayToBill,
                PaymentStatus = CASE WHEN (TotalPaid + @PayToBill) >= TotalPaymentDue THEN 'Paid' ELSE 'Partial' END,
                UpdatedAt = GETUTCDATE()
            WHERE Id = @PoReceivedId;
        END

        -- 2. Record Excess as Advance
        IF @Excess > 0
        BEGIN
            INSERT INTO [dbo].[VendorPayment]
            (VendorId, PaymentMethodId, PaymentType, Amount, PaymentDate, Status, Notes, CreatedBy, CreatedAt, PoReceivedId, PoRequestedId)
            VALUES
            (@RealFKId, @PaymentMethodId, 'Advance', @Excess, @PaymentDate, @Status, ISNULL(@Notes, '') + ' (Overpayment)', @CreatedBy, @CreatedAt, NULL, NULL);
            
            IF @Id IS NULL SET @Id = CAST(SCOPE_IDENTITY() AS INT);
        END
    END

    -- ==========================================================================================
    -- CASE B: GENERAL PAYMENT (AUTO-DISTRIBUTE / BULK PAYMENT)
    -- ==========================================================================================
    ELSE
    BEGIN
        -- Logic: Loop through unpaid bills (Oldest First) and pay them off until money runs out.
        
        DECLARE @CurrBillId INT;
        DECLARE @CurrBillDue DECIMAL(18,2);

        -- Cursor to fetch unpaid bills for this vendor
        -- We use the new VendorId column on PoReceived for speed
        DECLARE bill_cursor CURSOR FOR
        SELECT Id, (TotalPaymentDue - TotalPaid)
        FROM PoReceived
        WHERE VendorId = @VendorId
          AND PaymentStatus NOT IN ('Paid', 'Overpaid')
        ORDER BY ReceivedDate ASC; -- First-In-First-Out (FIFO)

        OPEN bill_cursor;
        FETCH NEXT FROM bill_cursor INTO @CurrBillId, @CurrBillDue;

        WHILE @@FETCH_STATUS = 0 AND @RemainingAmount > 0
        BEGIN
            DECLARE @AmountToApply DECIMAL(18,2);

            -- Decide how much to pay on THIS bill
            IF @RemainingAmount >= @CurrBillDue
                SET @AmountToApply = @CurrBillDue; -- Fully pay this bill
            ELSE
                SET @AmountToApply = @RemainingAmount; -- Partially pay this bill

            -- 1. Insert Payment Record for this Bill
            INSERT INTO [dbo].[VendorPayment]
            (VendorId, PaymentMethodId, PaymentType, Amount, PaymentDate, Status, Notes, CreatedBy, CreatedAt, PoReceivedId, PoRequestedId)
            VALUES
            (@RealFKId, @PaymentMethodId, 'Purchase', @AmountToApply, @PaymentDate, @Status, 
             ISNULL(@Notes, '') + ' [Auto-Applied]', 
             @CreatedBy, @CreatedAt, @CurrBillId, NULL);

            SET @Id = CAST(SCOPE_IDENTITY() AS INT);

            -- 2. Update the Bill
            UPDATE PoReceived 
            SET TotalPaid = TotalPaid + @AmountToApply,
                PaymentStatus = CASE WHEN (TotalPaid + @AmountToApply) >= TotalPaymentDue THEN 'Paid' ELSE 'Partial' END,
                UpdatedAt = GETUTCDATE()
            WHERE Id = @CurrBillId;

            -- 3. Reduce Remaining Amount
            SET @RemainingAmount = @RemainingAmount - @AmountToApply;

            FETCH NEXT FROM bill_cursor INTO @CurrBillId, @CurrBillDue;
        END

        CLOSE bill_cursor;
        DEALLOCATE bill_cursor;

        -- ======================================================================================
        -- IF MONEY STILL LEFT: Store as Advance (Credit)
        -- ======================================================================================
        IF @RemainingAmount > 0
        BEGIN
            INSERT INTO [dbo].[VendorPayment]
            (VendorId, PaymentMethodId, PaymentType, Amount, PaymentDate, Status, Notes, CreatedBy, CreatedAt, PoReceivedId, PoRequestedId)
            VALUES
            (@RealFKId, @PaymentMethodId, 'Advance', @RemainingAmount, @PaymentDate, @Status, 
             ISNULL(@Notes, '') + ' (Credit Balance)', 
             @CreatedBy, @CreatedAt, NULL, NULL);

            IF @Id IS NULL SET @Id = CAST(SCOPE_IDENTITY() AS INT);
        END
    END

    RETURN @Id;
END
GO
 