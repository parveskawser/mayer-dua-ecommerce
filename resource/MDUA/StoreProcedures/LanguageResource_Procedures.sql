USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertLanguageResource    Script Date: 12/29/2025 1:59:58 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertLanguageResource]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertLanguageResource]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertLanguageResource
(
	@Id int OUTPUT,
	@CompanyId int,
	@LanguageId int,
	@LKey nvarchar(255),
	@LValue nvarchar(max),
	@Description nvarchar(255),
	@IsActive bit,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    INSERT INTO [dbo].[LanguageResource] 
	(
	[CompanyId],
	[LanguageId],
	[LKey],
	[LValue],
	[Description],
	[IsActive],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
    ) 
	VALUES 
	(
	@CompanyId,
	@LanguageId,
	@LKey,
	@LValue,
	@Description,
	@IsActive,
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

/****** Object:  StoredProcedure [dbo].UpdateLanguageResource    Script Date: 12/29/2025 1:59:58 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateLanguageResource]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateLanguageResource]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateLanguageResource
(
	@Id int,
	@CompanyId int,
	@LanguageId int,
	@LKey nvarchar(255),
	@LValue nvarchar(max),
	@Description nvarchar(255),
	@IsActive bit,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    UPDATE [dbo].[LanguageResource] 
	SET
	[CompanyId] = @CompanyId,
	[LanguageId] = @LanguageId,
	[LKey] = @LKey,
	[LValue] = @LValue,
	[Description] = @Description,
	[IsActive] = @IsActive,
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

/****** Object:  StoredProcedure [dbo].DeleteLanguageResource    Script Date: 12/29/2025 1:59:58 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteLanguageResource]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteLanguageResource]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteLanguageResource
(
	@Id int
)
AS
	DELETE [dbo].[LanguageResource] 

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

/****** Object:  StoredProcedure [dbo].GetAllLanguageResource    Script Date: 12/29/2025 1:59:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllLanguageResource]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllLanguageResource]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllLanguageResource
AS
	SELECT *		
	FROM
		[dbo].[LanguageResource]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetLanguageResourceById    Script Date: 12/29/2025 1:59:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetLanguageResourceById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetLanguageResourceById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetLanguageResourceById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[LanguageResource]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllLanguageResourceByCompanyId    Script Date: 12/29/2025 1:59:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetLanguageResourceByCompanyId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetLanguageResourceByCompanyId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetLanguageResourceByCompanyId
(
	@CompanyId int
)
AS
	SELECT *		
	FROM
		[dbo].[LanguageResource]
	WHERE ( CompanyId = @CompanyId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllLanguageResourceByLanguageId    Script Date: 12/29/2025 1:59:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetLanguageResourceByLanguageId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetLanguageResourceByLanguageId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetLanguageResourceByLanguageId
(
	@LanguageId int
)
AS
	SELECT *		
	FROM
		[dbo].[LanguageResource]
	WHERE ( LanguageId = @LanguageId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetLanguageResourceMaximumId    Script Date: 12/29/2025 1:59:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetLanguageResourceMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetLanguageResourceMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetLanguageResourceMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[LanguageResource]

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

/****** Object:  StoredProcedure [dbo].GetLanguageResourceRowCount    Script Date: 12/29/2025 1:59:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetLanguageResourceRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetLanguageResourceRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetLanguageResourceRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[LanguageResource]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedLanguageResource    Script Date: 12/29/2025 1:59:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedLanguageResource]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedLanguageResource]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedLanguageResource
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

SET @SQL1 = 'WITH LanguageResourceEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[CompanyId],
	[LanguageId],
	[LKey],
	[LValue],
	[Description],
	[IsActive],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
				[dbo].[LanguageResource]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[CompanyId],
	[LanguageId],
	[LKey],
	[LValue],
	[Description],
	[IsActive],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
					LanguageResourceEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[LanguageResource] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetLanguageResourceByQuery    Script Date: 12/29/2025 1:59:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetLanguageResourceByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetLanguageResourceByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetLanguageResourceByQuery
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
				[dbo].[LanguageResource] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

