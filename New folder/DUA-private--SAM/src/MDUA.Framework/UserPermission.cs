using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MDUA.Framework
{
    public class UserPermissions
    {
        public static class LeftMenu
        { 
            public static int Dashboard { get { return 100; } } 
            public static int Club { get { return 101; } }
            public static int Team { get { return 102; } }
            public static int User { get { return 103; } }
            public static int Exercise { get { return 104; } }
            public static int Category { get { return 105; } } 
            public static int Permission { get { return 106; } }
            public static int Chat { get { return 108; } }
            public static int AccessAllPages { get { return 1000; } }

        }
        public static class Dashboard
        {
            public static int View { get { return 101; } }
            public static int TotalSales { get { return 102; } }
            public static int TotalTicketondashboard { get { return 153; } }
        } 
    }
}