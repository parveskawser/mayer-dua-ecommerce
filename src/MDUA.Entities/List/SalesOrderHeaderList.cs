using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "SalesOrderHeaderList", Namespace = "http://www.piistech.com//list")]	
	public class SalesOrderHeaderList : BaseCollection<SalesOrderHeader>
	{
		#region Constructors
	    public SalesOrderHeaderList() : base() { }
        public SalesOrderHeaderList(SalesOrderHeader[] list) : base(list) { }
        public SalesOrderHeaderList(List<SalesOrderHeader> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
