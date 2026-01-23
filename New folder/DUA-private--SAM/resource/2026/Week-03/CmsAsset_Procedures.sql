

/****** Object:  StoredProcedure [dbo]..InsertCmsAsset    Script Date: 1/12/2026 11:41:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertCmsAsset]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertCmsAsset]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertCmsAsset
(
	@Id int OUTPUT,
	@CompanyId int,
	@PageId int,
	@FileName nvarchar(255),
	@FilePath nvarchar(500),
	@FileType nvarchar(20),
	@Version int,
	@IsActive bit,
	@CreatedAt datetime
)
AS
    INSERT INTO [dbo].[CmsAsset] 
	(
	[CompanyId],
	[PageId],
	[FileName],
	[FilePath],
	[FileType],
	[Version],
	[IsActive],
	[CreatedAt]
    ) 
	VALUES 
	(
	@CompanyId,
	@PageId,
	@FileName,
	@FilePath,
	@FileType,
	@Version,
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

/****** Object:  StoredProcedure [dbo].UpdateCmsAsset    Script Date: 1/12/2026 11:41:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateCmsAsset]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateCmsAsset]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateCmsAsset
(
	@Id int,
	@CompanyId int,
	@PageId int,
	@FileName nvarchar(255),
	@FilePath nvarchar(500),
	@FileType nvarchar(20),
	@Version int,
	@IsActive bit,
	@CreatedAt datetime
)
AS
    UPDATE [dbo].[CmsAsset] 
	SET
	[CompanyId] = @CompanyId,
	[PageId] = @PageId,
	[FileName] = @FileName,
	[FilePath] = @FilePath,
	[FileType] = @FileType,
	[Version] = @Version,
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

/****** Object:  StoredProcedure [dbo].DeleteCmsAsset    Script Date: 1/12/2026 11:41:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteCmsAsset]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteCmsAsset]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteCmsAsset
(
	@Id int
)
AS
	DELETE [dbo].[CmsAsset] 

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

/****** Object:  StoredProcedure [dbo].GetAllCmsAsset    Script Date: 1/12/2026 11:41:03 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllCmsAsset]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllCmsAsset]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllCmsAsset
AS
	SELECT *		
	FROM
		[dbo].[CmsAsset]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetCmsAssetById    Script Date: 1/12/2026 11:41:03 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCmsAssetById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCmsAssetById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCmsAssetById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[CmsAsset]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllCmsAssetByPageId    Script Date: 1/12/2026 11:41:03 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCmsAssetByPageId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCmsAssetByPageId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCmsAssetByPageId
(
	@PageId int
)
AS
	SELECT *		
	FROM
		[dbo].[CmsAsset]
	WHERE ( PageId = @PageId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetCmsAssetMaximumId    Script Date: 1/12/2026 11:41:03 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCmsAssetMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCmsAssetMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCmsAssetMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[CmsAsset]

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

/****** Object:  StoredProcedure [dbo].GetCmsAssetRowCount    Script Date: 1/12/2026 11:41:03 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCmsAssetRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCmsAssetRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCmsAssetRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[CmsAsset]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedCmsAsset    Script Date: 1/12/2026 11:41:03 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedCmsAsset]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedCmsAsset]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedCmsAsset
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

SET @SQL1 = 'WITH CmsAssetEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[CompanyId],
	[PageId],
	[FileName],
	[FilePath],
	[FileType],
	[Version],
	[IsActive],
	[CreatedAt]
				FROM 
				[dbo].[CmsAsset]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[CompanyId],
	[PageId],
	[FileName],
	[FilePath],
	[FileType],
	[Version],
	[IsActive],
	[CreatedAt]
				FROM 
					CmsAssetEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[CmsAsset] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetCmsAssetByQuery    Script Date: 1/12/2026 11:41:03 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCmsAssetByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCmsAssetByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCmsAssetByQuery
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
				[dbo].[CmsAsset] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

