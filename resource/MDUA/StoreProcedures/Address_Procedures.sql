USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertAddress    Script Date: 12/29/2025 1:59:55 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertAddress]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertAddress]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertAddress
(
	@Id int OUTPUT,
	@CustomerId int,
	@Country nvarchar(100),
	@Divison nvarchar(100),
	@City nvarchar(100),
	@Thana nvarchar(100),
	@SubOffice nvarchar(100),
	@Street nvarchar(255),
	@PostalCode varchar(20),
	@ZipCode nchar(50),
	@AddressType nvarchar(50),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    INSERT INTO [dbo].[Address] 
	(
	[CustomerId],
	[Country],
	[Divison],
	[City],
	[Thana],
	[SubOffice],
	[Street],
	[PostalCode],
	[ZipCode],
	[AddressType],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
    ) 
	VALUES 
	(
	@CustomerId,
	@Country,
	@Divison,
	@City,
	@Thana,
	@SubOffice,
	@Street,
	@PostalCode,
	@ZipCode,
	@AddressType,
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

/****** Object:  StoredProcedure [dbo].UpdateAddress    Script Date: 12/29/2025 1:59:55 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateAddress]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateAddress]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateAddress
(
	@Id int,
	@CustomerId int,
	@Country nvarchar(100),
	@Divison nvarchar(100),
	@City nvarchar(100),
	@Thana nvarchar(100),
	@SubOffice nvarchar(100),
	@Street nvarchar(255),
	@PostalCode varchar(20),
	@ZipCode nchar(50),
	@AddressType nvarchar(50),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    UPDATE [dbo].[Address] 
	SET
	[CustomerId] = @CustomerId,
	[Country] = @Country,
	[Divison] = @Divison,
	[City] = @City,
	[Thana] = @Thana,
	[SubOffice] = @SubOffice,
	[Street] = @Street,
	[PostalCode] = @PostalCode,
	[ZipCode] = @ZipCode,
	[AddressType] = @AddressType,
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

/****** Object:  StoredProcedure [dbo].DeleteAddress    Script Date: 12/29/2025 1:59:55 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteAddress]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteAddress]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteAddress
(
	@Id int
)
AS
	DELETE [dbo].[Address] 

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

/****** Object:  StoredProcedure [dbo].GetAllAddress    Script Date: 12/29/2025 1:59:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllAddress]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllAddress]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllAddress
AS
	SELECT *		
	FROM
		[dbo].[Address]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAddressById    Script Date: 12/29/2025 1:59:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAddressById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAddressById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAddressById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[Address]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllAddressByCustomerId    Script Date: 12/29/2025 1:59:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAddressByCustomerId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAddressByCustomerId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAddressByCustomerId
(
	@CustomerId int
)
AS
	SELECT *		
	FROM
		[dbo].[Address]
	WHERE ( CustomerId = @CustomerId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAddressMaximumId    Script Date: 12/29/2025 1:59:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAddressMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAddressMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAddressMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[Address]

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

/****** Object:  StoredProcedure [dbo].GetAddressRowCount    Script Date: 12/29/2025 1:59:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAddressRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAddressRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAddressRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[Address]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedAddress    Script Date: 12/29/2025 1:59:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedAddress]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedAddress]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedAddress
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

SET @SQL1 = 'WITH AddressEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[CustomerId],
	[Country],
	[Divison],
	[City],
	[Thana],
	[SubOffice],
	[Street],
	[PostalCode],
	[ZipCode],
	[AddressType],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
				[dbo].[Address]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[CustomerId],
	[Country],
	[Divison],
	[City],
	[Thana],
	[SubOffice],
	[Street],
	[PostalCode],
	[ZipCode],
	[AddressType],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
					AddressEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[Address] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetAddressByQuery    Script Date: 12/29/2025 1:59:55 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAddressByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAddressByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAddressByQuery
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
				[dbo].[Address] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

