USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertCustomerPayment    Script Date: 12/29/2025 1:59:57 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertCustomerPayment]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertCustomerPayment]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertCustomerPayment
(
	@Id int OUTPUT,
	@CustomerId int,
	@PaymentMethodId int,
	@InventoryTransactionId int,
	@PaymentType nvarchar(20),
	@Amount decimal(18, 2),
	@PaymentDate datetime,
	@Status nvarchar(20),
	@Notes nvarchar(500),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime,
	@TransactionReference nvarchar(100)
)
AS
    INSERT INTO [dbo].[CustomerPayment] 
	(
	[CustomerId],
	[PaymentMethodId],
	[InventoryTransactionId],
	[PaymentType],
	[Amount],
	[PaymentDate],
	[Status],
	[Notes],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[TransactionReference]
    ) 
	VALUES 
	(
	@CustomerId,
	@PaymentMethodId,
	@InventoryTransactionId,
	@PaymentType,
	@Amount,
	@PaymentDate,
	@Status,
	@Notes,
	@CreatedBy,
	@CreatedAt,
	@UpdatedBy,
	@UpdatedAt,
	@TransactionReference
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

/****** Object:  StoredProcedure [dbo].UpdateCustomerPayment    Script Date: 12/29/2025 1:59:57 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateCustomerPayment]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateCustomerPayment]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateCustomerPayment
(
	@Id int,
	@CustomerId int,
	@PaymentMethodId int,
	@InventoryTransactionId int,
	@PaymentType nvarchar(20),
	@Amount decimal(18, 2),
	@PaymentDate datetime,
	@Status nvarchar(20),
	@Notes nvarchar(500),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime,
	@TransactionReference nvarchar(100)
)
AS
    UPDATE [dbo].[CustomerPayment] 
	SET
	[CustomerId] = @CustomerId,
	[PaymentMethodId] = @PaymentMethodId,
	[InventoryTransactionId] = @InventoryTransactionId,
	[PaymentType] = @PaymentType,
	[Amount] = @Amount,
	[PaymentDate] = @PaymentDate,
	[Status] = @Status,
	[Notes] = @Notes,
	[CreatedBy] = @CreatedBy,
	[CreatedAt] = @CreatedAt,
	[UpdatedBy] = @UpdatedBy,
	[UpdatedAt] = @UpdatedAt,
	[TransactionReference] = @TransactionReference
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

/****** Object:  StoredProcedure [dbo].DeleteCustomerPayment    Script Date: 12/29/2025 1:59:57 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteCustomerPayment]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteCustomerPayment]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteCustomerPayment
(
	@Id int
)
AS
	DELETE [dbo].[CustomerPayment] 

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

/****** Object:  StoredProcedure [dbo].GetAllCustomerPayment    Script Date: 12/29/2025 1:59:57 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllCustomerPayment]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllCustomerPayment]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllCustomerPayment
AS
	SELECT *		
	FROM
		[dbo].[CustomerPayment]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetCustomerPaymentById    Script Date: 12/29/2025 1:59:57 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCustomerPaymentById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCustomerPaymentById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCustomerPaymentById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[CustomerPayment]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllCustomerPaymentByCustomerId    Script Date: 12/29/2025 1:59:57 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCustomerPaymentByCustomerId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCustomerPaymentByCustomerId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCustomerPaymentByCustomerId
(
	@CustomerId int
)
AS
	SELECT *		
	FROM
		[dbo].[CustomerPayment]
	WHERE ( CustomerId = @CustomerId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllCustomerPaymentByPaymentMethodId    Script Date: 12/29/2025 1:59:57 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCustomerPaymentByPaymentMethodId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCustomerPaymentByPaymentMethodId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCustomerPaymentByPaymentMethodId
(
	@PaymentMethodId int
)
AS
	SELECT *		
	FROM
		[dbo].[CustomerPayment]
	WHERE ( PaymentMethodId = @PaymentMethodId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllCustomerPaymentByInventoryTransactionId    Script Date: 12/29/2025 1:59:57 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCustomerPaymentByInventoryTransactionId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCustomerPaymentByInventoryTransactionId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCustomerPaymentByInventoryTransactionId
(
	@InventoryTransactionId int
)
AS
	SELECT *		
	FROM
		[dbo].[CustomerPayment]
	WHERE ( InventoryTransactionId = @InventoryTransactionId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetCustomerPaymentMaximumId    Script Date: 12/29/2025 1:59:57 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCustomerPaymentMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCustomerPaymentMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCustomerPaymentMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[CustomerPayment]

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

/****** Object:  StoredProcedure [dbo].GetCustomerPaymentRowCount    Script Date: 12/29/2025 1:59:57 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCustomerPaymentRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCustomerPaymentRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCustomerPaymentRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[CustomerPayment]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedCustomerPayment    Script Date: 12/29/2025 1:59:57 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedCustomerPayment]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedCustomerPayment]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedCustomerPayment
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

SET @SQL1 = 'WITH CustomerPaymentEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[CustomerId],
	[PaymentMethodId],
	[InventoryTransactionId],
	[PaymentType],
	[Amount],
	[PaymentDate],
	[Status],
	[Notes],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[TransactionReference]
				FROM 
				[dbo].[CustomerPayment]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[CustomerId],
	[PaymentMethodId],
	[InventoryTransactionId],
	[PaymentType],
	[Amount],
	[PaymentDate],
	[Status],
	[Notes],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[TransactionReference]
				FROM 
					CustomerPaymentEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[CustomerPayment] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetCustomerPaymentByQuery    Script Date: 12/29/2025 1:59:57 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCustomerPaymentByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCustomerPaymentByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCustomerPaymentByQuery
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
				[dbo].[CustomerPayment] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

