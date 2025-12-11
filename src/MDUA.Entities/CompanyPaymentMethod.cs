using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
    public partial class CompanyPaymentMethod : CompanyPaymentMethodBase
    {
        [DataMember]
        public string MethodName { get; set; }
    }

    public class CompanyPaymentMethodResult
    {
        public int PaymentMethodId { get; set; }
        public string MethodName { get; set; }
        public string LogoUrl { get; set; }
        public string SystemCode { get; set; }

        // Global Definitions (Read Only)
        public bool GlobalSupportsManual { get; set; }
        public bool GlobalSupportsGateway { get; set; }
        public string DefaultInstruction { get; set; }

        // Company Configuration (Editable)
        public bool IsEnabled { get; set; }
        public bool IsManualEnabled { get; set; }
        public bool IsGatewayEnabled { get; set; }
        public string CustomInstruction { get; set; }

    }
}