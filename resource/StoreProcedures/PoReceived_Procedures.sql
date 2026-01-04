USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertPoReceived    Script Date: 12/21/2025 8:58:38 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertPoReceived]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertPoReceived]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertPoReceived
(
	@Id int OUTPUT,
	@PoRequestedId int,
	@ReceivedQuantity int,
	@BuyingPrice decimal(18, 2),
	@ReceivedDate datetime,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime,
	@Remarks nvarchar(500),
	@InvoiceNo nvarchar(100)
)
AS
    INSERT INTO [dbo].[PoReceived] 
	(
	[PoRequestedId],
	[ReceivedQuantity],
	[BuyingPrice],
	[ReceivedDate],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[Remarks],
	[InvoiceNo]
    ) 
	VALUES 
	(
	@PoRequestedId,
	@ReceivedQuantity,
	@BuyingPrice,
	@ReceivedDate,
	@CreatedBy,
	@CreatedAt,
	@UpdatedBy,
	@UpdatedAt,
	@Remarks,
	@InvoiceNo
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

/****** Object:  StoredProcedure [dbo].UpdatePoReceived    Script Date: 12/21/2025 8:58:38 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdatePoReceived]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdatePoReceived]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdatePoReceived
(
	@Id int,
	@PoRequestedId int,
	@ReceivedQuantity int,
	@BuyingPrice decimal(18, 2),
	@ReceivedDate datetime,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime,
	@Remarks nvarchar(500),
	@InvoiceNo nvarchar(100)
)
AS
    UPDATE [dbo].[PoReceived] 
	SET
	[PoRequestedId] = @PoRequestedId,
	[ReceivedQuantity] = @ReceivedQuantity,
	[BuyingPrice] = @BuyingPrice,
	[ReceivedDate] = @ReceivedDate,
	[CreatedBy] = @CreatedBy,
	[CreatedAt] = @CreatedAt,
	[UpdatedBy] = @UpdatedBy,
	[UpdatedAt] = @UpdatedAt,
	[Remarks] = @Remarks,
	[InvoiceNo] = @InvoiceNo
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

/****** Object:  StoredProcedure [dbo].DeletePoReceived    Script Date: 12/21/2025 8:58:38 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeletePoReceived]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeletePoReceived]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeletePoReceived
(
	@Id int
)
AS
	DELETE [dbo].[PoReceived] 

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

/****** Object:  StoredProcedure [dbo].GetAllPoReceived    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllPoReceived]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllPoReceived]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllPoReceived
AS
	SELECT *		
	FROM
		[dbo].[PoReceived]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetPoReceivedById    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPoReceivedById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPoReceivedById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPoReceivedById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[PoReceived]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllPoReceivedByPoRequestedId    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPoReceivedByPoRequestedId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPoReceivedByPoRequestedId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPoReceivedByPoRequestedId
(
	@PoRequestedId int
)
AS
	SELECT *		
	FROM
		[dbo].[PoReceived]
	WHERE ( PoRequestedId = @PoRequestedId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetPoReceivedMaximumId    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPoReceivedMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPoReceivedMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPoReceivedMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[PoReceived]

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

/****** Object:  StoredProcedure [dbo].GetPoReceivedRowCount    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPoReceivedRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPoReceivedRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPoReceivedRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[PoReceived]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedPoReceived    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedPoReceived]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedPoReceived]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedPoReceived
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

SET @SQL1 = 'WITH PoReceivedEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[PoRequestedId],
	[ReceivedQuantity],
	[BuyingPrice],
	[ReceivedDate],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[Remarks],
	[InvoiceNo]
				FROM 
				[dbo].[PoReceived]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[PoRequestedId],
	[ReceivedQuantity],
	[BuyingPrice],
	[ReceivedDate],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[Remarks],
	[InvoiceNo]
				FROM 
					PoReceivedEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[PoReceived] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetPoReceivedByQuery    Script Date: 12/21/2025 8:58:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPoReceivedByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPoReceivedByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPoReceivedByQuery
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
				[dbo].[PoReceived] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

