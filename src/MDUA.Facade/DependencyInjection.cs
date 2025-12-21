using Microsoft.Extensions.DependencyInjection;
using MDUA.DataAccess;
using MDUA.DataAccess.Interface;
using MDUA.Facade;
using MDUA.Facade.Interface;

namespace MDUA.Facade
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddService(this IServiceCollection services)
        {
            // Data Access Layer - User & Permissions
            services.AddScoped<IUserLoginDataAccess, UserLoginDataAccess>();
            services.AddScoped<IPermissionGroupMapDataAccess, PermissionGroupMapDataAccess>();
            services.AddScoped<IPermissionDataAccess, PermissionDataAccess>();
            services.AddScoped<IPermissionGroupDataAccess, PermissionGroupDataAccess>();
            services.AddScoped<IUserPermissionDataAccess, UserPermissionDataAccess>();
            services.AddScoped<IPostalCodesDataAccess, PostalCodesDataAccess>();
            services.AddScoped<IUserSessionDataAccess, UserSessionDataAccess>();

            // Product-related Data Access
            services.AddScoped<IAttributeNameDataAccess, AttributeNameDataAccess>();
            services.AddScoped<IProductDataAccess, ProductDataAccess>();
            services.AddScoped<IProductImageDataAccess, ProductImageDataAccess>();
            services.AddScoped<IProductReviewDataAccess, ProductReviewDataAccess>();
            services.AddScoped<IProductVariantDataAccess, ProductVariantDataAccess>();
            services.AddScoped<IProductDiscountDataAccess, ProductDiscountDataAccess>();
            services.AddScoped<IProductCategoryDataAccess, ProductCategoryDataAccess>();
            services.AddScoped<IProductAttributeDataAccess, ProductAttributeDataAccess>();
            services.AddScoped<IProductVideoDataAccess, ProductVideoDataAccess>();

            // Variant & Stock
            services.AddScoped<IVariantImageDataAccess, VariantImageDataAccess>();
            services.AddScoped<IVariantPriceStockDataAccess, VariantPriceStockDataAccess>();

            // Company
            services.AddScoped<ICompanyDataAccess, CompanyDataAccess>();

            // Order Facade Requirements
            services.AddScoped<ISalesOrderHeaderDataAccess, SalesOrderHeaderDataAccess>();
            services.AddScoped<ISalesOrderDetailDataAccess, SalesOrderDetailDataAccess>();
            services.AddScoped<ICustomerDataAccess, CustomerDataAccess>();
            services.AddScoped<ICompanyCustomerDataAccess, CompanyCustomerDataAccess>();
            services.AddScoped<IAddressDataAccess, AddressDataAccess>();

            // ✅ Purchase Facade Requirements (Inventory & POs)
            services.AddScoped<IPoRequestedDataAccess, PoRequestedDataAccess>();
            services.AddScoped<IPoReceivedDataAccess, PoReceivedDataAccess>();
            services.AddScoped<IVendorDataAccess, VendorDataAccess>();

            // ✅ Required for stock transaction logging
            services.AddScoped<IInventoryTransactionDataAccess, InventoryTransactionDataAccess>();

            // ✅ RESTORED MISSING LINE: Bulk Purchase Order
            services.AddScoped<IBulkPurchaseOrderDataAccess, BulkPurchaseOrderDataAccess>();

            services.AddScoped<IChatDataAccess, MDUA.DataAccess.ChatDataAccess>();

            // Payment Related
            services.AddScoped<IPaymentMethodDataAccess, PaymentMethodDataAccess>();
            services.AddScoped<ICompanyPaymentMethodDataAccess, CompanyPaymentMethodDataAccess>();

            // ✅ ADDED THIS (Required for PaymentFacade)
            services.AddScoped<ICustomerPaymentDataAccess, CustomerPaymentDataAccess>();

            //Global Settings
            services.AddScoped<IGlobalSettingDataAccess, GlobalSettingDataAccess>();

            services.AddScoped<IDeliveryDataAccess, DeliveryDataAccess>();

            // Facade Layer
            services.AddServiceFacade();

            return services;
        }

        private static void AddServiceFacade(this IServiceCollection services)
        {
            services.AddScoped<IUserLoginFacade, UserLoginFacade>();
            services.AddScoped<IProductFacade, ProductFacade>();
            services.AddScoped<IOrderFacade, OrderFacade>();
            services.AddScoped<ICustomerFacade, CustomerFacade>();
            services.AddScoped<ICompanyFacade, CompanyFacade>();
            services.AddScoped<IChatFacade, ChatFacade>();
            services.AddScoped<IPurchaseFacade, PurchaseFacade>();
            services.AddScoped<IPaymentMethodFacade, PaymentMethodFacade>(); // Admin setup facade
            services.AddScoped<ISettingsFacade, SettingsFacade>();
            services.AddScoped<IDeliveryFacade, DeliveryFacade>();
            // ✅ ADDED THIS (Required for OrderController)
            services.AddScoped<IPaymentFacade, PaymentFacade>();
        }
    }
}