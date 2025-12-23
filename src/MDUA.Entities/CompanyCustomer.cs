using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	public partial class CompanyCustomer 
	{
        [DataMember]
        public virtual Customer Customer { get; set; }
    }
}
