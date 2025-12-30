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

    -- 2. Define the Inner Query (The "Base Data")
    -- We store this distinct logic string to reuse it for both Count and Fetch
    -- This prevents code duplication and keeps logic in sync.
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

            -- Calculated Payment Columns
            ISNULL(pay.TotalPaid, 0) AS PaidAmount,
            (soh.NetAmount - ISNULL(pay.TotalPaid, 0)) AS DueAmount

        FROM [dbo].[SalesOrderHeader] soh
        LEFT JOIN [dbo].[Address] a ON soh.AddressId = a.Id
        LEFT JOIN [dbo].[CompanyCustomer] cc ON soh.CompanyCustomerId = cc.Id
        LEFT JOIN [dbo].[Customer] c ON cc.CustomerId = c.Id
        
        -- Optimized Subquery: Calculate Payment totals in a CROSS APPLY or OUTER APPLY 
        -- This is faster and cleaner than doing it in the SELECT list twice
        OUTER APPLY (
            SELECT SUM(cp.Amount) as TotalPaid
            FROM CustomerPayment cp 
            WHERE cp.TransactionReference = soh.SalesOrderId
        ) pay
    ';

    -- 3. Dynamic COUNT Query
    -- We wrap @BaseQuery in a derived table "T" so we can filter by "DueAmount" etc.
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
GO