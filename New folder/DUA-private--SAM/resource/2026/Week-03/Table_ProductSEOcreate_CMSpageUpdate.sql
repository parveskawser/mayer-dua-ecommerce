SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProductSEO](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [ProductId] [int] NOT NULL,
    
    -- Standard SEO
    [MetaTitle] [nvarchar](100) NULL,       -- Title tag (keeping it concise for SERP)
    [MetaKeywords] [nvarchar](255) NULL,    -- Keywords (less used by Google, but good for internal search)
    [MetaDescription] [nvarchar](300) NULL, -- Description for search results snippet
    [CanonicalUrl] [nvarchar](500) NULL,    -- To prevent duplicate content issues
    
    -- Social Media (Open Graph / Twitter Card)
    [OGTitle] [nvarchar](150) NULL,
    [OGDescription] [nvarchar](300) NULL,
    [OGImage] [nvarchar](500) NULL,         -- Specific image for social sharing
    
    -- Audit fields
    [CreatedBy] [nvarchar](100) NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] [nvarchar](100) NULL,
    [UpdatedAt] [datetime] NULL,

    CONSTRAINT [PK_ProductSEO] PRIMARY KEY CLUSTERED ([Id] ASC),
    
    -- Enforce 1:1 relationship: A product can only have ONE SEO record
    CONSTRAINT [UQ_ProductSEO_ProductId] UNIQUE NONCLUSTERED ([ProductId] ASC)
) ON [PRIMARY]
GO

-- Create Foreign Key Relationship
ALTER TABLE [dbo].[ProductSEO]  WITH CHECK ADD  CONSTRAINT [FK_ProductSEO_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
ON DELETE CASCADE -- If Product is deleted, delete its SEO data automatically
GO

ALTER TABLE [dbo].[ProductSEO] CHECK CONSTRAINT [FK_ProductSEO_Product]
GO


ALTER TABLE [dbo].[CmsPage] 

ADD [CustomHeaderTags] NVARCHAR(MAX) NULL;

GO
 