USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertSalesOrderHeader    Script Date: 12/10/2025 2:32:53 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertSalesOrderHeader]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertSalesOrderHeader]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertSalesOrderHeader
(
	@Id int OUTPUT,
	@CompanyCustomerId int,
	@AddressId int,
	@SalesChannelId int,
	@OnlineOrderId varchar(10),
	@DirectOrderId varchar(10),
	@OrderDate datetime,
	@TotalAmount decimal(18, 2),
	@DiscountAmount decimal(18, 2),
	@NetAmount decimal(19, 2),
	@SessionId nvarchar(100),
	@IPAddress varchar(45),
	@Status nvarchar(30),
	@IsActive bit,
	@Confirmed bit,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime,
	@SalesOrderId varchar(10)
)
AS
    INSERT INTO [dbo].[SalesOrderHeader] 
	(
	[CompanyCustomerId],
	[AddressId],
	[SalesChannelId],
	[OnlineOrderId],
	[DirectOrderId],
	[OrderDate],
	[TotalAmount],
	[DiscountAmount],
	[NetAmount],
	[SessionId],
	[IPAddress],
	[Status],
	[IsActive],
	[Confirmed],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[SalesOrderId]
    ) 
	VALUES 
	(
	@CompanyCustomerId,
	@AddressId,
	@SalesChannelId,
	@OnlineOrderId,
	@DirectOrderId,
	@OrderDate,
	@TotalAmount,
	@DiscountAmount,
	@NetAmount,
	@SessionId,
	@IPAddress,
	@Status,
	@IsActive,
	@Confirmed,
	@CreatedBy,
	@CreatedAt,
	@UpdatedBy,
	@UpdatedAt,
	@SalesOrderId
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

/****** Object:  StoredProcedure [dbo].UpdateSalesOrderHeader    Script Date: 12/10/2025 2:32:53 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateSalesOrderHeader]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateSalesOrderHeader]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateSalesOrderHeader
(
	@Id int,
	@CompanyCustomerId int,
	@AddressId int,
	@SalesChannelId int,
	@OnlineOrderId varchar(10),
	@DirectOrderId varchar(10),
	@OrderDate datetime,
	@TotalAmount decimal(18, 2),
	@DiscountAmount decimal(18, 2),
	@NetAmount decimal(19, 2),
	@SessionId nvarchar(100),
	@IPAddress varchar(45),
	@Status nvarchar(30),
	@IsActive bit,
	@Confirmed bit,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime,
	@SalesOrderId varchar(10)
)
AS
    UPDATE [dbo].[SalesOrderHeader] 
	SET
	[CompanyCustomerId] = @CompanyCustomerId,
	[AddressId] = @AddressId,
	[SalesChannelId] = @SalesChannelId,
	[OnlineOrderId] = @OnlineOrderId,
	[DirectOrderId] = @DirectOrderId,
	[OrderDate] = @OrderDate,
	[TotalAmount] = @TotalAmount,
	[DiscountAmount] = @DiscountAmount,
	[NetAmount] = @NetAmount,
	[SessionId] = @SessionId,
	[IPAddress] = @IPAddress,
	[Status] = @Status,
	[IsActive] = @IsActive,
	[Confirmed] = @Confirmed,
	[CreatedBy] = @CreatedBy,
	[CreatedAt] = @CreatedAt,
	[UpdatedBy] = @UpdatedBy,
	[UpdatedAt] = @UpdatedAt,
	[SalesOrderId] = @SalesOrderId
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

/****** Object:  StoredProcedure [dbo].DeleteSalesOrderHeader    Script Date: 12/10/2025 2:32:53 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteSalesOrderHeader]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteSalesOrderHeader]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteSalesOrderHeader
(
	@Id int
)
AS
	DELETE [dbo].[SalesOrderHeader] 

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

/****** Object:  StoredProcedure [dbo].GetAllSalesOrderHeader    Script Date: 12/10/2025 2:32:53 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllSalesOrderHeader]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllSalesOrderHeader]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllSalesOrderHeader
AS
	SELECT *		
	FROM
		[dbo].[SalesOrderHeader]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetSalesOrderHeaderById    Script Date: 12/10/2025 2:32:53 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSalesOrderHeaderById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSalesOrderHeaderById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSalesOrderHeaderById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[SalesOrderHeader]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllSalesOrderHeaderByCompanyCustomerId    Script Date: 12/10/2025 2:32:53 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSalesOrderHeaderByCompanyCustomerId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSalesOrderHeaderByCompanyCustomerId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSalesOrderHeaderByCompanyCustomerId
(
	@CompanyCustomerId int
)
AS
	SELECT *		
	FROM
		[dbo].[SalesOrderHeader]
	WHERE ( CompanyCustomerId = @CompanyCustomerId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllSalesOrderHeaderByAddressId    Script Date: 12/10/2025 2:32:53 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSalesOrderHeaderByAddressId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSalesOrderHeaderByAddressId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSalesOrderHeaderByAddressId
(
	@AddressId int
)
AS
	SELECT *		
	FROM
		[dbo].[SalesOrderHeader]
	WHERE ( AddressId = @AddressId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllSalesOrderHeaderBySalesChannelId    Script Date: 12/10/2025 2:32:53 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSalesOrderHeaderBySalesChannelId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSalesOrderHeaderBySalesChannelId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSalesOrderHeaderBySalesChannelId
(
	@SalesChannelId int
)
AS
	SELECT *		
	FROM
		[dbo].[SalesOrderHeader]
	WHERE ( SalesChannelId = @SalesChannelId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetSalesOrderHeaderMaximumId    Script Date: 12/10/2025 2:32:53 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSalesOrderHeaderMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSalesOrderHeaderMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSalesOrderHeaderMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[SalesOrderHeader]

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

/****** Object:  StoredProcedure [dbo].GetSalesOrderHeaderRowCount    Script Date: 12/10/2025 2:32:53 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSalesOrderHeaderRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSalesOrderHeaderRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSalesOrderHeaderRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[SalesOrderHeader]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedSalesOrderHeader    Script Date: 12/10/2025 2:32:53 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedSalesOrderHeader]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedSalesOrderHeader]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedSalesOrderHeader
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

SET @SQL1 = 'WITH SalesOrderHeaderEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[CompanyCustomerId],
	[AddressId],
	[SalesChannelId],
	[OnlineOrderId],
	[DirectOrderId],
	[OrderDate],
	[TotalAmount],
	[DiscountAmount],
	[NetAmount],
	[SessionId],
	[IPAddress],
	[Status],
	[IsActive],
	[Confirmed],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[SalesOrderId]
				FROM 
				[dbo].[SalesOrderHeader]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[CompanyCustomerId],
	[AddressId],
	[SalesChannelId],
	[OnlineOrderId],
	[DirectOrderId],
	[OrderDate],
	[TotalAmount],
	[DiscountAmount],
	[NetAmount],
	[SessionId],
	[IPAddress],
	[Status],
	[IsActive],
	[Confirmed],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt],
	[SalesOrderId]
				FROM 
					SalesOrderHeaderEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[SalesOrderHeader] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetSalesOrderHeaderByQuery    Script Date: 12/10/2025 2:32:53 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSalesOrderHeaderByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSalesOrderHeaderByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSalesOrderHeaderByQuery
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
				[dbo].[SalesOrderHeader] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

