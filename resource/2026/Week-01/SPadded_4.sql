USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertDeliveryStatusLog    Script Date: 12/29/2025 1:59:57 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertDeliveryStatusLog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertDeliveryStatusLog]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertDeliveryStatusLog
(
	@Id int OUTPUT,
	@EntityType nvarchar(50),
	@EntityId int,
	@SalesOrderId int,
	@OldStatus nvarchar(50),
	@NewStatus nvarchar(50),
	@ChangeReason nvarchar(500),
	@OldConfirmed bit,
	@NewConfirmed bit,
	@TrackingNumber nvarchar(100),
	@ChangedBy nvarchar(100),
	@ChangedAt datetime,
	@IPAddress varchar(45),
	@UserAgent nvarchar(500),
	@IsSystemGenerated bit,
	@SourceAction nvarchar(100)
)
AS
    INSERT INTO [dbo].[DeliveryStatusLog] 
	(
	[EntityType],
	[EntityId],
	[SalesOrderId],
	[OldStatus],
	[NewStatus],
	[ChangeReason],
	[OldConfirmed],
	[NewConfirmed],
	[TrackingNumber],
	[ChangedBy],
	[ChangedAt],
	[IPAddress],
	[UserAgent],
	[IsSystemGenerated],
	[SourceAction]
    ) 
	VALUES 
	(
	@EntityType,
	@EntityId,
	@SalesOrderId,
	@OldStatus,
	@NewStatus,
	@ChangeReason,
	@OldConfirmed,
	@NewConfirmed,
	@TrackingNumber,
	@ChangedBy,
	@ChangedAt,
	@IPAddress,
	@UserAgent,
	@IsSystemGenerated,
	@SourceAction
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

/****** Object:  StoredProcedure [dbo].UpdateDeliveryStatusLog    Script Date: 12/29/2025 1:59:57 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateDeliveryStatusLog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateDeliveryStatusLog]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateDeliveryStatusLog
(
	@Id int,
	@EntityType nvarchar(50),
	@EntityId int,
	@SalesOrderId int,
	@OldStatus nvarchar(50),
	@NewStatus nvarchar(50),
	@ChangeReason nvarchar(500),
	@OldConfirmed bit,
	@NewConfirmed bit,
	@TrackingNumber nvarchar(100),
	@ChangedBy nvarchar(100),
	@ChangedAt datetime,
	@IPAddress varchar(45),
	@UserAgent nvarchar(500),
	@IsSystemGenerated bit,
	@SourceAction nvarchar(100)
)
AS
    UPDATE [dbo].[DeliveryStatusLog] 
	SET
	[EntityType] = @EntityType,
	[EntityId] = @EntityId,
	[SalesOrderId] = @SalesOrderId,
	[OldStatus] = @OldStatus,
	[NewStatus] = @NewStatus,
	[ChangeReason] = @ChangeReason,
	[OldConfirmed] = @OldConfirmed,
	[NewConfirmed] = @NewConfirmed,
	[TrackingNumber] = @TrackingNumber,
	[ChangedBy] = @ChangedBy,
	[ChangedAt] = @ChangedAt,
	[IPAddress] = @IPAddress,
	[UserAgent] = @UserAgent,
	[IsSystemGenerated] = @IsSystemGenerated,
	[SourceAction] = @SourceAction
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

/****** Object:  StoredProcedure [dbo].DeleteDeliveryStatusLog    Script Date: 12/29/2025 1:59:57 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteDeliveryStatusLog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteDeliveryStatusLog]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteDeliveryStatusLog
(
	@Id int
)
AS
	DELETE [dbo].[DeliveryStatusLog] 

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

/****** Object:  StoredProcedure [dbo].GetAllDeliveryStatusLog    Script Date: 12/29/2025 1:59:57 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllDeliveryStatusLog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllDeliveryStatusLog]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllDeliveryStatusLog
AS
	SELECT *		
	FROM
		[dbo].[DeliveryStatusLog]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetDeliveryStatusLogById    Script Date: 12/29/2025 1:59:57 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDeliveryStatusLogById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDeliveryStatusLogById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetDeliveryStatusLogById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[DeliveryStatusLog]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllDeliveryStatusLogBySalesOrderId    Script Date: 12/29/2025 1:59:57 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDeliveryStatusLogBySalesOrderId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDeliveryStatusLogBySalesOrderId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetDeliveryStatusLogBySalesOrderId
(
	@SalesOrderId int
)
AS
	SELECT *		
	FROM
		[dbo].[DeliveryStatusLog]
	WHERE ( SalesOrderId = @SalesOrderId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetDeliveryStatusLogMaximumId    Script Date: 12/29/2025 1:59:57 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDeliveryStatusLogMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDeliveryStatusLogMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetDeliveryStatusLogMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[DeliveryStatusLog]

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

/****** Object:  StoredProcedure [dbo].GetDeliveryStatusLogRowCount    Script Date: 12/29/2025 1:59:57 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDeliveryStatusLogRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDeliveryStatusLogRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetDeliveryStatusLogRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[DeliveryStatusLog]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedDeliveryStatusLog    Script Date: 12/29/2025 1:59:57 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedDeliveryStatusLog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedDeliveryStatusLog]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedDeliveryStatusLog
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

SET @SQL1 = 'WITH DeliveryStatusLogEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[EntityType],
	[EntityId],
	[SalesOrderId],
	[OldStatus],
	[NewStatus],
	[ChangeReason],
	[OldConfirmed],
	[NewConfirmed],
	[TrackingNumber],
	[ChangedBy],
	[ChangedAt],
	[IPAddress],
	[UserAgent],
	[IsSystemGenerated],
	[SourceAction]
				FROM 
				[dbo].[DeliveryStatusLog]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[EntityType],
	[EntityId],
	[SalesOrderId],
	[OldStatus],
	[NewStatus],
	[ChangeReason],
	[OldConfirmed],
	[NewConfirmed],
	[TrackingNumber],
	[ChangedBy],
	[ChangedAt],
	[IPAddress],
	[UserAgent],
	[IsSystemGenerated],
	[SourceAction]
				FROM 
					DeliveryStatusLogEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[DeliveryStatusLog] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetDeliveryStatusLogByQuery    Script Date: 12/29/2025 1:59:57 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDeliveryStatusLogByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDeliveryStatusLogByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetDeliveryStatusLogByQuery
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
				[dbo].[DeliveryStatusLog] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

