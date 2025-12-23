USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertDeliveryItem    Script Date: 12/21/2025 8:58:37 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertDeliveryItem]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertDeliveryItem]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertDeliveryItem
(
	@Id int OUTPUT,
	@DeliveryId int,
	@SalesOrderDetailId int,
	@Quantity int
)
AS
    INSERT INTO [dbo].[DeliveryItem] 
	(
	[DeliveryId],
	[SalesOrderDetailId],
	[Quantity]
    ) 
	VALUES 
	(
	@DeliveryId,
	@SalesOrderDetailId,
	@Quantity
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

/****** Object:  StoredProcedure [dbo].UpdateDeliveryItem    Script Date: 12/21/2025 8:58:37 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateDeliveryItem]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateDeliveryItem]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateDeliveryItem
(
	@Id int,
	@DeliveryId int,
	@SalesOrderDetailId int,
	@Quantity int
)
AS
    UPDATE [dbo].[DeliveryItem] 
	SET
	[DeliveryId] = @DeliveryId,
	[SalesOrderDetailId] = @SalesOrderDetailId,
	[Quantity] = @Quantity
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

/****** Object:  StoredProcedure [dbo].DeleteDeliveryItem    Script Date: 12/21/2025 8:58:37 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteDeliveryItem]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteDeliveryItem]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteDeliveryItem
(
	@Id int
)
AS
	DELETE [dbo].[DeliveryItem] 

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

/****** Object:  StoredProcedure [dbo].GetAllDeliveryItem    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllDeliveryItem]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllDeliveryItem]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllDeliveryItem
AS
	SELECT *		
	FROM
		[dbo].[DeliveryItem]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetDeliveryItemById    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDeliveryItemById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDeliveryItemById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetDeliveryItemById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[DeliveryItem]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllDeliveryItemByDeliveryId    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDeliveryItemByDeliveryId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDeliveryItemByDeliveryId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetDeliveryItemByDeliveryId
(
	@DeliveryId int
)
AS
	SELECT *		
	FROM
		[dbo].[DeliveryItem]
	WHERE ( DeliveryId = @DeliveryId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllDeliveryItemBySalesOrderDetailId    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDeliveryItemBySalesOrderDetailId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDeliveryItemBySalesOrderDetailId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetDeliveryItemBySalesOrderDetailId
(
	@SalesOrderDetailId int
)
AS
	SELECT *		
	FROM
		[dbo].[DeliveryItem]
	WHERE ( SalesOrderDetailId = @SalesOrderDetailId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetDeliveryItemMaximumId    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDeliveryItemMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDeliveryItemMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetDeliveryItemMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[DeliveryItem]

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

/****** Object:  StoredProcedure [dbo].GetDeliveryItemRowCount    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDeliveryItemRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDeliveryItemRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetDeliveryItemRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[DeliveryItem]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedDeliveryItem    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedDeliveryItem]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedDeliveryItem]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedDeliveryItem
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

SET @SQL1 = 'WITH DeliveryItemEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[DeliveryId],
	[SalesOrderDetailId],
	[Quantity]
				FROM 
				[dbo].[DeliveryItem]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[DeliveryId],
	[SalesOrderDetailId],
	[Quantity]
				FROM 
					DeliveryItemEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[DeliveryItem] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetDeliveryItemByQuery    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDeliveryItemByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDeliveryItemByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetDeliveryItemByQuery
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
				[dbo].[DeliveryItem] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

