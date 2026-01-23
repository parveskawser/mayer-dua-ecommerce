-- 1. CMS Page Table
CREATE TABLE [dbo].[CmsPage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,             -- Multi-tenant support
	[Title] [nvarchar](200) NOT NULL,
	[Slug] [nvarchar](200) NOT NULL,        -- URL part
	[ContentHtml] [nvarchar](max) NULL,     -- Main Body (Summernote)
	[SidebarContentHtml] [nvarchar](max) NULL, -- For 2-column layout
	[LayoutView] [nvarchar](100) NOT NULL DEFAULT '_CmsBlank', -- _CmsBlank, _CmsTwoColumn
	[MetaTitle] [nvarchar](200) NULL,       -- SEO
	[MetaDescription] [nvarchar](500) NULL, -- SEO
	[CustomCss] [nvarchar](max) NULL,       -- Inline CSS
	[CustomJs] [nvarchar](max) NULL,        -- Inline JS
	[IsActive] [bit] NOT NULL DEFAULT 1,
	[Version] [int] NOT NULL DEFAULT 1,
	[PublishedAt] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT GETUTCDATE(),
	[UpdatedBy] [nvarchar](100) NULL,
	[UpdatedAt] [datetime] NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Index for fast URL lookups
CREATE UNIQUE NONCLUSTERED INDEX [IX_CmsPage_Slug_CompanyId] 
ON [dbo].[CmsPage] ([Slug], [CompanyId]) 
WHERE [IsActive] = 1;

-- 2. CMS Asset Table (File Uploads)
CREATE TABLE [dbo].[CmsAsset](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[PageId] [int] NULL,                    -- Nullable (Can be global assets)
	[FileName] [nvarchar](255) NOT NULL,    -- Original Name
	[FilePath] [nvarchar](500) NOT NULL,    -- /uploads/cms/1/css/style-v1.css
	[FileType] [nvarchar](20) NOT NULL,     -- 'CSS' or 'JS'
	[Version] [int] NOT NULL DEFAULT 1,
	[IsActive] [bit] NOT NULL DEFAULT 1,    -- Soft delete for version control
	[CreatedAt] [datetime] NOT NULL DEFAULT GETUTCDATE(),
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- FK Constraints (Optional but recommended)
ALTER TABLE [dbo].[CmsAsset] WITH CHECK ADD CONSTRAINT [FK_CmsAsset_CmsPage] FOREIGN KEY([PageId])
REFERENCES [dbo].[CmsPage] ([Id]);