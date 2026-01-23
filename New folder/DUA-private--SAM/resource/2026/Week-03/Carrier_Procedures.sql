

/****** Object:  StoredProcedure [dbo]..InsertCarrier    Script Date: 1/14/2026 4:18:09 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertCarrier]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertCarrier]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertCarrier
(
	@Id int OUTPUT,
	@CarrierName nvarchar(100),
	@ApiEndpoint nvarchar(500),
	@RequiresApi bit,
	@IsActive bit
)
AS
    INSERT INTO [dbo].[Carrier] 
	(
	[CarrierName],
	[ApiEndpoint],
	[RequiresApi],
	[IsActive]
    ) 
	VALUES 
	(
	@CarrierName,
	@ApiEndpoint,
	@RequiresApi,
	@IsActive
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

/****** Object:  StoredProcedure [dbo].UpdateCarrier    Script Date: 1/14/2026 4:18:09 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateCarrier]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateCarrier]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateCarrier
(
	@Id int,
	@CarrierName nvarchar(100),
	@ApiEndpoint nvarchar(500),
	@RequiresApi bit,
	@IsActive bit
)
AS
    UPDATE [dbo].[Carrier] 
	SET
	[CarrierName] = @CarrierName,
	[ApiEndpoint] = @ApiEndpoint,
	[RequiresApi] = @RequiresApi,
	[IsActive] = @IsActive
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

/****** Object:  StoredProcedure [dbo].DeleteCarrier    Script Date: 1/14/2026 4:18:09 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteCarrier]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteCarrier]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteCarrier
(
	@Id int
)
AS
	DELETE [dbo].[Carrier] 

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

/****** Object:  StoredProcedure [dbo].GetAllCarrier    Script Date: 1/14/2026 4:18:09 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllCarrier]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllCarrier]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllCarrier
AS
	SELECT *		
	FROM
		[dbo].[Carrier]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetCarrierById    Script Date: 1/14/2026 4:18:09 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCarrierById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCarrierById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCarrierById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[Carrier]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetCarrierMaximumId    Script Date: 1/14/2026 4:18:09 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCarrierMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCarrierMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCarrierMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[Carrier]

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

/****** Object:  StoredProcedure [dbo].GetCarrierRowCount    Script Date: 1/14/2026 4:18:09 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCarrierRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCarrierRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCarrierRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[Carrier]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedCarrier    Script Date: 1/14/2026 4:18:09 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedCarrier]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedCarrier]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedCarrier
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

SET @SQL1 = 'WITH CarrierEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[CarrierName],
	[ApiEndpoint],
	[RequiresApi],
	[IsActive]
				FROM 
				[dbo].[Carrier]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[CarrierName],
	[ApiEndpoint],
	[RequiresApi],
	[IsActive]
				FROM 
					CarrierEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[Carrier] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetCarrierByQuery    Script Date: 1/14/2026 4:18:09 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCarrierByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCarrierByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCarrierByQuery
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
				[dbo].[Carrier] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

