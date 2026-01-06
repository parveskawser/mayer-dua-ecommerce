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
	public partial class SubscriptionPlanDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTSUBSCRIPTIONPLAN = "InsertSubscriptionPlan";
		private const string UPDATESUBSCRIPTIONPLAN = "UpdateSubscriptionPlan";
		private const string DELETESUBSCRIPTIONPLAN = "DeleteSubscriptionPlan";
		private const string GETSUBSCRIPTIONPLANBYID = "GetSubscriptionPlanById";
		private const string GETALLSUBSCRIPTIONPLAN = "GetAllSubscriptionPlan";
		private const string GETPAGEDSUBSCRIPTIONPLAN = "GetPagedSubscriptionPlan";
		private const string GETSUBSCRIPTIONPLANMAXIMUMID = "GetSubscriptionPlanMaximumId";
		private const string GETSUBSCRIPTIONPLANROWCOUNT = "GetSubscriptionPlanRowCount";	
		private const string GETSUBSCRIPTIONPLANBYQUERY = "GetSubscriptionPlanByQuery";
		#endregion
		
		#region Constructors
		public SubscriptionPlanDataAccess(IConfiguration configuration) : base(configuration) { }
		public SubscriptionPlanDataAccess(ClientContext context) : base(context) { }
		public SubscriptionPlanDataAccess(SqlTransaction transaction) : base(transaction) { }
		public SubscriptionPlanDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="subscriptionPlanObject"></param>
		private void AddCommonParams(SqlCommand cmd, SubscriptionPlanBase subscriptionPlanObject)
		{	
			AddParameter(cmd, pNVarChar(SubscriptionPlanBase.Property_PlanCode, 50, subscriptionPlanObject.PlanCode));
			AddParameter(cmd, pNVarChar(SubscriptionPlanBase.Property_PlanName, 100, subscriptionPlanObject.PlanName));
			AddParameter(cmd, pNVarChar(SubscriptionPlanBase.Property_DefaultsJSON, subscriptionPlanObject.DefaultsJSON));
			AddParameter(cmd, pDecimal(SubscriptionPlanBase.Property_BasePrice, 9, subscriptionPlanObject.BasePrice));
			AddParameter(cmd, pDecimal(SubscriptionPlanBase.Property_DiscountPrice, 9, subscriptionPlanObject.DiscountPrice));
			AddParameter(cmd, pNVarChar(SubscriptionPlanBase.Property_CurrencyCode, 10, subscriptionPlanObject.CurrencyCode));
			AddParameter(cmd, pBool(SubscriptionPlanBase.Property_IsActive, subscriptionPlanObject.IsActive));
			AddParameter(cmd, pDateTime(SubscriptionPlanBase.Property_CreatedAt, subscriptionPlanObject.CreatedAt));
			AddParameter(cmd, pDateTime(SubscriptionPlanBase.Property_UpdatedAt, subscriptionPlanObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts SubscriptionPlan
        /// </summary>
        /// <param name="subscriptionPlanObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(SubscriptionPlanBase subscriptionPlanObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTSUBSCRIPTIONPLAN);
	
				AddParameter(cmd, pInt32Out(SubscriptionPlanBase.Property_Id));
				AddCommonParams(cmd, subscriptionPlanObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					subscriptionPlanObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					subscriptionPlanObject.Id = (Int32)GetOutParameter(cmd, SubscriptionPlanBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(subscriptionPlanObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates SubscriptionPlan
        /// </summary>
        /// <param name="subscriptionPlanObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(SubscriptionPlanBase subscriptionPlanObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATESUBSCRIPTIONPLAN);
				
				AddParameter(cmd, pInt32(SubscriptionPlanBase.Property_Id, subscriptionPlanObject.Id));
				AddCommonParams(cmd, subscriptionPlanObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					subscriptionPlanObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(subscriptionPlanObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes SubscriptionPlan
        /// </summary>
        /// <param name="Id">Id of the SubscriptionPlan object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETESUBSCRIPTIONPLAN);	
				
				AddParameter(cmd, pInt32(SubscriptionPlanBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(SubscriptionPlan), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves SubscriptionPlan object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the SubscriptionPlan object to retrieve</param>
        /// <returns>SubscriptionPlan object, null if not found</returns>
		public SubscriptionPlan Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETSUBSCRIPTIONPLANBYID))
			{
				AddParameter( cmd, pInt32(SubscriptionPlanBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all SubscriptionPlan objects 
        /// </summary>
        /// <returns>A list of SubscriptionPlan objects</returns>
		public SubscriptionPlanList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLSUBSCRIPTIONPLAN))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all SubscriptionPlan objects by PageRequest
        /// </summary>
        /// <returns>A list of SubscriptionPlan objects</returns>
		public SubscriptionPlanList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDSUBSCRIPTIONPLAN))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				SubscriptionPlanList _SubscriptionPlanList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _SubscriptionPlanList;
			}
		}
		
		/// <summary>
        /// Retrieves all SubscriptionPlan objects by query String
        /// </summary>
        /// <returns>A list of SubscriptionPlan objects</returns>
		public SubscriptionPlanList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETSUBSCRIPTIONPLANBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get SubscriptionPlan Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of SubscriptionPlan
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETSUBSCRIPTIONPLANMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get SubscriptionPlan Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of SubscriptionPlan
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _SubscriptionPlanRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETSUBSCRIPTIONPLANROWCOUNT))
			{
				SqlDataReader reader;
				_SubscriptionPlanRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _SubscriptionPlanRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills SubscriptionPlan object
        /// </summary>
        /// <param name="subscriptionPlanObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(SubscriptionPlanBase subscriptionPlanObject, SqlDataReader reader, int start)
		{
			
				subscriptionPlanObject.Id = reader.GetInt32( start + 0 );			
				subscriptionPlanObject.PlanCode = reader.GetString( start + 1 );			
				subscriptionPlanObject.PlanName = reader.GetString( start + 2 );			
				if(!reader.IsDBNull(3)) subscriptionPlanObject.DefaultsJSON = reader.GetString( start + 3 );			
				subscriptionPlanObject.BasePrice = reader.GetDecimal( start + 4 );			
				if(!reader.IsDBNull(5)) subscriptionPlanObject.DiscountPrice = reader.GetDecimal( start + 5 );			
				subscriptionPlanObject.CurrencyCode = reader.GetString( start + 6 );			
				subscriptionPlanObject.IsActive = reader.GetBoolean( start + 7 );			
				subscriptionPlanObject.CreatedAt = reader.GetDateTime( start + 8 );			
				if(!reader.IsDBNull(9)) subscriptionPlanObject.UpdatedAt = reader.GetDateTime( start + 9 );			
			FillBaseObject(subscriptionPlanObject, reader, (start + 10));

			
			subscriptionPlanObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills SubscriptionPlan object
        /// </summary>
        /// <param name="subscriptionPlanObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(SubscriptionPlanBase subscriptionPlanObject, SqlDataReader reader)
		{
			FillObject(subscriptionPlanObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves SubscriptionPlan object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>SubscriptionPlan object</returns>
		private SubscriptionPlan GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					SubscriptionPlan subscriptionPlanObject= new SubscriptionPlan();
					FillObject(subscriptionPlanObject, reader);
					return subscriptionPlanObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of SubscriptionPlan objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of SubscriptionPlan objects</returns>
		private SubscriptionPlanList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//SubscriptionPlan list
			SubscriptionPlanList list = new SubscriptionPlanList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					SubscriptionPlan subscriptionPlanObject = new SubscriptionPlan();
					FillObject(subscriptionPlanObject, reader);

					list.Add(subscriptionPlanObject);
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