using System;

namespace MDUA.Entities
{
    // A simple container for your KPI numbers
    public class DashboardStats
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int TotalCustomers { get; set; }
        public int TodayOrders { get; set; }
    }
}