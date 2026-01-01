USE AA4

GO
 
/****** Object:  StoredProcedure [dbo].[InsertPoReceived] ******/

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertPoReceived]') AND type in (N'P', N'PC'))

DROP PROCEDURE [dbo].[InsertPoReceived]

GO
 
SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO
 
CREATE PROCEDURE InsertPoReceived

(

	@Id int OUTPUT,

	@PoRequestedId int,

	@ReceivedQuantity int,

	@BuyingPrice decimal(18, 2),

	@ReceivedDate datetime,

	@CreatedBy nvarchar(100),

	@CreatedAt datetime,

	@UpdatedBy nvarchar(100),

	@UpdatedAt datetime,

	@Remarks nvarchar(500),

	@InvoiceNo nvarchar(100),

    -- Removed @TotalPaymentDue because it is calculated automatically

	@TotalPaid decimal(18, 2),

	@PaymentStatus nvarchar(20)

)

AS

BEGIN

    INSERT INTO [dbo].[PoReceived] 

	(

        [PoRequestedId],

        [ReceivedQuantity],

        [BuyingPrice],

        [ReceivedDate],

        [CreatedBy],

        [CreatedAt],

        [UpdatedBy],

        [UpdatedAt],

        [Remarks],

        [InvoiceNo],

        -- [TotalPaymentDue] removed

        [TotalPaid],

        [PaymentStatus]

    ) 

	VALUES 

	(

        @PoRequestedId,

        @ReceivedQuantity,

        @BuyingPrice,

        @ReceivedDate,

        @CreatedBy,

        @CreatedAt,

        @UpdatedBy,

        @UpdatedAt,

        @Remarks,

        @InvoiceNo,

        -- @TotalPaymentDue removed

        @TotalPaid,

        @PaymentStatus

    )
 
	SET @Id = SCOPE_IDENTITY()

	RETURN @Id

END

GO
 
/****** Object:  StoredProcedure [dbo].[UpdatePoReceived] ******/

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdatePoReceived]') AND type in (N'P', N'PC'))

DROP PROCEDURE [dbo].[UpdatePoReceived]

GO
 
SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO
 
CREATE PROCEDURE UpdatePoReceived

(

	@Id int,

	@PoRequestedId int,

	@ReceivedQuantity int,

	@BuyingPrice decimal(18, 2),

	@ReceivedDate datetime,

	@CreatedBy nvarchar(100),

	@CreatedAt datetime,

	@UpdatedBy nvarchar(100),

	@UpdatedAt datetime,

	@Remarks nvarchar(500),

	@InvoiceNo nvarchar(100),

    -- Removed @TotalPaymentDue

	@TotalPaid decimal(18, 2),

	@PaymentStatus nvarchar(20)

)

AS

BEGIN

    UPDATE [dbo].[PoReceived] 

	SET

        [PoRequestedId] = @PoRequestedId,

        [ReceivedQuantity] = @ReceivedQuantity,

        [BuyingPrice] = @BuyingPrice,

        [ReceivedDate] = @ReceivedDate,

        [CreatedBy] = @CreatedBy,

        [CreatedAt] = @CreatedAt,

        [UpdatedBy] = @UpdatedBy,

        [UpdatedAt] = @UpdatedAt,

        [Remarks] = @Remarks,

        [InvoiceNo] = @InvoiceNo,

        -- [TotalPaymentDue] removed (it updates automatically when Quantity/Price changes)

        [TotalPaid] = @TotalPaid,

        [PaymentStatus] = @PaymentStatus

	WHERE ( Id = @Id )
 
	RETURN @@ROWCOUNT

END

GO
 