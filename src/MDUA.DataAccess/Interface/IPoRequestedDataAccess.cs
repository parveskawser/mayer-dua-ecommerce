using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess.Interface
{
    /// <summary>
    /// ICommonDataAccess Provides a generic contract for data access operations 
    /// (CRUD and utility methods) that can be implemented for any entity type.
    /// 
    /// This interface defines common patterns such as:
    /// - Creating new records (Insert)
    /// - Reading single or multiple records (Get, GetAll, GetByQuery, GetPaged)
    /// - Updating existing records (Update)
    /// - Deleting records (Delete)
    /// - Utility operations like retrieving maximum ID and row count
    /// 
    /// By using generics (<typeparamref name="T"/>, <typeparamref name="L"/>, <typeparamref name="B"/>),
    /// it ensures reusability across multiple entities while maintaining 
    /// type safety and consistency.
    /// </summary>
    /// <typeparam name="T">Represents the entity type for a single record.</typeparam>
    /// <typeparam name="L">Represents the collection type for multiple records (e.g., a list).</typeparam>
    /// <typeparam name="B">Represents the base type used for insert and update operations (e.g., DTO or base entity).</typeparam>


    public interface IPoRequestedDataAccess : ICommonDataAccess<PoRequested, PoRequestedList, PoRequestedBase>
    {
        // ✅ NEW: Add this missing definition
        List<dynamic> GetInventoryStatus();
        dynamic GetPendingRequestByVariant(int variantId);
        void UpdateStatus(int poId, string status, SqlTransaction transaction);
        dynamic GetVariantStatus(int variantId);
        long Insert(PoRequestedBase obj, SqlTransaction trans);
        List<dynamic> GetVendorHistory(int vendorId);
        (List<dynamic> Items, int TotalCount) GetVendorHistoryPaged(int vendorId, int pageIndex, int pageSize);
        
        (List<dynamic> Items, int TotalCount) GetVendorHistoryPaged(int vendorId, int pageIndex, int pageSize, string search, string status, string type);
        
        
        (List<dynamic>, int) GetVendorHistoryPaged(int vendorId, int pageIndex, int pageSize, string search, string status, string type, DateTime? fromDate, DateTime? toDate);

    }
}
