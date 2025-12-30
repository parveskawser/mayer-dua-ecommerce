using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Framework; // <--- ADD THIS LINE

using MDUA.Entities.List;
using MDUA.Facade.Interface;

namespace MDUA.Facade
{
    public class VendorFacade : IVendorFacade
    {
        private readonly IVendorDataAccess _vendorDataAccess;

        public VendorFacade(IVendorDataAccess vendorDataAccess)
        {
            _vendorDataAccess = vendorDataAccess;
        }

        public long Insert(Vendor vendor)
        {
            // Set audit fields if necessary or handle in logic
            vendor.CreatedAt = System.DateTime.Now; 
            return _vendorDataAccess.Insert(vendor);
        }

        public long Update(Vendor vendor)
        {
            vendor.UpdatedAt = System.DateTime.Now;
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
    }
}