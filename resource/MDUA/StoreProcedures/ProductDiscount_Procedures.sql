USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertProductDiscount    Script Date: 12/29/2025 2:00:00 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertProductDiscount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertProductDiscount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertProductDiscount
(
	@Id int OUTPUT,
	@ProductId int,
	@DiscountType nvarchar(50),
	@DiscountValue decimal(18, 2),
	@MinQuantity int,
	@EffectiveFrom datetime,
	@EffectiveTo datetime,
	@IsActive bit,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    INSERT INTO [dbo].[ProductDiscount] 
	(
	[ProductId],
	[DiscountType],
	[DiscountValue],
	[MinQuantity],
	[EffectiveFrom],
	[EffectiveTo],
	[IsActive],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
    ) 
	VALUES 
	(
	@ProductId,
	@DiscountType,
	@DiscountValue,
	@MinQuantity,
	@EffectiveFrom,
	@EffectiveTo,
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

/****** Object:  StoredProcedure [dbo].UpdateProductDiscount    Script Date: 12/29/2025 2:00:00 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateProductDiscount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateProductDiscount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateProductDiscount
(
	@Id int,
	@ProductId int,
	@DiscountType nvarchar(50),
	@DiscountValue decimal(18, 2),
	@MinQuantity int,
	@EffectiveFrom datetime,
	@EffectiveTo datetime,
	@IsActive bit,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    UPDATE [dbo].[ProductDiscount] 
	SET
	[ProductId] = @ProductId,
	[DiscountType] = @DiscountType,
	[DiscountValue] = @DiscountValue,
	[MinQuantity] = @MinQuantity,
	[EffectiveFrom] = @EffectiveFrom,
	[EffectiveTo] = @EffectiveTo,
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

/****** Object:  StoredProcedure [dbo].DeleteProductDiscount    Script Date: 12/29/2025 2:00:00 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteProductDiscount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteProductDiscount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteProductDiscount
(
	@Id int
)
AS
	DELETE [dbo].[ProductDiscount] 

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

/****** Object:  StoredProcedure [dbo].GetAllProductDiscount    Script Date: 12/29/2025 2:00:00 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllProductDiscount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllProductDiscount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllProductDiscount
AS
	SELECT *		
	FROM
		[dbo].[ProductDiscount]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetProductDiscountById    Script Date: 12/29/2025 2:00:00 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductDiscountById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductDiscountById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductDiscountById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[ProductDiscount]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllProductDiscountByProductId    Script Date: 12/29/2025 2:00:00 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductDiscountByProductId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductDiscountByProductId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductDiscountByProductId
(
	@ProductId int
)
AS
	SELECT *		
	FROM
		[dbo].[ProductDiscount]
	WHERE ( ProductId = @ProductId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetProductDiscountMaximumId    Script Date: 12/29/2025 2:00:00 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductDiscountMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductDiscountMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductDiscountMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[ProductDiscount]

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

/****** Object:  StoredProcedure [dbo].GetProductDiscountRowCount    Script Date: 12/29/2025 2:00:00 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductDiscountRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductDiscountRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductDiscountRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[ProductDiscount]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedProductDiscount    Script Date: 12/29/2025 2:00:00 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedProductDiscount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedProductDiscount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedProductDiscount
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

SET @SQL1 = 'WITH ProductDiscountEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[ProductId],
	[DiscountType],
	[DiscountValue],
	[MinQuantity],
	[EffectiveFrom],
	[EffectiveTo],
	[IsActive],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
				[dbo].[ProductDiscount]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[ProductId],
	[DiscountType],
	[DiscountValue],
	[MinQuantity],
	[EffectiveFrom],
	[EffectiveTo],
	[IsActive],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
					ProductDiscountEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[ProductDiscount] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetProductDiscountByQuery    Script Date: 12/29/2025 2:00:00 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductDiscountByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductDiscountByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductDiscountByQuery
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
				[dbo].[ProductDiscount] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

