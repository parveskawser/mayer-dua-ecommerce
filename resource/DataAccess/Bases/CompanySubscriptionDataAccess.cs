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
	public partial class CompanySubscriptionDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTCOMPANYSUBSCRIPTION = "InsertCompanySubscription";
		private const string UPDATECOMPANYSUBSCRIPTION = "UpdateCompanySubscription";
		private const string DELETECOMPANYSUBSCRIPTION = "DeleteCompanySubscription";
		private const string GETCOMPANYSUBSCRIPTIONBYID = "GetCompanySubscriptionById";
		private const string GETALLCOMPANYSUBSCRIPTION = "GetAllCompanySubscription";
		private const string GETPAGEDCOMPANYSUBSCRIPTION = "GetPagedCompanySubscription";
		private const string GETCOMPANYSUBSCRIPTIONBYCOMPANYID = "GetCompanySubscriptionByCompanyId";
		private const string GETCOMPANYSUBSCRIPTIONBYSUBSCRIPTIONPLANID = "GetCompanySubscriptionBySubscriptionPlanId";
		private const string GETCOMPANYSUBSCRIPTIONMAXIMUMID = "GetCompanySubscriptionMaximumId";
		private const string GETCOMPANYSUBSCRIPTIONROWCOUNT = "GetCompanySubscriptionRowCount";	
		private const string GETCOMPANYSUBSCRIPTIONBYQUERY = "GetCompanySubscriptionByQuery";
		#endregion
		
		#region Constructors
		public CompanySubscriptionDataAccess(IConfiguration configuration) : base(configuration) { }
		public CompanySubscriptionDataAccess(ClientContext context) : base(context) { }
		public CompanySubscriptionDataAccess(SqlTransaction transaction) : base(transaction) { }
		public CompanySubscriptionDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="companySubscriptionObject"></param>
		private void AddCommonParams(SqlCommand cmd, CompanySubscriptionBase companySubscriptionObject)
		{	
			AddParameter(cmd, pInt32(CompanySubscriptionBase.Property_CompanyId, companySubscriptionObject.CompanyId));
			AddParameter(cmd, pInt32(CompanySubscriptionBase.Property_SubscriptionPlanId, companySubscriptionObject.SubscriptionPlanId));
			AddParameter(cmd, pNVarChar(CompanySubscriptionBase.Property_PlanNameSnapshot, 100, companySubscriptionObject.PlanNameSnapshot));
			AddParameter(cmd, pInt32(CompanySubscriptionBase.Property_MaxProducts, companySubscriptionObject.MaxProducts));
			AddParameter(cmd, pInt32(CompanySubscriptionBase.Property_MaxOrders, companySubscriptionObject.MaxOrders));
			AddParameter(cmd, pNVarChar(CompanySubscriptionBase.Property_OrderCycle, 20, companySubscriptionObject.OrderCycle));
			AddParameter(cmd, pDecimal(CompanySubscriptionBase.Property_PriceCharged, 9, companySubscriptionObject.PriceCharged));
			AddParameter(cmd, pNVarChar(CompanySubscriptionBase.Property_CurrencyCode, 10, companySubscriptionObject.CurrencyCode));
			AddParameter(cmd, pDateTime(CompanySubscriptionBase.Property_StartDate, companySubscriptionObject.StartDate));
			AddParameter(cmd, pDateTime(CompanySubscriptionBase.Property_EndDate, companySubscriptionObject.EndDate));
			AddParameter(cmd, pDateTime(CompanySubscriptionBase.Property_NextBillingDate, companySubscriptionObject.NextBillingDate));
			AddParameter(cmd, pDateTime(CompanySubscriptionBase.Property_CycleAnchorDate, companySubscriptionObject.CycleAnchorDate));
			AddParameter(cmd, pNVarChar(CompanySubscriptionBase.Property_Status, 20, companySubscriptionObject.Status));
			AddParameter(cmd, pNVarChar(CompanySubscriptionBase.Property_CreatedBy, 100, companySubscriptionObject.CreatedBy));
			AddParameter(cmd, pDateTime(CompanySubscriptionBase.Property_CreatedAt, companySubscriptionObject.CreatedAt));
			AddParameter(cmd, pNVarChar(CompanySubscriptionBase.Property_UpdatedBy, 100, companySubscriptionObject.UpdatedBy));
			AddParameter(cmd, pDateTime(CompanySubscriptionBase.Property_UpdatedAt, companySubscriptionObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts CompanySubscription
        /// </summary>
        /// <param name="companySubscriptionObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(CompanySubscriptionBase companySubscriptionObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTCOMPANYSUBSCRIPTION);
	
				AddParameter(cmd, pInt32Out(CompanySubscriptionBase.Property_Id));
				AddCommonParams(cmd, companySubscriptionObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					companySubscriptionObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					companySubscriptionObject.Id = (Int32)GetOutParameter(cmd, CompanySubscriptionBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(companySubscriptionObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates CompanySubscription
        /// </summary>
        /// <param name="companySubscriptionObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(CompanySubscriptionBase companySubscriptionObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATECOMPANYSUBSCRIPTION);
				
				AddParameter(cmd, pInt32(CompanySubscriptionBase.Property_Id, companySubscriptionObject.Id));
				AddCommonParams(cmd, companySubscriptionObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					companySubscriptionObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(companySubscriptionObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes CompanySubscription
        /// </summary>
        /// <param name="Id">Id of the CompanySubscription object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETECOMPANYSUBSCRIPTION);	
				
				AddParameter(cmd, pInt32(CompanySubscriptionBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(CompanySubscription), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves CompanySubscription object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the CompanySubscription object to retrieve</param>
        /// <returns>CompanySubscription object, null if not found</returns>
		public CompanySubscription Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYSUBSCRIPTIONBYID))
			{
				AddParameter( cmd, pInt32(CompanySubscriptionBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all CompanySubscription objects 
        /// </summary>
        /// <returns>A list of CompanySubscription objects</returns>
		public CompanySubscriptionList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLCOMPANYSUBSCRIPTION))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all CompanySubscription objects by CompanyId
        /// </summary>
        /// <returns>A list of CompanySubscription objects</returns>
		public CompanySubscriptionList GetByCompanyId(Int32 _CompanyId)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYSUBSCRIPTIONBYCOMPANYID))
			{
				
				AddParameter( cmd, pInt32(CompanySubscriptionBase.Property_CompanyId, _CompanyId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all CompanySubscription objects by SubscriptionPlanId
        /// </summary>
        /// <returns>A list of CompanySubscription objects</returns>
		public CompanySubscriptionList GetBySubscriptionPlanId(Nullable<Int32> _SubscriptionPlanId)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYSUBSCRIPTIONBYSUBSCRIPTIONPLANID))
			{
				
				AddParameter( cmd, pInt32(CompanySubscriptionBase.Property_SubscriptionPlanId, _SubscriptionPlanId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all CompanySubscription objects by PageRequest
        /// </summary>
        /// <returns>A list of CompanySubscription objects</returns>
		public CompanySubscriptionList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDCOMPANYSUBSCRIPTION))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				CompanySubscriptionList _CompanySubscriptionList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _CompanySubscriptionList;
			}
		}
		
		/// <summary>
        /// Retrieves all CompanySubscription objects by query String
        /// </summary>
        /// <returns>A list of CompanySubscription objects</returns>
		public CompanySubscriptionList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYSUBSCRIPTIONBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get CompanySubscription Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of CompanySubscription
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYSUBSCRIPTIONMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get CompanySubscription Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of CompanySubscription
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _CompanySubscriptionRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCOMPANYSUBSCRIPTIONROWCOUNT))
			{
				SqlDataReader reader;
				_CompanySubscriptionRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _CompanySubscriptionRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills CompanySubscription object
        /// </summary>
        /// <param name="companySubscriptionObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(CompanySubscriptionBase companySubscriptionObject, SqlDataReader reader, int start)
		{
			
				companySubscriptionObject.Id = reader.GetInt32( start + 0 );			
				companySubscriptionObject.CompanyId = reader.GetInt32( start + 1 );			
				if(!reader.IsDBNull(2)) companySubscriptionObject.SubscriptionPlanId = reader.GetInt32( start + 2 );			
				companySubscriptionObject.PlanNameSnapshot = reader.GetString( start + 3 );			
				companySubscriptionObject.MaxProducts = reader.GetInt32( start + 4 );			
				companySubscriptionObject.MaxOrders = reader.GetInt32( start + 5 );			
				companySubscriptionObject.OrderCycle = reader.GetString( start + 6 );			
				companySubscriptionObject.PriceCharged = reader.GetDecimal( start + 7 );			
				companySubscriptionObject.CurrencyCode = reader.GetString( start + 8 );			
				companySubscriptionObject.StartDate = reader.GetDateTime( start + 9 );			
				if(!reader.IsDBNull(10)) companySubscriptionObject.EndDate = reader.GetDateTime( start + 10 );			
				if(!reader.IsDBNull(11)) companySubscriptionObject.NextBillingDate = reader.GetDateTime( start + 11 );			
				if(!reader.IsDBNull(12)) companySubscriptionObject.CycleAnchorDate = reader.GetDateTime( start + 12 );			
				companySubscriptionObject.Status = reader.GetString( start + 13 );			
				if(!reader.IsDBNull(14)) companySubscriptionObject.CreatedBy = reader.GetString( start + 14 );			
				companySubscriptionObject.CreatedAt = reader.GetDateTime( start + 15 );			
				if(!reader.IsDBNull(16)) companySubscriptionObject.UpdatedBy = reader.GetString( start + 16 );			
				if(!reader.IsDBNull(17)) companySubscriptionObject.UpdatedAt = reader.GetDateTime( start + 17 );			
			FillBaseObject(companySubscriptionObject, reader, (start + 18));

			
			companySubscriptionObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills CompanySubscription object
        /// </summary>
        /// <param name="companySubscriptionObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(CompanySubscriptionBase companySubscriptionObject, SqlDataReader reader)
		{
			FillObject(companySubscriptionObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves CompanySubscription object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>CompanySubscription object</returns>
		private CompanySubscription GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					CompanySubscription companySubscriptionObject= new CompanySubscription();
					FillObject(companySubscriptionObject, reader);
					return companySubscriptionObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of CompanySubscription objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of CompanySubscription objects</returns>
		private CompanySubscriptionList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//CompanySubscription list
			CompanySubscriptionList list = new CompanySubscriptionList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					CompanySubscription companySubscriptionObject = new CompanySubscription();
					FillObject(companySubscriptionObject, reader);

					list.Add(companySubscriptionObject);
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