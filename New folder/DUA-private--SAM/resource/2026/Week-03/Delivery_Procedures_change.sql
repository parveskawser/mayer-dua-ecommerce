

-- 1. Drop existing default constraint for CreatedAt if it exists
DECLARE @ConstraintName nvarchar(200)
SELECT @ConstraintName = Name FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID('Delivery') AND parent_column_id = (SELECT column_id FROM sys.columns WHERE object_id = OBJECT_ID('Delivery') AND name = 'CreatedAt')
IF @ConstraintName IS NOT NULL
EXEC('ALTER TABLE [dbo].[Delivery] DROP CONSTRAINT ' + @ConstraintName)
GO

-- 2. Re-add default as UTC
ALTER TABLE [dbo].[Delivery] ADD DEFAULT (GETUTCDATE()) FOR [CreatedAt]
GO



/****** Object:  StoredProcedure [dbo].[InsertDelivery] ******/
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertDelivery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertDelivery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InsertDelivery]
(
	@Id int OUTPUT,
	@SalesOrderId int,
	@TrackingNumber nvarchar(100),
	@ShipDate datetime,
	@EstimatedArrival datetime,
	@ActualDeliveryDate datetime,
	@Status nvarchar(50),
	@ShippingCost decimal(18, 2),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime,
	@CarrierCharge decimal(18, 2),
	@PackageWeightGrams int,
	@CarrierResponse nvarchar(max),
	@ConsignmentId nvarchar(100),
	@CarrierId int
)
AS
BEGIN
    -- Force UTC if not provided
    IF @CreatedAt IS NULL SET @CreatedAt = GETUTCDATE();

    INSERT INTO [dbo].[Delivery] 
	(
        [SalesOrderId], [TrackingNumber], [ShipDate], [EstimatedArrival], [ActualDeliveryDate], 
        [Status], [ShippingCost], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt], 
        [CarrierCharge], [PackageWeightGrams], [CarrierResponse], [ConsignmentId], [CarrierId]
    ) 
	VALUES 
	(
        @SalesOrderId, @TrackingNumber, @ShipDate, @EstimatedArrival, @ActualDeliveryDate, 
        @Status, @ShippingCost, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, 
        @CarrierCharge, @PackageWeightGrams, @CarrierResponse, @ConsignmentId, @CarrierId
    );

    -- ✅ FIX: Use SCOPE_IDENTITY() for safety
	SET @Id = CAST(SCOPE_IDENTITY() AS INT);
	RETURN @Id;
END
GO

/****** Object:  StoredProcedure [dbo].[UpdateDelivery] ******/
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateDelivery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateDelivery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdateDelivery]
(
	@Id int,
	@SalesOrderId int,
	@TrackingNumber nvarchar(100),
	@ShipDate datetime,
	@EstimatedArrival datetime,
	@ActualDeliveryDate datetime,
	@Status nvarchar(50),
	@ShippingCost decimal(18, 2),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime,
	@CarrierCharge decimal(18, 2),
	@PackageWeightGrams int,
	@CarrierResponse nvarchar(max),
	@ConsignmentId nvarchar(100),
	@CarrierId int
)
AS
BEGIN
    UPDATE [dbo].[Delivery] 
	SET
        [SalesOrderId] = @SalesOrderId,
        [TrackingNumber] = @TrackingNumber,
        [ShipDate] = @ShipDate,
        [EstimatedArrival] = @EstimatedArrival,
        [ActualDeliveryDate] = @ActualDeliveryDate,
        [Status] = @Status,
        [ShippingCost] = @ShippingCost,
        [CreatedBy] = @CreatedBy,
        [CreatedAt] = @CreatedAt,
        [UpdatedBy] = @UpdatedBy,
        [UpdatedAt] = @UpdatedAt,
        [CarrierCharge] = @CarrierCharge,
        [PackageWeightGrams] = @PackageWeightGrams,
        [CarrierResponse] = @CarrierResponse,
        [ConsignmentId] = @ConsignmentId,
        [CarrierId] = @CarrierId
	WHERE ( Id = @Id );

	RETURN @@ROWCOUNT;
END
GO

/****** Object:  StoredProcedure [dbo].[GetDeliveryById] ******/
-- ✅ FIX: Joins CompanyCarrier -> Carrier to get the Name
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDeliveryById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDeliveryById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetDeliveryById]
(
	@Id int
)
AS
BEGIN
	SELECT 
        d.*,
        c.CarrierName, -- This gets the name (e.g., 'RedX')
        cc.ApiKey      -- Optional: if you need the key
	FROM [dbo].[Delivery] d
    LEFT JOIN [dbo].[CompanyCarrier] cc ON d.CarrierId = cc.Id
    LEFT JOIN [dbo].[Carrier] c ON cc.CarrierId = c.Id
	WHERE d.Id = @Id;

    RETURN @@ROWCOUNT;
END
GO

/****** Object:  StoredProcedure [dbo].[GetAllDelivery] ******/
-- ✅ FIX: Joins CompanyCarrier -> Carrier to get the Name
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllDelivery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllDelivery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAllDelivery]
AS
BEGIN
	SELECT 
        d.*,
        c.CarrierName
	FROM [dbo].[Delivery] d
    LEFT JOIN [dbo].[CompanyCarrier] cc ON d.CarrierId = cc.Id
    LEFT JOIN [dbo].[Carrier] c ON cc.CarrierId = c.Id
    ORDER BY d.Id DESC;

    RETURN @@ROWCOUNT;
END
GO

/****** Object:  StoredProcedure [dbo].[GetPagedDelivery] ******/
-- ✅ FIX: Uses JOIN inside the CTE to allow filtering/sorting by Carrier Name if needed
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedDelivery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedDelivery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetPagedDelivery]
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

    SET @PageIndex = ISNULL(@PageIndex, -1);
    SET @RowPerPage = ISNULL(@RowPerPage, -1);
    SET @WhereClause = ISNULL(@WhereClause, '');
    SET @SortColumn = ISNULL(@SortColumn, '');
    SET @SortOrder = ISNULL(@SortOrder, '');
    SET @TotalRows = 0;
    SET @RowPerPage = @RowPerPage - 1;

    DECLARE @SQL1 nvarchar(MAX);
    DECLARE @SQL2 nvarchar(MAX);

    IF (@WhereClause != '')
    BEGIN
	    SET @WhereClause = 'WHERE ' + CHAR(13) + @WhereClause	
    END

    IF (@SortColumn != '')
    BEGIN
	    SET @SortColumn = 'ORDER BY ' + @SortColumn
	    IF (@SortOrder != '') SET @SortColumn = @SortColumn + ' ' + @SortOrder
    END
    ELSE
    BEGIN
	    SET @SortColumn = 'ORDER BY d.[Id] DESC' -- Default to newest first
    END

    -- Dynamic SQL with Joins to get Carrier Name
    SET @SQL1 = 'WITH DeliveryEntries AS (
			    SELECT ROW_NUMBER() OVER ('+ @SortColumn +') AS Row,
                d.*,
                c.CarrierName
			    FROM [dbo].[Delivery] d
                LEFT JOIN [dbo].[CompanyCarrier] cc ON d.CarrierId = cc.Id
                LEFT JOIN [dbo].[Carrier] c ON cc.CarrierId = c.Id
			    '+ @WhereClause +'
			)
			SELECT * FROM DeliveryEntries
			WHERE Row BETWEEN '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +' AND ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + @RowPerPage + 1) +')';
	
    -- Count Total Rows
    SET @SQL2 = 'SELECT @TotalRows = COUNT(*) FROM [dbo].[Delivery] d 
                 LEFT JOIN [dbo].[CompanyCarrier] cc ON d.CarrierId = cc.Id 
                 LEFT JOIN [dbo].[Carrier] c ON cc.CarrierId = c.Id ' + @WhereClause;
								
    EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows OUTPUT;
    EXEC sp_executesql @SQL1;

    RETURN @@ROWCOUNT;
END
GO