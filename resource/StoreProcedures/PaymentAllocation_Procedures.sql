USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertPaymentAllocation    Script Date: 12/21/2025 8:58:37 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertPaymentAllocation]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertPaymentAllocation]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertPaymentAllocation
(
	@Id int OUTPUT,
	@CustomerPaymentId int,
	@SalesOrderId int,
	@AllocatedAmount decimal(18, 2),
	@AllocatedDate datetime,
	@Notes nvarchar(500),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    INSERT INTO [dbo].[PaymentAllocation] 
	(
	[CustomerPaymentId],
	[SalesOrderId],
	[AllocatedAmount],
	[AllocatedDate],
	[Notes],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
    ) 
	VALUES 
	(
	@CustomerPaymentId,
	@SalesOrderId,
	@AllocatedAmount,
	@AllocatedDate,
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

/****** Object:  StoredProcedure [dbo].UpdatePaymentAllocation    Script Date: 12/21/2025 8:58:37 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdatePaymentAllocation]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdatePaymentAllocation]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdatePaymentAllocation
(
	@Id int,
	@CustomerPaymentId int,
	@SalesOrderId int,
	@AllocatedAmount decimal(18, 2),
	@AllocatedDate datetime,
	@Notes nvarchar(500),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    UPDATE [dbo].[PaymentAllocation] 
	SET
	[CustomerPaymentId] = @CustomerPaymentId,
	[SalesOrderId] = @SalesOrderId,
	[AllocatedAmount] = @AllocatedAmount,
	[AllocatedDate] = @AllocatedDate,
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

/****** Object:  StoredProcedure [dbo].DeletePaymentAllocation    Script Date: 12/21/2025 8:58:37 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeletePaymentAllocation]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeletePaymentAllocation]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeletePaymentAllocation
(
	@Id int
)
AS
	DELETE [dbo].[PaymentAllocation] 

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

/****** Object:  StoredProcedure [dbo].GetAllPaymentAllocation    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllPaymentAllocation]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllPaymentAllocation]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllPaymentAllocation
AS
	SELECT *		
	FROM
		[dbo].[PaymentAllocation]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetPaymentAllocationById    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPaymentAllocationById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPaymentAllocationById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPaymentAllocationById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[PaymentAllocation]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllPaymentAllocationByCustomerPaymentId    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPaymentAllocationByCustomerPaymentId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPaymentAllocationByCustomerPaymentId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPaymentAllocationByCustomerPaymentId
(
	@CustomerPaymentId int
)
AS
	SELECT *		
	FROM
		[dbo].[PaymentAllocation]
	WHERE ( CustomerPaymentId = @CustomerPaymentId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllPaymentAllocationBySalesOrderId    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPaymentAllocationBySalesOrderId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPaymentAllocationBySalesOrderId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPaymentAllocationBySalesOrderId
(
	@SalesOrderId int
)
AS
	SELECT *		
	FROM
		[dbo].[PaymentAllocation]
	WHERE ( SalesOrderId = @SalesOrderId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetPaymentAllocationMaximumId    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPaymentAllocationMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPaymentAllocationMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPaymentAllocationMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[PaymentAllocation]

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

/****** Object:  StoredProcedure [dbo].GetPaymentAllocationRowCount    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPaymentAllocationRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPaymentAllocationRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPaymentAllocationRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[PaymentAllocation]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedPaymentAllocation    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedPaymentAllocation]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedPaymentAllocation]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedPaymentAllocation
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

SET @SQL1 = 'WITH PaymentAllocationEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[CustomerPaymentId],
	[SalesOrderId],
	[AllocatedAmount],
	[AllocatedDate],
	[Notes],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
				[dbo].[PaymentAllocation]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[CustomerPaymentId],
	[SalesOrderId],
	[AllocatedAmount],
	[AllocatedDate],
	[Notes],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
					PaymentAllocationEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[PaymentAllocation] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetPaymentAllocationByQuery    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPaymentAllocationByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPaymentAllocationByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPaymentAllocationByQuery
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
				[dbo].[PaymentAllocation] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

