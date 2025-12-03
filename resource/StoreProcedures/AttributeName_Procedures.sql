USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertAttributeName    Script Date: 12/2/2025 4:44:55 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertAttributeName]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertAttributeName]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertAttributeName
(
	@Id int OUTPUT,
	@Name nvarchar(100),
	@DisplayOrder int,
	@IsVariantAffecting bit
)
AS
    INSERT INTO [dbo].[AttributeName] 
	(
	[Name],
	[DisplayOrder],
	[IsVariantAffecting]
    ) 
	VALUES 
	(
	@Name,
	@DisplayOrder,
	@IsVariantAffecting
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

/****** Object:  StoredProcedure [dbo].UpdateAttributeName    Script Date: 12/2/2025 4:44:55 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateAttributeName]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateAttributeName]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateAttributeName
(
	@Id int,
	@Name nvarchar(100),
	@DisplayOrder int,
	@IsVariantAffecting bit
)
AS
    UPDATE [dbo].[AttributeName] 
	SET
	[Name] = @Name,
	[DisplayOrder] = @DisplayOrder,
	[IsVariantAffecting] = @IsVariantAffecting
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

/****** Object:  StoredProcedure [dbo].DeleteAttributeName    Script Date: 12/2/2025 4:44:55 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteAttributeName]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteAttributeName]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteAttributeName
(
	@Id int
)
AS
	DELETE [dbo].[AttributeName] 

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

/****** Object:  StoredProcedure [dbo].GetAllAttributeName    Script Date: 12/2/2025 4:44:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllAttributeName]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllAttributeName]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllAttributeName
AS
	SELECT *		
	FROM
		[dbo].[AttributeName]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAttributeNameById    Script Date: 12/2/2025 4:44:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAttributeNameById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAttributeNameById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAttributeNameById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[AttributeName]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAttributeNameMaximumId    Script Date: 12/2/2025 4:44:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAttributeNameMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAttributeNameMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAttributeNameMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[AttributeName]

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

/****** Object:  StoredProcedure [dbo].GetAttributeNameRowCount    Script Date: 12/2/2025 4:44:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAttributeNameRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAttributeNameRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAttributeNameRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[AttributeName]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedAttributeName    Script Date: 12/2/2025 4:44:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedAttributeName]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedAttributeName]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedAttributeName
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

SET @SQL1 = 'WITH AttributeNameEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[Name],
	[DisplayOrder],
	[IsVariantAffecting]
				FROM 
				[dbo].[AttributeName]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[Name],
	[DisplayOrder],
	[IsVariantAffecting]
				FROM 
					AttributeNameEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[AttributeName] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetAttributeNameByQuery    Script Date: 12/2/2025 4:44:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAttributeNameByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAttributeNameByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAttributeNameByQuery
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
				[dbo].[AttributeName] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

