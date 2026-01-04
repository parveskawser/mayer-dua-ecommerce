using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using System.Collections.Generic; // ✅ Required for List<> and Dictionary<>
using System.Threading.Tasks;

namespace MDUA.Facade.Interface
{
    public interface IOrderFacade : ICommonFacade<SalesOrderHeader, SalesOrderHeaderList, SalesOrderHeaderBase>
    {
        Task<string> PlaceGuestOrder(SalesOrderHeader orderData);
        Customer GetCustomerByPhone(string phone);
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
        dynamic PlaceAdminOrder(SalesOrderHeader orderData);

        DashboardStats GetDashboardMetrics();
        List<SalesOrderHeader> GetRecentOrders();
        List<ChartDataPoint> GetSalesTrend();
        List<ChartDataPoint> GetOrderStatusCounts();
        void UpdateDeliveryStatus(int deliveryId, string newStatus);
        SalesOrderHeaderList GetPagedOrdersForAdmin(int pageIndex, int pageSize, string whereClause, out int totalRows);
        void UpdateOrderStatus(int orderId, string newStatus);
        SalesOrderHeader GetOrderById(int id);
        int GetOrderPageNumber(int orderId, int pageSize);
        List<Dictionary<string, object>> GetExportData(ExportRequest request);
    }
}