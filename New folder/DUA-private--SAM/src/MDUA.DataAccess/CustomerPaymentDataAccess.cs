using System;
using System.Data;
using System.Data.SqlClient;

using MDUA.Framework;
using MDUA.Framework.Exceptions;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess
{
	public partial class CustomerPaymentDataAccess
	{
        public decimal GetTotalPaidByOrderRef(string orderRef)
        {
            string sql = @"
        SELECT ISNULL(SUM(Amount), 0)
        FROM CustomerPayment
        WHERE TransactionReference = @Ref";

            using (SqlCommand cmd = GetSQLCommand(sql))
            {
                AddParameter(cmd, pNVarChar("Ref", 50, orderRef));
                object result = SelectScaler(cmd);
                return Convert.ToDecimal(result);
            }
        }

    }
}
