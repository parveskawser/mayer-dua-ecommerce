USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertAuditLog    Script Date: 12/21/2025 8:58:36 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertAuditLog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertAuditLog]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertAuditLog
(
	@Id int OUTPUT,
	@TableName nvarchar(128),
	@Operation nvarchar(10),
	@PrimaryKeyValue int,
	@OldValues nvarchar(max),
	@NewValues nvarchar(max),
	@ChangedBy nvarchar(100),
	@ChangeDate datetime
)
AS
    INSERT INTO [dbo].[AuditLog] 
	(
	[TableName],
	[Operation],
	[PrimaryKeyValue],
	[OldValues],
	[NewValues],
	[ChangedBy],
	[ChangeDate]
    ) 
	VALUES 
	(
	@TableName,
	@Operation,
	@PrimaryKeyValue,
	@OldValues,
	@NewValues,
	@ChangedBy,
	@ChangeDate
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

/****** Object:  StoredProcedure [dbo].UpdateAuditLog    Script Date: 12/21/2025 8:58:36 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateAuditLog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateAuditLog]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateAuditLog
(
	@Id int,
	@TableName nvarchar(128),
	@Operation nvarchar(10),
	@PrimaryKeyValue int,
	@OldValues nvarchar(max),
	@NewValues nvarchar(max),
	@ChangedBy nvarchar(100),
	@ChangeDate datetime
)
AS
    UPDATE [dbo].[AuditLog] 
	SET
	[TableName] = @TableName,
	[Operation] = @Operation,
	[PrimaryKeyValue] = @PrimaryKeyValue,
	[OldValues] = @OldValues,
	[NewValues] = @NewValues,
	[ChangedBy] = @ChangedBy,
	[ChangeDate] = @ChangeDate
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

/****** Object:  StoredProcedure [dbo].DeleteAuditLog    Script Date: 12/21/2025 8:58:36 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteAuditLog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteAuditLog]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteAuditLog
(
	@Id int
)
AS
	DELETE [dbo].[AuditLog] 

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

/****** Object:  StoredProcedure [dbo].GetAllAuditLog    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllAuditLog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllAuditLog]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllAuditLog
AS
	SELECT *		
	FROM
		[dbo].[AuditLog]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAuditLogById    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAuditLogById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAuditLogById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAuditLogById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[AuditLog]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAuditLogMaximumId    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAuditLogMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAuditLogMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAuditLogMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[AuditLog]

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

/****** Object:  StoredProcedure [dbo].GetAuditLogRowCount    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAuditLogRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAuditLogRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAuditLogRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[AuditLog]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedAuditLog    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedAuditLog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedAuditLog]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedAuditLog
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

SET @SQL1 = 'WITH AuditLogEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[TableName],
	[Operation],
	[PrimaryKeyValue],
	[OldValues],
	[NewValues],
	[ChangedBy],
	[ChangeDate]
				FROM 
				[dbo].[AuditLog]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[TableName],
	[Operation],
	[PrimaryKeyValue],
	[OldValues],
	[NewValues],
	[ChangedBy],
	[ChangeDate]
				FROM 
					AuditLogEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[AuditLog] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetAuditLogByQuery    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAuditLogByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAuditLogByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAuditLogByQuery
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
				[dbo].[AuditLog] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

