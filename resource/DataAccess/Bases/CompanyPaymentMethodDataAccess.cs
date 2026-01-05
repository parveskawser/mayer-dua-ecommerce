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
	public partial class CompanyPaymentMethodDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTCOMPANYPAYMENTMETHOD = "InsertCompanyPaymentMethod";
		private const string UPDATECOMPANYPAYMENTMETHOD = "UpdateCompanyPaymentMethod";
		private const string DELETECOMPANYPAYMENTMETHOD = "DeleteCompanyPaymentMethod";
		private const string GETCOMPANYPAYMENTMETHODBYID = "GetCompanyPaymentMethodById";
		private const string GETALLCOMPANYPAYMENTMETHOD = "GetAllCompanyPaymentMethod";
		private const string GETPAGEDCOMPANYPAYMENTMETHOD = "GetPagedCompanyPaymentMethod";
		private const string GETCOMPANYPAYMENTMETHODBYCOMPANYID = "GetCompanyPaymentMethodByCompanyId";
		private const string GETCOMPANYPAYMENTMETHODBYPAYMENTMETHODID = "GetCompanyPaymentMethodByPaymentMethodId";
		private const string GETCOMPANYPAYMENTMETHODMAXIMUMID = "GetCompanyPaymentMethodMaximumId";
		private const string GETCOMPANYPAYMENTMETHODROWCOUNT = "GetCompanyPaymentMethodRowCount";	
		private const string GETCOMPANYPAYMENTMETHODBYQUERY = "GetCompanyPaymentMethodByQuery";
		#endregion
		
		#region Constructors
		public CompanyPaymentMethodDataAccess(IConfiguration configuration) : base(configuration) { }
		public CompanyPaymentMethodDataAccess(ClientContext context) : base(context) { }
		public CompanyPaymentMethodDataAccess(SqlTransaction transaction) : base(transaction) { }
		public CompanyPaymentMethodDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="companyPaymentMethodObject"></param>
		private void AddCommonParams(SqlCommand cmd, CompanyPaymentMethodBase companyPaymentMethodObject)
		{	
			AddParameter(cmd, pInt32(CompanyPaymentMethodBase.Property_CompanyId, companyPaymentMethodObject.CompanyId));
			AddParameter(cmd, pInt32(CompanyPaymentMethodBase.Property_PaymentMethodId, companyPaymentMethodObject.PaymentMethodId));
			AddParameter(cmd, pBool(CompanyPaymentMethodBase.Property_IsActive, companyPaymentMethodObject.IsActive));
			AddParameter(cmd, pNVarChar(CompanyPaymentMethodBase.Property_CreatedBy, 100, companyPaymentMethodObject.CreatedBy));
			AddParameter(cmd, pDateTime(CompanyPaymentMethodBase.Property_CreatedAt, companyPaymentMethodObject.CreatedAt));
			AddParameter(cmd, pNVarChar(CompanyPaymentMethodBase.Property_UpdatedBy, 100, companyPaymentMethodObject.UpdatedBy));
			AddParameter(cmd, pDateTime(CompanyPaymentMethodBase.Property_UpdatedAt, companyPaymentMethodObject.UpdatedAt));
			AddParameter(cmd, pNVarChar(CompanyPaymentMethodBase.Property_CustomInstruction, companyPaymentMethodObject.CustomInstruction));
			AddParameter(cmd, pBool(CompanyPaymentMethodBase.Property_IsManualEnabled, companyPaymentMethodObject.IsManualEnabled));
			AddParameter(cmd, pBool(CompanyPaymentMethodBase.Property_IsGatewayEnabled, companyPaymentMethodObject.IsGatewayEnabled));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts CompanyPaymentMethod
        /// </summary>
        /// <param name="companyPaymentMethodObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(CompanyPaymentMethodBase companyPaymentMethodObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTCOMPANYPAYMENTMETHOD);
	
				AddParameter(cmd, pInt32Out(CompanyPaymentMethodBase.Property_Id));
				AddCommonParams(cmd, companyPaymentMethodObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					companyPaymentMethodObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					companyPaymentMethodObject.Id = (Int32)GetOutParameter(cmd, CompanyPaymentMethodBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(companyPaymentMethodObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates CompanyPaymentMethod
        /// </summary>
        /// <param name="companyPaymentMethodObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(CompanyPaymentMethodBase companyPaymentMethodObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATECOMPANYPAYMENTMETHOD);
				
				AddParameter(cmd, pInt32(CompanyPaymentMethodBase.Property_Id, companyPaymentMethodObject.Id));
				AddCommonParams(cmd, companyPaymentMethodObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					companyPaymentMethodObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(companyPaymentMethodObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes CompanyPaymentMethod
        /// </summary>
        /// <param name="Id">Id of the CompanyPaymentMethod object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETECOMPANYPAYMENTMETHOD);	
				
				AddParameter(cmd, pInt32(CompanyPaymentMethodBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(CompanyPaymentMethod), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves CompanyPaymentMethod object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the CompanyPaymentMethod object to retrieve</param>
        /// <returns>CompanyPaymentMethod object, null if not found</returns>
		public CompanyPaymentMethod Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYPAYMENTMETHODBYID))
			{
				AddParameter( cmd, pInt32(CompanyPaymentMethodBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all CompanyPaymentMethod objects 
        /// </summary>
        /// <returns>A list of CompanyPaymentMethod objects</returns>
		public CompanyPaymentMethodList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLCOMPANYPAYMENTMETHOD))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all CompanyPaymentMethod objects by CompanyId
        /// </summary>
        /// <returns>A list of CompanyPaymentMethod objects</returns>
		public CompanyPaymentMethodList GetByCompanyId(Int32 _CompanyId)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYPAYMENTMETHODBYCOMPANYID))
			{
				
				AddParameter( cmd, pInt32(CompanyPaymentMethodBase.Property_CompanyId, _CompanyId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all CompanyPaymentMethod objects by PaymentMethodId
        /// </summary>
        /// <returns>A list of CompanyPaymentMethod objects</returns>
		public CompanyPaymentMethodList GetByPaymentMethodId(Int32 _PaymentMethodId)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYPAYMENTMETHODBYPAYMENTMETHODID))
			{
				
				AddParameter( cmd, pInt32(CompanyPaymentMethodBase.Property_PaymentMethodId, _PaymentMethodId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all CompanyPaymentMethod objects by PageRequest
        /// </summary>
        /// <returns>A list of CompanyPaymentMethod objects</returns>
		public CompanyPaymentMethodList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDCOMPANYPAYMENTMETHOD))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				CompanyPaymentMethodList _CompanyPaymentMethodList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _CompanyPaymentMethodList;
			}
		}
		
		/// <summary>
        /// Retrieves all CompanyPaymentMethod objects by query String
        /// </summary>
        /// <returns>A list of CompanyPaymentMethod objects</returns>
		public CompanyPaymentMethodList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYPAYMENTMETHODBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get CompanyPaymentMethod Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of CompanyPaymentMethod
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYPAYMENTMETHODMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get CompanyPaymentMethod Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of CompanyPaymentMethod
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _CompanyPaymentMethodRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYPAYMENTMETHODROWCOUNT))
			{
				SqlDataReader reader;
				_CompanyPaymentMethodRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _CompanyPaymentMethodRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills CompanyPaymentMethod object
        /// </summary>
        /// <param name="companyPaymentMethodObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(CompanyPaymentMethodBase companyPaymentMethodObject, SqlDataReader reader, int start)
		{
			
				companyPaymentMethodObject.Id = reader.GetInt32( start + 0 );			
				companyPaymentMethodObject.CompanyId = reader.GetInt32( start + 1 );			
				companyPaymentMethodObject.PaymentMethodId = reader.GetInt32( start + 2 );			
				companyPaymentMethodObject.IsActive = reader.GetBoolean( start + 3 );			
				if(!reader.IsDBNull(4)) companyPaymentMethodObject.CreatedBy = reader.GetString( start + 4 );			
				if(!reader.IsDBNull(5)) companyPaymentMethodObject.CreatedAt = reader.GetDateTime( start + 5 );			
				if(!reader.IsDBNull(6)) companyPaymentMethodObject.UpdatedBy = reader.GetString( start + 6 );			
				if(!reader.IsDBNull(7)) companyPaymentMethodObject.UpdatedAt = reader.GetDateTime( start + 7 );			
				if(!reader.IsDBNull(8)) companyPaymentMethodObject.CustomInstruction = reader.GetString( start + 8 );			
				companyPaymentMethodObject.IsManualEnabled = reader.GetBoolean( start + 9 );			
				companyPaymentMethodObject.IsGatewayEnabled = reader.GetBoolean( start + 10 );			
			FillBaseObject(companyPaymentMethodObject, reader, (start + 11));

			
			companyPaymentMethodObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills CompanyPaymentMethod object
        /// </summary>
        /// <param name="companyPaymentMethodObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(CompanyPaymentMethodBase companyPaymentMethodObject, SqlDataReader reader)
		{
			FillObject(companyPaymentMethodObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves CompanyPaymentMethod object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>CompanyPaymentMethod object</returns>
		private CompanyPaymentMethod GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					CompanyPaymentMethod companyPaymentMethodObject= new CompanyPaymentMethod();
					FillObject(companyPaymentMethodObject, reader);
					return companyPaymentMethodObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of CompanyPaymentMethod objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of CompanyPaymentMethod objects</returns>
		private CompanyPaymentMethodList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//CompanyPaymentMethod list
			CompanyPaymentMethodList list = new CompanyPaymentMethodList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					CompanyPaymentMethod companyPaymentMethodObject = new CompanyPaymentMethod();
					FillObject(companyPaymentMethodObject, reader);

					list.Add(companyPaymentMethodObject);
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