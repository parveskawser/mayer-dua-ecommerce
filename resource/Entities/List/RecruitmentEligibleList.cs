using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "RecruitmentEligibleList", Namespace = "http://www.piistech.com//list")]	
	public class RecruitmentEligibleList : BaseCollection<RecruitmentEligible>
	{
		#region Constructors
	    public RecruitmentEligibleList() : base() { }
        public RecruitmentEligibleList(RecruitmentEligible[] list) : base(list) { }
        public RecruitmentEligibleList(List<RecruitmentEligible> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
