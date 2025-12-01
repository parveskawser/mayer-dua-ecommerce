using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;


namespace MDUA.Facade.Interface
{
    public interface IOrderFacade : ICommonFacade<SalesOrderHeader, SalesOrderHeaderList, SalesOrderHeaderBase>
    {
        string PlaceGuestOrder(SalesOrderHeader orderData);
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

        //new
        string UpdateOrderConfirmation(int orderId, bool isConfirmed);
        List<dynamic> GetProductVariantsForAdmin();
        string PlaceAdminOrder(SalesOrderHeader orderData);


    }
}
