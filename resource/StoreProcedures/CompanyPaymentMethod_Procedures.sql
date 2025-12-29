USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertCompanyPaymentMethod    Script Date: 12/21/2025 8:58:36 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertCompanyPaymentMethod]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertCompanyPaymentMethod]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertCompanyPaymentMethod
(
	@Id int OUTPUT,
	@CompanyId int,
	@PaymentMethodId int,
	@IsActive bit,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime,
	@CustomInstruction nvarchar(max),
	@IsManualEnabled bit,
	@IsGatewayEnabled bit
)
AS
    INSERT INTO [dbo].[CompanyPaymentMethod] 
	(
	[CompanyId],
	[PaymentMethodId],
	[IsActive],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[CustomInstruction],
	[IsManualEnabled],
	[IsGatewayEnabled]
    ) 
	VALUES 
	(
	@CompanyId,
	@PaymentMethodId,
	@IsActive,
	@CreatedBy,
	@CreatedAt,
	@UpdatedBy,
	@UpdatedAt,
	@CustomInstruction,
	@IsManualEnabled,
	@IsGatewayEnabled
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

/****** Object:  StoredProcedure [dbo].UpdateCompanyPaymentMethod    Script Date: 12/21/2025 8:58:36 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateCompanyPaymentMethod]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateCompanyPaymentMethod]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateCompanyPaymentMethod
(
	@Id int,
	@CompanyId int,
	@PaymentMethodId int,
	@IsActive bit,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime,
	@CustomInstruction nvarchar(max),
	@IsManualEnabled bit,
	@IsGatewayEnabled bit
)
AS
    UPDATE [dbo].[CompanyPaymentMethod] 
	SET
	[CompanyId] = @CompanyId,
	[PaymentMethodId] = @PaymentMethodId,
	[IsActive] = @IsActive,
	[CreatedBy] = @CreatedBy,
	[CreatedAt] = @CreatedAt,
	[UpdatedBy] = @UpdatedBy,
	[UpdatedAt] = @UpdatedAt,
	[CustomInstruction] = @CustomInstruction,
	[IsManualEnabled] = @IsManualEnabled,
	[IsGatewayEnabled] = @IsGatewayEnabled
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

/****** Object:  StoredProcedure [dbo].DeleteCompanyPaymentMethod    Script Date: 12/21/2025 8:58:36 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteCompanyPaymentMethod]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteCompanyPaymentMethod]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteCompanyPaymentMethod
(
	@Id int
)
AS
	DELETE [dbo].[CompanyPaymentMethod] 

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

/****** Object:  StoredProcedure [dbo].GetAllCompanyPaymentMethod    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllCompanyPaymentMethod]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllCompanyPaymentMethod]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllCompanyPaymentMethod
AS
	SELECT *		
	FROM
		[dbo].[CompanyPaymentMethod]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetCompanyPaymentMethodById    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyPaymentMethodById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyPaymentMethodById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyPaymentMethodById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[CompanyPaymentMethod]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllCompanyPaymentMethodByCompanyId    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyPaymentMethodByCompanyId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyPaymentMethodByCompanyId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyPaymentMethodByCompanyId
(
	@CompanyId int
)
AS
	SELECT *		
	FROM
		[dbo].[CompanyPaymentMethod]
	WHERE ( CompanyId = @CompanyId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllCompanyPaymentMethodByPaymentMethodId    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyPaymentMethodByPaymentMethodId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyPaymentMethodByPaymentMethodId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyPaymentMethodByPaymentMethodId
(
	@PaymentMethodId int
)
AS
	SELECT *		
	FROM
		[dbo].[CompanyPaymentMethod]
	WHERE ( PaymentMethodId = @PaymentMethodId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetCompanyPaymentMethodMaximumId    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyPaymentMethodMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyPaymentMethodMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyPaymentMethodMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[CompanyPaymentMethod]

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

/****** Object:  StoredProcedure [dbo].GetCompanyPaymentMethodRowCount    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyPaymentMethodRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyPaymentMethodRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyPaymentMethodRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[CompanyPaymentMethod]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedCompanyPaymentMethod    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedCompanyPaymentMethod]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedCompanyPaymentMethod]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedCompanyPaymentMethod
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

SET @SQL1 = 'WITH CompanyPaymentMethodEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[CompanyId],
	[PaymentMethodId],
	[IsActive],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[CustomInstruction],
	[IsManualEnabled],
	[IsGatewayEnabled]
				FROM 
				[dbo].[CompanyPaymentMethod]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[CompanyId],
	[PaymentMethodId],
	[IsActive],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[CustomInstruction],
	[IsManualEnabled],
	[IsGatewayEnabled]
				FROM 
					CompanyPaymentMethodEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[CompanyPaymentMethod] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetCompanyPaymentMethodByQuery    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanyPaymentMethodByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanyPaymentMethodByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanyPaymentMethodByQuery
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
				[dbo].[CompanyPaymentMethod] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

