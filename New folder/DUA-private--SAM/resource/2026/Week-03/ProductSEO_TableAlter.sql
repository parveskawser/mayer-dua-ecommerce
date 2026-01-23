DECLARE @ConstraintName nvarchar(200)
 
-- 1. Find the name of the existing default constraint for 'CreatedAt'

SELECT @ConstraintName = Name 

FROM sys.default_constraints

WHERE parent_object_id = OBJECT_ID('dbo.ProductSEO')

AND parent_column_id = (

    SELECT column_id 

    FROM sys.columns 

    WHERE object_id = OBJECT_ID('dbo.ProductSEO') AND name = 'CreatedAt'

)
 
-- 2. If found, drop the old constraint

IF @ConstraintName IS NOT NULL

BEGIN

    EXEC('ALTER TABLE [dbo].[ProductSEO] DROP CONSTRAINT ' + @ConstraintName)

    PRINT 'Old constraint dropped: ' + @ConstraintName

END
 
-- 3. Add the new constraint using GETUTCDATE()

-- Best Practice: We explicitly name it [DF_ProductSEO_CreatedAt] so it's easier to manage later

ALTER TABLE [dbo].[ProductSEO]

ADD CONSTRAINT [DF_ProductSEO_CreatedAt] DEFAULT GETUTCDATE() FOR [CreatedAt];
 
PRINT 'New UTC constraint created successfully.';

GO
 