using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
    public partial class UserLogin
    {

    }

    public class UserLoginResult
    {
        public UserLogin UserLogin { get; set; }
        //User Permitted Ids
        public bool IsSuccess { get; set; }
        public bool IsAdmin { get; set; }
        public string RoleName { get; set; }
        public string Role { get; set; }
        public string ErrorMessage { get; set; }
        public List<int> ids { get; set; } = new List<int>();
        public List<string> PermissionNames { get; set; } = new List<string>();
        public List<string> AuthorizedActions { get; set; } = new List<string>();
        public bool CanViewProducts { get; set; }
        public List<Product> LastFiveProducts { get; set; } = new List<Product>();
        public List<ProductCategory> Categories { get; set; } = new List<ProductCategory>();
        public List<AttributeName> Attributes { get; set; } = new List<AttributeName>();

        public DashboardStats Stats { get; set; } = new DashboardStats(); //new
        public List<SalesOrderHeader> RecentOrders { get; set; } = new List<SalesOrderHeader>(); //new
        public List<ChartDataPoint> SalesTrend { get; set; } = new List<ChartDataPoint>(); //new
        public List<ChartDataPoint> OrderStatusCounts { get; set; } = new List<ChartDataPoint>(); //new

    }
}