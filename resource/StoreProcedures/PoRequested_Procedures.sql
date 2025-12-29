USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertPoRequested    Script Date: 12/21/2025 8:58:38 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertPoRequested]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertPoRequested]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertPoRequested
(
	@Id int OUTPUT,
	@VendorId int,
	@Quantity int,
	@RequestDate datetime,
	@Status nvarchar(50),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime,
	@Remarks nvarchar(500),
	@ReferenceNo nvarchar(100),
	@ProductVariantId int,
	@BulkPurchaseOrderId int
)
AS
    INSERT INTO [dbo].[PoRequested] 
	(
	[VendorId],
	[Quantity],
	[RequestDate],
	[Status],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[Remarks],
	[ReferenceNo],
	[ProductVariantId],
	[BulkPurchaseOrderId]
    ) 
	VALUES 
	(
	@VendorId,
	@Quantity,
	@RequestDate,
	@Status,
	@CreatedBy,
	@CreatedAt,
	@UpdatedBy,
	@UpdatedAt,
	@Remarks,
	@ReferenceNo,
	@ProductVariantId,
	@BulkPurchaseOrderId
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

/****** Object:  StoredProcedure [dbo].UpdatePoRequested    Script Date: 12/21/2025 8:58:38 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdatePoRequested]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdatePoRequested]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdatePoRequested
(
	@Id int,
	@VendorId int,
	@Quantity int,
	@RequestDate datetime,
	@Status nvarchar(50),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime,
	@Remarks nvarchar(500),
	@ReferenceNo nvarchar(100),
	@ProductVariantId int,
	@BulkPurchaseOrderId int
)
AS
    UPDATE [dbo].[PoRequested] 
	SET
	[VendorId] = @VendorId,
	[Quantity] = @Quantity,
	[RequestDate] = @RequestDate,
	[Status] = @Status,
	[CreatedBy] = @CreatedBy,
	[CreatedAt] = @CreatedAt,
	[UpdatedBy] = @UpdatedBy,
	[UpdatedAt] = @UpdatedAt,
	[Remarks] = @Remarks,
	[ReferenceNo] = @ReferenceNo,
	[ProductVariantId] = @ProductVariantId,
	[BulkPurchaseOrderId] = @BulkPurchaseOrderId
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

/****** Object:  StoredProcedure [dbo].DeletePoRequested    Script Date: 12/21/2025 8:58:38 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeletePoRequested]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeletePoRequested]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeletePoRequested
(
	@Id int
)
AS
	DELETE [dbo].[PoRequested] 

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

/****** Object:  StoredProcedure [dbo].GetAllPoRequested    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllPoRequested]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllPoRequested]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllPoRequested
AS
	SELECT *		
	FROM
		[dbo].[PoRequested]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetPoRequestedById    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPoRequestedById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPoRequestedById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPoRequestedById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[PoRequested]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllPoRequestedByVendorId    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPoRequestedByVendorId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPoRequestedByVendorId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPoRequestedByVendorId
(
	@VendorId int
)
AS
	SELECT *		
	FROM
		[dbo].[PoRequested]
	WHERE ( VendorId = @VendorId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllPoRequestedByProductVariantId    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPoRequestedByProductVariantId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPoRequestedByProductVariantId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPoRequestedByProductVariantId
(
	@ProductVariantId int
)
AS
	SELECT *		
	FROM
		[dbo].[PoRequested]
	WHERE ( ProductVariantId = @ProductVariantId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllPoRequestedByBulkPurchaseOrderId    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPoRequestedByBulkPurchaseOrderId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPoRequestedByBulkPurchaseOrderId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPoRequestedByBulkPurchaseOrderId
(
	@BulkPurchaseOrderId int
)
AS
	SELECT *		
	FROM
		[dbo].[PoRequested]
	WHERE ( BulkPurchaseOrderId = @BulkPurchaseOrderId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetPoRequestedMaximumId    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPoRequestedMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPoRequestedMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPoRequestedMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[PoRequested]

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

/****** Object:  StoredProcedure [dbo].GetPoRequestedRowCount    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPoRequestedRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPoRequestedRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPoRequestedRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[PoRequested]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedPoRequested    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedPoRequested]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedPoRequested]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedPoRequested
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

SET @SQL1 = 'WITH PoRequestedEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[VendorId],
	[Quantity],
	[RequestDate],
	[Status],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[Remarks],
	[ReferenceNo],
	[ProductVariantId],
	[BulkPurchaseOrderId]
				FROM 
				[dbo].[PoRequested]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[VendorId],
	[Quantity],
	[RequestDate],
	[Status],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[Remarks],
	[ReferenceNo],
	[ProductVariantId],
	[BulkPurchaseOrderId]
				FROM 
					PoRequestedEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[PoRequested] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetPoRequestedByQuery    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPoRequestedByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPoRequestedByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPoRequestedByQuery
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
				[dbo].[PoRequested] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

