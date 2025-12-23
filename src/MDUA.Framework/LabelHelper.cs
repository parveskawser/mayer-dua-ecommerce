namespace MDUA.Framework;

public static class LabelHelper
{
    public static string MinimumDateTime = " 00:00:00.000";
    public static string MaximumDateTime = " 23:59:59.999";
    public static string Currency = "K";
    public const string EmailCredential = "EmailCredential";
    public const string PaySlipSalary = "Earning";
    public const string PaySlipSalaryDead = "Deduction";
    public static string CallingFuncExcDoActivityRestaurantMenuCategory = "GotoRestaurantMenuCategory";
    public static string CallingFuncExcDoActivityRestaurantMenuItem = "GotoRestaurantMenuItem";
    public static string CallingFuncExcDoActivityRestaurantLocation = "GotoRestaurantLocation";
    public static string CallingFuncExcDoActivityRestaurantTableSetup = "GotoRestaurantTableSetup";
    public static string CallingFuncExcDoActivityRestaurantCustomerOrder = "GotoRestCustomerOrder";
    public static string CallingFuncExcDoActivityRestaurantCustomerOrderQueue = "GotoRestCustomerOrderQueue";
    public static string CallingFuncExcDoActivityUserList = "GotoUserList";
    public static string CallingFuncExcDoActivityOrganizationList = "GotoOrganizationList";
    public static string CallingFuncExcDoActivityFaqList = "GotoFaqList";
    public static string CallingFuncExcDoActivityQuickLinksList = "GotoQuickLinksList";

    public static class LblUrl
    { 
        public const String GetAllChapter = "/api/v1/get-laws-heading";
        public const String GetChapterByChapterId = "/api/v1/get-law-details";
        public const String GetChapterSearch= "/api/v1/get-search-result";
        public const String GetChapterLawDhara= "/api/v1/get-law-dhara";
        public const String GetChapterLawDetail = "/api/v1/get-law-Detail";

    }
    public static class UserType
    {
        public const string None = "none";
        public const string Merchant = "Merchant";
    }
    public static class AccountDeletionStatus
    {
        public static string Pending { get { return "Pending"; } }
        public static string Approved { get { return "Approved"; } }
        public static string Reject { get { return "Reject"; } }
        public static string Cancel { get { return "Cancel"; } }
    }
}
