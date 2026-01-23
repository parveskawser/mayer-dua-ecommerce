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
	public partial class SubscriptionUsageDataAccess : BaseDataAccess, ISubscriptionUsageDataAccess
    {
		#region Constants
		private const string INSERTSUBSCRIPTIONUSAGE = "InsertSubscriptionUsage";
		private const string UPDATESUBSCRIPTIONUSAGE = "UpdateSubscriptionUsage";
		private const string DELETESUBSCRIPTIONUSAGE = "DeleteSubscriptionUsage";
		private const string GETSUBSCRIPTIONUSAGEBYID = "GetSubscriptionUsageById";
		private const string GETALLSUBSCRIPTIONUSAGE = "GetAllSubscriptionUsage";
		private const string GETPAGEDSUBSCRIPTIONUSAGE = "GetPagedSubscriptionUsage";
		private const string GETSUBSCRIPTIONUSAGEBYSUBSCRIPTIONID = "GetSubscriptionUsageBySubscriptionId";
		private const string GETSUBSCRIPTIONUSAGEMAXIMUMID = "GetSubscriptionUsageMaximumId";
		private const string GETSUBSCRIPTIONUSAGEROWCOUNT = "GetSubscriptionUsageRowCount";	
		private const string GETSUBSCRIPTIONUSAGEBYQUERY = "GetSubscriptionUsageByQuery";
		#endregion
		
		#region Constructors
		public SubscriptionUsageDataAccess(IConfiguration configuration) : base(configuration) { }
		public SubscriptionUsageDataAccess(ClientContext context) : base(context) { }
		public SubscriptionUsageDataAccess(SqlTransaction transaction) : base(transaction) { }
		public SubscriptionUsageDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="subscriptionUsageObject"></param>
		private void AddCommonParams(SqlCommand cmd, SubscriptionUsageBase subscriptionUsageObject)
		{	
			AddParameter(cmd, pInt32(SubscriptionUsageBase.Property_SubscriptionId, subscriptionUsageObject.SubscriptionId));
			AddParameter(cmd, pDateTime(SubscriptionUsageBase.Property_CycleStart, subscriptionUsageObject.CycleStart));
			AddParameter(cmd, pDateTime(SubscriptionUsageBase.Property_CycleEnd, subscriptionUsageObject.CycleEnd));
			AddParameter(cmd, pInt32(SubscriptionUsageBase.Property_OrdersProcessed, subscriptionUsageObject.OrdersProcessed));
			AddParameter(cmd, pDateTime(SubscriptionUsageBase.Property_CreatedAt, subscriptionUsageObject.CreatedAt));
			AddParameter(cmd, pDateTime(SubscriptionUsageBase.Property_UpdatedAt, subscriptionUsageObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts SubscriptionUsage
        /// </summary>
        /// <param name="subscriptionUsageObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(SubscriptionUsageBase subscriptionUsageObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTSUBSCRIPTIONUSAGE);
	
				AddParameter(cmd, pInt32Out(SubscriptionUsageBase.Property_Id));
				AddCommonParams(cmd, subscriptionUsageObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					subscriptionUsageObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					subscriptionUsageObject.Id = (Int32)GetOutParameter(cmd, SubscriptionUsageBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(subscriptionUsageObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates SubscriptionUsage
        /// </summary>
        /// <param name="subscriptionUsageObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(SubscriptionUsageBase subscriptionUsageObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATESUBSCRIPTIONUSAGE);
				
				AddParameter(cmd, pInt32(SubscriptionUsageBase.Property_Id, subscriptionUsageObject.Id));
				AddCommonParams(cmd, subscriptionUsageObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					subscriptionUsageObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(subscriptionUsageObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes SubscriptionUsage
        /// </summary>
        /// <param name="Id">Id of the SubscriptionUsage object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETESUBSCRIPTIONUSAGE);	
				
				AddParameter(cmd, pInt32(SubscriptionUsageBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(SubscriptionUsage), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves SubscriptionUsage object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the SubscriptionUsage object to retrieve</param>
        /// <returns>SubscriptionUsage object, null if not found</returns>
		public SubscriptionUsage Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETSUBSCRIPTIONUSAGEBYID))
			{
				AddParameter( cmd, pInt32(SubscriptionUsageBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all SubscriptionUsage objects 
        /// </summary>
        /// <returns>A list of SubscriptionUsage objects</returns>
		public SubscriptionUsageList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLSUBSCRIPTIONUSAGE))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all SubscriptionUsage objects by SubscriptionId
        /// </summary>
        /// <returns>A list of SubscriptionUsage objects</returns>
		public SubscriptionUsageList GetBySubscriptionId(Int32 _SubscriptionId)
		{
			using( SqlCommand cmd = GetSPCommand(GETSUBSCRIPTIONUSAGEBYSUBSCRIPTIONID))
			{
				
				AddParameter( cmd, pInt32(SubscriptionUsageBase.Property_SubscriptionId, _SubscriptionId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all SubscriptionUsage objects by PageRequest
        /// </summary>
        /// <returns>A list of SubscriptionUsage objects</returns>
		public SubscriptionUsageList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDSUBSCRIPTIONUSAGE))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				SubscriptionUsageList _SubscriptionUsageList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _SubscriptionUsageList;
			}
		}
		
		/// <summary>
        /// Retrieves all SubscriptionUsage objects by query String
        /// </summary>
        /// <returns>A list of SubscriptionUsage objects</returns>
		public SubscriptionUsageList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETSUBSCRIPTIONUSAGEBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get SubscriptionUsage Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of SubscriptionUsage
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETSUBSCRIPTIONUSAGEMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get SubscriptionUsage Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of SubscriptionUsage
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _SubscriptionUsageRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETSUBSCRIPTIONUSAGEROWCOUNT))
			{
				SqlDataReader reader;
				_SubscriptionUsageRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _SubscriptionUsageRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills SubscriptionUsage object
        /// </summary>
        /// <param name="subscriptionUsageObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(SubscriptionUsageBase subscriptionUsageObject, SqlDataReader reader, int start)
		{
			
				subscriptionUsageObject.Id = reader.GetInt32( start + 0 );			
				subscriptionUsageObject.SubscriptionId = reader.GetInt32( start + 1 );			
				subscriptionUsageObject.CycleStart = reader.GetDateTime( start + 2 );			
				subscriptionUsageObject.CycleEnd = reader.GetDateTime( start + 3 );			
				subscriptionUsageObject.OrdersProcessed = reader.GetInt32( start + 4 );			
				subscriptionUsageObject.CreatedAt = reader.GetDateTime( start + 5 );			
				if(!reader.IsDBNull(6)) subscriptionUsageObject.UpdatedAt = reader.GetDateTime( start + 6 );			
			FillBaseObject(subscriptionUsageObject, reader, (start + 7));

			
			subscriptionUsageObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills SubscriptionUsage object
        /// </summary>
        /// <param name="subscriptionUsageObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(SubscriptionUsageBase subscriptionUsageObject, SqlDataReader reader)
		{
			FillObject(subscriptionUsageObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves SubscriptionUsage object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>SubscriptionUsage object</returns>
		private SubscriptionUsage GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					SubscriptionUsage subscriptionUsageObject= new SubscriptionUsage();
					FillObject(subscriptionUsageObject, reader);
					return subscriptionUsageObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of SubscriptionUsage objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of SubscriptionUsage objects</returns>
		private SubscriptionUsageList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//SubscriptionUsage list
			SubscriptionUsageList list = new SubscriptionUsageList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					SubscriptionUsage subscriptionUsageObject = new SubscriptionUsage();
					FillObject(subscriptionUsageObject, reader);

					list.Add(subscriptionUsageObject);
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