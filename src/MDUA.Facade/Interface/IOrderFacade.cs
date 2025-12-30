using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;


namespace MDUA.Facade.Interface
{
    public interface IOrderFacade : ICommonFacade<SalesOrderHeader, SalesOrderHeaderList, SalesOrderHeaderBase>
    {
        Task<string> PlaceGuestOrder(SalesOrderHeader orderData);        Customer GetCustomerByPhone(string phone);
        PostalCodes GetPostalCodeDetails(string code);
        Customer GetCustomerByEmail(string email);
        (Customer customer, Address address) GetCustomerDetailsForAutofill(string phone);
        List<string> GetDivisions();
        List<string> GetDistricts(string division);
        List<string> GetThanas(string district);
        List<dynamic> GetSubOffices(string thana);
        List<object> GetOrderReceiptByOnlineId(string onlineOrderId);
        List<SalesOrderHeader> GetAllOrdersForAdmin();
        string UpdateOrderConfirmation(int orderId, bool isConfirmed, string username);
        List<dynamic> GetProductVariantsForAdmin();
        dynamic PlaceAdminOrder(SalesOrderHeader orderData); //change

        DashboardStats GetDashboardMetrics(); //new
        List<SalesOrderHeader> GetRecentOrders(); //new
        List<ChartDataPoint> GetSalesTrend(); //new
        List<ChartDataPoint> GetOrderStatusCounts(); //new
        void UpdateDeliveryStatus(int deliveryId, string newStatus);
        SalesOrderHeaderList GetPagedOrdersForAdmin(int pageIndex, int pageSize, string whereClause, out int totalRows);
        void UpdateOrderStatus(int orderId, string newStatus);
        SalesOrderHeader GetOrderById(int id);
    }
}
