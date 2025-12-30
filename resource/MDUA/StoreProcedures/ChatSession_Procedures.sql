USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertChatSession    Script Date: 12/29/2025 1:59:56 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertChatSession]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertChatSession]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertChatSession
(
	@Id int OUTPUT,
	@SessionGuid uniqueidentifier,
	@UserLoginId int,
	@GuestName nvarchar(100),
	@Status nvarchar(20),
	@StartedAt datetime,
	@LastMessageAt datetime,
	@IsActive bit
)
AS
    INSERT INTO [dbo].[ChatSession] 
	(
	[SessionGuid],
	[UserLoginId],
	[GuestName],
	[Status],
	[StartedAt],
	[LastMessageAt],
	[IsActive]
    ) 
	VALUES 
	(
	@SessionGuid,
	@UserLoginId,
	@GuestName,
	@Status,
	@StartedAt,
	@LastMessageAt,
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

/****** Object:  StoredProcedure [dbo].UpdateChatSession    Script Date: 12/29/2025 1:59:56 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateChatSession]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateChatSession]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateChatSession
(
	@Id int,
	@SessionGuid uniqueidentifier,
	@UserLoginId int,
	@GuestName nvarchar(100),
	@Status nvarchar(20),
	@StartedAt datetime,
	@LastMessageAt datetime,
	@IsActive bit
)
AS
    UPDATE [dbo].[ChatSession] 
	SET
	[SessionGuid] = @SessionGuid,
	[UserLoginId] = @UserLoginId,
	[GuestName] = @GuestName,
	[Status] = @Status,
	[StartedAt] = @StartedAt,
	[LastMessageAt] = @LastMessageAt,
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

/****** Object:  StoredProcedure [dbo].DeleteChatSession    Script Date: 12/29/2025 1:59:56 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteChatSession]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteChatSession]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteChatSession
(
	@Id int
)
AS
	DELETE [dbo].[ChatSession] 

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

/****** Object:  StoredProcedure [dbo].GetAllChatSession    Script Date: 12/29/2025 1:59:56 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllChatSession]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllChatSession]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllChatSession
AS
	SELECT *		
	FROM
		[dbo].[ChatSession]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetChatSessionById    Script Date: 12/29/2025 1:59:56 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetChatSessionById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetChatSessionById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetChatSessionById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[ChatSession]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllChatSessionByUserLoginId    Script Date: 12/29/2025 1:59:56 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetChatSessionByUserLoginId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetChatSessionByUserLoginId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetChatSessionByUserLoginId
(
	@UserLoginId int
)
AS
	SELECT *		
	FROM
		[dbo].[ChatSession]
	WHERE ( UserLoginId = @UserLoginId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetChatSessionMaximumId    Script Date: 12/29/2025 1:59:56 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetChatSessionMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetChatSessionMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetChatSessionMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[ChatSession]

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

/****** Object:  StoredProcedure [dbo].GetChatSessionRowCount    Script Date: 12/29/2025 1:59:56 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetChatSessionRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetChatSessionRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetChatSessionRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[ChatSession]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedChatSession    Script Date: 12/29/2025 1:59:56 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedChatSession]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedChatSession]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedChatSession
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

SET @SQL1 = 'WITH ChatSessionEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[SessionGuid],
	[UserLoginId],
	[GuestName],
	[Status],
	[StartedAt],
	[LastMessageAt],
	[IsActive]
				FROM 
				[dbo].[ChatSession]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[SessionGuid],
	[UserLoginId],
	[GuestName],
	[Status],
	[StartedAt],
	[LastMessageAt],
	[IsActive]
				FROM 
					ChatSessionEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[ChatSession] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetChatSessionByQuery    Script Date: 12/29/2025 1:59:56 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetChatSessionByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetChatSessionByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetChatSessionByQuery
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
				[dbo].[ChatSession] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

