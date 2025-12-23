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
using MDUA.DataAccess.Interface;

namespace MDUA.DataAccess
{
	public partial class CompanyVendorDataAccess : BaseDataAccess, ICompanyVendorDataAccess
	{
		#region Constants
		private const string INSERTCOMPANYVENDOR = "InsertCompanyVendor";
		private const string UPDATECOMPANYVENDOR = "UpdateCompanyVendor";
		private const string DELETECOMPANYVENDOR = "DeleteCompanyVendor";
		private const string GETCOMPANYVENDORBYID = "GetCompanyVendorById";
		private const string GETALLCOMPANYVENDOR = "GetAllCompanyVendor";
		private const string GETPAGEDCOMPANYVENDOR = "GetPagedCompanyVendor";
		private const string GETCOMPANYVENDORBYCOMPANYID = "GetCompanyVendorByCompanyId";
		private const string GETCOMPANYVENDORBYVENDORID = "GetCompanyVendorByVendorId";
		private const string GETCOMPANYVENDORMAXIMUMID = "GetCompanyVendorMaximumId";
		private const string GETCOMPANYVENDORROWCOUNT = "GetCompanyVendorRowCount";	
		private const string GETCOMPANYVENDORBYQUERY = "GetCompanyVendorByQuery";
		#endregion
		
		#region Constructors
		public CompanyVendorDataAccess(IConfiguration configuration) : base(configuration) { }
		public CompanyVendorDataAccess(ClientContext context) : base(context) { }
		public CompanyVendorDataAccess(SqlTransaction transaction) : base(transaction) { }
		public CompanyVendorDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="companyVendorObject"></param>
		private void AddCommonParams(SqlCommand cmd, CompanyVendorBase companyVendorObject)
		{	
			AddParameter(cmd, pInt32(CompanyVendorBase.Property_CompanyId, companyVendorObject.CompanyId));
			AddParameter(cmd, pInt32(CompanyVendorBase.Property_VendorId, companyVendorObject.VendorId));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts CompanyVendor
        /// </summary>
        /// <param name="companyVendorObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(CompanyVendorBase companyVendorObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTCOMPANYVENDOR);
	
				AddParameter(cmd, pInt32Out(CompanyVendorBase.Property_Id));
				AddCommonParams(cmd, companyVendorObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					companyVendorObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					companyVendorObject.Id = (Int32)GetOutParameter(cmd, CompanyVendorBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(companyVendorObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates CompanyVendor
        /// </summary>
        /// <param name="companyVendorObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(CompanyVendorBase companyVendorObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATECOMPANYVENDOR);
				
				AddParameter(cmd, pInt32(CompanyVendorBase.Property_Id, companyVendorObject.Id));
				AddCommonParams(cmd, companyVendorObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					companyVendorObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(companyVendorObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes CompanyVendor
        /// </summary>
        /// <param name="Id">Id of the CompanyVendor object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETECOMPANYVENDOR);	
				
				AddParameter(cmd, pInt32(CompanyVendorBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(CompanyVendor), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves CompanyVendor object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the CompanyVendor object to retrieve</param>
        /// <returns>CompanyVendor object, null if not found</returns>
		public CompanyVendor Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYVENDORBYID))
			{
				AddParameter( cmd, pInt32(CompanyVendorBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all CompanyVendor objects 
        /// </summary>
        /// <returns>A list of CompanyVendor objects</returns>
		public CompanyVendorList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLCOMPANYVENDOR))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all CompanyVendor objects by CompanyId
        /// </summary>
        /// <returns>A list of CompanyVendor objects</returns>
		public CompanyVendorList GetByCompanyId(Int32 _CompanyId)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYVENDORBYCOMPANYID))
			{
				
				AddParameter( cmd, pInt32(CompanyVendorBase.Property_CompanyId, _CompanyId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all CompanyVendor objects by VendorId
        /// </summary>
        /// <returns>A list of CompanyVendor objects</returns>
		public CompanyVendorList GetByVendorId(Int32 _VendorId)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYVENDORBYVENDORID))
			{
				
				AddParameter( cmd, pInt32(CompanyVendorBase.Property_VendorId, _VendorId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all CompanyVendor objects by PageRequest
        /// </summary>
        /// <returns>A list of CompanyVendor objects</returns>
		public CompanyVendorList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDCOMPANYVENDOR))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				CompanyVendorList _CompanyVendorList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _CompanyVendorList;
			}
		}
		
		/// <summary>
        /// Retrieves all CompanyVendor objects by query String
        /// </summary>
        /// <returns>A list of CompanyVendor objects</returns>
		public CompanyVendorList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYVENDORBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get CompanyVendor Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of CompanyVendor
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYVENDORMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get CompanyVendor Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of CompanyVendor
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _CompanyVendorRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYVENDORROWCOUNT))
			{
				SqlDataReader reader;
				_CompanyVendorRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _CompanyVendorRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills CompanyVendor object
        /// </summary>
        /// <param name="companyVendorObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(CompanyVendorBase companyVendorObject, SqlDataReader reader, int start)
		{
			
				companyVendorObject.Id = reader.GetInt32( start + 0 );			
				companyVendorObject.CompanyId = reader.GetInt32( start + 1 );			
				companyVendorObject.VendorId = reader.GetInt32( start + 2 );			
			FillBaseObject(companyVendorObject, reader, (start + 3));

			
			companyVendorObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills CompanyVendor object
        /// </summary>
        /// <param name="companyVendorObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(CompanyVendorBase companyVendorObject, SqlDataReader reader)
		{
			FillObject(companyVendorObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves CompanyVendor object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>CompanyVendor object</returns>
		private CompanyVendor GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					CompanyVendor companyVendorObject= new CompanyVendor();
					FillObject(companyVendorObject, reader);
					return companyVendorObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of CompanyVendor objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of CompanyVendor objects</returns>
		private CompanyVendorList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//CompanyVendor list
			CompanyVendorList list = new CompanyVendorList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					CompanyVendor companyVendorObject = new CompanyVendor();
					FillObject(companyVendorObject, reader);

					list.Add(companyVendorObject);
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
