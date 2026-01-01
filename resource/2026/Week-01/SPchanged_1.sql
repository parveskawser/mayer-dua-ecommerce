ALTER PROCEDURE [dbo].[GetPagedSalesOrderHeader]  

(  

    @TotalRows      int OUTPUT,  

    @PageIndex      int,  

    @RowPerPage     int,  

    @WhereClause    nvarchar(4000) = '',   

    @SortColumn     nvarchar(128) = 'OrderDate',  

    @SortOrder      nvarchar(4) = 'DESC'  

)  

AS  

BEGIN   

    SET NOCOUNT ON;  

    -- 1. Defaults  

    SET @PageIndex = ISNULL(@PageIndex, 1);  

    SET @RowPerPage = ISNULL(@RowPerPage, 10);  

    IF (@PageIndex < 1) SET @PageIndex = 1;  

    DECLARE @Offset int = (@PageIndex - 1) * @RowPerPage;  

    DECLARE @Sql nvarchar(MAX);  

    DECLARE @ParamDefinition nvarchar(500);  

    -- 2. Define the Inner Query  

    DECLARE @BaseQuery nvarchar(MAX) = N'  

        SELECT   

            soh.[Id],   

            soh.[CompanyCustomerId],   

            soh.[AddressId],   

            soh.[SalesChannelId],   

            soh.[OnlineOrderId],   

            soh.[DirectOrderId],   

            soh.[OrderDate],   

            soh.[TotalAmount],   

            soh.[DiscountAmount],   

            soh.[NetAmount],   

            -- ✅ CALCULATED DELIVERY CHARGE

            -- Formula: NetAmount - (Sum of Item Prices)

            (soh.NetAmount - ISNULL(items.ItemTotal, 0)) AS DeliveryCharge,
 
            soh.[SessionId],   

            soh.[IPAddress],   

            soh.[Status],   

            soh.[IsActive],   

            soh.[Confirmed],   

            soh.[CreatedBy],   

            soh.[CreatedAt],   

            soh.[UpdatedBy],   

            soh.[UpdatedAt],   

            soh.[SalesOrderId],  

            -- Address  

            ISNULL(a.[Street], '''') AS Street,  

            ISNULL(a.[City], '''') AS City,  

            ISNULL(a.[Divison], '''') AS Divison,  

            ISNULL(a.[Thana], '''') AS Thana,  

            ISNULL(a.[SubOffice], '''') AS SubOffice,  

            ISNULL(a.[PostalCode], '''') AS PostalCode,  

            ISNULL(a.[Country], ''Bangladesh'') AS Country,  

            -- Customer  

            ISNULL(c.[CustomerName], ''Guest'') AS CustomerName,  

            ISNULL(c.[Phone], '''') AS CustomerPhone,  

            ISNULL(c.[Email], '''') AS CustomerEmail,  

            -- Payment Columns  

            ISNULL(pay.TotalPaid, 0) AS PaidAmount,  

            (soh.NetAmount - ISNULL(pay.TotalPaid, 0)) AS DueAmount  

        FROM [dbo].[SalesOrderHeader] soh  

        LEFT JOIN [dbo].[Address] a ON soh.AddressId = a.Id  

        LEFT JOIN [dbo].[CompanyCustomer] cc ON soh.CompanyCustomerId = cc.Id  

        LEFT JOIN [dbo].[Customer] c ON cc.CustomerId = c.Id  

        -- ✅ 1. JOIN TO GET LINE ITEM TOTALS

        OUTER APPLY (

            SELECT SUM(sod.Quantity * sod.UnitPrice) as ItemTotal

            FROM [dbo].[SalesOrderDetail] sod

            WHERE sod.SalesOrderId = soh.Id

        ) items
 
        -- ✅ 2. EXISTING PAYMENT CALCULATION

        OUTER APPLY (  

            SELECT SUM(cp.Amount) as TotalPaid  

            FROM CustomerPayment cp   

            WHERE cp.TransactionReference = soh.SalesOrderId  

        ) pay  

    ';  

    -- 3. Dynamic COUNT Query  

    SET @Sql = N'SELECT @TotalRows = COUNT(*) FROM (' + @BaseQuery + N') AS T WHERE ' + @WhereClause;  

    SET @ParamDefinition = N'@TotalRows int OUTPUT';  

    EXEC sp_executesql @Sql, @ParamDefinition, @TotalRows = @TotalRows OUTPUT;  

    -- 4. Dynamic FETCH Query  

    SET @Sql = N'SELECT * FROM (' + @BaseQuery + N') AS T   

                 WHERE ' + @WhereClause + N'  

                 ORDER BY ' + @SortColumn + N' ' + @SortOrder + N'  

                 OFFSET @Offset ROWS  

                 FETCH NEXT @RowPerPage ROWS ONLY';  

    SET @ParamDefinition = N'@Offset int, @RowPerPage int';  

    EXEC sp_executesql @Sql, @ParamDefinition, @Offset = @Offset, @RowPerPage = @RowPerPage;  

END
 