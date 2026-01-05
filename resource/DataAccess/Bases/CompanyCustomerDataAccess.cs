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
	public partial class CompanyCustomerDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTCOMPANYCUSTOMER = "InsertCompanyCustomer";
		private const string UPDATECOMPANYCUSTOMER = "UpdateCompanyCustomer";
		private const string DELETECOMPANYCUSTOMER = "DeleteCompanyCustomer";
		private const string GETCOMPANYCUSTOMERBYID = "GetCompanyCustomerById";
		private const string GETALLCOMPANYCUSTOMER = "GetAllCompanyCustomer";
		private const string GETPAGEDCOMPANYCUSTOMER = "GetPagedCompanyCustomer";
		private const string GETCOMPANYCUSTOMERBYCOMPANYID = "GetCompanyCustomerByCompanyId";
		private const string GETCOMPANYCUSTOMERBYCUSTOMERID = "GetCompanyCustomerByCustomerId";
		private const string GETCOMPANYCUSTOMERMAXIMUMID = "GetCompanyCustomerMaximumId";
		private const string GETCOMPANYCUSTOMERROWCOUNT = "GetCompanyCustomerRowCount";	
		private const string GETCOMPANYCUSTOMERBYQUERY = "GetCompanyCustomerByQuery";
		#endregion
		
		#region Constructors
		public CompanyCustomerDataAccess(IConfiguration configuration) : base(configuration) { }
		public CompanyCustomerDataAccess(ClientContext context) : base(context) { }
		public CompanyCustomerDataAccess(SqlTransaction transaction) : base(transaction) { }
		public CompanyCustomerDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="companyCustomerObject"></param>
		private void AddCommonParams(SqlCommand cmd, CompanyCustomerBase companyCustomerObject)
		{	
			AddParameter(cmd, pInt32(CompanyCustomerBase.Property_CompanyId, companyCustomerObject.CompanyId));
			AddParameter(cmd, pInt32(CompanyCustomerBase.Property_CustomerId, companyCustomerObject.CustomerId));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts CompanyCustomer
        /// </summary>
        /// <param name="companyCustomerObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(CompanyCustomerBase companyCustomerObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTCOMPANYCUSTOMER);
	
				AddParameter(cmd, pInt32Out(CompanyCustomerBase.Property_Id));
				AddCommonParams(cmd, companyCustomerObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					companyCustomerObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					companyCustomerObject.Id = (Int32)GetOutParameter(cmd, CompanyCustomerBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(companyCustomerObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates CompanyCustomer
        /// </summary>
        /// <param name="companyCustomerObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(CompanyCustomerBase companyCustomerObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATECOMPANYCUSTOMER);
				
				AddParameter(cmd, pInt32(CompanyCustomerBase.Property_Id, companyCustomerObject.Id));
				AddCommonParams(cmd, companyCustomerObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					companyCustomerObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(companyCustomerObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes CompanyCustomer
        /// </summary>
        /// <param name="Id">Id of the CompanyCustomer object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETECOMPANYCUSTOMER);	
				
				AddParameter(cmd, pInt32(CompanyCustomerBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(CompanyCustomer), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves CompanyCustomer object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the CompanyCustomer object to retrieve</param>
        /// <returns>CompanyCustomer object, null if not found</returns>
		public CompanyCustomer Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYCUSTOMERBYID))
			{
				AddParameter( cmd, pInt32(CompanyCustomerBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all CompanyCustomer objects 
        /// </summary>
        /// <returns>A list of CompanyCustomer objects</returns>
		public CompanyCustomerList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLCOMPANYCUSTOMER))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all CompanyCustomer objects by CompanyId
        /// </summary>
        /// <returns>A list of CompanyCustomer objects</returns>
		public CompanyCustomerList GetByCompanyId(Int32 _CompanyId)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYCUSTOMERBYCOMPANYID))
			{
				
				AddParameter( cmd, pInt32(CompanyCustomerBase.Property_CompanyId, _CompanyId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all CompanyCustomer objects by CustomerId
        /// </summary>
        /// <returns>A list of CompanyCustomer objects</returns>
		public CompanyCustomerList GetByCustomerId(Int32 _CustomerId)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYCUSTOMERBYCUSTOMERID))
			{
				
				AddParameter( cmd, pInt32(CompanyCustomerBase.Property_CustomerId, _CustomerId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all CompanyCustomer objects by PageRequest
        /// </summary>
        /// <returns>A list of CompanyCustomer objects</returns>
		public CompanyCustomerList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDCOMPANYCUSTOMER))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				CompanyCustomerList _CompanyCustomerList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _CompanyCustomerList;
			}
		}
		
		/// <summary>
        /// Retrieves all CompanyCustomer objects by query String
        /// </summary>
        /// <returns>A list of CompanyCustomer objects</returns>
		public CompanyCustomerList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYCUSTOMERBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get CompanyCustomer Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of CompanyCustomer
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYCUSTOMERMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get CompanyCustomer Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of CompanyCustomer
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _CompanyCustomerRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYCUSTOMERROWCOUNT))
			{
				SqlDataReader reader;
				_CompanyCustomerRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _CompanyCustomerRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills CompanyCustomer object
        /// </summary>
        /// <param name="companyCustomerObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(CompanyCustomerBase companyCustomerObject, SqlDataReader reader, int start)
		{
			
				companyCustomerObject.Id = reader.GetInt32( start + 0 );			
				companyCustomerObject.CompanyId = reader.GetInt32( start + 1 );			
				companyCustomerObject.CustomerId = reader.GetInt32( start + 2 );			
			FillBaseObject(companyCustomerObject, reader, (start + 3));

			
			companyCustomerObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills CompanyCustomer object
        /// </summary>
        /// <param name="companyCustomerObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(CompanyCustomerBase companyCustomerObject, SqlDataReader reader)
		{
			FillObject(companyCustomerObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves CompanyCustomer object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>CompanyCustomer object</returns>
		private CompanyCustomer GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					CompanyCustomer companyCustomerObject= new CompanyCustomer();
					FillObject(companyCustomerObject, reader);
					return companyCustomerObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of CompanyCustomer objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of CompanyCustomer objects</returns>
		private CompanyCustomerList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//CompanyCustomer list
			CompanyCustomerList list = new CompanyCustomerList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					CompanyCustomer companyCustomerObject = new CompanyCustomer();
					FillObject(companyCustomerObject, reader);

					list.Add(companyCustomerObject);
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