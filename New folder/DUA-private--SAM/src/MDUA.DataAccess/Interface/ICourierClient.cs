using MDUA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDUA.DataAccess.Interface
{
    public interface ICourierClient
    {
        string CarrierName { get; }

        Task<CourierShipmentResult> CreateShipmentAsync(CourierShipmentRequest request);
    }

}
