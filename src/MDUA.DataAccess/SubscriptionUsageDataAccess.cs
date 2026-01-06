using System;
using System.Data;
using System.Data.SqlClient;
using static MDUA.Entities.SubscriptionPlan;

namespace MDUA.DataAccess
{
    public partial class SubscriptionUsageDataAccess
    {
        private const string SP_TRY_CONSUME_ORDER_QUOTA = "sp_TryConsumeOrderQuota";

        public QuotaCheckResult TryConsumeOrderQuota(int companyId)
        {
            QuotaCheckResult result = new QuotaCheckResult();

            using (SqlCommand cmd = GetSPCommand(SP_TRY_CONSUME_ORDER_QUOTA))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId));

                // ✅ FIX: Manually Open the Connection
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                // ✅ FIX: Use CommandBehavior.CloseConnection
                // This ensures the connection closes when the 'using (reader)' block ends
                using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (reader.Read())
                    {
                        // Safely handle potential DBNulls or Index issues
                        result.IsAllowed = reader.GetBoolean(0);
                        result.Reason = reader.GetString(1);
                    }
                }
            }
            return result;
        }
    }
}