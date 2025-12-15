USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertJournalEntry    Script Date: 12/10/2025 2:32:50 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertJournalEntry]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertJournalEntry]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertJournalEntry
(
	@Id int OUTPUT,
	@ReferenceType nvarchar(50),
	@ReferenceId int,
	@EntryDate datetime,
	@Description nvarchar(255),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    INSERT INTO [dbo].[JournalEntry] 
	(
	[ReferenceType],
	[ReferenceId],
	[EntryDate],
	[Description],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
    ) 
	VALUES 
	(
	@ReferenceType,
	@ReferenceId,
	@EntryDate,
	@Description,
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

/****** Object:  StoredProcedure [dbo].UpdateJournalEntry    Script Date: 12/10/2025 2:32:50 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateJournalEntry]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateJournalEntry]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateJournalEntry
(
	@Id int,
	@ReferenceType nvarchar(50),
	@ReferenceId int,
	@EntryDate datetime,
	@Description nvarchar(255),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    UPDATE [dbo].[JournalEntry] 
	SET
	[ReferenceType] = @ReferenceType,
	[ReferenceId] = @ReferenceId,
	[EntryDate] = @EntryDate,
	[Description] = @Description,
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

/****** Object:  StoredProcedure [dbo].DeleteJournalEntry    Script Date: 12/10/2025 2:32:50 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteJournalEntry]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteJournalEntry]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteJournalEntry
(
	@Id int
)
AS
	DELETE [dbo].[JournalEntry] 

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

/****** Object:  StoredProcedure [dbo].GetAllJournalEntry    Script Date: 12/10/2025 2:32:50 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllJournalEntry]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllJournalEntry]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllJournalEntry
AS
	SELECT *		
	FROM
		[dbo].[JournalEntry]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetJournalEntryById    Script Date: 12/10/2025 2:32:50 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetJournalEntryById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetJournalEntryById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetJournalEntryById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[JournalEntry]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetJournalEntryMaximumId    Script Date: 12/10/2025 2:32:50 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetJournalEntryMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetJournalEntryMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetJournalEntryMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[JournalEntry]

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

/****** Object:  StoredProcedure [dbo].GetJournalEntryRowCount    Script Date: 12/10/2025 2:32:50 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetJournalEntryRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetJournalEntryRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetJournalEntryRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[JournalEntry]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedJournalEntry    Script Date: 12/10/2025 2:32:50 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedJournalEntry]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedJournalEntry]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedJournalEntry
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

SET @SQL1 = 'WITH JournalEntryEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[ReferenceType],
	[ReferenceId],
	[EntryDate],
	[Description],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
				[dbo].[JournalEntry]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[ReferenceType],
	[ReferenceId],
	[EntryDate],
	[Description],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
					JournalEntryEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[JournalEntry] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetJournalEntryByQuery    Script Date: 12/10/2025 2:32:50 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetJournalEntryByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetJournalEntryByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetJournalEntryByQuery
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
				[dbo].[JournalEntry] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

