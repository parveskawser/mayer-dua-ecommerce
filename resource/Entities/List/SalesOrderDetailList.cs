using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "SalesOrderDetailList", Namespace = "http://www.piistech.com//list")]	
	public class SalesOrderDetailList : BaseCollection<SalesOrderDetail>
	{
		#region Constructors
	    public SalesOrderDetailList() : base() { }
        public SalesOrderDetailList(SalesOrderDetail[] list) : base(list) { }
        public SalesOrderDetailList(List<SalesOrderDetail> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
