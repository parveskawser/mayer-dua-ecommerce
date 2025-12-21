using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	public partial class SalesOrderDetail 
	{
        [DataMember]
        public int ProductVariantId { get; set; }
        [DataMember]
        public virtual ProductVariant ProductVariant { get; set; }
    }
}
