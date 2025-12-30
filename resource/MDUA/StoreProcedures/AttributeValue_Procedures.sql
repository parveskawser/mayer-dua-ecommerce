USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertAttributeValue    Script Date: 12/29/2025 1:59:55 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertAttributeValue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertAttributeValue]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertAttributeValue
(
	@Id int OUTPUT,
	@AttributeId int,
	@Value nvarchar(100),
	@DisplayOrder int
)
AS
    INSERT INTO [dbo].[AttributeValue] 
	(
	[AttributeId],
	[Value],
	[DisplayOrder]
    ) 
	VALUES 
	(
	@AttributeId,
	@Value,
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

/****** Object:  StoredProcedure [dbo].UpdateAttributeValue    Script Date: 12/29/2025 1:59:55 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateAttributeValue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateAttributeValue]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateAttributeValue
(
	@Id int,
	@AttributeId int,
	@Value nvarchar(100),
	@DisplayOrder int
)
AS
    UPDATE [dbo].[AttributeValue] 
	SET
	[AttributeId] = @AttributeId,
	[Value] = @Value,
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

/****** Object:  StoredProcedure [dbo].DeleteAttributeValue    Script Date: 12/29/2025 1:59:55 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteAttributeValue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteAttributeValue]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteAttributeValue
(
	@Id int
)
AS
	DELETE [dbo].[AttributeValue] 

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

/****** Object:  StoredProcedure [dbo].GetAllAttributeValue    Script Date: 12/29/2025 1:59:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllAttributeValue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllAttributeValue]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllAttributeValue
AS
	SELECT *		
	FROM
		[dbo].[AttributeValue]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAttributeValueById    Script Date: 12/29/2025 1:59:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAttributeValueById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAttributeValueById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAttributeValueById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[AttributeValue]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllAttributeValueByAttributeId    Script Date: 12/29/2025 1:59:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAttributeValueByAttributeId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAttributeValueByAttributeId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAttributeValueByAttributeId
(
	@AttributeId int
)
AS
	SELECT *		
	FROM
		[dbo].[AttributeValue]
	WHERE ( AttributeId = @AttributeId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAttributeValueMaximumId    Script Date: 12/29/2025 1:59:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAttributeValueMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAttributeValueMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAttributeValueMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[AttributeValue]

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

/****** Object:  StoredProcedure [dbo].GetAttributeValueRowCount    Script Date: 12/29/2025 1:59:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAttributeValueRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAttributeValueRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAttributeValueRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[AttributeValue]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedAttributeValue    Script Date: 12/29/2025 1:59:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedAttributeValue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedAttributeValue]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedAttributeValue
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

SET @SQL1 = 'WITH AttributeValueEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[AttributeId],
	[Value],
	[DisplayOrder]
				FROM 
				[dbo].[AttributeValue]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[AttributeId],
	[Value],
	[DisplayOrder]
				FROM 
					AttributeValueEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[AttributeValue] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetAttributeValueByQuery    Script Date: 12/29/2025 1:59:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAttributeValueByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAttributeValueByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAttributeValueByQuery
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
				[dbo].[AttributeValue] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

