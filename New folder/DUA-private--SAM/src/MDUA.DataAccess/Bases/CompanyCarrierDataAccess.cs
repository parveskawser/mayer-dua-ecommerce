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
	public partial class CompanyCarrierDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTCOMPANYCARRIER = "InsertCompanyCarrier";
		private const string UPDATECOMPANYCARRIER = "UpdateCompanyCarrier";
		private const string DELETECOMPANYCARRIER = "DeleteCompanyCarrier";
		private const string GETCOMPANYCARRIERBYID = "GetCompanyCarrierById";
		private const string GETALLCOMPANYCARRIER = "GetAllCompanyCarrier";
		private const string GETPAGEDCOMPANYCARRIER = "GetPagedCompanyCarrier";
		private const string GETCOMPANYCARRIERBYCOMPANYID = "GetCompanyCarrierByCompanyId";
		private const string GETCOMPANYCARRIERBYCARRIERID = "GetCompanyCarrierByCarrierId";
		private const string GETCOMPANYCARRIERMAXIMUMID = "GetCompanyCarrierMaximumId";
		private const string GETCOMPANYCARRIERROWCOUNT = "GetCompanyCarrierRowCount";	
		private const string GETCOMPANYCARRIERBYQUERY = "GetCompanyCarrierByQuery";
		#endregion
		
		#region Constructors
		public CompanyCarrierDataAccess(IConfiguration configuration) : base(configuration) { }
		public CompanyCarrierDataAccess(ClientContext context) : base(context) { }
		public CompanyCarrierDataAccess(SqlTransaction transaction) : base(transaction) { }
		public CompanyCarrierDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="companyCarrierObject"></param>
		private void AddCommonParams(SqlCommand cmd, CompanyCarrierBase companyCarrierObject)
		{	
			AddParameter(cmd, pInt32(CompanyCarrierBase.Property_CompanyId, companyCarrierObject.CompanyId));
			AddParameter(cmd, pInt32(CompanyCarrierBase.Property_CarrierId, companyCarrierObject.CarrierId));
			AddParameter(cmd, pBool(CompanyCarrierBase.Property_IsActive, companyCarrierObject.IsActive));
			AddParameter(cmd, pNVarChar(CompanyCarrierBase.Property_ApiKeyEncrypted, companyCarrierObject.ApiKeyEncrypted));
			AddParameter(cmd, pNVarChar(CompanyCarrierBase.Property_ApiSecretEncrypted, companyCarrierObject.ApiSecretEncrypted));
			AddParameter(cmd, pNVarChar(CompanyCarrierBase.Property_ApiKeyLast4, 4, companyCarrierObject.ApiKeyLast4));
			AddParameter(cmd, pDateTime(CompanyCarrierBase.Property_SecretUpdatedAt, companyCarrierObject.SecretUpdatedAt));
			AddParameter(cmd, pNVarChar(CompanyCarrierBase.Property_ApiUsernameEncrypted, 2000, companyCarrierObject.ApiUsernameEncrypted));
			AddParameter(cmd, pNVarChar(CompanyCarrierBase.Property_ApiPasswordEncrypted, 2000, companyCarrierObject.ApiPasswordEncrypted));
			AddParameter(cmd, pInt32(CompanyCarrierBase.Property_StoreId, companyCarrierObject.StoreId));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts CompanyCarrier
        /// </summary>
        /// <param name="companyCarrierObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(CompanyCarrierBase companyCarrierObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTCOMPANYCARRIER);
	
				AddParameter(cmd, pInt32Out(CompanyCarrierBase.Property_Id));
				AddCommonParams(cmd, companyCarrierObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					companyCarrierObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					companyCarrierObject.Id = (Int32)GetOutParameter(cmd, CompanyCarrierBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(companyCarrierObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates CompanyCarrier
        /// </summary>
        /// <param name="companyCarrierObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(CompanyCarrierBase companyCarrierObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATECOMPANYCARRIER);
				
				AddParameter(cmd, pInt32(CompanyCarrierBase.Property_Id, companyCarrierObject.Id));
				AddCommonParams(cmd, companyCarrierObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					companyCarrierObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(companyCarrierObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes CompanyCarrier
        /// </summary>
        /// <param name="Id">Id of the CompanyCarrier object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETECOMPANYCARRIER);	
				
				AddParameter(cmd, pInt32(CompanyCarrierBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(CompanyCarrier), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves CompanyCarrier object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the CompanyCarrier object to retrieve</param>
        /// <returns>CompanyCarrier object, null if not found</returns>
		public CompanyCarrier Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYCARRIERBYID))
			{
				AddParameter( cmd, pInt32(CompanyCarrierBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all CompanyCarrier objects 
        /// </summary>
        /// <returns>A list of CompanyCarrier objects</returns>
		public CompanyCarrierList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLCOMPANYCARRIER))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all CompanyCarrier objects by CompanyId
        /// </summary>
        /// <returns>A list of CompanyCarrier objects</returns>
		public CompanyCarrierList GetByCompanyId(Int32 _CompanyId)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYCARRIERBYCOMPANYID))
			{
				
				AddParameter( cmd, pInt32(CompanyCarrierBase.Property_CompanyId, _CompanyId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all CompanyCarrier objects by CarrierId
        /// </summary>
        /// <returns>A list of CompanyCarrier objects</returns>
		public CompanyCarrierList GetByCarrierId(Int32 _CarrierId)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYCARRIERBYCARRIERID))
			{
				
				AddParameter( cmd, pInt32(CompanyCarrierBase.Property_CarrierId, _CarrierId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all CompanyCarrier objects by PageRequest
        /// </summary>
        /// <returns>A list of CompanyCarrier objects</returns>
		public CompanyCarrierList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDCOMPANYCARRIER))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				CompanyCarrierList _CompanyCarrierList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _CompanyCarrierList;
			}
		}
		
		/// <summary>
        /// Retrieves all CompanyCarrier objects by query String
        /// </summary>
        /// <returns>A list of CompanyCarrier objects</returns>
		public CompanyCarrierList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYCARRIERBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get CompanyCarrier Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of CompanyCarrier
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYCARRIERMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get CompanyCarrier Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of CompanyCarrier
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _CompanyCarrierRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYCARRIERROWCOUNT))
			{
				SqlDataReader reader;
				_CompanyCarrierRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _CompanyCarrierRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills CompanyCarrier object
        /// </summary>
        /// <param name="companyCarrierObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(CompanyCarrierBase companyCarrierObject, SqlDataReader reader, int start)
		{
			
				companyCarrierObject.Id = reader.GetInt32( start + 0 );			
				companyCarrierObject.CompanyId = reader.GetInt32( start + 1 );			
				companyCarrierObject.CarrierId = reader.GetInt32( start + 2 );			
				companyCarrierObject.IsActive = reader.GetBoolean( start + 3 );			
				if(!reader.IsDBNull(4)) companyCarrierObject.ApiKeyEncrypted = reader.GetString( start + 4 );			
				if(!reader.IsDBNull(5)) companyCarrierObject.ApiSecretEncrypted = reader.GetString( start + 5 );			
				if(!reader.IsDBNull(6)) companyCarrierObject.ApiKeyLast4 = reader.GetString( start + 6 );			
				if(!reader.IsDBNull(7)) companyCarrierObject.SecretUpdatedAt = reader.GetDateTime( start + 7 );			
				if(!reader.IsDBNull(8)) companyCarrierObject.ApiUsernameEncrypted = reader.GetString( start + 8 );			
				if(!reader.IsDBNull(9)) companyCarrierObject.ApiPasswordEncrypted = reader.GetString( start + 9 );			
				if(!reader.IsDBNull(10)) companyCarrierObject.StoreId = reader.GetInt32( start + 10 );			
			FillBaseObject(companyCarrierObject, reader, (start + 11));

			
			companyCarrierObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills CompanyCarrier object
        /// </summary>
        /// <param name="companyCarrierObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(CompanyCarrierBase companyCarrierObject, SqlDataReader reader)
		{
			FillObject(companyCarrierObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves CompanyCarrier object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>CompanyCarrier object</returns>
		private CompanyCarrier GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					CompanyCarrier companyCarrierObject= new CompanyCarrier();
					FillObject(companyCarrierObject, reader);
					return companyCarrierObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of CompanyCarrier objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of CompanyCarrier objects</returns>
		private CompanyCarrierList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//CompanyCarrier list
			CompanyCarrierList list = new CompanyCarrierList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					CompanyCarrier companyCarrierObject = new CompanyCarrier();
					FillObject(companyCarrierObject, reader);

					list.Add(companyCarrierObject);
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