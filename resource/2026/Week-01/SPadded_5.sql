USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertEmailHistory    Script Date: 12/29/2025 1:59:57 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertEmailHistory]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertEmailHistory]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertEmailHistory
(
	@Id int OUTPUT,
	@TemplateKey nvarchar(250),
	@ToEmail nvarchar(250),
	@CcEmail nvarchar(250),
	@BccEmail nvarchar(250),
	@FromEmail nvarchar(250),
	@FromName nvarchar(250),
	@EmailSubject nvarchar(250),
	@EmailBodyContent nvarchar(max),
	@EmailSentDate datetime,
	@IsSystemAutoSent bit,
	@IsRead bit,
	@ReadCount int,
	@LastUpdatedDate datetime
)
AS
    INSERT INTO [dbo].[EmailHistory] 
	(
	[TemplateKey],
	[ToEmail],
	[CcEmail],
	[BccEmail],
	[FromEmail],
	[FromName],
	[EmailSubject],
	[EmailBodyContent],
	[EmailSentDate],
	[IsSystemAutoSent],
	[IsRead],
	[ReadCount],
	[LastUpdatedDate]
    ) 
	VALUES 
	(
	@TemplateKey,
	@ToEmail,
	@CcEmail,
	@BccEmail,
	@FromEmail,
	@FromName,
	@EmailSubject,
	@EmailBodyContent,
	@EmailSentDate,
	@IsSystemAutoSent,
	@IsRead,
	@ReadCount,
	@LastUpdatedDate
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

/****** Object:  StoredProcedure [dbo].UpdateEmailHistory    Script Date: 12/29/2025 1:59:57 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateEmailHistory]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateEmailHistory]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateEmailHistory
(
	@Id int,
	@TemplateKey nvarchar(250),
	@ToEmail nvarchar(250),
	@CcEmail nvarchar(250),
	@BccEmail nvarchar(250),
	@FromEmail nvarchar(250),
	@FromName nvarchar(250),
	@EmailSubject nvarchar(250),
	@EmailBodyContent nvarchar(max),
	@EmailSentDate datetime,
	@IsSystemAutoSent bit,
	@IsRead bit,
	@ReadCount int,
	@LastUpdatedDate datetime
)
AS
    UPDATE [dbo].[EmailHistory] 
	SET
	[TemplateKey] = @TemplateKey,
	[ToEmail] = @ToEmail,
	[CcEmail] = @CcEmail,
	[BccEmail] = @BccEmail,
	[FromEmail] = @FromEmail,
	[FromName] = @FromName,
	[EmailSubject] = @EmailSubject,
	[EmailBodyContent] = @EmailBodyContent,
	[EmailSentDate] = @EmailSentDate,
	[IsSystemAutoSent] = @IsSystemAutoSent,
	[IsRead] = @IsRead,
	[ReadCount] = @ReadCount,
	[LastUpdatedDate] = @LastUpdatedDate
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

/****** Object:  StoredProcedure [dbo].DeleteEmailHistory    Script Date: 12/29/2025 1:59:57 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteEmailHistory]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteEmailHistory]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteEmailHistory
(
	@Id int
)
AS
	DELETE [dbo].[EmailHistory] 

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

/****** Object:  StoredProcedure [dbo].GetAllEmailHistory    Script Date: 12/29/2025 1:59:57 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllEmailHistory]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllEmailHistory]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllEmailHistory
AS
	SELECT *		
	FROM
		[dbo].[EmailHistory]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetEmailHistoryById    Script Date: 12/29/2025 1:59:57 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetEmailHistoryById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetEmailHistoryById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetEmailHistoryById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[EmailHistory]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetEmailHistoryMaximumId    Script Date: 12/29/2025 1:59:57 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetEmailHistoryMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetEmailHistoryMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetEmailHistoryMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[EmailHistory]

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

/****** Object:  StoredProcedure [dbo].GetEmailHistoryRowCount    Script Date: 12/29/2025 1:59:57 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetEmailHistoryRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetEmailHistoryRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetEmailHistoryRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[EmailHistory]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedEmailHistory    Script Date: 12/29/2025 1:59:57 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedEmailHistory]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedEmailHistory]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedEmailHistory
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

SET @SQL1 = 'WITH EmailHistoryEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[TemplateKey],
	[ToEmail],
	[CcEmail],
	[BccEmail],
	[FromEmail],
	[FromName],
	[EmailSubject],
	[EmailBodyContent],
	[EmailSentDate],
	[IsSystemAutoSent],
	[IsRead],
	[ReadCount],
	[LastUpdatedDate]
				FROM 
				[dbo].[EmailHistory]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[TemplateKey],
	[ToEmail],
	[CcEmail],
	[BccEmail],
	[FromEmail],
	[FromName],
	[EmailSubject],
	[EmailBodyContent],
	[EmailSentDate],
	[IsSystemAutoSent],
	[IsRead],
	[ReadCount],
	[LastUpdatedDate]
				FROM 
					EmailHistoryEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[EmailHistory] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetEmailHistoryByQuery    Script Date: 12/29/2025 1:59:57 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetEmailHistoryByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetEmailHistoryByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetEmailHistoryByQuery
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
				[dbo].[EmailHistory] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

