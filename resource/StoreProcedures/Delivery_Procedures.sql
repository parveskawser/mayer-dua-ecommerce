USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertDelivery    Script Date: 12/21/2025 8:58:37 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertDelivery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertDelivery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertDelivery
(
	@Id int OUTPUT,
	@SalesOrderId int,
	@TrackingNumber nvarchar(100),
	@CarrierName nvarchar(100),
	@ShipDate datetime,
	@EstimatedArrival datetime,
	@ActualDeliveryDate datetime,
	@Status nvarchar(50),
	@ShippingCost decimal(18, 2),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    INSERT INTO [dbo].[Delivery] 
	(
	[SalesOrderId],
	[TrackingNumber],
	[CarrierName],
	[ShipDate],
	[EstimatedArrival],
	[ActualDeliveryDate],
	[Status],
	[ShippingCost],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
    ) 
	VALUES 
	(
	@SalesOrderId,
	@TrackingNumber,
	@CarrierName,
	@ShipDate,
	@EstimatedArrival,
	@ActualDeliveryDate,
	@Status,
	@ShippingCost,
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

/****** Object:  StoredProcedure [dbo].UpdateDelivery    Script Date: 12/21/2025 8:58:37 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateDelivery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateDelivery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateDelivery
(
	@Id int,
	@SalesOrderId int,
	@TrackingNumber nvarchar(100),
	@CarrierName nvarchar(100),
	@ShipDate datetime,
	@EstimatedArrival datetime,
	@ActualDeliveryDate datetime,
	@Status nvarchar(50),
	@ShippingCost decimal(18, 2),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    UPDATE [dbo].[Delivery] 
	SET
	[SalesOrderId] = @SalesOrderId,
	[TrackingNumber] = @TrackingNumber,
	[CarrierName] = @CarrierName,
	[ShipDate] = @ShipDate,
	[EstimatedArrival] = @EstimatedArrival,
	[ActualDeliveryDate] = @ActualDeliveryDate,
	[Status] = @Status,
	[ShippingCost] = @ShippingCost,
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

/****** Object:  StoredProcedure [dbo].DeleteDelivery    Script Date: 12/21/2025 8:58:37 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteDelivery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteDelivery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteDelivery
(
	@Id int
)
AS
	DELETE [dbo].[Delivery] 

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

/****** Object:  StoredProcedure [dbo].GetAllDelivery    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllDelivery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllDelivery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllDelivery
AS
	SELECT *		
	FROM
		[dbo].[Delivery]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetDeliveryById    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDeliveryById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDeliveryById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetDeliveryById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[Delivery]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllDeliveryBySalesOrderId    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDeliveryBySalesOrderId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDeliveryBySalesOrderId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetDeliveryBySalesOrderId
(
	@SalesOrderId int
)
AS
	SELECT *		
	FROM
		[dbo].[Delivery]
	WHERE ( SalesOrderId = @SalesOrderId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetDeliveryMaximumId    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDeliveryMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDeliveryMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetDeliveryMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[Delivery]

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

/****** Object:  StoredProcedure [dbo].GetDeliveryRowCount    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDeliveryRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDeliveryRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetDeliveryRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[Delivery]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedDelivery    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedDelivery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedDelivery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedDelivery
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

SET @SQL1 = 'WITH DeliveryEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[SalesOrderId],
	[TrackingNumber],
	[CarrierName],
	[ShipDate],
	[EstimatedArrival],
	[ActualDeliveryDate],
	[Status],
	[ShippingCost],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
				[dbo].[Delivery]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[SalesOrderId],
	[TrackingNumber],
	[CarrierName],
	[ShipDate],
	[EstimatedArrival],
	[ActualDeliveryDate],
	[Status],
	[ShippingCost],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
					DeliveryEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[Delivery] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetDeliveryByQuery    Script Date: 12/21/2025 8:58:37 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDeliveryByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDeliveryByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetDeliveryByQuery
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
				[dbo].[Delivery] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

