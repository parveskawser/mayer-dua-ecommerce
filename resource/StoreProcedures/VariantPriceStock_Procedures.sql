USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertVariantPriceStock    Script Date: 12/21/2025 8:58:41 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertVariantPriceStock]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertVariantPriceStock]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertVariantPriceStock
(
	@Id int,
	@Price decimal(18, 2),
	@CompareAtPrice decimal(18, 2),
	@CostPrice decimal(18, 2),
	@StockQty int,
	@TrackInventory bit,
	@AllowBackorder bit,
	@WeightGrams int
)
AS
    INSERT INTO [dbo].[VariantPriceStock] 
	(
	[Id],
	[Price],
	[CompareAtPrice],
	[CostPrice],
	[StockQty],
	[TrackInventory],
	[AllowBackorder],
	[WeightGrams]
    ) 
	VALUES 
	(
	@Id,
	@Price,
	@CompareAtPrice,
	@CostPrice,
	@StockQty,
	@TrackInventory,
	@AllowBackorder,
	@WeightGrams
    )
	DECLARE @Err int
	DECLARE @Result int

	SET @Result = @@ROWCOUNT
	SET @Err = @@ERROR 
	If @Err <> 0 
	BEGIN
		RETURN @Err
	END
	ELSE
	BEGIN
		If @Result = 1 
		BEGIN
			-- Everything is OK
			RETURN @Result
		END
		ELSE
		BEGIN
			RETURN 0
		END
	END

	RETURN @Result
	
GO

/****** Object:  StoredProcedure [dbo].UpdateVariantPriceStock    Script Date: 12/21/2025 8:58:41 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateVariantPriceStock]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateVariantPriceStock]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateVariantPriceStock
(
	@Id int,
	@Price decimal(18, 2),
	@CompareAtPrice decimal(18, 2),
	@CostPrice decimal(18, 2),
	@StockQty int,
	@TrackInventory bit,
	@AllowBackorder bit,
	@WeightGrams int
)
AS
    UPDATE [dbo].[VariantPriceStock] 
	SET
	[Price] = @Price,
	[CompareAtPrice] = @CompareAtPrice,
	[CostPrice] = @CostPrice,
	[StockQty] = @StockQty,
	[TrackInventory] = @TrackInventory,
	[AllowBackorder] = @AllowBackorder,
	[WeightGrams] = @WeightGrams
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

/****** Object:  StoredProcedure [dbo].DeleteVariantPriceStock    Script Date: 12/21/2025 8:58:41 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteVariantPriceStock]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteVariantPriceStock]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteVariantPriceStock
(
	@Id int
)
AS
	DELETE [dbo].[VariantPriceStock] 

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

/****** Object:  StoredProcedure [dbo].GetAllVariantPriceStock    Script Date: 12/21/2025 8:58:41 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllVariantPriceStock]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllVariantPriceStock]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllVariantPriceStock
AS
	SELECT *		
	FROM
		[dbo].[VariantPriceStock]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetVariantPriceStockById    Script Date: 12/21/2025 8:58:41 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVariantPriceStockById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVariantPriceStockById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVariantPriceStockById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[VariantPriceStock]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetVariantPriceStockMaximumId    Script Date: 12/21/2025 8:58:41 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVariantPriceStockMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVariantPriceStockMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVariantPriceStockMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[VariantPriceStock]

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

/****** Object:  StoredProcedure [dbo].GetVariantPriceStockRowCount    Script Date: 12/21/2025 8:58:41 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVariantPriceStockRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVariantPriceStockRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVariantPriceStockRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[VariantPriceStock]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedVariantPriceStock    Script Date: 12/21/2025 8:58:41 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedVariantPriceStock]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedVariantPriceStock]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedVariantPriceStock
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

SET @SQL1 = 'WITH VariantPriceStockEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[Price],
	[CompareAtPrice],
	[CostPrice],
	[StockQty],
	[TrackInventory],
	[AllowBackorder],
	[WeightGrams]
				FROM 
				[dbo].[VariantPriceStock]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[Price],
	[CompareAtPrice],
	[CostPrice],
	[StockQty],
	[TrackInventory],
	[AllowBackorder],
	[WeightGrams]
				FROM 
					VariantPriceStockEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[VariantPriceStock] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetVariantPriceStockByQuery    Script Date: 12/21/2025 8:58:41 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVariantPriceStockByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVariantPriceStockByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVariantPriceStockByQuery
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
				[dbo].[VariantPriceStock] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

