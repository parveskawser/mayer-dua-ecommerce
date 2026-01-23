SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- 0. SAFETY CHECK: Ensure the column exists first (In case it wasn't added yet)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ProductSEO]') AND name = 'CustomHeaderTags')
BEGIN
    ALTER TABLE [dbo].[ProductSEO] ADD [CustomHeaderTags] NVARCHAR(MAX) NULL
END
GO

-------------------------------------------------------------------
-- 1. InsertProductSEO
-------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertProductSEO]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertProductSEO]
GO

CREATE PROCEDURE InsertProductSEO
(
	@Id int OUTPUT,
	@ProductId int,
	@MetaTitle nvarchar(100),
	@MetaKeywords nvarchar(255),
	@MetaDescription nvarchar(300),
	@CanonicalUrl nvarchar(500),
	@OGTitle nvarchar(150),
	@OGDescription nvarchar(300),
	@OGImage nvarchar(500),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime,
	@CustomHeaderTags nvarchar(MAX) -- Added as last parameter to match DB
)
AS
    INSERT INTO [dbo].[ProductSEO] 
	(
	[ProductId],
	[MetaTitle],
	[MetaKeywords],
	[MetaDescription],
	[CanonicalUrl],
	[OGTitle],
	[OGDescription],
	[OGImage],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[CustomHeaderTags] -- Added as last column
    ) 
	VALUES 
	(
	@ProductId,
	@MetaTitle,
	@MetaKeywords,
	@MetaDescription,
	@CanonicalUrl,
	@OGTitle,
	@OGDescription,
	@OGImage,
	@CreatedBy,
	@CreatedAt,
	@UpdatedBy,
	@UpdatedAt,
	@CustomHeaderTags
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

-------------------------------------------------------------------
-- 2. UpdateProductSEO
-------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateProductSEO]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateProductSEO]
GO

CREATE PROCEDURE UpdateProductSEO
(
	@Id int,
	@ProductId int,
	@MetaTitle nvarchar(100),
	@MetaKeywords nvarchar(255),
	@MetaDescription nvarchar(300),
	@CanonicalUrl nvarchar(500),
	@OGTitle nvarchar(150),
	@OGDescription nvarchar(300),
	@OGImage nvarchar(500),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime,
	@CustomHeaderTags nvarchar(MAX) -- Added as last parameter
)
AS
    UPDATE [dbo].[ProductSEO] 
	SET
	[ProductId] = @ProductId,
	[MetaTitle] = @MetaTitle,
	[MetaKeywords] = @MetaKeywords,
	[MetaDescription] = @MetaDescription,
	[CanonicalUrl] = @CanonicalUrl,
	[OGTitle] = @OGTitle,
	[OGDescription] = @OGDescription,
	[OGImage] = @OGImage,
	[CreatedBy] = @CreatedBy,
	[CreatedAt] = @CreatedAt,
	[UpdatedBy] = @UpdatedBy,
	[UpdatedAt] = @UpdatedAt,
	[CustomHeaderTags] = @CustomHeaderTags -- Added here
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

-------------------------------------------------------------------
-- 3. DeleteProductSEO
-------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteProductSEO]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteProductSEO]
GO

CREATE PROCEDURE DeleteProductSEO
(
	@Id int
)
AS
	DELETE [dbo].[ProductSEO] 
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

-------------------------------------------------------------------
-- 4. GetAllProductSEO
-------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllProductSEO]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllProductSEO]
GO

CREATE PROCEDURE GetAllProductSEO
AS
	SELECT *		
	FROM
		[dbo].[ProductSEO]

RETURN @@ROWCOUNT
GO

-------------------------------------------------------------------
-- 5. GetProductSEOById
-------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductSEOById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductSEOById]
GO

CREATE PROCEDURE GetProductSEOById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[ProductSEO]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

-------------------------------------------------------------------
-- 6. GetProductSEOByProductId
-------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductSEOByProductId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductSEOByProductId]
GO

CREATE PROCEDURE GetProductSEOByProductId
(
	@ProductId int
)
AS
	SELECT *		
	FROM
		[dbo].[ProductSEO]
	WHERE ( ProductId = @ProductId  )

RETURN @@ROWCOUNT
GO

-------------------------------------------------------------------
-- 7. GetProductSEOMaximumId
-------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductSEOMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductSEOMaximumId]
GO

CREATE PROCEDURE GetProductSEOMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[ProductSEO]

	If @Result > 0 
		BEGIN
			RETURN @Result
		END
		ELSE
		BEGIN
			RETURN 0
		END
RETURN @Result
GO

-------------------------------------------------------------------
-- 8. GetProductSEORowCount
-------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductSEORowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductSEORowCount]
GO

CREATE PROCEDURE GetProductSEORowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[ProductSEO]
		
RETURN @Result
GO

-------------------------------------------------------------------
-- 9. GetPagedProductSEO
-------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedProductSEO]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedProductSEO]
GO

CREATE PROCEDURE GetPagedProductSEO
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

	SET @SQL1 = 'WITH ProductSEOEntries AS (
				SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
		[Id],
		[ProductId],
		[MetaTitle],
		[MetaKeywords],
		[MetaDescription],
		[CanonicalUrl],
		[OGTitle],
		[OGDescription],
		[OGImage],
		[CreatedBy],
		[CreatedAt],
		[UpdatedBy],
		[UpdatedAt],
		[CustomHeaderTags] -- Added explicitly here
					FROM 
					[dbo].[ProductSEO]
						'+ @WhereClause +'
					)
					SELECT 
		[Id],
		[ProductId],
		[MetaTitle],
		[MetaKeywords],
		[MetaDescription],
		[CanonicalUrl],
		[OGTitle],
		[OGDescription],
		[OGImage],
		[CreatedBy],
		[CreatedAt],
		[UpdatedBy],
		[UpdatedAt],
		[CustomHeaderTags] -- Added explicitly here
					FROM 
						ProductSEOEntries
					WHERE 
						Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
		

	SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
					FROM 
					[dbo].[ProductSEO] ' + @WhereClause
									
	EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

	EXEC sp_executesql @SQL1

	RETURN @@ROWCOUNT
END
GO

-------------------------------------------------------------------
-- 10. GetProductSEOByQuery
-------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductSEOByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductSEOByQuery]
GO

CREATE PROCEDURE GetProductSEOByQuery
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

	SET @SQL1 =		'SELECT * FROM 
					[dbo].[ProductSEO] ' + @Query
									
	EXEC sp_executesql @SQL1

	RETURN @@ROWCOUNT
END
GO