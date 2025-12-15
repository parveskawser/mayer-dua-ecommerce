USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertPaymentMethod    Script Date: 12/10/2025 2:32:51 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertPaymentMethod]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertPaymentMethod]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertPaymentMethod
(
	@Id int OUTPUT,
	@Name nvarchar(50),
	@IsActive bit,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime,
	@SystemCode nvarchar(20),
	@LogoUrl nvarchar(500),
	@DisplayOrder int,
	@SupportsManual bit,
	@SupportsGateway bit,
	@ManualInstruction nvarchar(max)
)
AS
    INSERT INTO [dbo].[PaymentMethod] 
	(
	[Name],
	[IsActive],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[SystemCode],
	[LogoUrl],
	[DisplayOrder],
	[SupportsManual],
	[SupportsGateway],
	[ManualInstruction]
    ) 
	VALUES 
	(
	@Name,
	@IsActive,
	@CreatedBy,
	@CreatedAt,
	@UpdatedBy,
	@UpdatedAt,
	@SystemCode,
	@LogoUrl,
	@DisplayOrder,
	@SupportsManual,
	@SupportsGateway,
	@ManualInstruction
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

/****** Object:  StoredProcedure [dbo].UpdatePaymentMethod    Script Date: 12/10/2025 2:32:51 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdatePaymentMethod]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdatePaymentMethod]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdatePaymentMethod
(
	@Id int,
	@Name nvarchar(50),
	@IsActive bit,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime,
	@SystemCode nvarchar(20),
	@LogoUrl nvarchar(500),
	@DisplayOrder int,
	@SupportsManual bit,
	@SupportsGateway bit,
	@ManualInstruction nvarchar(max)
)
AS
    UPDATE [dbo].[PaymentMethod] 
	SET
	[Name] = @Name,
	[IsActive] = @IsActive,
	[CreatedBy] = @CreatedBy,
	[CreatedAt] = @CreatedAt,
	[UpdatedBy] = @UpdatedBy,
	[UpdatedAt] = @UpdatedAt,
	[SystemCode] = @SystemCode,
	[LogoUrl] = @LogoUrl,
	[DisplayOrder] = @DisplayOrder,
	[SupportsManual] = @SupportsManual,
	[SupportsGateway] = @SupportsGateway,
	[ManualInstruction] = @ManualInstruction
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

/****** Object:  StoredProcedure [dbo].DeletePaymentMethod    Script Date: 12/10/2025 2:32:51 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeletePaymentMethod]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeletePaymentMethod]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeletePaymentMethod
(
	@Id int
)
AS
	DELETE [dbo].[PaymentMethod] 

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

/****** Object:  StoredProcedure [dbo].GetAllPaymentMethod    Script Date: 12/10/2025 2:32:51 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllPaymentMethod]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllPaymentMethod]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllPaymentMethod
AS
	SELECT *		
	FROM
		[dbo].[PaymentMethod]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetPaymentMethodById    Script Date: 12/10/2025 2:32:51 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPaymentMethodById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPaymentMethodById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPaymentMethodById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[PaymentMethod]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetPaymentMethodMaximumId    Script Date: 12/10/2025 2:32:51 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPaymentMethodMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPaymentMethodMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPaymentMethodMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[PaymentMethod]

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

/****** Object:  StoredProcedure [dbo].GetPaymentMethodRowCount    Script Date: 12/10/2025 2:32:51 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPaymentMethodRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPaymentMethodRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPaymentMethodRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[PaymentMethod]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedPaymentMethod    Script Date: 12/10/2025 2:32:51 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedPaymentMethod]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedPaymentMethod]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedPaymentMethod
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

SET @SQL1 = 'WITH PaymentMethodEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[Name],
	[IsActive],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[SystemCode],
	[LogoUrl],
	[DisplayOrder],
	[SupportsManual],
	[SupportsGateway],
	[ManualInstruction]
				FROM 
				[dbo].[PaymentMethod]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[Name],
	[IsActive],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[SystemCode],
	[LogoUrl],
	[DisplayOrder],
	[SupportsManual],
	[SupportsGateway],
	[ManualInstruction]
				FROM 
					PaymentMethodEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[PaymentMethod] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetPaymentMethodByQuery    Script Date: 12/10/2025 2:32:51 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPaymentMethodByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPaymentMethodByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPaymentMethodByQuery
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
				[dbo].[PaymentMethod] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

