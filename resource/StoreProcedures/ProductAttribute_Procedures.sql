USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertProductAttribute    Script Date: 12/10/2025 2:32:52 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertProductAttribute]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertProductAttribute]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertProductAttribute
(
	@Id int OUTPUT,
	@ProductId int,
	@AttributeId int,
	@DisplayOrder int
)
AS
    INSERT INTO [dbo].[ProductAttribute] 
	(
	[ProductId],
	[AttributeId],
	[DisplayOrder]
    ) 
	VALUES 
	(
	@ProductId,
	@AttributeId,
	@DisplayOrder
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

/****** Object:  StoredProcedure [dbo].UpdateProductAttribute    Script Date: 12/10/2025 2:32:52 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateProductAttribute]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateProductAttribute]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateProductAttribute
(
	@Id int,
	@ProductId int,
	@AttributeId int,
	@DisplayOrder int
)
AS
    UPDATE [dbo].[ProductAttribute] 
	SET
	[ProductId] = @ProductId,
	[AttributeId] = @AttributeId,
	[DisplayOrder] = @DisplayOrder
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

/****** Object:  StoredProcedure [dbo].DeleteProductAttribute    Script Date: 12/10/2025 2:32:52 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteProductAttribute]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteProductAttribute]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteProductAttribute
(
	@Id int
)
AS
	DELETE [dbo].[ProductAttribute] 

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

/****** Object:  StoredProcedure [dbo].GetAllProductAttribute    Script Date: 12/10/2025 2:32:52 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllProductAttribute]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllProductAttribute]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllProductAttribute
AS
	SELECT *		
	FROM
		[dbo].[ProductAttribute]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetProductAttributeById    Script Date: 12/10/2025 2:32:52 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductAttributeById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductAttributeById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductAttributeById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[ProductAttribute]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllProductAttributeByProductId    Script Date: 12/10/2025 2:32:52 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductAttributeByProductId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductAttributeByProductId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductAttributeByProductId
(
	@ProductId int
)
AS
	SELECT *		
	FROM
		[dbo].[ProductAttribute]
	WHERE ( ProductId = @ProductId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllProductAttributeByAttributeId    Script Date: 12/10/2025 2:32:52 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductAttributeByAttributeId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductAttributeByAttributeId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductAttributeByAttributeId
(
	@AttributeId int
)
AS
	SELECT *		
	FROM
		[dbo].[ProductAttribute]
	WHERE ( AttributeId = @AttributeId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetProductAttributeMaximumId    Script Date: 12/10/2025 2:32:52 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductAttributeMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductAttributeMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductAttributeMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[ProductAttribute]

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

/****** Object:  StoredProcedure [dbo].GetProductAttributeRowCount    Script Date: 12/10/2025 2:32:52 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductAttributeRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductAttributeRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductAttributeRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[ProductAttribute]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedProductAttribute    Script Date: 12/10/2025 2:32:52 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedProductAttribute]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedProductAttribute]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedProductAttribute
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

SET @SQL1 = 'WITH ProductAttributeEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[ProductId],
	[AttributeId],
	[DisplayOrder]
				FROM 
				[dbo].[ProductAttribute]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[ProductId],
	[AttributeId],
	[DisplayOrder]
				FROM 
					ProductAttributeEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[ProductAttribute] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetProductAttributeByQuery    Script Date: 12/10/2025 2:32:52 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProductAttributeByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProductAttributeByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetProductAttributeByQuery
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
				[dbo].[ProductAttribute] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

