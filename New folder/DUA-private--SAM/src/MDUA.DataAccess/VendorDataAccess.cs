using System;
using System.Collections.Generic; // Required for List<>
using System.Data;
using System.Data.SqlClient;
using MDUA.Framework;
using MDUA.Framework.Exceptions;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.DataAccess.Interface; // Ensure this is using the interface namespace

namespace MDUA.DataAccess
{
    public partial class VendorDataAccess : IVendorDataAccess
    {
        public long InsertPayment(VendorPayment payment)
        {
            const string sql = "InsertVendorPayment";

            using (var cmd = GetSPCommand(sql))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // 1. Fixed Parameter Name: Matches SQL script "@Id", not "@NewPaymentId"
                var idParam = new SqlParameter("@Id", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(idParam);

                cmd.Parameters.AddWithValue("@VendorId", payment.VendorId);
                cmd.Parameters.AddWithValue("@PaymentMethodId", payment.PaymentMethodId);

                // Handle PaymentType (if null, default to Manual)
                cmd.Parameters.AddWithValue("@PaymentType", payment.PaymentType ?? "Manual");

                cmd.Parameters.AddWithValue("@Amount", payment.Amount);
                cmd.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
                cmd.Parameters.AddWithValue("@Status", "Completed"); // Default status
                cmd.Parameters.AddWithValue("@CreatedBy", payment.CreatedBy ?? "System");
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

                // Optional/Nullable Parameters
                cmd.Parameters.AddWithValue("@PoReceivedId", payment.PoReceivedId.HasValue ? (object)payment.PoReceivedId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@PoRequestedId", payment.PoRequestedId.HasValue ? (object)payment.PoRequestedId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@ReferenceNumber", (object)payment.ReferenceNumber ?? DBNull.Value); // Check Property Name vs Entity
                cmd.Parameters.AddWithValue("@Notes", (object)payment.Notes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@InventoryTransactionId", DBNull.Value); // Required by SP structure if not passed

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                cmd.ExecuteNonQuery();

                return Convert.ToInt64(idParam.Value);
            }
        }

        // =========================================================
        // ✅ FIXED: Implemented the missing interface method
        // =========================================================
        // MDUA.DataAccess/VendorDataAccess.cs

        public List<dynamic> GetPendingBills(int vendorId, int companyId) // <--- Updated Signature
        {
            var list = new List<dynamic>();
            const string sql = "GetUnpaidPoReceivedByVendor";

            using (var cmd = GetSPCommand(sql))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@VendorId", vendorId);

                // ✅ Add CompanyId Parameter
                cmd.Parameters.AddWithValue("@CompanyId", companyId);

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new
                        {
                            Id = reader["Id"],
                            InvoiceNo = reader["InvoiceNo"].ToString(),
                            BalanceDue = reader["BalanceDue"] != DBNull.Value ? Convert.ToDecimal(reader["BalanceDue"]) : 0,
                            ReceivedDate = Convert.ToDateTime(reader["ReceivedDate"]).ToString("dd-MMM-yyyy"),
                            TotalAmount = reader["TotalPaymentDue"]
                        });
                    }
                }
            }
            return list;
        }
        public void ApplyCredit(int creditPaymentId, int billId, decimal amount, string username)
        {
            const string sql = "ApplyVendorCredit";
            using (var cmd = GetSPCommand(sql))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CreditPaymentId", creditPaymentId);
                cmd.Parameters.AddWithValue("@TargetPoReceivedId", billId);
                cmd.Parameters.AddWithValue("@AmountToApply", amount);
                cmd.Parameters.AddWithValue("@UpdatedBy", username);

                if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Helper to get available credits for dropdown
        public List<dynamic> GetAvailableCredits(int vendorId)
        {
            var list = new List<dynamic>();
            const string sql = "GetVendorAvailableCredits"; // The new SP we created

            using (var cmd = GetSPCommand(sql)) // Now using GetSPCommand (Safe)
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@VendorId", vendorId);

                if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new
                        {
                            Id = reader["Id"],
                            Amount = reader["Amount"],
                            // Safe Date Conversion
                            Date = reader["PaymentDate"] != DBNull.Value
                                   ? Convert.ToDateTime(reader["PaymentDate"]).ToString("dd-MMM-yyyy")
                                   : "-",
                            Ref = reader["ReferenceNumber"] != DBNull.Value
                                  ? reader["ReferenceNumber"].ToString()
                                  : ""
                        });
                    }
                }
            }
            return list;
        }


        public VendorList GetByCompany(int companyId)
        {
            // Added Created/Updated columns to the SELECT list
            string SQL = @"
    SELECT 
        v.Id, 
        v.VendorName, 
        v.Email, 
        v.Phone,
        v.CreatedBy,
        v.CreatedAt,
        v.UpdatedBy,
        v.UpdatedAt,
        
        -- Custom Calculated Columns
        CAST(ISNULL(req.ReqCount, 0) AS INT)     AS TotalRequestedCount,
        CAST(ISNULL(req.ReqQty, 0) AS INT)       AS TotalRequestedQty,

        CAST(ISNULL(rec.RecCount, 0) AS INT)     AS TotalReceivedCount,
        CAST(ISNULL(rec.RecQty, 0) AS INT)       AS TotalReceivedQty,
        CAST(ISNULL(rec.UnpaidCount, 0) AS INT)  AS TotalUnpaidCount,

        CAST(ISNULL(rec.TotalAmt, 0) AS DECIMAL(18,2)) AS TotalAmount,
        CAST(ISNULL(rec.PaidAmt, 0) AS DECIMAL(18,2))  AS TotalPaidAmount,
        
        CAST((ISNULL(rec.TotalAmt, 0) - ISNULL(rec.PaidAmt, 0)) AS DECIMAL(18,2)) AS TotalDueAmount

    FROM Vendor v
    INNER JOIN CompanyVendor cv ON v.Id = cv.VendorId
    
    -- 1. Aggregates for Requested
    OUTER APPLY (
        SELECT COUNT(Id) as ReqCount, SUM(Quantity) as ReqQty 
        FROM PoRequested WHERE VendorId = v.Id
    ) req

    -- 2. Aggregates for Received
    OUTER APPLY (
        SELECT 
            COUNT(Id) as RecCount,
            SUM(ReceivedQuantity) as RecQty,
            SUM(CASE WHEN PaymentStatus IN ('Unpaid', 'Partial') THEN 1 ELSE 0 END) as UnpaidCount,
            SUM(TotalPaymentDue) as TotalAmt,
            SUM(TotalPaid) as PaidAmt
        FROM PoReceived WHERE VendorId = v.Id
    ) rec

    WHERE cv.CompanyId = @CompanyId
    ORDER BY v.VendorName";

            var list = new VendorList();

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId));

                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var entity = new Vendor();

                        // --- 1. Map Base Properties ---
                        entity.Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0;
                        entity.VendorName = reader["VendorName"] != DBNull.Value ? reader["VendorName"].ToString() : "";
                        entity.Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "";
                        entity.Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : "";

                        // --- Map Audit Columns ---
                        entity.CreatedBy = reader["CreatedBy"] != DBNull.Value ? reader["CreatedBy"].ToString() : "";
                        entity.CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : (DateTime?)null;
                        entity.UpdatedBy = reader["UpdatedBy"] != DBNull.Value ? reader["UpdatedBy"].ToString() : "";
                        entity.UpdatedAt = reader["UpdatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["UpdatedAt"]) : (DateTime?)null;

                        // --- 2. Map Aggregate Properties ---
                        entity.TotalRequestedCount = (int)reader["TotalRequestedCount"];
                        entity.TotalRequestedQty = (int)reader["TotalRequestedQty"];

                        entity.TotalReceivedCount = (int)reader["TotalReceivedCount"];
                        entity.TotalReceivedQty = (int)reader["TotalReceivedQty"];
                        entity.TotalUnpaidCount = (int)reader["TotalUnpaidCount"];

                        entity.TotalAmount = (decimal)reader["TotalAmount"];
                        entity.TotalPaidAmount = (decimal)reader["TotalPaidAmount"];
                        entity.TotalDueAmount = (decimal)reader["TotalDueAmount"];

                        list.Add(entity);
                    }
                }
            }

            return list;
        }
    }

    }
