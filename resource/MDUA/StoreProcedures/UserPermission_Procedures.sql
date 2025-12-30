USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertUserPermission    Script Date: 12/29/2025 2:00:02 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertUserPermission]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertUserPermission]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertUserPermission
(
	@Id int OUTPUT,
	@UserId int,
	@PermissionId int,
	@PermissionGroupId int
)
AS
    INSERT INTO [dbo].[UserPermission] 
	(
	[UserId],
	[PermissionId],
	[PermissionGroupId]
    ) 
	VALUES 
	(
	@UserId,
	@PermissionId,
	@PermissionGroupId
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

/****** Object:  StoredProcedure [dbo].UpdateUserPermission    Script Date: 12/29/2025 2:00:02 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateUserPermission]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateUserPermission]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateUserPermission
(
	@Id int,
	@UserId int,
	@PermissionId int,
	@PermissionGroupId int
)
AS
    UPDATE [dbo].[UserPermission] 
	SET
	[UserId] = @UserId,
	[PermissionId] = @PermissionId,
	[PermissionGroupId] = @PermissionGroupId
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

/****** Object:  StoredProcedure [dbo].DeleteUserPermission    Script Date: 12/29/2025 2:00:02 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteUserPermission]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteUserPermission]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteUserPermission
(
	@Id int
)
AS
	DELETE [dbo].[UserPermission] 

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

/****** Object:  StoredProcedure [dbo].GetAllUserPermission    Script Date: 12/29/2025 2:00:02 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllUserPermission]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllUserPermission]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllUserPermission
AS
	SELECT *		
	FROM
		[dbo].[UserPermission]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetUserPermissionById    Script Date: 12/29/2025 2:00:02 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUserPermissionById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetUserPermissionById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetUserPermissionById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[UserPermission]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllUserPermissionByUserId    Script Date: 12/29/2025 2:00:02 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUserPermissionByUserId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetUserPermissionByUserId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetUserPermissionByUserId
(
	@UserId int
)
AS
	SELECT *		
	FROM
		[dbo].[UserPermission]
	WHERE ( UserId = @UserId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllUserPermissionByPermissionId    Script Date: 12/29/2025 2:00:02 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUserPermissionByPermissionId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetUserPermissionByPermissionId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetUserPermissionByPermissionId
(
	@PermissionId int
)
AS
	SELECT *		
	FROM
		[dbo].[UserPermission]
	WHERE ( PermissionId = @PermissionId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllUserPermissionByPermissionGroupId    Script Date: 12/29/2025 2:00:02 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUserPermissionByPermissionGroupId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetUserPermissionByPermissionGroupId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetUserPermissionByPermissionGroupId
(
	@PermissionGroupId int
)
AS
	SELECT *		
	FROM
		[dbo].[UserPermission]
	WHERE ( PermissionGroupId = @PermissionGroupId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetUserPermissionMaximumId    Script Date: 12/29/2025 2:00:02 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUserPermissionMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetUserPermissionMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetUserPermissionMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[UserPermission]

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

/****** Object:  StoredProcedure [dbo].GetUserPermissionRowCount    Script Date: 12/29/2025 2:00:02 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUserPermissionRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetUserPermissionRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetUserPermissionRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[UserPermission]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedUserPermission    Script Date: 12/29/2025 2:00:02 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedUserPermission]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedUserPermission]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedUserPermission
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

SET @SQL1 = 'WITH UserPermissionEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[UserId],
	[PermissionId],
	[PermissionGroupId]
				FROM 
				[dbo].[UserPermission]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[UserId],
	[PermissionId],
	[PermissionGroupId]
				FROM 
					UserPermissionEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[UserPermission] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetUserPermissionByQuery    Script Date: 12/29/2025 2:00:02 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUserPermissionByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetUserPermissionByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetUserPermissionByQuery
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
				[dbo].[UserPermission] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

