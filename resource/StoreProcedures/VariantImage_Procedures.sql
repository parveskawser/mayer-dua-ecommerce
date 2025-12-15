USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertVariantImage    Script Date: 12/10/2025 2:32:54 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertVariantImage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertVariantImage]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertVariantImage
(
	@Id int OUTPUT,
	@VariantId int,
	@ImageUrl nvarchar(400),
	@AltText nvarchar(200),
	@DisplayOrder int
)
AS
    INSERT INTO [dbo].[VariantImage] 
	(
	[VariantId],
	[ImageUrl],
	[AltText],
	[DisplayOrder]
    ) 
	VALUES 
	(
	@VariantId,
	@ImageUrl,
	@AltText,
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

/****** Object:  StoredProcedure [dbo].UpdateVariantImage    Script Date: 12/10/2025 2:32:54 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateVariantImage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateVariantImage]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateVariantImage
(
	@Id int,
	@VariantId int,
	@ImageUrl nvarchar(400),
	@AltText nvarchar(200),
	@DisplayOrder int
)
AS
    UPDATE [dbo].[VariantImage] 
	SET
	[VariantId] = @VariantId,
	[ImageUrl] = @ImageUrl,
	[AltText] = @AltText,
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

/****** Object:  StoredProcedure [dbo].DeleteVariantImage    Script Date: 12/10/2025 2:32:54 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteVariantImage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteVariantImage]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteVariantImage
(
	@Id int
)
AS
	DELETE [dbo].[VariantImage] 

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

/****** Object:  StoredProcedure [dbo].GetAllVariantImage    Script Date: 12/10/2025 2:32:54 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllVariantImage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllVariantImage]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllVariantImage
AS
	SELECT *		
	FROM
		[dbo].[VariantImage]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetVariantImageById    Script Date: 12/10/2025 2:32:54 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVariantImageById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVariantImageById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVariantImageById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[VariantImage]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllVariantImageByVariantId    Script Date: 12/10/2025 2:32:54 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVariantImageByVariantId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVariantImageByVariantId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVariantImageByVariantId
(
	@VariantId int
)
AS
	SELECT *		
	FROM
		[dbo].[VariantImage]
	WHERE ( VariantId = @VariantId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetVariantImageMaximumId    Script Date: 12/10/2025 2:32:54 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVariantImageMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVariantImageMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVariantImageMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[VariantImage]

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

/****** Object:  StoredProcedure [dbo].GetVariantImageRowCount    Script Date: 12/10/2025 2:32:54 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVariantImageRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVariantImageRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVariantImageRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[VariantImage]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedVariantImage    Script Date: 12/10/2025 2:32:54 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedVariantImage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedVariantImage]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedVariantImage
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

SET @SQL1 = 'WITH VariantImageEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[VariantId],
	[ImageUrl],
	[AltText],
	[DisplayOrder]
				FROM 
				[dbo].[VariantImage]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[VariantId],
	[ImageUrl],
	[AltText],
	[DisplayOrder]
				FROM 
					VariantImageEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[VariantImage] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetVariantImageByQuery    Script Date: 12/10/2025 2:32:54 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVariantImageByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVariantImageByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVariantImageByQuery
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
				[dbo].[VariantImage] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

