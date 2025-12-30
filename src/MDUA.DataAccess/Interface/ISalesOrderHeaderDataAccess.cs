using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
 
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess.Interface
{
	/// <summary>
	/// ICommonDataAccess Provides a generic contract for data access operations 
	/// (CRUD and utility methods) that can be implemented for any entity type.
	/// 
	/// This interface defines common patterns such as:
	/// - Creating new records (Insert)
	/// - Reading single or multiple records (Get, GetAll, GetByQuery, GetPaged)
	/// - Updating existing records (Update)
	/// - Deleting records (Delete)
	/// - Utility operations like retrieving maximum ID and row count
	/// 
	/// By using generics (<typeparamref name="T"/>, <typeparamref name="L"/>, <typeparamref name="B"/>),
	/// it ensures reusability across multiple entities while maintaining 
	/// type safety and consistency.
	/// </summary>
	/// <typeparam name="T">Represents the entity type for a single record.</typeparam>
	/// <typeparam name="L">Represents the collection type for multiple records (e.g., a list).</typeparam>
	/// <typeparam name="B">Represents the base type used for insert and update operations (e.g., DTO or base entity).</typeparam>


	public interface ISalesOrderHeaderDataAccess : ICommonDataAccess<SalesOrderHeader, SalesOrderHeaderList, SalesOrderHeaderBase>
	{
        long InsertSalesOrderHeaderSafe(SalesOrderHeader order);
        SalesOrderHeaderList GetOrdersByCompanyCustomer(int companyCustomerId);
        SalesOrderHeaderList GetOrdersByCustomerId(int customerId);
        List<object> GetOrderReceiptByOnlineId(string onlineOrderId);
        SalesOrderHeaderList GetAllSalesOrderHeaders();
        void UpdateStatusSafe(int orderId, string status, bool confirmed);
        List<Dictionary<string, object>> GetVariantsForDropdown();
        (int StockQty, decimal Price)? GetVariantStockAndPrice(int variantId);
        DashboardStats GetDashboardStats(); //new
        List<SalesOrderHeader> GetRecentOrders(int count = 5); //new
        List<ChartDataPoint> GetSalesTrend(int months = 6); //new
        List<ChartDataPoint> GetOrderStatusCounts(); //new

        void UpdateNetAmountSafe(int orderId, decimal newNetAmount);
        //  void UpdatePaymentInfoSafe(int orderId, decimal paidAmount, decimal dueAmount);
        void UpdateTotalAmountSafe(int orderId, decimal newTotalAmount);

        decimal GetProductTotalFromDetails(int orderId);

        void UpdateOrderDeliveryCharge(int orderId, decimal newDeliveryCharge);

        SalesOrderHeader GetBySalesOrderRef(string salesOrderRef);
        SalesOrderHeader GetOrderTotalsSafe(int orderId);
        List<Dictionary<string, object>> GetExportDataDynamic(string whereClause, List<string> columns);
        bool GetConfirmedFlag(int orderId);
        int GetOrderPageNumber(int orderId, int pageSize);
    }
}
