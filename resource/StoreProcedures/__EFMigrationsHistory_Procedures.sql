USE AA4
GO

/****** Object:  StoredProcedure [dbo]..Insert__EFMigrationsHistory    Script Date: 11/25/2025 1:36:24 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Insert__EFMigrationsHistory]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Insert__EFMigrationsHistory]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE Insert__EFMigrationsHistory
(
	@MigrationId nvarchar,
	@ProductVersion nvarchar(32)
)
AS
    INSERT INTO [dbo].[__EFMigrationsHistory] 
	(
	[MigrationId],
	[ProductVersion]
    ) 
	VALUES 
	(
	@MigrationId,
	@ProductVersion
    )
	DECLARE @Err int
	DECLARE @Result int

	SET @Result = @@ROWCOUNT
	SET @Err = @@ERROR 
	If @Err <> 0 
	BEGIN
		RETURN @Err
	END
	ELSE
	BEGIN
		If @Result = 1 
		BEGIN
			-- Everything is OK
			RETURN @Result
		END
		ELSE
		BEGIN
			RETURN 0
		END
	END

	RETURN @Result
	
GO

/****** Object:  StoredProcedure [dbo].Update__EFMigrationsHistory    Script Date: 11/25/2025 1:36:24 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Update__EFMigrationsHistory]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Update__EFMigrationsHistory]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE Update__EFMigrationsHistory
(
	@MigrationId nvarchar(150),
	@ProductVersion nvarchar(32)
)
AS
    UPDATE [dbo].[__EFMigrationsHistory] 
	SET
	[ProductVersion] = @ProductVersion
	WHERE ( MigrationId = @MigrationId )

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

/****** Object:  StoredProcedure [dbo].Delete__EFMigrationsHistory    Script Date: 11/25/2025 1:36:24 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Delete__EFMigrationsHistory]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Delete__EFMigrationsHistory]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE Delete__EFMigrationsHistory
(
	@MigrationId nvarchar
)
AS
	DELETE [dbo].[__EFMigrationsHistory] 

    WHERE ( MigrationId = @MigrationId )

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

/****** Object:  StoredProcedure [dbo].GetAll__EFMigrationsHistory    Script Date: 11/25/2025 1:36:24 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAll__EFMigrationsHistory]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAll__EFMigrationsHistory]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAll__EFMigrationsHistory
AS
	SELECT *		
	FROM
		[dbo].[__EFMigrationsHistory]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].Get__EFMigrationsHistoryByMigrationId    Script Date: 11/25/2025 1:36:24 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Get__EFMigrationsHistoryByMigrationId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Get__EFMigrationsHistoryByMigrationId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE Get__EFMigrationsHistoryByMigrationId
(
	@MigrationId nvarchar
)
AS
	SELECT *		
	FROM
		[dbo].[__EFMigrationsHistory]
	WHERE ( MigrationId = @MigrationId )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].Get__EFMigrationsHistoryMaximumMigrationId    Script Date: 11/25/2025 1:36:24 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Get__EFMigrationsHistoryMaximumMigrationId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Get__EFMigrationsHistoryMaximumMigrationId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE Get__EFMigrationsHistoryMaximumMigrationId
AS
	DECLARE @Result nvarchar
	SET @Result = 0
	
	SELECT @Result = MAX(MigrationId) 		
	FROM
		[dbo].[__EFMigrationsHistory]

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

/****** Object:  StoredProcedure [dbo].Get__EFMigrationsHistoryRowCount    Script Date: 11/25/2025 1:36:24 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Get__EFMigrationsHistoryRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Get__EFMigrationsHistoryRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE Get__EFMigrationsHistoryRowCount
AS
	DECLARE @Result nvarchar
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[__EFMigrationsHistory]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPaged__EFMigrationsHistory    Script Date: 11/25/2025 1:36:24 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPaged__EFMigrationsHistory]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPaged__EFMigrationsHistory]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPaged__EFMigrationsHistory
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
	SET @SortColumn = @SortColumn + ' ORDER BY [MigrationId] ASC'
END

SET @SQL1 = 'WITH __EFMigrationsHistoryEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[MigrationId],
	[ProductVersion]
				FROM 
				[dbo].[__EFMigrationsHistory]
					'+ @WhereClause +'
				)
				SELECT 
	[MigrationId],
	[ProductVersion]
				FROM 
					__EFMigrationsHistoryEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[__EFMigrationsHistory] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].Get__EFMigrationsHistoryByQuery    Script Date: 11/25/2025 1:36:24 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Get__EFMigrationsHistoryByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Get__EFMigrationsHistoryByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE Get__EFMigrationsHistoryByQuery
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
				[dbo].[__EFMigrationsHistory] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

