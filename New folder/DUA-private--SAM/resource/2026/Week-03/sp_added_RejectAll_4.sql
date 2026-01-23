CREATE OR ALTER PROCEDURE [dbo].[RejectBulkOrderRemaining]
    @BulkOrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[PoRequested]
    SET Status = 'Rejected', UpdatedAt = GETUTCDATE()
    WHERE BulkPurchaseOrderId = @BulkOrderId AND Status = 'Pending';
END