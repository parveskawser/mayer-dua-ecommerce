USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertGlobalSetting    Script Date: 12/21/2025 8:58:37 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertGlobalSetting]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertGlobalSetting]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertGlobalSetting
(
	@Id int,
	@CompanyId int,
	@GKey nvarchar(50),
	@GContent nvarchar(max)
)
AS
    INSERT INTO [dbo].[GlobalSetting] 
	(
	[Id],
	[CompanyId],
	[GKey],
	[GContent]
    ) 
	VALUES 
	(
	@Id,
	@CompanyId,
	@GKey,
	@GContent
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

/****** Object:  StoredProcedure [dbo].UpdateGlobalSetting    Script Date: 12/21/2025 8:58:37 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateGlobalSetting]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateGlobalSetting]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateGlobalSetting
(
	@Id int,
	@CompanyId int,
	@GKey nvarchar(50),
	@GContent nvarchar(max)
)
AS
    UPDATE [dbo].[GlobalSetting] 
	SET
	[CompanyId] = @CompanyId,
	[GKey] = @GKey,
	[GContent] = @GContent
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

/****** Object:  StoredProcedure [dbo].DeleteGlobalSetting    Script Date: 12/21/2025 8:58:37 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteGlobalSetting]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteGlobalSetting]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteGlobalSetting
(
	@Id int
)
AS
	DELETE [dbo].[GlobalSetting] 

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

/****** Object:  StoredProcedure [dbo].GetAllGlobalSetting    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllGlobalSetting]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllGlobalSetting]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllGlobalSetting
AS
	SELECT *		
	FROM
		[dbo].[GlobalSetting]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetGlobalSettingById    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetGlobalSettingById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetGlobalSettingById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetGlobalSettingById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[GlobalSetting]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetGlobalSettingMaximumId    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetGlobalSettingMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetGlobalSettingMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetGlobalSettingMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[GlobalSetting]

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

/****** Object:  StoredProcedure [dbo].GetGlobalSettingRowCount    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetGlobalSettingRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetGlobalSettingRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetGlobalSettingRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[GlobalSetting]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedGlobalSetting    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedGlobalSetting]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedGlobalSetting]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedGlobalSetting
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

SET @SQL1 = 'WITH GlobalSettingEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[CompanyId],
	[GKey],
	[GContent]
				FROM 
				[dbo].[GlobalSetting]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[CompanyId],
	[GKey],
	[GContent]
				FROM 
					GlobalSettingEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[GlobalSetting] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetGlobalSettingByQuery    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetGlobalSettingByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetGlobalSettingByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetGlobalSettingByQuery
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
				[dbo].[GlobalSetting] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

