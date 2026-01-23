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
	public partial class CompanyDomainDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTCOMPANYDOMAIN = "InsertCompanyDomain";
		private const string UPDATECOMPANYDOMAIN = "UpdateCompanyDomain";
		private const string DELETECOMPANYDOMAIN = "DeleteCompanyDomain";
		private const string GETCOMPANYDOMAINBYID = "GetCompanyDomainById";
		private const string GETALLCOMPANYDOMAIN = "GetAllCompanyDomain";
		private const string GETPAGEDCOMPANYDOMAIN = "GetPagedCompanyDomain";
		private const string GETCOMPANYDOMAINBYCOMPANYID = "GetCompanyDomainByCompanyId";
		private const string GETCOMPANYDOMAINMAXIMUMID = "GetCompanyDomainMaximumId";
		private const string GETCOMPANYDOMAINROWCOUNT = "GetCompanyDomainRowCount";	
		private const string GETCOMPANYDOMAINBYQUERY = "GetCompanyDomainByQuery";
		#endregion
		
		#region Constructors
		public CompanyDomainDataAccess(IConfiguration configuration) : base(configuration) { }
		public CompanyDomainDataAccess(ClientContext context) : base(context) { }
		public CompanyDomainDataAccess(SqlTransaction transaction) : base(transaction) { }
		public CompanyDomainDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="companyDomainObject"></param>
		private void AddCommonParams(SqlCommand cmd, CompanyDomainBase companyDomainObject)
		{	
			AddParameter(cmd, pInt32(CompanyDomainBase.Property_CompanyId, companyDomainObject.CompanyId));
			AddParameter(cmd, pNVarChar(CompanyDomainBase.Property_Domain, 255, companyDomainObject.Domain));
			AddParameter(cmd, pBool(CompanyDomainBase.Property_IsPrimary, companyDomainObject.IsPrimary));
			AddParameter(cmd, pBool(CompanyDomainBase.Property_IsActive, companyDomainObject.IsActive));
			AddParameter(cmd, pDateTime(CompanyDomainBase.Property_VerifiedAt, companyDomainObject.VerifiedAt));
			AddParameter(cmd, pNVarChar(CompanyDomainBase.Property_CreatedBy, 100, companyDomainObject.CreatedBy));
			AddParameter(cmd, pDateTime(CompanyDomainBase.Property_CreatedAt, companyDomainObject.CreatedAt));
			AddParameter(cmd, pNVarChar(CompanyDomainBase.Property_UpdatedBy, 100, companyDomainObject.UpdatedBy));
			AddParameter(cmd, pDateTime(CompanyDomainBase.Property_UpdatedAt, companyDomainObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts CompanyDomain
        /// </summary>
        /// <param name="companyDomainObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(CompanyDomainBase companyDomainObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTCOMPANYDOMAIN);
	
				AddParameter(cmd, pInt32Out(CompanyDomainBase.Property_Id));
				AddCommonParams(cmd, companyDomainObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					companyDomainObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					companyDomainObject.Id = (Int32)GetOutParameter(cmd, CompanyDomainBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(companyDomainObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates CompanyDomain
        /// </summary>
        /// <param name="companyDomainObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(CompanyDomainBase companyDomainObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATECOMPANYDOMAIN);
				
				AddParameter(cmd, pInt32(CompanyDomainBase.Property_Id, companyDomainObject.Id));
				AddCommonParams(cmd, companyDomainObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					companyDomainObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(companyDomainObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes CompanyDomain
        /// </summary>
        /// <param name="Id">Id of the CompanyDomain object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETECOMPANYDOMAIN);	
				
				AddParameter(cmd, pInt32(CompanyDomainBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(CompanyDomain), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves CompanyDomain object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the CompanyDomain object to retrieve</param>
        /// <returns>CompanyDomain object, null if not found</returns>
		public CompanyDomain Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYDOMAINBYID))
			{
				AddParameter( cmd, pInt32(CompanyDomainBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all CompanyDomain objects 
        /// </summary>
        /// <returns>A list of CompanyDomain objects</returns>
		public CompanyDomainList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLCOMPANYDOMAIN))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all CompanyDomain objects by CompanyId
        /// </summary>
        /// <returns>A list of CompanyDomain objects</returns>
		public CompanyDomainList GetByCompanyId(Int32 _CompanyId)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYDOMAINBYCOMPANYID))
			{
				
				AddParameter( cmd, pInt32(CompanyDomainBase.Property_CompanyId, _CompanyId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all CompanyDomain objects by PageRequest
        /// </summary>
        /// <returns>A list of CompanyDomain objects</returns>
		public CompanyDomainList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDCOMPANYDOMAIN))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				CompanyDomainList _CompanyDomainList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _CompanyDomainList;
			}
		}
		
		/// <summary>
        /// Retrieves all CompanyDomain objects by query String
        /// </summary>
        /// <returns>A list of CompanyDomain objects</returns>
		public CompanyDomainList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYDOMAINBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get CompanyDomain Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of CompanyDomain
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYDOMAINMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get CompanyDomain Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of CompanyDomain
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _CompanyDomainRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYDOMAINROWCOUNT))
			{
				SqlDataReader reader;
				_CompanyDomainRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _CompanyDomainRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills CompanyDomain object
        /// </summary>
        /// <param name="companyDomainObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(CompanyDomainBase companyDomainObject, SqlDataReader reader, int start)
		{
			
				companyDomainObject.Id = reader.GetInt32( start + 0 );			
				companyDomainObject.CompanyId = reader.GetInt32( start + 1 );			
				companyDomainObject.Domain = reader.GetString( start + 2 );			
				companyDomainObject.IsPrimary = reader.GetBoolean( start + 3 );			
				companyDomainObject.IsActive = reader.GetBoolean( start + 4 );			
				if(!reader.IsDBNull(5)) companyDomainObject.VerifiedAt = reader.GetDateTime( start + 5 );			
				companyDomainObject.CreatedBy = reader.GetString( start + 6 );			
				companyDomainObject.CreatedAt = reader.GetDateTime( start + 7 );			
				if(!reader.IsDBNull(8)) companyDomainObject.UpdatedBy = reader.GetString( start + 8 );			
				if(!reader.IsDBNull(9)) companyDomainObject.UpdatedAt = reader.GetDateTime( start + 9 );			
			FillBaseObject(companyDomainObject, reader, (start + 10));

			
			companyDomainObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills CompanyDomain object
        /// </summary>
        /// <param name="companyDomainObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(CompanyDomainBase companyDomainObject, SqlDataReader reader)
		{
			FillObject(companyDomainObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves CompanyDomain object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>CompanyDomain object</returns>
		private CompanyDomain GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					CompanyDomain companyDomainObject= new CompanyDomain();
					FillObject(companyDomainObject, reader);
					return companyDomainObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of CompanyDomain objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of CompanyDomain objects</returns>
		private CompanyDomainList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//CompanyDomain list
			CompanyDomainList list = new CompanyDomainList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					CompanyDomain companyDomainObject = new CompanyDomain();
					FillObject(companyDomainObject, reader);

					list.Add(companyDomainObject);
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