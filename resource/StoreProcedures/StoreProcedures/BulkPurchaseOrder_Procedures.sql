USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertBulkPurchaseOrder    Script Date: 12/21/2025 8:58:36 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertBulkPurchaseOrder]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertBulkPurchaseOrder]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertBulkPurchaseOrder
(
	@Id int OUTPUT,
	@VendorId int,
	@AgreementNumber nvarchar(50),
	@Title nvarchar(200),
	@AgreementDate datetime,
	@ExpiryDate datetime,
	@TotalTargetQuantity int,
	@TotalTargetAmount decimal(18, 2),
	@Status nvarchar(20),
	@Remarks nvarchar(500),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    INSERT INTO [dbo].[BulkPurchaseOrder] 
	(
	[VendorId],
	[AgreementNumber],
	[Title],
	[AgreementDate],
	[ExpiryDate],
	[TotalTargetQuantity],
	[TotalTargetAmount],
	[Status],
	[Remarks],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
    ) 
	VALUES 
	(
	@VendorId,
	@AgreementNumber,
	@Title,
	@AgreementDate,
	@ExpiryDate,
	@TotalTargetQuantity,
	@TotalTargetAmount,
	@Status,
	@Remarks,
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

/****** Object:  StoredProcedure [dbo].UpdateBulkPurchaseOrder    Script Date: 12/21/2025 8:58:36 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateBulkPurchaseOrder]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateBulkPurchaseOrder]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateBulkPurchaseOrder
(
	@Id int,
	@VendorId int,
	@AgreementNumber nvarchar(50),
	@Title nvarchar(200),
	@AgreementDate datetime,
	@ExpiryDate datetime,
	@TotalTargetQuantity int,
	@TotalTargetAmount decimal(18, 2),
	@Status nvarchar(20),
	@Remarks nvarchar(500),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    UPDATE [dbo].[BulkPurchaseOrder] 
	SET
	[VendorId] = @VendorId,
	[AgreementNumber] = @AgreementNumber,
	[Title] = @Title,
	[AgreementDate] = @AgreementDate,
	[ExpiryDate] = @ExpiryDate,
	[TotalTargetQuantity] = @TotalTargetQuantity,
	[TotalTargetAmount] = @TotalTargetAmount,
	[Status] = @Status,
	[Remarks] = @Remarks,
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

/****** Object:  StoredProcedure [dbo].DeleteBulkPurchaseOrder    Script Date: 12/21/2025 8:58:36 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteBulkPurchaseOrder]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteBulkPurchaseOrder]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteBulkPurchaseOrder
(
	@Id int
)
AS
	DELETE [dbo].[BulkPurchaseOrder] 

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

/****** Object:  StoredProcedure [dbo].GetAllBulkPurchaseOrder    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllBulkPurchaseOrder]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllBulkPurchaseOrder]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllBulkPurchaseOrder
AS
	SELECT *		
	FROM
		[dbo].[BulkPurchaseOrder]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetBulkPurchaseOrderById    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetBulkPurchaseOrderById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetBulkPurchaseOrderById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetBulkPurchaseOrderById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[BulkPurchaseOrder]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllBulkPurchaseOrderByVendorId    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetBulkPurchaseOrderByVendorId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetBulkPurchaseOrderByVendorId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetBulkPurchaseOrderByVendorId
(
	@VendorId int
)
AS
	SELECT *		
	FROM
		[dbo].[BulkPurchaseOrder]
	WHERE ( VendorId = @VendorId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetBulkPurchaseOrderMaximumId    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetBulkPurchaseOrderMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetBulkPurchaseOrderMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetBulkPurchaseOrderMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[BulkPurchaseOrder]

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

/****** Object:  StoredProcedure [dbo].GetBulkPurchaseOrderRowCount    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetBulkPurchaseOrderRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetBulkPurchaseOrderRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetBulkPurchaseOrderRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[BulkPurchaseOrder]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedBulkPurchaseOrder    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedBulkPurchaseOrder]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedBulkPurchaseOrder]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedBulkPurchaseOrder
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

SET @SQL1 = 'WITH BulkPurchaseOrderEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[VendorId],
	[AgreementNumber],
	[Title],
	[AgreementDate],
	[ExpiryDate],
	[TotalTargetQuantity],
	[TotalTargetAmount],
	[Status],
	[Remarks],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
				[dbo].[BulkPurchaseOrder]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[VendorId],
	[AgreementNumber],
	[Title],
	[AgreementDate],
	[ExpiryDate],
	[TotalTargetQuantity],
	[TotalTargetAmount],
	[Status],
	[Remarks],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
					BulkPurchaseOrderEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[BulkPurchaseOrder] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetBulkPurchaseOrderByQuery    Script Date: 12/21/2025 8:58:36 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetBulkPurchaseOrderByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetBulkPurchaseOrderByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetBulkPurchaseOrderByQuery
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
				[dbo].[BulkPurchaseOrder] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

