USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertSalesOrderDetail    Script Date: 12/29/2025 2:00:01 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertSalesOrderDetail]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertSalesOrderDetail]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertSalesOrderDetail
(
	@Id int OUTPUT,
	@SalesOrderId int,
	@ProductId int,
	@Quantity int,
	@UnitPrice decimal(18, 2),
	@LineTotal decimal(29, 2),
	@ProfitAmount decimal(18, 2),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    INSERT INTO [dbo].[SalesOrderDetail] 
	(
	[SalesOrderId],
	[ProductId],
	[Quantity],
	[UnitPrice],
	[LineTotal],
	[ProfitAmount],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
    ) 
	VALUES 
	(
	@SalesOrderId,
	@ProductId,
	@Quantity,
	@UnitPrice,
	@LineTotal,
	@ProfitAmount,
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

/****** Object:  StoredProcedure [dbo].UpdateSalesOrderDetail    Script Date: 12/29/2025 2:00:01 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateSalesOrderDetail]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateSalesOrderDetail]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateSalesOrderDetail
(
	@Id int,
	@SalesOrderId int,
	@ProductId int,
	@Quantity int,
	@UnitPrice decimal(18, 2),
	@LineTotal decimal(29, 2),
	@ProfitAmount decimal(18, 2),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    UPDATE [dbo].[SalesOrderDetail] 
	SET
	[SalesOrderId] = @SalesOrderId,
	[ProductId] = @ProductId,
	[Quantity] = @Quantity,
	[UnitPrice] = @UnitPrice,
	[LineTotal] = @LineTotal,
	[ProfitAmount] = @ProfitAmount,
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

/****** Object:  StoredProcedure [dbo].DeleteSalesOrderDetail    Script Date: 12/29/2025 2:00:01 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteSalesOrderDetail]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteSalesOrderDetail]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteSalesOrderDetail
(
	@Id int
)
AS
	DELETE [dbo].[SalesOrderDetail] 

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

/****** Object:  StoredProcedure [dbo].GetAllSalesOrderDetail    Script Date: 12/29/2025 2:00:01 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllSalesOrderDetail]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllSalesOrderDetail]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllSalesOrderDetail
AS
	SELECT *		
	FROM
		[dbo].[SalesOrderDetail]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetSalesOrderDetailById    Script Date: 12/29/2025 2:00:01 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSalesOrderDetailById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSalesOrderDetailById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSalesOrderDetailById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[SalesOrderDetail]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllSalesOrderDetailBySalesOrderId    Script Date: 12/29/2025 2:00:01 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSalesOrderDetailBySalesOrderId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSalesOrderDetailBySalesOrderId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSalesOrderDetailBySalesOrderId
(
	@SalesOrderId int
)
AS
	SELECT *		
	FROM
		[dbo].[SalesOrderDetail]
	WHERE ( SalesOrderId = @SalesOrderId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllSalesOrderDetailByProductId    Script Date: 12/29/2025 2:00:01 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSalesOrderDetailByProductId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSalesOrderDetailByProductId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSalesOrderDetailByProductId
(
	@ProductId int
)
AS
	SELECT *		
	FROM
		[dbo].[SalesOrderDetail]
	WHERE ( ProductId = @ProductId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetSalesOrderDetailMaximumId    Script Date: 12/29/2025 2:00:01 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSalesOrderDetailMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSalesOrderDetailMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSalesOrderDetailMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[SalesOrderDetail]

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

/****** Object:  StoredProcedure [dbo].GetSalesOrderDetailRowCount    Script Date: 12/29/2025 2:00:01 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSalesOrderDetailRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSalesOrderDetailRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSalesOrderDetailRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[SalesOrderDetail]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedSalesOrderDetail    Script Date: 12/29/2025 2:00:01 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedSalesOrderDetail]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedSalesOrderDetail]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedSalesOrderDetail
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

SET @SQL1 = 'WITH SalesOrderDetailEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[SalesOrderId],
	[ProductId],
	[Quantity],
	[UnitPrice],
	[LineTotal],
	[ProfitAmount],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
				[dbo].[SalesOrderDetail]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[SalesOrderId],
	[ProductId],
	[Quantity],
	[UnitPrice],
	[LineTotal],
	[ProfitAmount],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
					SalesOrderDetailEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[SalesOrderDetail] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetSalesOrderDetailByQuery    Script Date: 12/29/2025 2:00:01 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSalesOrderDetailByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSalesOrderDetailByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSalesOrderDetailByQuery
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
				[dbo].[SalesOrderDetail] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

