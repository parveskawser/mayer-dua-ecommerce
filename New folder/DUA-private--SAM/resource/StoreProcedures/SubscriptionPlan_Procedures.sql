USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertSubscriptionPlan    Script Date: 1/22/2026 6:49:11 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertSubscriptionPlan]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertSubscriptionPlan]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertSubscriptionPlan
(
	@Id int OUTPUT,
	@PlanCode nvarchar(50),
	@PlanName nvarchar(100),
	@DefaultsJSON nvarchar(max),
	@BasePrice decimal(18, 2),
	@DiscountPrice decimal(18, 2),
	@CurrencyCode nvarchar(10),
	@IsActive bit,
	@CreatedAt datetime,
	@UpdatedAt datetime
)
AS
    INSERT INTO [dbo].[SubscriptionPlan] 
	(
	[PlanCode],
	[PlanName],
	[DefaultsJSON],
	[BasePrice],
	[DiscountPrice],
	[CurrencyCode],
	[IsActive],
	[CreatedAt],
	[UpdatedAt]
    ) 
	VALUES 
	(
	@PlanCode,
	@PlanName,
	@DefaultsJSON,
	@BasePrice,
	@DiscountPrice,
	@CurrencyCode,
	@IsActive,
	@CreatedAt,
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

/****** Object:  StoredProcedure [dbo].UpdateSubscriptionPlan    Script Date: 1/22/2026 6:49:11 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateSubscriptionPlan]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateSubscriptionPlan]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateSubscriptionPlan
(
	@Id int,
	@PlanCode nvarchar(50),
	@PlanName nvarchar(100),
	@DefaultsJSON nvarchar(max),
	@BasePrice decimal(18, 2),
	@DiscountPrice decimal(18, 2),
	@CurrencyCode nvarchar(10),
	@IsActive bit,
	@CreatedAt datetime,
	@UpdatedAt datetime
)
AS
    UPDATE [dbo].[SubscriptionPlan] 
	SET
	[PlanCode] = @PlanCode,
	[PlanName] = @PlanName,
	[DefaultsJSON] = @DefaultsJSON,
	[BasePrice] = @BasePrice,
	[DiscountPrice] = @DiscountPrice,
	[CurrencyCode] = @CurrencyCode,
	[IsActive] = @IsActive,
	[CreatedAt] = @CreatedAt,
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

/****** Object:  StoredProcedure [dbo].DeleteSubscriptionPlan    Script Date: 1/22/2026 6:49:11 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteSubscriptionPlan]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteSubscriptionPlan]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteSubscriptionPlan
(
	@Id int
)
AS
	DELETE [dbo].[SubscriptionPlan] 

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

/****** Object:  StoredProcedure [dbo].GetAllSubscriptionPlan    Script Date: 1/22/2026 6:49:11 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllSubscriptionPlan]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllSubscriptionPlan]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllSubscriptionPlan
AS
	SELECT *		
	FROM
		[dbo].[SubscriptionPlan]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetSubscriptionPlanById    Script Date: 1/22/2026 6:49:11 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSubscriptionPlanById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSubscriptionPlanById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSubscriptionPlanById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[SubscriptionPlan]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetSubscriptionPlanMaximumId    Script Date: 1/22/2026 6:49:11 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSubscriptionPlanMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSubscriptionPlanMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSubscriptionPlanMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[SubscriptionPlan]

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

/****** Object:  StoredProcedure [dbo].GetSubscriptionPlanRowCount    Script Date: 1/22/2026 6:49:11 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSubscriptionPlanRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSubscriptionPlanRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSubscriptionPlanRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[SubscriptionPlan]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedSubscriptionPlan    Script Date: 1/22/2026 6:49:11 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedSubscriptionPlan]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedSubscriptionPlan]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedSubscriptionPlan
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

SET @SQL1 = 'WITH SubscriptionPlanEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[PlanCode],
	[PlanName],
	[DefaultsJSON],
	[BasePrice],
	[DiscountPrice],
	[CurrencyCode],
	[IsActive],
	[CreatedAt],
	[UpdatedAt]
				FROM 
				[dbo].[SubscriptionPlan]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[PlanCode],
	[PlanName],
	[DefaultsJSON],
	[BasePrice],
	[DiscountPrice],
	[CurrencyCode],
	[IsActive],
	[CreatedAt],
	[UpdatedAt]
				FROM 
					SubscriptionPlanEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[SubscriptionPlan] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetSubscriptionPlanByQuery    Script Date: 1/22/2026 6:49:11 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSubscriptionPlanByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSubscriptionPlanByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSubscriptionPlanByQuery
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
				[dbo].[SubscriptionPlan] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

