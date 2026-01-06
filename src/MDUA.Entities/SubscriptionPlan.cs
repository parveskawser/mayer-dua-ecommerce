using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	public partial class SubscriptionPlan 
	{
        public class QuotaCheckResult
        {
            public bool IsAllowed { get; set; }
            public string Reason { get; set; }

            public QuotaCheckResult()
            {
                IsAllowed = false;
                Reason = "UNKNOWN";
            }
        }
    }
}