using MDUA.Entities;
using System;
using System.Collections.Generic;

namespace MDUA.Web.UI.Models
{
    public class PagedOrderViewModel
    {
        public List<SalesOrderHeader> Orders { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRows { get; set; }

        public int TotalPages { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }
}