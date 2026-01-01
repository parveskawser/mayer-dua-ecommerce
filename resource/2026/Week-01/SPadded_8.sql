 
-- =============================================

-- STEP 2: CREATE STORED PROCEDURES

-- =============================================
 
/****** Object:  StoredProcedure [dbo].UpdateVendorPayment ******/

CREATE OR ALTER PROCEDURE [dbo].[UpdateVendorPayment]

(

    @Id int,

    @VendorId int,

    @PaymentMethodId int,

    @InventoryTransactionId int = NULL,

    @ReferenceNumber nvarchar(100) = NULL,

    @PaymentType nvarchar(20),

    @Amount decimal(18, 2),

    @PaymentDate datetime,

    @Status nvarchar(20),

    @Notes nvarchar(500) = NULL,

    @CreatedBy nvarchar(100) = NULL, -- Kept for parameter compatibility, but typically not updated

    @CreatedAt datetime = NULL,      -- Kept for parameter compatibility

    @UpdatedBy nvarchar(100),

    @UpdatedAt datetime,

    @PoReceivedId int = NULL,

    @PoRequestedId int = NULL

)

AS

BEGIN

    SET NOCOUNT ON;
 
    UPDATE [dbo].[VendorPayment] 

    SET

        [VendorId] = @VendorId,

        [PaymentMethodId] = @PaymentMethodId,

        [InventoryTransactionId] = @InventoryTransactionId,

        [ReferenceNumber] = @ReferenceNumber,

        [PaymentType] = @PaymentType,

        [Amount] = @Amount,

        [PaymentDate] = @PaymentDate,

        [Status] = @Status,

        [Notes] = @Notes,

        -- Note: Usually we DO NOT update CreatedBy/CreatedAt, but I included them per your request

        [CreatedBy] = ISNULL(@CreatedBy, [CreatedBy]), 

        [CreatedAt] = ISNULL(@CreatedAt, [CreatedAt]),

        [UpdatedBy] = @UpdatedBy,

        [UpdatedAt] = GETUTCDATE(), -- Always use server time for updates

        [PoReceivedId] = @PoReceivedId,

        [PoRequestedId] = @PoRequestedId

    WHERE ( Id = @Id );
 
    RETURN @@ROWCOUNT;

END

GO
 
/****** Object:  StoredProcedure [dbo].DeleteVendorPayment ******/

CREATE OR ALTER PROCEDURE [dbo].[DeleteVendorPayment]

(

    @Id int

)

AS

BEGIN

    SET NOCOUNT ON;

    DELETE FROM [dbo].[VendorPayment] WHERE ( Id = @Id );

    RETURN @@ROWCOUNT;

END

GO
 
/****** Object:  StoredProcedure [dbo].GetAllVendorPayment ******/

CREATE OR ALTER PROCEDURE [dbo].[GetAllVendorPayment]

AS

BEGIN

    SET NOCOUNT ON;

    SELECT * FROM [dbo].[VendorPayment];

END

GO
 
/****** Object:  StoredProcedure [dbo].GetVendorPaymentById ******/

CREATE OR ALTER PROCEDURE [dbo].[GetVendorPaymentById]

(

    @Id int

)

AS

BEGIN

    SET NOCOUNT ON;

    SELECT * FROM [dbo].[VendorPayment] WHERE ( Id = @Id );

END

GO
 
/****** Object:  StoredProcedure [dbo].GetVendorPaymentByVendorId ******/

CREATE OR ALTER PROCEDURE [dbo].[GetVendorPaymentByVendorId]

(

    @VendorId int

)

AS

BEGIN

    SET NOCOUNT ON;

    SELECT * FROM [dbo].[VendorPayment] WHERE ( VendorId = @VendorId );

END

GO
 
/****** Object:  StoredProcedure [dbo].GetVendorPaymentByPoReceivedId ******/

CREATE OR ALTER PROCEDURE [dbo].[GetVendorPaymentByPoReceivedId]

(

    @PoReceivedId int

)

AS

BEGIN

    SET NOCOUNT ON;

    SELECT * FROM [dbo].[VendorPayment] WHERE ( PoReceivedId = @PoReceivedId );

END

GO
 
/****** Object:  StoredProcedure [dbo].GetVendorPaymentByPoRequestedId ******/

CREATE OR ALTER PROCEDURE [dbo].[GetVendorPaymentByPoRequestedId]

(

    @PoRequestedId int

)

AS

BEGIN

    SET NOCOUNT ON;

    SELECT * FROM [dbo].[VendorPayment] WHERE ( PoRequestedId = @PoRequestedId );

END

GO
 
/****** Object:  StoredProcedure [dbo].GetPagedVendorPayment ******/

CREATE OR ALTER PROCEDURE [dbo].[GetPagedVendorPayment]

(

    @TotalRows      int OUTPUT,

    @PageIndex      int,

    @RowPerPage     int,

    @WhereClause    nvarchar(4000) = '',

    @SortColumn     nvarchar(128) = '',

    @SortOrder      nvarchar(4) = 'ASC'

)

AS

BEGIN 

    SET NOCOUNT ON;
 
    SET @PageIndex = ISNULL(@PageIndex, 0); -- Fixed: Page Index usually starts at 0 or 1, -1 is risky

    If @PageIndex < 0 SET @PageIndex = 0;

    SET @RowPerPage = ISNULL(@RowPerPage, 10);

    SET @WhereClause = ISNULL(@WhereClause, '');

    SET @SortColumn = ISNULL(@SortColumn, '');

    SET @SortOrder = ISNULL(@SortOrder, '');

    SET @TotalRows = 0;
 
    DECLARE @SQL1 nvarchar(MAX);

    DECLARE @SQL2 nvarchar(MAX);
 
    IF (@WhereClause != '')

        SET @WhereClause = 'WHERE ' + @WhereClause;
 
    IF (@SortColumn != '')

    BEGIN

        SET @SortColumn = 'ORDER BY ' + QUOTENAME(@SortColumn);

        IF (@SortOrder != '') SET @SortColumn = @SortColumn + ' ' + @SortOrder;

    END

    ELSE

    BEGIN

        SET @SortColumn = 'ORDER BY [Id] DESC'; -- Default to newest first

    END
 
    -- 1. Get Total Count

    SET @SQL2 = 'SELECT @TotalRows = COUNT(*) FROM [dbo].[VendorPayment] ' + @WhereClause;

    EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output;
 
    -- 2. Get Paged Data

    SET @SQL1 = '

        SELECT * FROM (

            SELECT ROW_NUMBER() OVER (' + @SortColumn + ') AS Row, *

            FROM [dbo].[VendorPayment]

            ' + @WhereClause + '

        ) AS VendorPaymentEntries

        WHERE Row BETWEEN ' + CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) + 

        ' AND ' + CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + @RowPerPage);
 
    EXEC sp_executesql @SQL1;

END

GO
 