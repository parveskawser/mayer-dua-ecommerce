using System;
using System.Data;
using System.Data.SqlClient;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Framework;             // <--- FIX: Added this
using MDUA.Framework.Exceptions;

namespace MDUA.DataAccess
{
    public partial class BulkPurchaseOrderDataAccess
    {
        /// <summary>
        /// Custom Insert implementation to support SqlTransaction.
        /// </summary>
        public long Insert(BulkPurchaseOrder obj, SqlTransaction trans = null)
        {
            try
            {
                // Note: Make sure "InsertBulkPurchaseOrder" matches the const in the generated file
                // If the const is private, hardcoding string here is fine.
                SqlCommand cmd = GetSPCommand("InsertBulkPurchaseOrder");

                // Apply the transaction if provided
                if (trans != null)
                {
                    cmd.Connection = trans.Connection;
                    cmd.Transaction = trans;
                }

                // Output Parameter
                AddParameter(cmd, pInt32Out(BulkPurchaseOrderBase.Property_Id));

                // Common Parameters
                AddParameter(cmd, pInt32(BulkPurchaseOrderBase.Property_VendorId, obj.VendorId));
                AddParameter(cmd, pNVarChar(BulkPurchaseOrderBase.Property_AgreementNumber, 50, obj.AgreementNumber));
                AddParameter(cmd, pNVarChar(BulkPurchaseOrderBase.Property_Title, 200, obj.Title));
                AddParameter(cmd, pDateTime(BulkPurchaseOrderBase.Property_AgreementDate, obj.AgreementDate));
                AddParameter(cmd, pDateTime(BulkPurchaseOrderBase.Property_ExpiryDate, obj.ExpiryDate));
                AddParameter(cmd, pInt32(BulkPurchaseOrderBase.Property_TotalTargetQuantity, obj.TotalTargetQuantity));
                AddParameter(cmd, pDecimal(BulkPurchaseOrderBase.Property_TotalTargetAmount, 9, obj.TotalTargetAmount));
                AddParameter(cmd, pNVarChar(BulkPurchaseOrderBase.Property_Status, 20, obj.Status));
                AddParameter(cmd, pNVarChar(BulkPurchaseOrderBase.Property_Remarks, 500, obj.Remarks));
                AddParameter(cmd, pNVarChar(BulkPurchaseOrderBase.Property_CreatedBy, 100, obj.CreatedBy));
                AddParameter(cmd, pDateTime(BulkPurchaseOrderBase.Property_CreatedAt, obj.CreatedAt));
                AddParameter(cmd, pNVarChar(BulkPurchaseOrderBase.Property_UpdatedBy, 100, obj.UpdatedBy));
                AddParameter(cmd, pDateTime(BulkPurchaseOrderBase.Property_UpdatedAt, obj.UpdatedAt));

                long result = InsertRecord(cmd);

                if (result > 0)
                {
                    // FIX: This now works because we added 'using MDUA.Framework;'
                    obj.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
                    obj.Id = (Int32)GetOutParameter(cmd, BulkPurchaseOrderBase.Property_Id);
                }
                return result;
            }
            catch (SqlException x)
            {
                throw new ObjectInsertException(obj, x);
            }
        }
    }
}