using MDUA.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using static MDUA.Entities.SubscriptionPlan;

namespace MDUA.DataAccess
{
    public partial class CompanySubscriptionDataAccess
    {
        private const string SP_CAN_ADD_PRODUCT = "sp_CanAddProduct";

        public QuotaCheckResult CanAddProduct(int companyId)
        {
            QuotaCheckResult result = new QuotaCheckResult();

            using (SqlCommand cmd = GetSPCommand(SP_CAN_ADD_PRODUCT))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId));

                // ✅ FIX: Manually Open the Connection
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                // ✅ FIX: Use CommandBehavior.CloseConnection
                using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (reader.Read())
                    {
                        result.IsAllowed = reader.GetBoolean(0);
                        result.Reason = reader.GetString(1);
                    }
                }
            }
            return result;
        }

    }
}