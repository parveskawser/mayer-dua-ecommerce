USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertSalesReturn    Script Date: 12/21/2025 8:58:40 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertSalesReturn]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertSalesReturn]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertSalesReturn
(
	@Id int OUTPUT,
	@SalesOrderDetailId int,
	@ReturnDate datetime,
	@Quantity int,
	@Reason nvarchar(50),
	@RestockToInventory bit,
	@RefundAmount decimal(18, 2),
	@Status nvarchar(20),
	@Remarks nvarchar(500),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    INSERT INTO [dbo].[SalesReturn] 
	(
	[SalesOrderDetailId],
	[ReturnDate],
	[Quantity],
	[Reason],
	[RestockToInventory],
	[RefundAmount],
	[Status],
	[Remarks],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
    ) 
	VALUES 
	(
	@SalesOrderDetailId,
	@ReturnDate,
	@Quantity,
	@Reason,
	@RestockToInventory,
	@RefundAmount,
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

/****** Object:  StoredProcedure [dbo].UpdateSalesReturn    Script Date: 12/21/2025 8:58:40 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateSalesReturn]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateSalesReturn]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateSalesReturn
(
	@Id int,
	@SalesOrderDetailId int,
	@ReturnDate datetime,
	@Quantity int,
	@Reason nvarchar(50),
	@RestockToInventory bit,
	@RefundAmount decimal(18, 2),
	@Status nvarchar(20),
	@Remarks nvarchar(500),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    UPDATE [dbo].[SalesReturn] 
	SET
	[SalesOrderDetailId] = @SalesOrderDetailId,
	[ReturnDate] = @ReturnDate,
	[Quantity] = @Quantity,
	[Reason] = @Reason,
	[RestockToInventory] = @RestockToInventory,
	[RefundAmount] = @RefundAmount,
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

/****** Object:  StoredProcedure [dbo].DeleteSalesReturn    Script Date: 12/21/2025 8:58:40 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteSalesReturn]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteSalesReturn]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteSalesReturn
(
	@Id int
)
AS
	DELETE [dbo].[SalesReturn] 

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

/****** Object:  StoredProcedure [dbo].GetAllSalesReturn    Script Date: 12/21/2025 8:58:40 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllSalesReturn]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllSalesReturn]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllSalesReturn
AS
	SELECT *		
	FROM
		[dbo].[SalesReturn]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetSalesReturnById    Script Date: 12/21/2025 8:58:40 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSalesReturnById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSalesReturnById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSalesReturnById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[SalesReturn]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllSalesReturnBySalesOrderDetailId    Script Date: 12/21/2025 8:58:40 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSalesReturnBySalesOrderDetailId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSalesReturnBySalesOrderDetailId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSalesReturnBySalesOrderDetailId
(
	@SalesOrderDetailId int
)
AS
	SELECT *		
	FROM
		[dbo].[SalesReturn]
	WHERE ( SalesOrderDetailId = @SalesOrderDetailId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetSalesReturnMaximumId    Script Date: 12/21/2025 8:58:40 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSalesReturnMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSalesReturnMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSalesReturnMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[SalesReturn]

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

/****** Object:  StoredProcedure [dbo].GetSalesReturnRowCount    Script Date: 12/21/2025 8:58:40 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSalesReturnRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSalesReturnRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSalesReturnRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[SalesReturn]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedSalesReturn    Script Date: 12/21/2025 8:58:40 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedSalesReturn]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedSalesReturn]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedSalesReturn
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

SET @SQL1 = 'WITH SalesReturnEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[SalesOrderDetailId],
	[ReturnDate],
	[Quantity],
	[Reason],
	[RestockToInventory],
	[RefundAmount],
	[Status],
	[Remarks],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
				[dbo].[SalesReturn]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[SalesOrderDetailId],
	[ReturnDate],
	[Quantity],
	[Reason],
	[RestockToInventory],
	[RefundAmount],
	[Status],
	[Remarks],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
					SalesReturnEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[SalesReturn] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetSalesReturnByQuery    Script Date: 12/21/2025 8:58:40 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSalesReturnByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSalesReturnByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSalesReturnByQuery
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
				[dbo].[SalesReturn] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

