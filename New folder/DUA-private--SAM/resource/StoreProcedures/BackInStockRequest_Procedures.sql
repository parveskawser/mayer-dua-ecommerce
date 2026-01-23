USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertBackInStockRequest    Script Date: 1/14/2026 4:18:14 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertBackInStockRequest]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertBackInStockRequest]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertBackInStockRequest
(
	@Id int OUTPUT,
	@ProductVariantId int,
	@ContactNumber nvarchar(50),
	@RequestDate datetime,
	@IsNotified bit,
	@NotifiedDate datetime,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    INSERT INTO [dbo].[BackInStockRequest] 
	(
	[ProductVariantId],
	[ContactNumber],
	[RequestDate],
	[IsNotified],
	[NotifiedDate],
	[CreatedBy],
	[CreatedAt],
	[UpdatedBy],
	[UpdatedAt]
    ) 
	VALUES 
	(
	@ProductVariantId,
	@ContactNumber,
	ISNULL(@RequestDate, GETUTCDATE()),
	@IsNotified,
	@NotifiedDate,
	@CreatedBy,
	ISNULL(@CreatedAt, GETUTCDATE()),
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

/****** Object:  Index [IX_BackInStockRequest_ProductVariantId_Id] ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_BackInStockRequest_ProductVariantId_Id' AND object_id = OBJECT_ID(N'[dbo].[BackInStockRequest]'))
DROP INDEX [IX_BackInStockRequest_ProductVariantId_Id] ON [dbo].[BackInStockRequest]
GO
CREATE NONCLUSTERED INDEX [IX_BackInStockRequest_ProductVariantId_Id]
ON [dbo].[BackInStockRequest] ([ProductVariantId] ASC, [Id] ASC)
GO

/****** Object:  StoredProcedure [dbo].UpdateBackInStockRequest    Script Date: 1/14/2026 4:18:14 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateBackInStockRequest]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateBackInStockRequest]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateBackInStockRequest
(
	@Id int,
	@ProductVariantId int,
	@ContactNumber nvarchar(50),
	@RequestDate datetime,
	@IsNotified bit,
	@NotifiedDate datetime,
	@CreatedBy nvarchar(100),
	@CreatedAt datetime,
	@UpdatedBy nvarchar(100),
	@UpdatedAt datetime
)
AS
    UPDATE [dbo].[BackInStockRequest] 
	SET
	[ProductVariantId] = @ProductVariantId,
	[ContactNumber] = @ContactNumber,
	[RequestDate] = @RequestDate,
	[IsNotified] = @IsNotified,
	[NotifiedDate] = @NotifiedDate,
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

/****** Object:  StoredProcedure [dbo].DeleteBackInStockRequest    Script Date: 1/14/2026 4:18:14 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteBackInStockRequest]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteBackInStockRequest]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteBackInStockRequest
(
	@Id int
)
AS
	DELETE [dbo].[BackInStockRequest] 

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

/****** Object:  StoredProcedure [dbo].GetAllBackInStockRequest    Script Date: 1/14/2026 4:18:14 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllBackInStockRequest]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllBackInStockRequest]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllBackInStockRequest
AS
	SELECT *		
	FROM
		[dbo].[BackInStockRequest]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetBackInStockRequestById    Script Date: 1/14/2026 4:18:14 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetBackInStockRequestById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetBackInStockRequestById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetBackInStockRequestById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[BackInStockRequest]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetAllBackInStockRequestByProductVariantId    Script Date: 1/14/2026 4:18:14 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetBackInStockRequestByProductVariantId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetBackInStockRequestByProductVariantId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetBackInStockRequestByProductVariantId
(
	@ProductVariantId int
)
AS
	SELECT *		
	FROM
		[dbo].[BackInStockRequest]
	WHERE ( ProductVariantId = @ProductVariantId  )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetBackInStockRequestMaximumId    Script Date: 1/14/2026 4:18:14 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetBackInStockRequestMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetBackInStockRequestMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetBackInStockRequestMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[BackInStockRequest]

	If @Result > 0 
		BEGIN
			-- Everything is OK
			RETURN @Result
		END
		ELSE
		BEGIN
			RETURN 0
		END
GO

/****** Object:  StoredProcedure [dbo].GetBackInStockRequestRowCount    Script Date: 1/14/2026 4:18:14 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetBackInStockRequestRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetBackInStockRequestRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetBackInStockRequestRowCount
AS
	DECLARE @Result int
	SET @Result = 0

	SELECT @Result = Count(*) 		
	FROM
		[dbo].[BackInStockRequest]

	If @Result > 0 
		BEGIN
			-- Everything is OK
			RETURN @Result
		END
		ELSE
		BEGIN
			RETURN 0
		END
GO

/****** Object:  StoredProcedure [dbo].GetPagedBackInStockRequest    Script Date: 1/14/2026 4:18:14 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedBackInStockRequest]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedBackInStockRequest]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedBackInStockRequest
(
	@TotalRows int output,
	@PageIndex int,
	@RowPerPage int,
	@WhereClause nvarchar(4000),
	@SortColumn nvarchar(128),
	@SortOrder nvarchar(4)
)
AS

	DECLARE @SQL1 nvarchar(max)
	DECLARE @SQL2 nvarchar(max)
	DECLARE @SQL3 nvarchar(max)
	DECLARE @SQL4 nvarchar(max)

	SET @SQL1 = 'WITH BackInStockRequestEntries AS (
					SELECT ROW_NUMBER() OVER ( ORDER BY ' + @SortColumn + ' ' + @SortOrder + ' ) AS RowNumber, *
					FROM [dbo].[BackInStockRequest] ' + @WhereClause + '
				)
				SELECT * FROM BackInStockRequestEntries
				WHERE RowNumber BETWEEN ' + CONVERT(nvarchar(10), ((@PageIndex - 1) * @RowPerPage) + 1) + ' AND ' + CONVERT(nvarchar(10), @PageIndex * @RowPerPage)

	SET @SQL2 = 'SELECT @TotalRows = Count(*)
				FROM [dbo].[BackInStockRequest] ' + @WhereClause

	EXEC sp_executesql @SQL1
	EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows output
GO

/****** Object:  StoredProcedure [dbo].GetBackInStockRequestByQuery    Script Date: 1/14/2026 4:18:14 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetBackInStockRequestByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetBackInStockRequestByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetBackInStockRequestByQuery
(
	@Query nvarchar(4000)
)
AS

	DECLARE @SQL1 nvarchar(max)
	DECLARE @SQL2 nvarchar(max)
	DECLARE @SQL3 nvarchar(max)
	DECLARE @SQL4 nvarchar(max)

	SET @SQL1 = 'SELECT *
				FROM [dbo].[BackInStockRequest] ' + @Query

	EXEC sp_executesql @SQL1

GO
