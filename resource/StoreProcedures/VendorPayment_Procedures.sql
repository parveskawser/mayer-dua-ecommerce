USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertVendorPayment    Script Date: 12/10/2025 2:32:54 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertVendorPayment]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertVendorPayment]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertVendorPayment
(
	@Id int OUTPUT,
	@VendorId int,
	@PaymentMethodId int,
	@InventoryTransactionId int,
	@ReferenceNumber nvarchar(100),
	@PaymentType nvarchar(20),
	@Amount decimal(18, 2),
	@PaymentDate datetime,
	@Status nvarchar(20),
	@Notes nvarchar(500),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    INSERT INTO [dbo].[VendorPayment] 
	(
	[VendorId],
	[PaymentMethodId],
	[InventoryTransactionId],
	[ReferenceNumber],
	[PaymentType],
	[Amount],
	[PaymentDate],
	[Status],
	[Notes],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
    ) 
	VALUES 
	(
	@VendorId,
	@PaymentMethodId,
	@InventoryTransactionId,
	@ReferenceNumber,
	@PaymentType,
	@Amount,
	@PaymentDate,
	@Status,
	@Notes,
	@CreatedBy,
	@CreatedAt,
	@UpdatedBy,
	@UpdatedAt
    )
	DECLARE @Err int
	DECLARE @Result int

	SET @Result = @@ROWCOUNT
	SET @Err = @@ERROR 
	If @Err <> 0 
	BEGIN
		SET @Id = -1
		RETURN @Err
	END
	ELSE
	BEGIN
		If @Result = 1 
		BEGIN
			-- Everything is OK
			SET @Id = @@IDENTITY
		END
		ELSE
		BEGIN
			SET @Id = -1
			RETURN 0
		END
	END

	RETURN @Id
GO

/****** Object:  StoredProcedure [dbo].UpdateVendorPayment    Script Date: 12/10/2025 2:32:54 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateVendorPayment]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateVendorPayment]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateVendorPayment
(
	@Id int,
	@VendorId int,
	@PaymentMethodId int,
	@InventoryTransactionId int,
	@ReferenceNumber nvarchar(100),
	@PaymentType nvarchar(20),
	@Amount decimal(18, 2),
	@PaymentDate datetime,
	@Status nvarchar(20),
	@Notes nvarchar(500),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
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
	[CreatedBy] = @CreatedBy,
	[CreatedAt] = @CreatedAt,
	[UpdatedBy] = @UpdatedBy,
	[UpdatedAt] = @UpdatedAt
	WHERE ( Id = @Id )

	DECLARE @Err int
	DECLARE @Result int
	SET @Result = @@ROWCOUNT
	SET @Err = @@ERROR 

	If @Err <> 0 
	BEGIN
		SET @Result = -1
	END

	RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].DeleteVendorPayment    Script Date: 12/10/2025 2:32:54 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteVendorPayment]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteVendorPayment]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteVendorPayment
(
	@Id int
)
AS
	DELETE [dbo].[VendorPayment] 

    WHERE ( Id = @Id )

	DECLARE @Err int
	DECLARE @Result int

	SET @Result = @@ROWCOUNT
	SET @Err = @@ERROR 

	If @Err <> 0 
	BEGIN
		SET @Result = -1
	END

	RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetAllVendorPayment    Script Date: 12/10/2025 2:32:54 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllVendorPayment]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllVendorPayment]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllVendorPayment
AS
	SELECT *		
	FROM
		[dbo].[VendorPayment]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetVendorPaymentById    Script Date: 12/10/2025 2:32:54 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVendorPaymentById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVendorPaymentById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVendorPaymentById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[VendorPayment]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllVendorPaymentByVendorId    Script Date: 12/10/2025 2:32:54 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVendorPaymentByVendorId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVendorPaymentByVendorId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVendorPaymentByVendorId
(
	@VendorId int
)
AS
	SELECT *		
	FROM
		[dbo].[VendorPayment]
	WHERE ( VendorId = @VendorId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllVendorPaymentByPaymentMethodId    Script Date: 12/10/2025 2:32:54 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVendorPaymentByPaymentMethodId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVendorPaymentByPaymentMethodId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVendorPaymentByPaymentMethodId
(
	@PaymentMethodId int
)
AS
	SELECT *		
	FROM
		[dbo].[VendorPayment]
	WHERE ( PaymentMethodId = @PaymentMethodId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllVendorPaymentByInventoryTransactionId    Script Date: 12/10/2025 2:32:54 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVendorPaymentByInventoryTransactionId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVendorPaymentByInventoryTransactionId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVendorPaymentByInventoryTransactionId
(
	@InventoryTransactionId int
)
AS
	SELECT *		
	FROM
		[dbo].[VendorPayment]
	WHERE ( InventoryTransactionId = @InventoryTransactionId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetVendorPaymentMaximumId    Script Date: 12/10/2025 2:32:54 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVendorPaymentMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVendorPaymentMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVendorPaymentMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[VendorPayment]

	If @Result > 0 
		BEGIN
			-- Everything is OK
			RETURN @Result
		END
		ELSE
		BEGIN
			RETURN 0
		END
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetVendorPaymentRowCount    Script Date: 12/10/2025 2:32:54 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVendorPaymentRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVendorPaymentRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVendorPaymentRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[VendorPayment]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedVendorPayment    Script Date: 12/10/2025 2:32:54 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedVendorPayment]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedVendorPayment]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedVendorPayment
(
	@TotalRows		int	OUTPUT,
	@PageIndex	int,
	@RowPerPage		int,
	@WhereClause	nvarchar(4000),
	@SortColumn		nvarchar(128),
	@SortOrder		nvarchar(4)
)
AS
BEGIN 

SET @PageIndex = isnull(@PageIndex, -1)
SET @RowPerPage = isnull(@RowPerPage, -1)
SET @WhereClause = isnull(@WhereClause, '')
SET @SortColumn = isnull(@SortColumn, '')
SET @SortOrder = isnull(@SortOrder, '')
SET @TotalRows = 0
SET @RowPerPage = @RowPerPage -1
DECLARE @SQL1 nvarchar(4000)
DECLARE @SQL2 nvarchar(4000)

IF (@WhereClause != '')
BEGIN
	SET @WhereClause = 'WHERE ' + char(13) + @WhereClause	
END

IF (@SortColumn != '')
BEGIN
	SET @SortColumn = 'ORDER BY ' + @SortColumn

	IF (@SortOrder != '')
	BEGIN
		SET @SortColumn = @SortColumn + ' ' + @SortOrder
	END
END
ELSE
BEGIN
	SET @SortColumn = @SortColumn + ' ORDER BY [Id] ASC'
END

SET @SQL1 = 'WITH VendorPaymentEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[VendorId],
	[PaymentMethodId],
	[InventoryTransactionId],
	[ReferenceNumber],
	[PaymentType],
	[Amount],
	[PaymentDate],
	[Status],
	[Notes],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
				[dbo].[VendorPayment]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[VendorId],
	[PaymentMethodId],
	[InventoryTransactionId],
	[ReferenceNumber],
	[PaymentType],
	[Amount],
	[PaymentDate],
	[Status],
	[Notes],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
					VendorPaymentEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[VendorPayment] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetVendorPaymentByQuery    Script Date: 12/10/2025 2:32:54 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVendorPaymentByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVendorPaymentByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVendorPaymentByQuery
(
	@Query	nvarchar(4000)
)
AS
BEGIN 

SET @Query = isnull(@Query, '')
DECLARE @SQL1 nvarchar(4000)

IF (@Query != '')
BEGIN
	SET @Query = 'WHERE ' + char(13) + @Query	
END

SET @SQL1 =		'SELECT * 
				FROM 
				[dbo].[VendorPayment] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

