using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using MDUA.Framework;
using MDUA.Framework.DataAccess;
using MDUA.Framework.Exceptions;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess
{
	public partial class LanguageResourceDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTLANGUAGERESOURCE = "InsertLanguageResource";
		private const string UPDATELANGUAGERESOURCE = "UpdateLanguageResource";
		private const string DELETELANGUAGERESOURCE = "DeleteLanguageResource";
		private const string GETLANGUAGERESOURCEBYID = "GetLanguageResourceById";
		private const string GETALLLANGUAGERESOURCE = "GetAllLanguageResource";
		private const string GETPAGEDLANGUAGERESOURCE = "GetPagedLanguageResource";
		private const string GETLANGUAGERESOURCEBYCOMPANYID = "GetLanguageResourceByCompanyId";
		private const string GETLANGUAGERESOURCEBYLANGUAGEID = "GetLanguageResourceByLanguageId";
		private const string GETLANGUAGERESOURCEMAXIMUMID = "GetLanguageResourceMaximumId";
		private const string GETLANGUAGERESOURCEROWCOUNT = "GetLanguageResourceRowCount";	
		private const string GETLANGUAGERESOURCEBYQUERY = "GetLanguageResourceByQuery";
		#endregion
		
		#region Constructors
		public LanguageResourceDataAccess(IConfiguration configuration) : base(configuration) { }
		public LanguageResourceDataAccess(ClientContext context) : base(context) { }
		public LanguageResourceDataAccess(SqlTransaction transaction) : base(transaction) { }
		public LanguageResourceDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="languageResourceObject"></param>
		private void AddCommonParams(SqlCommand cmd, LanguageResourceBase languageResourceObject)
		{	
			AddParameter(cmd, pInt32(LanguageResourceBase.Property_CompanyId, languageResourceObject.CompanyId));
			AddParameter(cmd, pInt32(LanguageResourceBase.Property_LanguageId, languageResourceObject.LanguageId));
			AddParameter(cmd, pNVarChar(LanguageResourceBase.Property_LKey, 255, languageResourceObject.LKey));
			AddParameter(cmd, pNVarChar(LanguageResourceBase.Property_LValue, languageResourceObject.LValue));
			AddParameter(cmd, pNVarChar(LanguageResourceBase.Property_Description, 255, languageResourceObject.Description));
			AddParameter(cmd, pBool(LanguageResourceBase.Property_IsActive, languageResourceObject.IsActive));
			AddParameter(cmd, pNVarChar(LanguageResourceBase.Property_CreatedBy, 100, languageResourceObject.CreatedBy));
			AddParameter(cmd, pDateTime(LanguageResourceBase.Property_CreatedAt, languageResourceObject.CreatedAt));
			AddParameter(cmd, pNVarChar(LanguageResourceBase.Property_UpdatedBy, 100, languageResourceObject.UpdatedBy));
			AddParameter(cmd, pDateTime(LanguageResourceBase.Property_UpdatedAt, languageResourceObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts LanguageResource
        /// </summary>
        /// <param name="languageResourceObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(LanguageResourceBase languageResourceObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTLANGUAGERESOURCE);
	
				AddParameter(cmd, pInt32Out(LanguageResourceBase.Property_Id));
				AddCommonParams(cmd, languageResourceObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					languageResourceObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					languageResourceObject.Id = (Int32)GetOutParameter(cmd, LanguageResourceBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(languageResourceObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates LanguageResource
        /// </summary>
        /// <param name="languageResourceObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(LanguageResourceBase languageResourceObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATELANGUAGERESOURCE);
				
				AddParameter(cmd, pInt32(LanguageResourceBase.Property_Id, languageResourceObject.Id));
				AddCommonParams(cmd, languageResourceObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					languageResourceObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(languageResourceObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes LanguageResource
        /// </summary>
        /// <param name="Id">Id of the LanguageResource object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETELANGUAGERESOURCE);	
				
				AddParameter(cmd, pInt32(LanguageResourceBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(LanguageResource), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves LanguageResource object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the LanguageResource object to retrieve</param>
        /// <returns>LanguageResource object, null if not found</returns>
		public LanguageResource Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETLANGUAGERESOURCEBYID))
			{
				AddParameter( cmd, pInt32(LanguageResourceBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all LanguageResource objects 
        /// </summary>
        /// <returns>A list of LanguageResource objects</returns>
		public LanguageResourceList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLLANGUAGERESOURCE))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all LanguageResource objects by CompanyId
        /// </summary>
        /// <returns>A list of LanguageResource objects</returns>
		public LanguageResourceList GetByCompanyId(Nullable<Int32> _CompanyId)
		{
			using( SqlCommand cmd = GetSPCommand(GETLANGUAGERESOURCEBYCOMPANYID))
			{
				
				AddParameter( cmd, pInt32(LanguageResourceBase.Property_CompanyId, _CompanyId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all LanguageResource objects by LanguageId
        /// </summary>
        /// <returns>A list of LanguageResource objects</returns>
		public LanguageResourceList GetByLanguageId(Int32 _LanguageId)
		{
			using( SqlCommand cmd = GetSPCommand(GETLANGUAGERESOURCEBYLANGUAGEID))
			{
				
				AddParameter( cmd, pInt32(LanguageResourceBase.Property_LanguageId, _LanguageId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all LanguageResource objects by PageRequest
        /// </summary>
        /// <returns>A list of LanguageResource objects</returns>
		public LanguageResourceList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDLANGUAGERESOURCE))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				LanguageResourceList _LanguageResourceList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _LanguageResourceList;
			}
		}
		
		/// <summary>
        /// Retrieves all LanguageResource objects by query String
        /// </summary>
        /// <returns>A list of LanguageResource objects</returns>
		public LanguageResourceList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETLANGUAGERESOURCEBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get LanguageResource Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of LanguageResource
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETLANGUAGERESOURCEMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get LanguageResource Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of LanguageResource
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _LanguageResourceRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETLANGUAGERESOURCEROWCOUNT))
			{
				SqlDataReader reader;
				_LanguageResourceRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _LanguageResourceRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills LanguageResource object
        /// </summary>
        /// <param name="languageResourceObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(LanguageResourceBase languageResourceObject, SqlDataReader reader, int start)
		{
			
				languageResourceObject.Id = reader.GetInt32( start + 0 );			
				if(!reader.IsDBNull(1)) languageResourceObject.CompanyId = reader.GetInt32( start + 1 );			
				languageResourceObject.LanguageId = reader.GetInt32( start + 2 );			
				languageResourceObject.LKey = reader.GetString( start + 3 );			
				languageResourceObject.LValue = reader.GetString( start + 4 );			
				if(!reader.IsDBNull(5)) languageResourceObject.Description = reader.GetString( start + 5 );			
				languageResourceObject.IsActive = reader.GetBoolean( start + 6 );			
				if(!reader.IsDBNull(7)) languageResourceObject.CreatedBy = reader.GetString( start + 7 );			
				languageResourceObject.CreatedAt = reader.GetDateTime( start + 8 );			
				if(!reader.IsDBNull(9)) languageResourceObject.UpdatedBy = reader.GetString( start + 9 );			
				if(!reader.IsDBNull(10)) languageResourceObject.UpdatedAt = reader.GetDateTime( start + 10 );			
			FillBaseObject(languageResourceObject, reader, (start + 11));

			
			languageResourceObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills LanguageResource object
        /// </summary>
        /// <param name="languageResourceObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(LanguageResourceBase languageResourceObject, SqlDataReader reader)
		{
			FillObject(languageResourceObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves LanguageResource object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>LanguageResource object</returns>
		private LanguageResource GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					LanguageResource languageResourceObject= new LanguageResource();
					FillObject(languageResourceObject, reader);
					return languageResourceObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of LanguageResource objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of LanguageResource objects</returns>
		private LanguageResourceList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//LanguageResource list
			LanguageResourceList list = new LanguageResourceList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					LanguageResource languageResourceObject = new LanguageResource();
					FillObject(languageResourceObject, reader);

					list.Add(languageResourceObject);
				}
				
				// Close the reader in order to receive output parameters
				// Output parameters are not available until reader is closed.
				reader.Close();
			}

			return list;
		}
		
		#endregion
	}	
}