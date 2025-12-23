USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertProduct    Script Date: 12/21/2025 8:58:38 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertProduct]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertProduct]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertProduct
(
	@Id int OUTPUT,
	@CompanyId int,
	@ProductName nvarchar(200),
	@ReorderLevel int,
	@Barcode nvarchar(100),
	@CategoryId int,
	@Description nvarchar(max),
	@Slug nvarchar(400),
	@BasePrice decimal(18, 2),
	@IsVariantBased bit,
	@IsActive bit,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    INSERT INTO [dbo].[Product] 
	(
	[CompanyId],
	[ProductName],
	[ReorderLevel],
	[Barcode],
	[CategoryId],
	[Description],
	[Slug],
	[BasePrice],
	[IsVariantBased],
	[IsActive],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
    ) 
	VALUES 
	(
	@CompanyId,
	@ProductName,
	@ReorderLevel,
	@Barcode,
	@CategoryId,
	@Description,
	@Slug,
	@BasePrice,
	@IsVariantBased,
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

/****** Object:  StoredProcedure [dbo].UpdateProduct    Script Date: 12/21/2025 8:58:38 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateProduct]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateProduct]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateProduct
(
	@Id int,
	@CompanyId int,
	@ProductName nvarchar(200),
	@ReorderLevel int,
	@Barcode nvarchar(100),
	@CategoryId int,
	@Description nvarchar(max),
	@Slug nvarchar(400),
	@BasePrice decimal(18, 2),
	@IsVariantBased bit,
	@IsActive bit,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    UPDATE [dbo].[Product] 
	SET
	[CompanyId] = @CompanyId,
	[ProductName] = @ProductName,
	[ReorderLevel] = @ReorderLevel,
	[Barcode] = @Barcode,
	[CategoryId] = @CategoryId,
	[Description] = @Description,
	[Slug] = @Slug,
	[BasePrice] = @BasePrice,
	[IsVariantBased] = @IsVariantBased,
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

/****** Object:  StoredProcedure [dbo].DeleteProduct    Script Date: 12/21/2025 8:58:38 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteProduct]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteProduct]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteProduct
(
	@Id int
)
AS
	DELETE [dbo].[Product] 

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

/****** Object:  StoredProcedure [dbo].GetAllProduct    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllProduct]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllProduct]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllProduct
AS
	SELECT *		
	FROM
		[dbo].[Product]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetProductById    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[Product]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllProductByCompanyId    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductByCompanyId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductByCompanyId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductByCompanyId
(
	@CompanyId int
)
AS
	SELECT *		
	FROM
		[dbo].[Product]
	WHERE ( CompanyId = @CompanyId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetProductMaximumId    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[Product]

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

/****** Object:  StoredProcedure [dbo].GetProductRowCount    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[Product]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedProduct    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedProduct]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedProduct]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedProduct
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

SET @SQL1 = 'WITH ProductEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[CompanyId],
	[ProductName],
	[ReorderLevel],
	[Barcode],
	[CategoryId],
	[Description],
	[Slug],
	[BasePrice],
	[IsVariantBased],
	[IsActive],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
				[dbo].[Product]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[CompanyId],
	[ProductName],
	[ReorderLevel],
	[Barcode],
	[CategoryId],
	[Description],
	[Slug],
	[BasePrice],
	[IsVariantBased],
	[IsActive],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
					ProductEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[Product] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetProductByQuery    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductByQuery
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
				[dbo].[Product] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

