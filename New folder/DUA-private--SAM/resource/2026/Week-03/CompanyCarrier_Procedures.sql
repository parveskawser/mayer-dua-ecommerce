
/****** Object:  StoredProcedure [dbo]..InsertCompanyCarrier    Script Date: 1/14/2026 4:18:10 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertCompanyCarrier]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertCompanyCarrier]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertCompanyCarrier
(
	@Id int OUTPUT,
	@CompanyId int,
	@CarrierId int,
	@ApiKey nvarchar(500),
	@ApiSecret nvarchar(500),
	@IsActive bit
)
AS
    INSERT INTO [dbo].[CompanyCarrier] 
	(
	[CompanyId],
	[CarrierId],
	[ApiKey],
	[ApiSecret],
	[IsActive]
    ) 
	VALUES 
	(
	@CompanyId,
	@CarrierId,
	@ApiKey,
	@ApiSecret,
	@IsActive
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

/****** Object:  StoredProcedure [dbo].UpdateCompanyCarrier    Script Date: 1/14/2026 4:18:10 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateCompanyCarrier]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateCompanyCarrier]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateCompanyCarrier
(
	@Id int,
	@CompanyId int,
	@CarrierId int,
	@ApiKey nvarchar(500),
	@ApiSecret nvarchar(500),
	@IsActive bit
)
AS
    UPDATE [dbo].[CompanyCarrier] 
	SET
	[CompanyId] = @CompanyId,
	[CarrierId] = @CarrierId,
	[ApiKey] = @ApiKey,
	[ApiSecret] = @ApiSecret,
	[IsActive] = @IsActive
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

/****** Object:  StoredProcedure [dbo].DeleteCompanyCarrier    Script Date: 1/14/2026 4:18:10 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteCompanyCarrier]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteCompanyCarrier]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteCompanyCarrier
(
	@Id int
)
AS
	DELETE [dbo].[CompanyCarrier] 

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

/****** Object:  StoredProcedure [dbo].GetAllCompanyCarrier    Script Date: 1/14/2026 4:18:10 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllCompanyCarrier]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllCompanyCarrier]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllCompanyCarrier
AS
	SELECT *		
	FROM
		[dbo].[CompanyCarrier]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetCompanyCarrierById    Script Date: 1/14/2026 4:18:10 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyCarrierById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyCarrierById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyCarrierById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[CompanyCarrier]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllCompanyCarrierByCompanyId    Script Date: 1/14/2026 4:18:10 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyCarrierByCompanyId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyCarrierByCompanyId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyCarrierByCompanyId
(
	@CompanyId int
)
AS
	SELECT *		
	FROM
		[dbo].[CompanyCarrier]
	WHERE ( CompanyId = @CompanyId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllCompanyCarrierByCarrierId    Script Date: 1/14/2026 4:18:10 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyCarrierByCarrierId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyCarrierByCarrierId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyCarrierByCarrierId
(
	@CarrierId int
)
AS
	SELECT *		
	FROM
		[dbo].[CompanyCarrier]
	WHERE ( CarrierId = @CarrierId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetCompanyCarrierMaximumId    Script Date: 1/14/2026 4:18:10 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyCarrierMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyCarrierMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyCarrierMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[CompanyCarrier]

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

/****** Object:  StoredProcedure [dbo].GetCompanyCarrierRowCount    Script Date: 1/14/2026 4:18:10 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyCarrierRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyCarrierRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyCarrierRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[CompanyCarrier]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedCompanyCarrier    Script Date: 1/14/2026 4:18:10 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedCompanyCarrier]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedCompanyCarrier]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedCompanyCarrier
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

SET @SQL1 = 'WITH CompanyCarrierEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[CompanyId],
	[CarrierId],
	[ApiKey],
	[ApiSecret],
	[IsActive]
				FROM 
				[dbo].[CompanyCarrier]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[CompanyId],
	[CarrierId],
	[ApiKey],
	[ApiSecret],
	[IsActive]
				FROM 
					CompanyCarrierEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[CompanyCarrier] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetCompanyCarrierByQuery    Script Date: 1/14/2026 4:18:10 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyCarrierByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyCarrierByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyCarrierByQuery
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
				[dbo].[CompanyCarrier] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

