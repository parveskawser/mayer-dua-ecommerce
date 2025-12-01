using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Facade.Interface;
using System.Collections.Generic;

namespace MDUA.Facade
{
    public class CompanyFacade : ICompanyFacade
    {
        private readonly ICompanyDataAccess _companyDataAccess;

        public CompanyFacade(ICompanyDataAccess companyDataAccess)
        {
            _companyDataAccess = companyDataAccess;
        }

        #region common implementation 
        //new

        public Company Get(int _Id)
        {
            return _companyDataAccess.Get(_Id);
        }

        #endregion

        #region extented implementation

        // This is where we could add specific logic, like 
        // "GetCompanyByCode" or specific validation if needed later.
        // For now, the standard Get(id) in the common region is sufficient 
        // to populate the sidebar.

        #endregion
    }
}