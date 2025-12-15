USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertChatMessage    Script Date: 12/10/2025 2:32:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertChatMessage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertChatMessage]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertChatMessage
(
	@Id int OUTPUT,
	@ChatSessionId int,
	@SenderId int,
	@SenderName nvarchar(100),
	@MessageText nvarchar(max),
	@IsFromAdmin bit,
	@IsRead bit,
	@SentAt datetime
)
AS
    INSERT INTO [dbo].[ChatMessage] 
	(
	[ChatSessionId],
	[SenderId],
	[SenderName],
	[MessageText],
	[IsFromAdmin],
	[IsRead],
	[SentAt]
    ) 
	VALUES 
	(
	@ChatSessionId,
	@SenderId,
	@SenderName,
	@MessageText,
	@IsFromAdmin,
	@IsRead,
	@SentAt
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

/****** Object:  StoredProcedure [dbo].UpdateChatMessage    Script Date: 12/10/2025 2:32:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateChatMessage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateChatMessage]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateChatMessage
(
	@Id int,
	@ChatSessionId int,
	@SenderId int,
	@SenderName nvarchar(100),
	@MessageText nvarchar(max),
	@IsFromAdmin bit,
	@IsRead bit,
	@SentAt datetime
)
AS
    UPDATE [dbo].[ChatMessage] 
	SET
	[ChatSessionId] = @ChatSessionId,
	[SenderId] = @SenderId,
	[SenderName] = @SenderName,
	[MessageText] = @MessageText,
	[IsFromAdmin] = @IsFromAdmin,
	[IsRead] = @IsRead,
	[SentAt] = @SentAt
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

/****** Object:  StoredProcedure [dbo].DeleteChatMessage    Script Date: 12/10/2025 2:32:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteChatMessage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteChatMessage]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteChatMessage
(
	@Id int
)
AS
	DELETE [dbo].[ChatMessage] 

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

/****** Object:  StoredProcedure [dbo].GetAllChatMessage    Script Date: 12/10/2025 2:32:48 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllChatMessage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllChatMessage]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllChatMessage
AS
	SELECT *		
	FROM
		[dbo].[ChatMessage]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetChatMessageById    Script Date: 12/10/2025 2:32:48 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetChatMessageById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetChatMessageById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetChatMessageById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[ChatMessage]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllChatMessageByChatSessionId    Script Date: 12/10/2025 2:32:48 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetChatMessageByChatSessionId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetChatMessageByChatSessionId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetChatMessageByChatSessionId
(
	@ChatSessionId int
)
AS
	SELECT *		
	FROM
		[dbo].[ChatMessage]
	WHERE ( ChatSessionId = @ChatSessionId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetChatMessageMaximumId    Script Date: 12/10/2025 2:32:48 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetChatMessageMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetChatMessageMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetChatMessageMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[ChatMessage]

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

/****** Object:  StoredProcedure [dbo].GetChatMessageRowCount    Script Date: 12/10/2025 2:32:48 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetChatMessageRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetChatMessageRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetChatMessageRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[ChatMessage]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedChatMessage    Script Date: 12/10/2025 2:32:48 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedChatMessage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedChatMessage]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedChatMessage
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

SET @SQL1 = 'WITH ChatMessageEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[ChatSessionId],
	[SenderId],
	[SenderName],
	[MessageText],
	[IsFromAdmin],
	[IsRead],
	[SentAt]
				FROM 
				[dbo].[ChatMessage]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[ChatSessionId],
	[SenderId],
	[SenderName],
	[MessageText],
	[IsFromAdmin],
	[IsRead],
	[SentAt]
				FROM 
					ChatMessageEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[ChatMessage] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetChatMessageByQuery    Script Date: 12/10/2025 2:32:48 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetChatMessageByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetChatMessageByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetChatMessageByQuery
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
				[dbo].[ChatMessage] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

