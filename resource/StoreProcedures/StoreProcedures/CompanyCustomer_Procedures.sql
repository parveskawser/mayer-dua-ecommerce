USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertCompanyCustomer    Script Date: 12/21/2025 8:58:36 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertCompanyCustomer]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertCompanyCustomer]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertCompanyCustomer
(
	@Id int OUTPUT,
	@CompanyId int,
	@CustomerId int
)
AS
    INSERT INTO [dbo].[CompanyCustomer] 
	(
	[CompanyId],
	[CustomerId]
    ) 
	VALUES 
	(
	@CompanyId,
	@CustomerId
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

/****** Object:  StoredProcedure [dbo].UpdateCompanyCustomer    Script Date: 12/21/2025 8:58:36 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateCompanyCustomer]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateCompanyCustomer]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateCompanyCustomer
(
	@Id int,
	@CompanyId int,
	@CustomerId int
)
AS
    UPDATE [dbo].[CompanyCustomer] 
	SET
	[CompanyId] = @CompanyId,
	[CustomerId] = @CustomerId
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

/****** Object:  StoredProcedure [dbo].DeleteCompanyCustomer    Script Date: 12/21/2025 8:58:36 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteCompanyCustomer]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteCompanyCustomer]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteCompanyCustomer
(
	@Id int
)
AS
	DELETE [dbo].[CompanyCustomer] 

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

/****** Object:  StoredProcedure [dbo].GetAllCompanyCustomer    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllCompanyCustomer]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllCompanyCustomer]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllCompanyCustomer
AS
	SELECT *		
	FROM
		[dbo].[CompanyCustomer]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetCompanyCustomerById    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyCustomerById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyCustomerById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyCustomerById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[CompanyCustomer]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllCompanyCustomerByCompanyId    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyCustomerByCompanyId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyCustomerByCompanyId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyCustomerByCompanyId
(
	@CompanyId int
)
AS
	SELECT *		
	FROM
		[dbo].[CompanyCustomer]
	WHERE ( CompanyId = @CompanyId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllCompanyCustomerByCustomerId    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyCustomerByCustomerId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyCustomerByCustomerId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyCustomerByCustomerId
(
	@CustomerId int
)
AS
	SELECT *		
	FROM
		[dbo].[CompanyCustomer]
	WHERE ( CustomerId = @CustomerId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetCompanyCustomerMaximumId    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyCustomerMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyCustomerMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyCustomerMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[CompanyCustomer]

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

/****** Object:  StoredProcedure [dbo].GetCompanyCustomerRowCount    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyCustomerRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyCustomerRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyCustomerRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[CompanyCustomer]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedCompanyCustomer    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedCompanyCustomer]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedCompanyCustomer]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedCompanyCustomer
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

SET @SQL1 = 'WITH CompanyCustomerEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[CompanyId],
	[CustomerId]
				FROM 
				[dbo].[CompanyCustomer]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[CompanyId],
	[CustomerId]
				FROM 
					CompanyCustomerEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[CompanyCustomer] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetCompanyCustomerByQuery    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyCustomerByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyCustomerByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyCustomerByQuery
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
				[dbo].[CompanyCustomer] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

