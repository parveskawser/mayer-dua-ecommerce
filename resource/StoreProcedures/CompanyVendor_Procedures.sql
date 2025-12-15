USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertCompanyVendor    Script Date: 12/10/2025 2:32:49 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertCompanyVendor]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertCompanyVendor]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertCompanyVendor
(
	@Id int OUTPUT,
	@CompanyId int,
	@VendorId int
)
AS
    INSERT INTO [dbo].[CompanyVendor] 
	(
	[CompanyId],
	[VendorId]
    ) 
	VALUES 
	(
	@CompanyId,
	@VendorId
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

/****** Object:  StoredProcedure [dbo].UpdateCompanyVendor    Script Date: 12/10/2025 2:32:49 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateCompanyVendor]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateCompanyVendor]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateCompanyVendor
(
	@Id int,
	@CompanyId int,
	@VendorId int
)
AS
    UPDATE [dbo].[CompanyVendor] 
	SET
	[CompanyId] = @CompanyId,
	[VendorId] = @VendorId
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

/****** Object:  StoredProcedure [dbo].DeleteCompanyVendor    Script Date: 12/10/2025 2:32:49 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteCompanyVendor]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteCompanyVendor]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteCompanyVendor
(
	@Id int
)
AS
	DELETE [dbo].[CompanyVendor] 

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

/****** Object:  StoredProcedure [dbo].GetAllCompanyVendor    Script Date: 12/10/2025 2:32:49 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllCompanyVendor]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllCompanyVendor]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllCompanyVendor
AS
	SELECT *		
	FROM
		[dbo].[CompanyVendor]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetCompanyVendorById    Script Date: 12/10/2025 2:32:49 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyVendorById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyVendorById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyVendorById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[CompanyVendor]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllCompanyVendorByCompanyId    Script Date: 12/10/2025 2:32:49 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyVendorByCompanyId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyVendorByCompanyId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyVendorByCompanyId
(
	@CompanyId int
)
AS
	SELECT *		
	FROM
		[dbo].[CompanyVendor]
	WHERE ( CompanyId = @CompanyId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllCompanyVendorByVendorId    Script Date: 12/10/2025 2:32:49 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyVendorByVendorId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyVendorByVendorId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyVendorByVendorId
(
	@VendorId int
)
AS
	SELECT *		
	FROM
		[dbo].[CompanyVendor]
	WHERE ( VendorId = @VendorId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetCompanyVendorMaximumId    Script Date: 12/10/2025 2:32:49 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyVendorMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyVendorMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyVendorMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[CompanyVendor]

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

/****** Object:  StoredProcedure [dbo].GetCompanyVendorRowCount    Script Date: 12/10/2025 2:32:49 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyVendorRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyVendorRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyVendorRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[CompanyVendor]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedCompanyVendor    Script Date: 12/10/2025 2:32:49 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedCompanyVendor]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedCompanyVendor]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedCompanyVendor
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

SET @SQL1 = 'WITH CompanyVendorEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[CompanyId],
	[VendorId]
				FROM 
				[dbo].[CompanyVendor]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[CompanyId],
	[VendorId]
				FROM 
					CompanyVendorEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[CompanyVendor] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetCompanyVendorByQuery    Script Date: 12/10/2025 2:32:49 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyVendorByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyVendorByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyVendorByQuery
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
				[dbo].[CompanyVendor] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

