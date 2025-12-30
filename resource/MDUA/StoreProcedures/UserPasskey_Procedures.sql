USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertUserPasskey    Script Date: 12/29/2025 2:00:02 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertUserPasskey]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertUserPasskey]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertUserPasskey
(
	@Id int OUTPUT,
	@UserId int,
	@CredentialId varbinary(450),
	@PublicKey varbinary(max),
	@SignatureCounter int,
	@CredType nvarchar(50),
	@RegDate datetime,
	@AaGuid uniqueidentifier,
	@FriendlyName nvarchar(100),
	@DeviceType nvarchar(100)
)
AS
    INSERT INTO [dbo].[UserPasskey] 
	(
	[UserId],
	[CredentialId],
	[PublicKey],
	[SignatureCounter],
	[CredType],
	[RegDate],
	[AaGuid],
	[FriendlyName],
	[DeviceType]
    ) 
	VALUES 
	(
	@UserId,
	@CredentialId,
	@PublicKey,
	@SignatureCounter,
	@CredType,
	@RegDate,
	@AaGuid,
	@FriendlyName,
	@DeviceType
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

/****** Object:  StoredProcedure [dbo].UpdateUserPasskey    Script Date: 12/29/2025 2:00:02 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateUserPasskey]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateUserPasskey]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateUserPasskey
(
	@Id int,
	@UserId int,
	@CredentialId varbinary(450),
	@PublicKey varbinary(max),
	@SignatureCounter int,
	@CredType nvarchar(50),
	@RegDate datetime,
	@AaGuid uniqueidentifier,
	@FriendlyName nvarchar(100),
	@DeviceType nvarchar(100)
)
AS
    UPDATE [dbo].[UserPasskey] 
	SET
	[UserId] = @UserId,
	[CredentialId] = @CredentialId,
	[PublicKey] = @PublicKey,
	[SignatureCounter] = @SignatureCounter,
	[CredType] = @CredType,
	[RegDate] = @RegDate,
	[AaGuid] = @AaGuid,
	[FriendlyName] = @FriendlyName,
	[DeviceType] = @DeviceType
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

/****** Object:  StoredProcedure [dbo].DeleteUserPasskey    Script Date: 12/29/2025 2:00:02 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteUserPasskey]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteUserPasskey]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteUserPasskey
(
	@Id int
)
AS
	DELETE [dbo].[UserPasskey] 

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

/****** Object:  StoredProcedure [dbo].GetAllUserPasskey    Script Date: 12/29/2025 2:00:02 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllUserPasskey]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllUserPasskey]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllUserPasskey
AS
	SELECT *		
	FROM
		[dbo].[UserPasskey]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetUserPasskeyById    Script Date: 12/29/2025 2:00:02 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUserPasskeyById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetUserPasskeyById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetUserPasskeyById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[UserPasskey]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllUserPasskeyByUserId    Script Date: 12/29/2025 2:00:02 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUserPasskeyByUserId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetUserPasskeyByUserId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetUserPasskeyByUserId
(
	@UserId int
)
AS
	SELECT *		
	FROM
		[dbo].[UserPasskey]
	WHERE ( UserId = @UserId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetUserPasskeyMaximumId    Script Date: 12/29/2025 2:00:02 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUserPasskeyMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetUserPasskeyMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetUserPasskeyMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[UserPasskey]

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

/****** Object:  StoredProcedure [dbo].GetUserPasskeyRowCount    Script Date: 12/29/2025 2:00:02 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUserPasskeyRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetUserPasskeyRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetUserPasskeyRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[UserPasskey]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedUserPasskey    Script Date: 12/29/2025 2:00:02 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedUserPasskey]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedUserPasskey]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedUserPasskey
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

SET @SQL1 = 'WITH UserPasskeyEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[UserId],
	[CredentialId],
	[PublicKey],
	[SignatureCounter],
	[CredType],
	[RegDate],
	[AaGuid],
	[FriendlyName],
	[DeviceType]
				FROM 
				[dbo].[UserPasskey]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[UserId],
	[CredentialId],
	[PublicKey],
	[SignatureCounter],
	[CredType],
	[RegDate],
	[AaGuid],
	[FriendlyName],
	[DeviceType]
				FROM 
					UserPasskeyEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[UserPasskey] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetUserPasskeyByQuery    Script Date: 12/29/2025 2:00:02 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUserPasskeyByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetUserPasskeyByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetUserPasskeyByQuery
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
				[dbo].[UserPasskey] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

