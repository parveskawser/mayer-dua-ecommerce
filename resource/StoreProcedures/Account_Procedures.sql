USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertAccount    Script Date: 12/2/2025 4:44:55 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertAccount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertAccount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertAccount
(
	@Id int OUTPUT,
	@AccountCode nvarchar(20),
	@AccountName nvarchar(100),
	@AccountTypeId int,
	@IsActive bit,
	@Description nvarchar(255),
	@Balance decimal(18, 2),
	@CurrencyCode nvarchar(10),
	@ParentAccountId int,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    INSERT INTO [dbo].[Account] 
	(
	[AccountCode],
	[AccountName],
	[AccountTypeId],
	[IsActive],
	[Description],
	[Balance],
	[CurrencyCode],
	[ParentAccountId],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
    ) 
	VALUES 
	(
	@AccountCode,
	@AccountName,
	@AccountTypeId,
	@IsActive,
	@Description,
	@Balance,
	@CurrencyCode,
	@ParentAccountId,
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

/****** Object:  StoredProcedure [dbo].UpdateAccount    Script Date: 12/2/2025 4:44:55 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateAccount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateAccount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateAccount
(
	@Id int,
	@AccountCode nvarchar(20),
	@AccountName nvarchar(100),
	@AccountTypeId int,
	@IsActive bit,
	@Description nvarchar(255),
	@Balance decimal(18, 2),
	@CurrencyCode nvarchar(10),
	@ParentAccountId int,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    UPDATE [dbo].[Account] 
	SET
	[AccountCode] = @AccountCode,
	[AccountName] = @AccountName,
	[AccountTypeId] = @AccountTypeId,
	[IsActive] = @IsActive,
	[Description] = @Description,
	[Balance] = @Balance,
	[CurrencyCode] = @CurrencyCode,
	[ParentAccountId] = @ParentAccountId,
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

/****** Object:  StoredProcedure [dbo].DeleteAccount    Script Date: 12/2/2025 4:44:55 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteAccount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteAccount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteAccount
(
	@Id int
)
AS
	DELETE [dbo].[Account] 

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

/****** Object:  StoredProcedure [dbo].GetAllAccount    Script Date: 12/2/2025 4:44:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllAccount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllAccount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllAccount
AS
	SELECT *		
	FROM
		[dbo].[Account]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAccountById    Script Date: 12/2/2025 4:44:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAccountById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAccountById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAccountById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[Account]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllAccountByAccountTypeId    Script Date: 12/2/2025 4:44:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAccountByAccountTypeId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAccountByAccountTypeId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAccountByAccountTypeId
(
	@AccountTypeId int
)
AS
	SELECT *		
	FROM
		[dbo].[Account]
	WHERE ( AccountTypeId = @AccountTypeId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAccountMaximumId    Script Date: 12/2/2025 4:44:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAccountMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAccountMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAccountMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[Account]

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

/****** Object:  StoredProcedure [dbo].GetAccountRowCount    Script Date: 12/2/2025 4:44:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAccountRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAccountRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAccountRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[Account]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedAccount    Script Date: 12/2/2025 4:44:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedAccount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedAccount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedAccount
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

SET @SQL1 = 'WITH AccountEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[AccountCode],
	[AccountName],
	[AccountTypeId],
	[IsActive],
	[Description],
	[Balance],
	[CurrencyCode],
	[ParentAccountId],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
				[dbo].[Account]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[AccountCode],
	[AccountName],
	[AccountTypeId],
	[IsActive],
	[Description],
	[Balance],
	[CurrencyCode],
	[ParentAccountId],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
					AccountEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[Account] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetAccountByQuery    Script Date: 12/2/2025 4:44:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAccountByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAccountByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAccountByQuery
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
				[dbo].[Account] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

