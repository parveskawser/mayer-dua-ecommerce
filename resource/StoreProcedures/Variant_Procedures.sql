USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertVariant    Script Date: 12/2/2025 4:44:59 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertVariant]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertVariant]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertVariant
(
	@Id int OUTPUT,
	@ProductId int,
	@Sku nvarchar(100),
	@UpcBarcode nvarchar(64),
	@VariantKeyHash nvarchar(64),
	@IsActive bit,
	@CreatedAt datetime
)
AS
    INSERT INTO [dbo].[Variant] 
	(
	[ProductId],
	[Sku],
	[UpcBarcode],
	[VariantKeyHash],
	[IsActive],
	[CreatedAt]
    ) 
	VALUES 
	(
	@ProductId,
	@Sku,
	@UpcBarcode,
	@VariantKeyHash,
	@IsActive,
	@CreatedAt
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

/****** Object:  StoredProcedure [dbo].UpdateVariant    Script Date: 12/2/2025 4:44:59 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateVariant]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateVariant]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateVariant
(
	@Id int,
	@ProductId int,
	@Sku nvarchar(100),
	@UpcBarcode nvarchar(64),
	@VariantKeyHash nvarchar(64),
	@IsActive bit,
	@CreatedAt datetime
)
AS
    UPDATE [dbo].[Variant] 
	SET
	[ProductId] = @ProductId,
	[Sku] = @Sku,
	[UpcBarcode] = @UpcBarcode,
	[VariantKeyHash] = @VariantKeyHash,
	[IsActive] = @IsActive,
	[CreatedAt] = @CreatedAt
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

/****** Object:  StoredProcedure [dbo].DeleteVariant    Script Date: 12/2/2025 4:44:59 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteVariant]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteVariant]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteVariant
(
	@Id int
)
AS
	DELETE [dbo].[Variant] 

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

/****** Object:  StoredProcedure [dbo].GetAllVariant    Script Date: 12/2/2025 4:44:59 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllVariant]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllVariant]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllVariant
AS
	SELECT *		
	FROM
		[dbo].[Variant]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetVariantById    Script Date: 12/2/2025 4:44:59 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVariantById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVariantById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVariantById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[Variant]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllVariantByProductId    Script Date: 12/2/2025 4:44:59 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVariantByProductId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVariantByProductId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVariantByProductId
(
	@ProductId int
)
AS
	SELECT *		
	FROM
		[dbo].[Variant]
	WHERE ( ProductId = @ProductId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetVariantMaximumId    Script Date: 12/2/2025 4:44:59 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVariantMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVariantMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVariantMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[Variant]

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

/****** Object:  StoredProcedure [dbo].GetVariantRowCount    Script Date: 12/2/2025 4:44:59 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVariantRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVariantRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVariantRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[Variant]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedVariant    Script Date: 12/2/2025 4:44:59 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedVariant]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedVariant]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedVariant
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

SET @SQL1 = 'WITH VariantEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[ProductId],
	[Sku],
	[UpcBarcode],
	[VariantKeyHash],
	[IsActive],
	[CreatedAt]
				FROM 
				[dbo].[Variant]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[ProductId],
	[Sku],
	[UpcBarcode],
	[VariantKeyHash],
	[IsActive],
	[CreatedAt]
				FROM 
					VariantEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[Variant] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetVariantByQuery    Script Date: 12/2/2025 4:44:59 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVariantByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVariantByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVariantByQuery
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
				[dbo].[Variant] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

