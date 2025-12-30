USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertPostalCodes    Script Date: 12/29/2025 1:59:59 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertPostalCodes]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertPostalCodes]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertPostalCodes
(
	@Id int OUTPUT,
	@PostCode nvarchar(10),
	@SubOfficeEn nvarchar(100),
	@SubOfficeBn nvarchar(100),
	@ThanaEn nvarchar(100),
	@ThanaBn nvarchar(100),
	@DistrictBn nvarchar(100),
	@DistrictEn nvarchar(100),
	@DivisionBn nvarchar(100),
	@DivisionEn nvarchar(100)
)
AS
    INSERT INTO [dbo].[PostalCodes] 
	(
	[PostCode],
	[SubOfficeEn],
	[SubOfficeBn],
	[ThanaEn],
	[ThanaBn],
	[DistrictBn],
	[DistrictEn],
	[DivisionBn],
	[DivisionEn]
    ) 
	VALUES 
	(
	@PostCode,
	@SubOfficeEn,
	@SubOfficeBn,
	@ThanaEn,
	@ThanaBn,
	@DistrictBn,
	@DistrictEn,
	@DivisionBn,
	@DivisionEn
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

/****** Object:  StoredProcedure [dbo].UpdatePostalCodes    Script Date: 12/29/2025 1:59:59 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdatePostalCodes]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdatePostalCodes]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdatePostalCodes
(
	@Id int,
	@PostCode nvarchar(10),
	@SubOfficeEn nvarchar(100),
	@SubOfficeBn nvarchar(100),
	@ThanaEn nvarchar(100),
	@ThanaBn nvarchar(100),
	@DistrictBn nvarchar(100),
	@DistrictEn nvarchar(100),
	@DivisionBn nvarchar(100),
	@DivisionEn nvarchar(100)
)
AS
    UPDATE [dbo].[PostalCodes] 
	SET
	[PostCode] = @PostCode,
	[SubOfficeEn] = @SubOfficeEn,
	[SubOfficeBn] = @SubOfficeBn,
	[ThanaEn] = @ThanaEn,
	[ThanaBn] = @ThanaBn,
	[DistrictBn] = @DistrictBn,
	[DistrictEn] = @DistrictEn,
	[DivisionBn] = @DivisionBn,
	[DivisionEn] = @DivisionEn
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

/****** Object:  StoredProcedure [dbo].DeletePostalCodes    Script Date: 12/29/2025 1:59:59 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeletePostalCodes]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeletePostalCodes]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeletePostalCodes
(
	@Id int
)
AS
	DELETE [dbo].[PostalCodes] 

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

/****** Object:  StoredProcedure [dbo].GetAllPostalCodes    Script Date: 12/29/2025 1:59:59 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllPostalCodes]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllPostalCodes]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllPostalCodes
AS
	SELECT *		
	FROM
		[dbo].[PostalCodes]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetPostalCodesById    Script Date: 12/29/2025 1:59:59 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPostalCodesById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPostalCodesById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPostalCodesById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[PostalCodes]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetPostalCodesMaximumId    Script Date: 12/29/2025 1:59:59 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPostalCodesMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPostalCodesMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPostalCodesMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[PostalCodes]

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

/****** Object:  StoredProcedure [dbo].GetPostalCodesRowCount    Script Date: 12/29/2025 1:59:59 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPostalCodesRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPostalCodesRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPostalCodesRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[PostalCodes]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedPostalCodes    Script Date: 12/29/2025 1:59:59 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedPostalCodes]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedPostalCodes]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedPostalCodes
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

SET @SQL1 = 'WITH PostalCodesEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[PostCode],
	[SubOfficeEn],
	[SubOfficeBn],
	[ThanaEn],
	[ThanaBn],
	[DistrictBn],
	[DistrictEn],
	[DivisionBn],
	[DivisionEn]
				FROM 
				[dbo].[PostalCodes]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[PostCode],
	[SubOfficeEn],
	[SubOfficeBn],
	[ThanaEn],
	[ThanaBn],
	[DistrictBn],
	[DistrictEn],
	[DivisionBn],
	[DivisionEn]
				FROM 
					PostalCodesEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[PostalCodes] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetPostalCodesByQuery    Script Date: 12/29/2025 1:59:59 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPostalCodesByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPostalCodesByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPostalCodesByQuery
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
				[dbo].[PostalCodes] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

