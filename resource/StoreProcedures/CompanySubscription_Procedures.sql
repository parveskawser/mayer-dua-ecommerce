USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertCompanySubscription    Script Date: 1/7/2026 11:54:38 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertCompanySubscription]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertCompanySubscription]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertCompanySubscription
(
	@Id int OUTPUT,
	@CompanyId int,
	@SubscriptionPlanId int,
	@PlanNameSnapshot nvarchar(100),
	@MaxProducts int,
	@MaxOrders int,
	@OrderCycle nvarchar(20),
	@PriceCharged decimal(18, 2),
	@CurrencyCode nvarchar(10),
	@StartDate datetime,
	@EndDate datetime,
	@NextBillingDate datetime,
	@CycleAnchorDate datetime,
	@Status nvarchar(20),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    INSERT INTO [dbo].[CompanySubscription] 
	(
	[CompanyId],
	[SubscriptionPlanId],
	[PlanNameSnapshot],
	[MaxProducts],
	[MaxOrders],
	[OrderCycle],
	[PriceCharged],
	[CurrencyCode],
	[StartDate],
	[EndDate],
	[NextBillingDate],
	[CycleAnchorDate],
	[Status],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
    ) 
	VALUES 
	(
	@CompanyId,
	@SubscriptionPlanId,
	@PlanNameSnapshot,
	@MaxProducts,
	@MaxOrders,
	@OrderCycle,
	@PriceCharged,
	@CurrencyCode,
	@StartDate,
	@EndDate,
	@NextBillingDate,
	@CycleAnchorDate,
	@Status,
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

/****** Object:  StoredProcedure [dbo].UpdateCompanySubscription    Script Date: 1/7/2026 11:54:38 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateCompanySubscription]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateCompanySubscription]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateCompanySubscription
(
	@Id int,
	@CompanyId int,
	@SubscriptionPlanId int,
	@PlanNameSnapshot nvarchar(100),
	@MaxProducts int,
	@MaxOrders int,
	@OrderCycle nvarchar(20),
	@PriceCharged decimal(18, 2),
	@CurrencyCode nvarchar(10),
	@StartDate datetime,
	@EndDate datetime,
	@NextBillingDate datetime,
	@CycleAnchorDate datetime,
	@Status nvarchar(20),
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    UPDATE [dbo].[CompanySubscription] 
	SET
	[CompanyId] = @CompanyId,
	[SubscriptionPlanId] = @SubscriptionPlanId,
	[PlanNameSnapshot] = @PlanNameSnapshot,
	[MaxProducts] = @MaxProducts,
	[MaxOrders] = @MaxOrders,
	[OrderCycle] = @OrderCycle,
	[PriceCharged] = @PriceCharged,
	[CurrencyCode] = @CurrencyCode,
	[StartDate] = @StartDate,
	[EndDate] = @EndDate,
	[NextBillingDate] = @NextBillingDate,
	[CycleAnchorDate] = @CycleAnchorDate,
	[Status] = @Status,
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

/****** Object:  StoredProcedure [dbo].DeleteCompanySubscription    Script Date: 1/7/2026 11:54:38 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteCompanySubscription]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteCompanySubscription]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteCompanySubscription
(
	@Id int
)
AS
	DELETE [dbo].[CompanySubscription] 

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

/****** Object:  StoredProcedure [dbo].GetAllCompanySubscription    Script Date: 1/7/2026 11:54:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllCompanySubscription]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllCompanySubscription]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllCompanySubscription
AS
	SELECT *		
	FROM
		[dbo].[CompanySubscription]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetCompanySubscriptionById    Script Date: 1/7/2026 11:54:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanySubscriptionById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanySubscriptionById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanySubscriptionById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[CompanySubscription]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllCompanySubscriptionByCompanyId    Script Date: 1/7/2026 11:54:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanySubscriptionByCompanyId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanySubscriptionByCompanyId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanySubscriptionByCompanyId
(
	@CompanyId int
)
AS
	SELECT *		
	FROM
		[dbo].[CompanySubscription]
	WHERE ( CompanyId = @CompanyId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllCompanySubscriptionBySubscriptionPlanId    Script Date: 1/7/2026 11:54:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanySubscriptionBySubscriptionPlanId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanySubscriptionBySubscriptionPlanId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanySubscriptionBySubscriptionPlanId
(
	@SubscriptionPlanId int
)
AS
	SELECT *		
	FROM
		[dbo].[CompanySubscription]
	WHERE ( SubscriptionPlanId = @SubscriptionPlanId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetCompanySubscriptionMaximumId    Script Date: 1/7/2026 11:54:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanySubscriptionMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanySubscriptionMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanySubscriptionMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[CompanySubscription]

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

/****** Object:  StoredProcedure [dbo].GetCompanySubscriptionRowCount    Script Date: 1/7/2026 11:54:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanySubscriptionRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanySubscriptionRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanySubscriptionRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[CompanySubscription]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedCompanySubscription    Script Date: 1/7/2026 11:54:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedCompanySubscription]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedCompanySubscription]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedCompanySubscription
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

SET @SQL1 = 'WITH CompanySubscriptionEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[CompanyId],
	[SubscriptionPlanId],
	[PlanNameSnapshot],
	[MaxProducts],
	[MaxOrders],
	[OrderCycle],
	[PriceCharged],
	[CurrencyCode],
	[StartDate],
	[EndDate],
	[NextBillingDate],
	[CycleAnchorDate],
	[Status],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
				[dbo].[CompanySubscription]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[CompanyId],
	[SubscriptionPlanId],
	[PlanNameSnapshot],
	[MaxProducts],
	[MaxOrders],
	[OrderCycle],
	[PriceCharged],
	[CurrencyCode],
	[StartDate],
	[EndDate],
	[NextBillingDate],
	[CycleAnchorDate],
	[Status],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
				FROM 
					CompanySubscriptionEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[CompanySubscription] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetCompanySubscriptionByQuery    Script Date: 1/7/2026 11:54:38 AM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCompanySubscriptionByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCompanySubscriptionByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetCompanySubscriptionByQuery
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
				[dbo].[CompanySubscription] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

