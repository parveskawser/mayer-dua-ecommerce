USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertAccountType    Script Date: 12/21/2025 8:58:35 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertAccountType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertAccountType]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertAccountType
(
	@Id int OUTPUT,
	@Name nvarchar(50),
	@Description nvarchar(200),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    INSERT INTO [dbo].[AccountType] 
	(
	[Name],
	[Description],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
    ) 
	VALUES 
	(
	@Name,
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

/****** Object:  StoredProcedure [dbo].UpdateAccountType    Script Date: 12/21/2025 8:58:35 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateAccountType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateAccountType]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateAccountType
(
	@Id int,
	@Name nvarchar(50),
	@Description nvarchar(200),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    UPDATE [dbo].[AccountType] 
	SET
	[Name] = @Name,
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

/****** Object:  StoredProcedure [dbo].DeleteAccountType    Script Date: 12/21/2025 8:58:35 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteAccountType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteAccountType]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteAccountType
(
	@Id int
)
AS
	DELETE [dbo].[AccountType] 

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

/****** Object:  StoredProcedure [dbo].GetAllAccountType    Script Date: 12/21/2025 8:58:35 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllAccountType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllAccountType]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllAccountType
AS
	SELECT *		
	FROM
		[dbo].[AccountType]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAccountTypeById    Script Date: 12/21/2025 8:58:35 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAccountTypeById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAccountTypeById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAccountTypeById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[AccountType]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAccountTypeMaximumId    Script Date: 12/21/2025 8:58:35 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAccountTypeMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAccountTypeMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAccountTypeMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[AccountType]

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

/****** Object:  StoredProcedure [dbo].GetAccountTypeRowCount    Script Date: 12/21/2025 8:58:35 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAccountTypeRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAccountTypeRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAccountTypeRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[AccountType]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedAccountType    Script Date: 12/21/2025 8:58:35 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedAccountType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedAccountType]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedAccountType
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

SET @SQL1 = 'WITH AccountTypeEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[Name],
	[Description],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
				[dbo].[AccountType]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[Name],
	[Description],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
					AccountTypeEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[AccountType] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetAccountTypeByQuery    Script Date: 12/21/2025 8:58:35 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAccountTypeByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAccountTypeByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAccountTypeByQuery
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
				[dbo].[AccountType] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

