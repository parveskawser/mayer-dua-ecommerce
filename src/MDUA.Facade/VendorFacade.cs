using System;
using System.Collections.Generic;
using MDUA.Framework;
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.List;
using MDUA.Facade.Interface;

namespace MDUA.Facade
{
    public class VendorFacade : IVendorFacade
    {
        private readonly IVendorDataAccess _vendorDataAccess;
        // 1. Define the private field
        private readonly IPoRequestedDataAccess _poRequestedDataAccess; 

        // 2. Inject it in the constructor
        public VendorFacade(IVendorDataAccess vendorDataAccess, IPoRequestedDataAccess poRequestedDataAccess)
        {
            _vendorDataAccess = vendorDataAccess;
            
            // ✅ FIX FOR CS0103: Assign the parameter to the field
            _poRequestedDataAccess = poRequestedDataAccess;
        }

        public long Insert(Vendor vendor)
        {
            vendor.CreatedAt = DateTime.Now; 
            return _vendorDataAccess.Insert(vendor);
        }

        public long Update(Vendor vendor)
        {
            vendor.UpdatedAt = DateTime.UtcNow;
            return _vendorDataAccess.Update(vendor);
        }

        public long Delete(int id)
        {
            return _vendorDataAccess.Delete(id);
        }

        public Vendor Get(int id)
        {
            return _vendorDataAccess.Get(id);
        }

        public VendorList GetAll()
        {
            return _vendorDataAccess.GetAll();
        }
        
        public VendorList GetPaged(PagedRequest request)
        {
            return _vendorDataAccess.GetPaged(request);
        }

        // ✅ FIX FOR CS1061: This will now work because the Interface has the method
        public List<dynamic> GetVendorOrderHistory(int vendorId)
        {
            return _poRequestedDataAccess.GetVendorHistory(vendorId);
        }
    }
}