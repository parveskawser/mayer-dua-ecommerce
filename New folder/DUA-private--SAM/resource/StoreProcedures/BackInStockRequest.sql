/****** Object:  Table [dbo].[BackInStockRequest] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BackInStockRequest](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductVariantId] [int] NOT NULL,
	[ContactNumber] [nvarchar](50) NOT NULL,
	[RequestDate] [datetime] NOT NULL DEFAULT (GETUTCDATE()),
	[IsNotified] [bit] NOT NULL DEFAULT ((0)),
	[NotifiedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT (GETUTCDATE()),
	[UpdatedBy] [nvarchar](100) NULL,
	[UpdatedAt] [datetime] NULL,
 CONSTRAINT [PK_BackInStockRequest] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_BackInStockRequest_ProductVariantId_Id]
ON [dbo].[BackInStockRequest] ([ProductVariantId] ASC, [Id] ASC)
GO

ALTER TABLE [dbo].[BackInStockRequest]  WITH CHECK ADD  CONSTRAINT [FK_BackInStockRequest_ProductVariant] FOREIGN KEY([ProductVariantId])
REFERENCES [dbo].[ProductVariant] ([Id])
GO
ALTER TABLE [dbo].[BackInStockRequest] CHECK CONSTRAINT [FK_BackInStockRequest_ProductVariant]
GO
