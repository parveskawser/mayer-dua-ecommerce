USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertSubscriptionUsage    Script Date: 1/22/2026 6:49:11 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertSubscriptionUsage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertSubscriptionUsage]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertSubscriptionUsage
(
	@Id int OUTPUT,
	@SubscriptionId int,
	@CycleStart datetime,
	@CycleEnd datetime,
	@OrdersProcessed int,
	@CreatedAt datetime,
	@UpdatedAt datetime
)
AS
    INSERT INTO [dbo].[SubscriptionUsage] 
	(
	[SubscriptionId],
	[CycleStart],
	[CycleEnd],
	[OrdersProcessed],
	[CreatedAt],
	[UpdatedAt]
    ) 
	VALUES 
	(
	@SubscriptionId,
	@CycleStart,
	@CycleEnd,
	@OrdersProcessed,
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

/****** Object:  StoredProcedure [dbo].UpdateSubscriptionUsage    Script Date: 1/22/2026 6:49:11 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateSubscriptionUsage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateSubscriptionUsage]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateSubscriptionUsage
(
	@Id int,
	@SubscriptionId int,
	@CycleStart datetime,
	@CycleEnd datetime,
	@OrdersProcessed int,
	@CreatedAt datetime,
	@UpdatedAt datetime
)
AS
    UPDATE [dbo].[SubscriptionUsage] 
	SET
	[SubscriptionId] = @SubscriptionId,
	[CycleStart] = @CycleStart,
	[CycleEnd] = @CycleEnd,
	[OrdersProcessed] = @OrdersProcessed,
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

/****** Object:  StoredProcedure [dbo].DeleteSubscriptionUsage    Script Date: 1/22/2026 6:49:11 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteSubscriptionUsage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteSubscriptionUsage]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteSubscriptionUsage
(
	@Id int
)
AS
	DELETE [dbo].[SubscriptionUsage] 

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

/****** Object:  StoredProcedure [dbo].GetAllSubscriptionUsage    Script Date: 1/22/2026 6:49:11 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllSubscriptionUsage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllSubscriptionUsage]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllSubscriptionUsage
AS
	SELECT *		
	FROM
		[dbo].[SubscriptionUsage]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetSubscriptionUsageById    Script Date: 1/22/2026 6:49:11 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSubscriptionUsageById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSubscriptionUsageById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSubscriptionUsageById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[SubscriptionUsage]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllSubscriptionUsageBySubscriptionId    Script Date: 1/22/2026 6:49:11 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSubscriptionUsageBySubscriptionId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSubscriptionUsageBySubscriptionId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSubscriptionUsageBySubscriptionId
(
	@SubscriptionId int
)
AS
	SELECT *		
	FROM
		[dbo].[SubscriptionUsage]
	WHERE ( SubscriptionId = @SubscriptionId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetSubscriptionUsageMaximumId    Script Date: 1/22/2026 6:49:11 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSubscriptionUsageMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSubscriptionUsageMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSubscriptionUsageMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[SubscriptionUsage]

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

/****** Object:  StoredProcedure [dbo].GetSubscriptionUsageRowCount    Script Date: 1/22/2026 6:49:11 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSubscriptionUsageRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSubscriptionUsageRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSubscriptionUsageRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[SubscriptionUsage]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedSubscriptionUsage    Script Date: 1/22/2026 6:49:11 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedSubscriptionUsage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedSubscriptionUsage]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedSubscriptionUsage
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

SET @SQL1 = 'WITH SubscriptionUsageEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[SubscriptionId],
	[CycleStart],
	[CycleEnd],
	[OrdersProcessed],
	[CreatedAt],
	[UpdatedAt]
				FROM 
				[dbo].[SubscriptionUsage]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[SubscriptionId],
	[CycleStart],
	[CycleEnd],
	[OrdersProcessed],
	[CreatedAt],
	[UpdatedAt]
				FROM 
					SubscriptionUsageEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[SubscriptionUsage] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetSubscriptionUsageByQuery    Script Date: 1/22/2026 6:49:11 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSubscriptionUsageByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSubscriptionUsageByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetSubscriptionUsageByQuery
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
				[dbo].[SubscriptionUsage] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

