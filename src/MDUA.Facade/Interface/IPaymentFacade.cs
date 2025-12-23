using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDUA.Facade.Interface
{
    public interface IPaymentFacade : ICommonFacade<CustomerPayment, CustomerPaymentList, CustomerPaymentBase>
    {
        long AddPayment(CustomerPayment payment, decimal? deliveryCharge = null);
        List<CompanyPaymentMethod> GetActivePaymentMethods(int companyId);
    }
}
