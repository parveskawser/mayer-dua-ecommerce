USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertProductVariant    Script Date: 12/29/2025 2:00:00 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertProductVariant]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertProductVariant]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertProductVariant
(
	@Id int OUTPUT,
	@ProductId int,
	@VariantName nvarchar(150),
	@SKU nvarchar(50),
	@Barcode nvarchar(100),
	@VariantPrice decimal(18, 2),
	@IsActive bit,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    INSERT INTO [dbo].[ProductVariant] 
	(
	[ProductId],
	[VariantName],
	[SKU],
	[Barcode],
	[VariantPrice],
	[IsActive],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
    ) 
	VALUES 
	(
	@ProductId,
	@VariantName,
	@SKU,
	@Barcode,
	@VariantPrice,
	@IsActive,
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

/****** Object:  StoredProcedure [dbo].UpdateProductVariant    Script Date: 12/29/2025 2:00:00 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateProductVariant]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateProductVariant]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateProductVariant
(
	@Id int,
	@ProductId int,
	@VariantName nvarchar(150),
	@SKU nvarchar(50),
	@Barcode nvarchar(100),
	@VariantPrice decimal(18, 2),
	@IsActive bit,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    UPDATE [dbo].[ProductVariant] 
	SET
	[ProductId] = @ProductId,
	[VariantName] = @VariantName,
	[SKU] = @SKU,
	[Barcode] = @Barcode,
	[VariantPrice] = @VariantPrice,
	[IsActive] = @IsActive,
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

/****** Object:  StoredProcedure [dbo].DeleteProductVariant    Script Date: 12/29/2025 2:00:00 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteProductVariant]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteProductVariant]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteProductVariant
(
	@Id int
)
AS
	DELETE [dbo].[ProductVariant] 

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

/****** Object:  StoredProcedure [dbo].GetAllProductVariant    Script Date: 12/29/2025 2:00:00 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllProductVariant]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllProductVariant]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllProductVariant
AS
	SELECT *		
	FROM
		[dbo].[ProductVariant]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetProductVariantById    Script Date: 12/29/2025 2:00:00 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductVariantById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductVariantById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductVariantById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[ProductVariant]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllProductVariantByProductId    Script Date: 12/29/2025 2:00:00 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductVariantByProductId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductVariantByProductId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductVariantByProductId
(
	@ProductId int
)
AS
	SELECT *		
	FROM
		[dbo].[ProductVariant]
	WHERE ( ProductId = @ProductId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetProductVariantMaximumId    Script Date: 12/29/2025 2:00:00 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductVariantMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductVariantMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductVariantMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[ProductVariant]

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

/****** Object:  StoredProcedure [dbo].GetProductVariantRowCount    Script Date: 12/29/2025 2:00:00 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductVariantRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductVariantRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductVariantRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[ProductVariant]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedProductVariant    Script Date: 12/29/2025 2:00:00 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedProductVariant]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedProductVariant]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedProductVariant
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

SET @SQL1 = 'WITH ProductVariantEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[ProductId],
	[VariantName],
	[SKU],
	[Barcode],
	[VariantPrice],
	[IsActive],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
				[dbo].[ProductVariant]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[ProductId],
	[VariantName],
	[SKU],
	[Barcode],
	[VariantPrice],
	[IsActive],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
					ProductVariantEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[ProductVariant] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetProductVariantByQuery    Script Date: 12/29/2025 2:00:00 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductVariantByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductVariantByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductVariantByQuery
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
				[dbo].[ProductVariant] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

