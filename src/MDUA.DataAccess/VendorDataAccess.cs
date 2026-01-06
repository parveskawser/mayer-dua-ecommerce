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


        public List<Vendor> GetByCompanyId(int companyId)
        {
            string SQL = @"
            SELECT v.* FROM Vendor v
            INNER JOIN CompanyVendor cv ON v.Id = cv.VendorId
            WHERE cv.CompanyId = @CompanyId
            ORDER BY v.VendorName";

            using (SqlCommand cmd = GetSQLCommand(SQL))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId));

                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }

        private const string GETVENDORBYCOMPANYID = "GetVendorByCompanyId";

        public VendorList GetByCompany(int companyId)
        {
            using (SqlCommand cmd = GetSPCommand(GETVENDORBYCOMPANYID))
            {
                AddParameter(cmd, pInt32("CompanyId", companyId));
                return GetList(cmd, ALL_AVAILABLE_RECORDS);
            }
        }
    }

    }
