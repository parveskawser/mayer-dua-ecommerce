USE AA4
GO

/****** Object:  StoredProcedure [dbo]..InsertRecruitmentEligible    Script Date: 12/29/2025 2:00:01 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertRecruitmentEligible]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[InsertRecruitmentEligible]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE InsertRecruitmentEligible
(
	@Id int OUTPUT,
	@CompanyId uniqueidentifier,
	@RecruitmentEligibleId uniqueidentifier,
	@TestResultMark decimal(18, 2),
	@InterViewResultMark decimal(18, 2),
	@Status bit,
	@CreatedBy uniqueidentifier,
	@CreatedDate datetime,
	@LastUpdatedBy uniqueidentifier,
	@LastUpdatedDate datetime,
	@CandidateId uniqueidentifier
)
AS
    INSERT INTO [dbo].[RecruitmentEligible] 
	(
	[CompanyId],
	[RecruitmentEligibleId],
	[TestResultMark],
	[InterViewResultMark],
	[Status],
	[CreatedBy],
	[CreatedDate],
	[LastUpdatedBy],
	[LastUpdatedDate],
	[CandidateId]
    ) 
	VALUES 
	(
	@CompanyId,
	@RecruitmentEligibleId,
	@TestResultMark,
	@InterViewResultMark,
	@Status,
	@CreatedBy,
	@CreatedDate,
	@LastUpdatedBy,
	@LastUpdatedDate,
	@CandidateId
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

/****** Object:  StoredProcedure [dbo].UpdateRecruitmentEligible    Script Date: 12/29/2025 2:00:01 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateRecruitmentEligible]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateRecruitmentEligible]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE UpdateRecruitmentEligible
(
	@Id int,
	@CompanyId uniqueidentifier,
	@RecruitmentEligibleId uniqueidentifier,
	@TestResultMark decimal(18, 2),
	@InterViewResultMark decimal(18, 2),
	@Status bit,
	@CreatedBy uniqueidentifier,
	@CreatedDate datetime,
	@LastUpdatedBy uniqueidentifier,
	@LastUpdatedDate datetime,
	@CandidateId uniqueidentifier
)
AS
    UPDATE [dbo].[RecruitmentEligible] 
	SET
	[CompanyId] = @CompanyId,
	[RecruitmentEligibleId] = @RecruitmentEligibleId,
	[TestResultMark] = @TestResultMark,
	[InterViewResultMark] = @InterViewResultMark,
	[Status] = @Status,
	[CreatedBy] = @CreatedBy,
	[CreatedDate] = @CreatedDate,
	[LastUpdatedBy] = @LastUpdatedBy,
	[LastUpdatedDate] = @LastUpdatedDate,
	[CandidateId] = @CandidateId
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

/****** Object:  StoredProcedure [dbo].DeleteRecruitmentEligible    Script Date: 12/29/2025 2:00:01 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteRecruitmentEligible]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteRecruitmentEligible]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE DeleteRecruitmentEligible
(
	@Id int
)
AS
	DELETE [dbo].[RecruitmentEligible] 

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

/****** Object:  StoredProcedure [dbo].GetAllRecruitmentEligible    Script Date: 12/29/2025 2:00:01 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllRecruitmentEligible]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllRecruitmentEligible]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetAllRecruitmentEligible
AS
	SELECT *		
	FROM
		[dbo].[RecruitmentEligible]

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetRecruitmentEligibleById    Script Date: 12/29/2025 2:00:01 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetRecruitmentEligibleById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetRecruitmentEligibleById]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetRecruitmentEligibleById
(
	@Id int
)
AS
	SELECT *		
	FROM
		[dbo].[RecruitmentEligible]
	WHERE ( Id = @Id )

RETURN @@ROWCOUNT
GO

/****** Object:  StoredProcedure [dbo].GetRecruitmentEligibleMaximumId    Script Date: 12/29/2025 2:00:01 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetRecruitmentEligibleMaximumId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetRecruitmentEligibleMaximumId]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetRecruitmentEligibleMaximumId
AS
	DECLARE @Result int
	SET @Result = 0
	
	SELECT @Result = MAX(Id) 		
	FROM
		[dbo].[RecruitmentEligible]

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

/****** Object:  StoredProcedure [dbo].GetRecruitmentEligibleRowCount    Script Date: 12/29/2025 2:00:01 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetRecruitmentEligibleRowCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetRecruitmentEligibleRowCount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetRecruitmentEligibleRowCount
AS
	DECLARE @Result int
	SET @Result = 0
	SELECT @Result = COUNT(*) 		
	FROM
		[dbo].[RecruitmentEligible]
		
RETURN @Result
GO

/****** Object:  StoredProcedure [dbo].GetPagedRecruitmentEligible    Script Date: 12/29/2025 2:00:01 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPagedRecruitmentEligible]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPagedRecruitmentEligible]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetPagedRecruitmentEligible
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

SET @SQL1 = 'WITH RecruitmentEligibleEntries AS (
			SELECT ROW_NUMBER() OVER ('+ @SortColumn +')AS Row,
	[Id],
	[CompanyId],
	[RecruitmentEligibleId],
	[TestResultMark],
	[InterViewResultMark],
	[Status],
	[CreatedBy],
	[CreatedDate],
	[LastUpdatedBy],
	[LastUpdatedDate],
	[CandidateId]
				FROM 
				[dbo].[RecruitmentEligible]
					'+ @WhereClause +'
				)
				SELECT 
	[Id],
	[CompanyId],
	[RecruitmentEligibleId],
	[TestResultMark],
	[InterViewResultMark],
	[Status],
	[CreatedBy],
	[CreatedDate],
	[LastUpdatedBy],
	[LastUpdatedDate],
	[CandidateId]
				FROM 
					RecruitmentEligibleEntries
				WHERE 
					Row between '+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) + 1) +'And ('+ CONVERT(nvarchar(10), (@PageIndex * @RowPerPage) +@RowPerPage+ 1) +')'
	

SET @SQL2 =		' SELECT @TotalRows = COUNT(*) 
				FROM 
				[dbo].[RecruitmentEligible] ' + @WhereClause
								
EXEC sp_executesql @SQL2, N'@TotalRows int output', @TotalRows = @TotalRows output

EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

/****** Object:  StoredProcedure [dbo].GetRecruitmentEligibleByQuery    Script Date: 12/29/2025 2:00:01 PM  ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetRecruitmentEligibleByQuery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetRecruitmentEligibleByQuery]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE GetRecruitmentEligibleByQuery
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
				[dbo].[RecruitmentEligible] ' + @Query
								
EXEC sp_executesql @SQL1

RETURN @@ROWCOUNT
END
GO

