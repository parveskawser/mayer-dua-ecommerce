ALTER TABLE [dbo].[ProductCategory]
ADD [IsActive] BIT NOT NULL
CONSTRAINT DF_ProductCategory_IsActive DEFAULT (1);
 