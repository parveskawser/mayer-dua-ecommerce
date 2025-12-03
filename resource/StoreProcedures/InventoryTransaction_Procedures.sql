USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertInventoryTransaction    Script Date: 12/2/2025 4:44:56 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertInventoryTransaction]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertInventoryTransaction]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertInventoryTransaction
(
	@Id int OUTPUT,
	@SalesOrderDetailId int,
	@PoReceivedId int,
	@InOut nvarchar(3),
	@Date datetime,
	@Price decimal(18, 2),
	@Quantity int,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime,
	@Remarks nvarchar(255),
	@ProductVariantId int
)
AS
    INSERT INTO [dbo].[InventoryTransaction] 
	(
	[SalesOrderDetailId],
	[PoReceivedId],
	[InOut],
	[Date],
	[Price],
	[Quantity],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[Remarks],
	[ProductVariantId]
    ) 
	VALUES 
	(
	@SalesOrderDetailId,
	@PoReceivedId,
	@InOut,
	@Date,
	@Price,
	@Quantity,
	@CreatedBy,
	@CreatedAt,
	@UpdatedBy,
	@UpdatedAt,
	@Remarks,
	@ProductVariantId
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

/****** Object:  StoredProcedure [dbo].UpdateInventoryTransaction    Script Date: 12/2/2025 4:44:56 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateInventoryTransaction]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateInventoryTransaction]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateInventoryTransaction
(
	@Id int,
	@SalesOrderDetailId int,
	@PoReceivedId int,
	@InOut nvarchar(3),
	@Date datetime,
	@Price decimal(18, 2),
	@Quantity int,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime,
	@Remarks nvarchar(255),
	@ProductVariantId int
)
AS
    UPDATE [dbo].[InventoryTransaction] 
	SET
	[SalesOrderDetailId] = @SalesOrderDetailId,
	[PoReceivedId] = @PoReceivedId,
	[InOut] = @InOut,
	[Date] = @Date,
	[Price] = @Price,
	[Quantity] = @Quantity,
	[CreatedBy] = @CreatedBy,
	[CreatedAt] = @CreatedAt,
	[UpdatedBy] = @UpdatedBy,
	[UpdatedAt] = @UpdatedAt,
	[Remarks] = @Remarks,
	[ProductVariantId] = @ProductVariantId
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

/****** Object:  StoredProcedure [dbo].DeleteInventoryTransaction    Script Date: 12/2/2025 4:44:56 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteInventoryTransaction]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteInventoryTransaction]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteInventoryTransaction
(
	@Id int
)
AS
	DELETE [dbo].[InventoryTransaction] 

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

/****** Object:  StoredProcedure [dbo].GetAllInventoryTransaction    Script Date: 12/2/2025 4:44:56 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllInventoryTransaction]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllInventoryTransaction]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllInventoryTransaction
AS
	SELECT *		
	FROM
		[dbo].[InventoryTransaction]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetInventoryTransactionById    Script Date: 12/2/2025 4:44:56 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetInventoryTransactionById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetInventoryTransactionById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetInventoryTransactionById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[InventoryTransaction]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllInventoryTransactionBySalesOrderDetailId    Script Date: 12/2/2025 4:44:56 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetInventoryTransactionBySalesOrderDetailId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetInventoryTransactionBySalesOrderDetailId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetInventoryTransactionBySalesOrderDetailId
(
	@SalesOrderDetailId int
)
AS
	SELECT *		
	FROM
		[dbo].[InventoryTransaction]
	WHERE ( SalesOrderDetailId = @SalesOrderDetailId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllInventoryTransactionByPoReceivedId    Script Date: 12/2/2025 4:44:56 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetInventoryTransactionByPoReceivedId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetInventoryTransactionByPoReceivedId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetInventoryTransactionByPoReceivedId
(
	@PoReceivedId int
)
AS
	SELECT *		
	FROM
		[dbo].[InventoryTransaction]
	WHERE ( PoReceivedId = @PoReceivedId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllInventoryTransactionByProductVariantId    Script Date: 12/2/2025 4:44:56 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetInventoryTransactionByProductVariantId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetInventoryTransactionByProductVariantId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetInventoryTransactionByProductVariantId
(
	@ProductVariantId int
)
AS
	SELECT *		
	FROM
		[dbo].[InventoryTransaction]
	WHERE ( ProductVariantId = @ProductVariantId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetInventoryTransactionMaximumId    Script Date: 12/2/2025 4:44:56 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetInventoryTransactionMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetInventoryTransactionMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetInventoryTransactionMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[InventoryTransaction]

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

/****** Object:  StoredProcedure [dbo].GetInventoryTransactionRowCount    Script Date: 12/2/2025 4:44:56 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetInventoryTransactionRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetInventoryTransactionRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetInventoryTransactionRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[InventoryTransaction]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedInventoryTransaction    Script Date: 12/2/2025 4:44:56 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedInventoryTransaction]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedInventoryTransaction]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedInventoryTransaction
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

SET @SQL1 = 'WITH InventoryTransactionEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[SalesOrderDetailId],
	[PoReceivedId],
	[InOut],
	[Date],
	[Price],
	[Quantity],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[Remarks],
	[ProductVariantId]
				FROM 
				[dbo].[InventoryTransaction]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[SalesOrderDetailId],
	[PoReceivedId],
	[InOut],
	[Date],
	[Price],
	[Quantity],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[Remarks],
	[ProductVariantId]
				FROM 
					InventoryTransactionEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[InventoryTransaction] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetInventoryTransactionByQuery    Script Date: 12/2/2025 4:44:56 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetInventoryTransactionByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetInventoryTransactionByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetInventoryTransactionByQuery
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
				[dbo].[InventoryTransaction] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

