USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertUserSession    Script Date: 12/2/2025 4:44:58 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertUserSession]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertUserSession]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertUserSession
(
	@Id int OUTPUT,
	@SessionKey uniqueidentifier,
	@UserId int,
	@IPAddress nvarchar(50),
	@DeviceInfo nvarchar(200),
	@CreatedAt datetime,
	@LastActiveAt datetime,
	@IsActive bit
)
AS
    INSERT INTO [dbo].[UserSession] 
	(
	[SessionKey],
	[UserId],
	[IPAddress],
	[DeviceInfo],
	[CreatedAt],
	[LastActiveAt],
	[IsActive]
    ) 
	VALUES 
	(
	@SessionKey,
	@UserId,
	@IPAddress,
	@DeviceInfo,
	@CreatedAt,
	@LastActiveAt,
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

/****** Object:  StoredProcedure [dbo].UpdateUserSession    Script Date: 12/2/2025 4:44:58 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateUserSession]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateUserSession]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateUserSession
(
	@Id int,
	@SessionKey uniqueidentifier,
	@UserId int,
	@IPAddress nvarchar(50),
	@DeviceInfo nvarchar(200),
	@CreatedAt datetime,
	@LastActiveAt datetime,
	@IsActive bit
)
AS
    UPDATE [dbo].[UserSession] 
	SET
	[SessionKey] = @SessionKey,
	[UserId] = @UserId,
	[IPAddress] = @IPAddress,
	[DeviceInfo] = @DeviceInfo,
	[CreatedAt] = @CreatedAt,
	[LastActiveAt] = @LastActiveAt,
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

/****** Object:  StoredProcedure [dbo].DeleteUserSession    Script Date: 12/2/2025 4:44:58 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteUserSession]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteUserSession]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteUserSession
(
	@Id int
)
AS
	DELETE [dbo].[UserSession] 

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

/****** Object:  StoredProcedure [dbo].GetAllUserSession    Script Date: 12/2/2025 4:44:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllUserSession]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllUserSession]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllUserSession
AS
	SELECT *		
	FROM
		[dbo].[UserSession]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetUserSessionById    Script Date: 12/2/2025 4:44:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUserSessionById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetUserSessionById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetUserSessionById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[UserSession]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllUserSessionByUserId    Script Date: 12/2/2025 4:44:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUserSessionByUserId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetUserSessionByUserId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetUserSessionByUserId
(
	@UserId int
)
AS
	SELECT *		
	FROM
		[dbo].[UserSession]
	WHERE ( UserId = @UserId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetUserSessionMaximumId    Script Date: 12/2/2025 4:44:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUserSessionMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetUserSessionMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetUserSessionMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[UserSession]

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

/****** Object:  StoredProcedure [dbo].GetUserSessionRowCount    Script Date: 12/2/2025 4:44:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUserSessionRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetUserSessionRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetUserSessionRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[UserSession]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedUserSession    Script Date: 12/2/2025 4:44:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedUserSession]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedUserSession]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedUserSession
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

SET @SQL1 = 'WITH UserSessionEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[SessionKey],
	[UserId],
	[IPAddress],
	[DeviceInfo],
	[CreatedAt],
	[LastActiveAt],
	[IsActive]
				FROM 
				[dbo].[UserSession]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[SessionKey],
	[UserId],
	[IPAddress],
	[DeviceInfo],
	[CreatedAt],
	[LastActiveAt],
	[IsActive]
				FROM 
					UserSessionEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[UserSession] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetUserSessionByQuery    Script Date: 12/2/2025 4:44:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUserSessionByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetUserSessionByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetUserSessionByQuery
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
				[dbo].[UserSession] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

