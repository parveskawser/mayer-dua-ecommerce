USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertVariantAttributeValue    Script Date: 12/21/2025 8:58:40 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertVariantAttributeValue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertVariantAttributeValue]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertVariantAttributeValue
(
	@Id int OUTPUT,
	@VariantId int,
	@AttributeId int,
	@AttributeValueId int,
	@DisplayOrder int
)
AS
    INSERT INTO [dbo].[VariantAttributeValue] 
	(
	[VariantId],
	[AttributeId],
	[AttributeValueId],
	[DisplayOrder]
    ) 
	VALUES 
	(
	@VariantId,
	@AttributeId,
	@AttributeValueId,
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

/****** Object:  StoredProcedure [dbo].UpdateVariantAttributeValue    Script Date: 12/21/2025 8:58:40 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateVariantAttributeValue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateVariantAttributeValue]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateVariantAttributeValue
(
	@Id int,
	@VariantId int,
	@AttributeId int,
	@AttributeValueId int,
	@DisplayOrder int
)
AS
    UPDATE [dbo].[VariantAttributeValue] 
	SET
	[VariantId] = @VariantId,
	[AttributeId] = @AttributeId,
	[AttributeValueId] = @AttributeValueId,
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

/****** Object:  StoredProcedure [dbo].DeleteVariantAttributeValue    Script Date: 12/21/2025 8:58:40 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteVariantAttributeValue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteVariantAttributeValue]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteVariantAttributeValue
(
	@Id int
)
AS
	DELETE [dbo].[VariantAttributeValue] 

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

/****** Object:  StoredProcedure [dbo].GetAllVariantAttributeValue    Script Date: 12/21/2025 8:58:40 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllVariantAttributeValue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllVariantAttributeValue]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllVariantAttributeValue
AS
	SELECT *		
	FROM
		[dbo].[VariantAttributeValue]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetVariantAttributeValueById    Script Date: 12/21/2025 8:58:40 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVariantAttributeValueById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVariantAttributeValueById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVariantAttributeValueById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[VariantAttributeValue]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllVariantAttributeValueByVariantId    Script Date: 12/21/2025 8:58:40 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVariantAttributeValueByVariantId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVariantAttributeValueByVariantId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVariantAttributeValueByVariantId
(
	@VariantId int
)
AS
	SELECT *		
	FROM
		[dbo].[VariantAttributeValue]
	WHERE ( VariantId = @VariantId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllVariantAttributeValueByAttributeId    Script Date: 12/21/2025 8:58:40 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVariantAttributeValueByAttributeId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVariantAttributeValueByAttributeId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVariantAttributeValueByAttributeId
(
	@AttributeId int
)
AS
	SELECT *		
	FROM
		[dbo].[VariantAttributeValue]
	WHERE ( AttributeId = @AttributeId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllVariantAttributeValueByAttributeValueId    Script Date: 12/21/2025 8:58:40 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVariantAttributeValueByAttributeValueId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVariantAttributeValueByAttributeValueId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVariantAttributeValueByAttributeValueId
(
	@AttributeValueId int
)
AS
	SELECT *		
	FROM
		[dbo].[VariantAttributeValue]
	WHERE ( AttributeValueId = @AttributeValueId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetVariantAttributeValueMaximumId    Script Date: 12/21/2025 8:58:40 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVariantAttributeValueMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVariantAttributeValueMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVariantAttributeValueMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[VariantAttributeValue]

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

/****** Object:  StoredProcedure [dbo].GetVariantAttributeValueRowCount    Script Date: 12/21/2025 8:58:40 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVariantAttributeValueRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVariantAttributeValueRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVariantAttributeValueRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[VariantAttributeValue]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedVariantAttributeValue    Script Date: 12/21/2025 8:58:40 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedVariantAttributeValue]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedVariantAttributeValue]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedVariantAttributeValue
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

SET @SQL1 = 'WITH VariantAttributeValueEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[VariantId],
	[AttributeId],
	[AttributeValueId],
	[DisplayOrder]
				FROM 
				[dbo].[VariantAttributeValue]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[VariantId],
	[AttributeId],
	[AttributeValueId],
	[DisplayOrder]
				FROM 
					VariantAttributeValueEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[VariantAttributeValue] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetVariantAttributeValueByQuery    Script Date: 12/21/2025 8:58:40 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetVariantAttributeValueByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetVariantAttributeValueByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetVariantAttributeValueByQuery
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
				[dbo].[VariantAttributeValue] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

