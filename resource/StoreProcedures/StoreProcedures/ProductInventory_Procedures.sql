USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertProductInventory    Script Date: 12/2/2025 4:44:58 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertProductInventory]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertProductInventory]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertProductInventory
(
	@Id int OUTPUT,
	@ProductId int,
	@CurrentStock int,
	@AverageCost decimal(18, 2),
	@UpdatedAt datetime,
	@SuggestedSellingPrice decimal(18, 2),
	@LastRestockedAt datetime,
	@ReorderNeeded bit,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100)
)
AS
    INSERT INTO [dbo].[ProductInventory] 
	(
	[ProductId],
	[CurrentStock],
	[AverageCost],
	[UpdatedAt],
	[SuggestedSellingPrice],
	[LastRestockedAt],
	[ReorderNeeded],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy]
    ) 
	VALUES 
	(
	@ProductId,
	@CurrentStock,
	@AverageCost,
	@UpdatedAt,
	@SuggestedSellingPrice,
	@LastRestockedAt,
	@ReorderNeeded,
	@CreatedBy,
	@CreatedAt,
	@UpdatedBy
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

/****** Object:  StoredProcedure [dbo].UpdateProductInventory    Script Date: 12/2/2025 4:44:58 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateProductInventory]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateProductInventory]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateProductInventory
(
	@Id int,
	@ProductId int,
	@CurrentStock int,
	@AverageCost decimal(18, 2),
	@UpdatedAt datetime,
	@SuggestedSellingPrice decimal(18, 2),
	@LastRestockedAt datetime,
	@ReorderNeeded bit,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100)
)
AS
    UPDATE [dbo].[ProductInventory] 
	SET
	[ProductId] = @ProductId,
	[CurrentStock] = @CurrentStock,
	[AverageCost] = @AverageCost,
	[UpdatedAt] = @UpdatedAt,
	[SuggestedSellingPrice] = @SuggestedSellingPrice,
	[LastRestockedAt] = @LastRestockedAt,
	[ReorderNeeded] = @ReorderNeeded,
	[CreatedBy] = @CreatedBy,
	[CreatedAt] = @CreatedAt,
	[UpdatedBy] = @UpdatedBy
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

/****** Object:  StoredProcedure [dbo].DeleteProductInventory    Script Date: 12/2/2025 4:44:58 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteProductInventory]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteProductInventory]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteProductInventory
(
	@Id int
)
AS
	DELETE [dbo].[ProductInventory] 

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

/****** Object:  StoredProcedure [dbo].GetAllProductInventory    Script Date: 12/2/2025 4:44:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllProductInventory]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllProductInventory]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllProductInventory
AS
	SELECT *		
	FROM
		[dbo].[ProductInventory]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetProductInventoryById    Script Date: 12/2/2025 4:44:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductInventoryById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductInventoryById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductInventoryById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[ProductInventory]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllProductInventoryByProductId    Script Date: 12/2/2025 4:44:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductInventoryByProductId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductInventoryByProductId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductInventoryByProductId
(
	@ProductId int
)
AS
	SELECT *		
	FROM
		[dbo].[ProductInventory]
	WHERE ( ProductId = @ProductId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetProductInventoryMaximumId    Script Date: 12/2/2025 4:44:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductInventoryMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductInventoryMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductInventoryMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[ProductInventory]

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

/****** Object:  StoredProcedure [dbo].GetProductInventoryRowCount    Script Date: 12/2/2025 4:44:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductInventoryRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductInventoryRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductInventoryRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[ProductInventory]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedProductInventory    Script Date: 12/2/2025 4:44:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedProductInventory]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedProductInventory]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedProductInventory
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

SET @SQL1 = 'WITH ProductInventoryEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[ProductId],
	[CurrentStock],
	[AverageCost],
	[UpdatedAt],
	[SuggestedSellingPrice],
	[LastRestockedAt],
	[ReorderNeeded],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy]
				FROM 
				[dbo].[ProductInventory]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[ProductId],
	[CurrentStock],
	[AverageCost],
	[UpdatedAt],
	[SuggestedSellingPrice],
	[LastRestockedAt],
	[ReorderNeeded],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy]
				FROM 
					ProductInventoryEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[ProductInventory] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetProductInventoryByQuery    Script Date: 12/2/2025 4:44:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductInventoryByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductInventoryByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductInventoryByQuery
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
				[dbo].[ProductInventory] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

