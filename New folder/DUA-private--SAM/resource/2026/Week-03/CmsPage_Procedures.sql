

/****** Object:  StoredProcedure [dbo]..InsertCmsPage    Script Date: 1/12/2026 11:41:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertCmsPage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertCmsPage]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertCmsPage
(
	@Id int OUTPUT,
	@CompanyId int,
	@Title nvarchar(200),
	@Slug nvarchar(200),
	@ContentHtml nvarchar(max),
	@SidebarContentHtml nvarchar(max),
	@LayoutView nvarchar(100),
	@MetaTitle nvarchar(200),
	@MetaDescription nvarchar(500),
	@CustomCss nvarchar(max),
	@CustomJs nvarchar(max),
	@IsActive bit,
	@Version int,
	@PublishedAt datetime,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    INSERT INTO [dbo].[CmsPage] 
	(
	[CompanyId],
	[Title],
	[Slug],
	[ContentHtml],
	[SidebarContentHtml],
	[LayoutView],
	[MetaTitle],
	[MetaDescription],
	[CustomCss],
	[CustomJs],
	[IsActive],
	[Version],
	[PublishedAt],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
    ) 
	VALUES 
	(
	@CompanyId,
	@Title,
	@Slug,
	@ContentHtml,
	@SidebarContentHtml,
	@LayoutView,
	@MetaTitle,
	@MetaDescription,
	@CustomCss,
	@CustomJs,
	@IsActive,
	@Version,
	@PublishedAt,
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

/****** Object:  StoredProcedure [dbo].UpdateCmsPage    Script Date: 1/12/2026 11:41:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateCmsPage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateCmsPage]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateCmsPage
(
	@Id int,
	@CompanyId int,
	@Title nvarchar(200),
	@Slug nvarchar(200),
	@ContentHtml nvarchar(max),
	@SidebarContentHtml nvarchar(max),
	@LayoutView nvarchar(100),
	@MetaTitle nvarchar(200),
	@MetaDescription nvarchar(500),
	@CustomCss nvarchar(max),
	@CustomJs nvarchar(max),
	@IsActive bit,
	@Version int,
	@PublishedAt datetime,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    UPDATE [dbo].[CmsPage] 
	SET
	[CompanyId] = @CompanyId,
	[Title] = @Title,
	[Slug] = @Slug,
	[ContentHtml] = @ContentHtml,
	[SidebarContentHtml] = @SidebarContentHtml,
	[LayoutView] = @LayoutView,
	[MetaTitle] = @MetaTitle,
	[MetaDescription] = @MetaDescription,
	[CustomCss] = @CustomCss,
	[CustomJs] = @CustomJs,
	[IsActive] = @IsActive,
	[Version] = @Version,
	[PublishedAt] = @PublishedAt,
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

/****** Object:  StoredProcedure [dbo].DeleteCmsPage    Script Date: 1/12/2026 11:41:03 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteCmsPage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteCmsPage]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteCmsPage
(
	@Id int
)
AS
	DELETE [dbo].[CmsPage] 

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

/****** Object:  StoredProcedure [dbo].GetAllCmsPage    Script Date: 1/12/2026 11:41:03 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllCmsPage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllCmsPage]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllCmsPage
AS
	SELECT *		
	FROM
		[dbo].[CmsPage]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetCmsPageById    Script Date: 1/12/2026 11:41:03 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCmsPageById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCmsPageById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCmsPageById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[CmsPage]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetCmsPageMaximumId    Script Date: 1/12/2026 11:41:03 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCmsPageMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCmsPageMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCmsPageMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[CmsPage]

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

/****** Object:  StoredProcedure [dbo].GetCmsPageRowCount    Script Date: 1/12/2026 11:41:03 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCmsPageRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCmsPageRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCmsPageRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[CmsPage]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedCmsPage    Script Date: 1/12/2026 11:41:03 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedCmsPage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedCmsPage]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedCmsPage
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

SET @SQL1 = 'WITH CmsPageEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[CompanyId],
	[Title],
	[Slug],
	[ContentHtml],
	[SidebarContentHtml],
	[LayoutView],
	[MetaTitle],
	[MetaDescription],
	[CustomCss],
	[CustomJs],
	[IsActive],
	[Version],
	[PublishedAt],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
				[dbo].[CmsPage]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[CompanyId],
	[Title],
	[Slug],
	[ContentHtml],
	[SidebarContentHtml],
	[LayoutView],
	[MetaTitle],
	[MetaDescription],
	[CustomCss],
	[CustomJs],
	[IsActive],
	[Version],
	[PublishedAt],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
					CmsPageEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[CmsPage] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetCmsPageByQuery    Script Date: 1/12/2026 11:41:03 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCmsPageByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCmsPageByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCmsPageByQuery
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
				[dbo].[CmsPage] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

