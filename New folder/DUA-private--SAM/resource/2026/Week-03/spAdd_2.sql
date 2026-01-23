-- 1. Get Page by Slug (Fast Lookup for Routing)

CREATE OR ALTER PROCEDURE [dbo].[sp_CmsPage_GetBySlug]

	@Slug nvarchar(200),

	@CompanyId int

AS

BEGIN

	SET NOCOUNT ON;

	SELECT TOP 1 * FROM [dbo].[CmsPage] WITH(NOLOCK)

	WHERE [Slug] = @Slug 

	  AND [CompanyId] = @CompanyId 

	  AND [IsActive] = 1

END

GO
 
-- 2. Check Duplicate Slug (For Validation)

CREATE OR ALTER PROCEDURE [dbo].[sp_CmsPage_CheckSlugExists]

	@Slug nvarchar(200),

	@CompanyId int,

	@ExcludeId int = 0

AS

BEGIN

	SET NOCOUNT ON;

	SELECT CAST(CASE WHEN EXISTS (

		SELECT 1 FROM [dbo].[CmsPage] 

		WHERE [Slug] = @Slug 

		  AND [CompanyId] = @CompanyId 

		  AND [Id] <> @ExcludeId

	) THEN 1 ELSE 0 END AS BIT)

END

GO
 
-- 3. Get Only Active Assets for a Page (For Rendering)

CREATE OR ALTER PROCEDURE [dbo].[sp_CmsAsset_GetActiveByPageId]

	@PageId int,

	@FileType nvarchar(20) -- 'CSS' or 'JS'

AS

BEGIN

	SET NOCOUNT ON;

	SELECT * FROM [dbo].[CmsAsset] WITH(NOLOCK)

	WHERE [PageId] = @PageId 

	  AND [FileType] = @FileType 

	  AND [IsActive] = 1 

	ORDER BY [Id] DESC -- Get latest uploaded if multiple exist active

END

GO
 
-- 4. Deactivate Old Versions (For Asset Upload Logic)

CREATE OR ALTER PROCEDURE [dbo].[sp_CmsAsset_DeactivateOldVersions]

	@PageId int,

	@FileType nvarchar(20),

	@CompanyId int

AS

BEGIN

	UPDATE [dbo].[CmsAsset]

	SET [IsActive] = 0

	WHERE [PageId] = @PageId 

	  AND [FileType] = @FileType 

	  AND [CompanyId] = @CompanyId

END

GO
 