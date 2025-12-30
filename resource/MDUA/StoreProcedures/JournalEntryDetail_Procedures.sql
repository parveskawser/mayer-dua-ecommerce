USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertJournalEntryDetail    Script Date: 12/29/2025 1:59:58 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertJournalEntryDetail]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertJournalEntryDetail]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertJournalEntryDetail
(
	@Id int OUTPUT,
	@JournalEntryId int,
	@AccountId int,
	@Debit decimal(18, 2),
	@Credit decimal(18, 2),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime,
	@LineDescription nvarchar(255),
	@CurrencyCode nvarchar(10),
	@ExchangeRate decimal(18, 6)
)
AS
    INSERT INTO [dbo].[JournalEntryDetail] 
	(
	[JournalEntryId],
	[AccountId],
	[Debit],
	[Credit],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[LineDescription],
	[CurrencyCode],
	[ExchangeRate]
    ) 
	VALUES 
	(
	@JournalEntryId,
	@AccountId,
	@Debit,
	@Credit,
	@CreatedBy,
	@CreatedAt,
	@UpdatedBy,
	@UpdatedAt,
	@LineDescription,
	@CurrencyCode,
	@ExchangeRate
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

/****** Object:  StoredProcedure [dbo].UpdateJournalEntryDetail    Script Date: 12/29/2025 1:59:58 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateJournalEntryDetail]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateJournalEntryDetail]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateJournalEntryDetail
(
	@Id int,
	@JournalEntryId int,
	@AccountId int,
	@Debit decimal(18, 2),
	@Credit decimal(18, 2),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime,
	@LineDescription nvarchar(255),
	@CurrencyCode nvarchar(10),
	@ExchangeRate decimal(18, 6)
)
AS
    UPDATE [dbo].[JournalEntryDetail] 
	SET
	[JournalEntryId] = @JournalEntryId,
	[AccountId] = @AccountId,
	[Debit] = @Debit,
	[Credit] = @Credit,
	[CreatedBy] = @CreatedBy,
	[CreatedAt] = @CreatedAt,
	[UpdatedBy] = @UpdatedBy,
	[UpdatedAt] = @UpdatedAt,
	[LineDescription] = @LineDescription,
	[CurrencyCode] = @CurrencyCode,
	[ExchangeRate] = @ExchangeRate
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

/****** Object:  StoredProcedure [dbo].DeleteJournalEntryDetail    Script Date: 12/29/2025 1:59:58 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteJournalEntryDetail]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteJournalEntryDetail]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteJournalEntryDetail
(
	@Id int
)
AS
	DELETE [dbo].[JournalEntryDetail] 

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

/****** Object:  StoredProcedure [dbo].GetAllJournalEntryDetail    Script Date: 12/29/2025 1:59:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllJournalEntryDetail]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllJournalEntryDetail]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllJournalEntryDetail
AS
	SELECT *		
	FROM
		[dbo].[JournalEntryDetail]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetJournalEntryDetailById    Script Date: 12/29/2025 1:59:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetJournalEntryDetailById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetJournalEntryDetailById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetJournalEntryDetailById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[JournalEntryDetail]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllJournalEntryDetailByJournalEntryId    Script Date: 12/29/2025 1:59:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetJournalEntryDetailByJournalEntryId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetJournalEntryDetailByJournalEntryId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetJournalEntryDetailByJournalEntryId
(
	@JournalEntryId int
)
AS
	SELECT *		
	FROM
		[dbo].[JournalEntryDetail]
	WHERE ( JournalEntryId = @JournalEntryId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllJournalEntryDetailByAccountId    Script Date: 12/29/2025 1:59:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetJournalEntryDetailByAccountId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetJournalEntryDetailByAccountId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetJournalEntryDetailByAccountId
(
	@AccountId int
)
AS
	SELECT *		
	FROM
		[dbo].[JournalEntryDetail]
	WHERE ( AccountId = @AccountId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetJournalEntryDetailMaximumId    Script Date: 12/29/2025 1:59:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetJournalEntryDetailMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetJournalEntryDetailMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetJournalEntryDetailMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[JournalEntryDetail]

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

/****** Object:  StoredProcedure [dbo].GetJournalEntryDetailRowCount    Script Date: 12/29/2025 1:59:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetJournalEntryDetailRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetJournalEntryDetailRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetJournalEntryDetailRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[JournalEntryDetail]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedJournalEntryDetail    Script Date: 12/29/2025 1:59:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedJournalEntryDetail]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedJournalEntryDetail]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedJournalEntryDetail
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

SET @SQL1 = 'WITH JournalEntryDetailEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[JournalEntryId],
	[AccountId],
	[Debit],
	[Credit],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[LineDescription],
	[CurrencyCode],
	[ExchangeRate]
				FROM 
				[dbo].[JournalEntryDetail]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[JournalEntryId],
	[AccountId],
	[Debit],
	[Credit],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[LineDescription],
	[CurrencyCode],
	[ExchangeRate]
				FROM 
					JournalEntryDetailEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[JournalEntryDetail] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetJournalEntryDetailByQuery    Script Date: 12/29/2025 1:59:58 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetJournalEntryDetailByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetJournalEntryDetailByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetJournalEntryDetailByQuery
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
				[dbo].[JournalEntryDetail] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

