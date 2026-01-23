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


	public interface IGlobalSettingDataAccess : ICommonDataAccess<GlobalSetting, GlobalSettingList, GlobalSettingBase>
	{
        string GetValue(int companyId, string key);
        void SaveValue(int companyId, string key, string value);
        // Used for Favicon
        string GetSetting(int companyId, string key);
        void SaveSetting(int companyId, string key, string content);
    }	
}
