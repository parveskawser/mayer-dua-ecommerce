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
	public partial class DeliveryStatusLogDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTDELIVERYSTATUSLOG = "InsertDeliveryStatusLog";
		private const string UPDATEDELIVERYSTATUSLOG = "UpdateDeliveryStatusLog";
		private const string DELETEDELIVERYSTATUSLOG = "DeleteDeliveryStatusLog";
		private const string GETDELIVERYSTATUSLOGBYID = "GetDeliveryStatusLogById";
		private const string GETALLDELIVERYSTATUSLOG = "GetAllDeliveryStatusLog";
		private const string GETPAGEDDELIVERYSTATUSLOG = "GetPagedDeliveryStatusLog";
		private const string GETDELIVERYSTATUSLOGBYSALESORDERID = "GetDeliveryStatusLogBySalesOrderId";
		private const string GETDELIVERYSTATUSLOGMAXIMUMID = "GetDeliveryStatusLogMaximumId";
		private const string GETDELIVERYSTATUSLOGROWCOUNT = "GetDeliveryStatusLogRowCount";	
		private const string GETDELIVERYSTATUSLOGBYQUERY = "GetDeliveryStatusLogByQuery";
		#endregion
		
		#region Constructors
		public DeliveryStatusLogDataAccess(IConfiguration configuration) : base(configuration) { }
		public DeliveryStatusLogDataAccess(ClientContext context) : base(context) { }
		public DeliveryStatusLogDataAccess(SqlTransaction transaction) : base(transaction) { }
		public DeliveryStatusLogDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="deliveryStatusLogObject"></param>
		private void AddCommonParams(SqlCommand cmd, DeliveryStatusLogBase deliveryStatusLogObject)
		{	
			AddParameter(cmd, pNVarChar(DeliveryStatusLogBase.Property_EntityType, 50, deliveryStatusLogObject.EntityType));
			AddParameter(cmd, pInt32(DeliveryStatusLogBase.Property_EntityId, deliveryStatusLogObject.EntityId));
			AddParameter(cmd, pInt32(DeliveryStatusLogBase.Property_SalesOrderId, deliveryStatusLogObject.SalesOrderId));
			AddParameter(cmd, pNVarChar(DeliveryStatusLogBase.Property_OldStatus, 50, deliveryStatusLogObject.OldStatus));
			AddParameter(cmd, pNVarChar(DeliveryStatusLogBase.Property_NewStatus, 50, deliveryStatusLogObject.NewStatus));
			AddParameter(cmd, pNVarChar(DeliveryStatusLogBase.Property_ChangeReason, 500, deliveryStatusLogObject.ChangeReason));
			AddParameter(cmd, pBool(DeliveryStatusLogBase.Property_OldConfirmed, deliveryStatusLogObject.OldConfirmed));
			AddParameter(cmd, pBool(DeliveryStatusLogBase.Property_NewConfirmed, deliveryStatusLogObject.NewConfirmed));
			AddParameter(cmd, pNVarChar(DeliveryStatusLogBase.Property_TrackingNumber, 100, deliveryStatusLogObject.TrackingNumber));
			AddParameter(cmd, pNVarChar(DeliveryStatusLogBase.Property_ChangedBy, 100, deliveryStatusLogObject.ChangedBy));
			AddParameter(cmd, pDateTime(DeliveryStatusLogBase.Property_ChangedAt, deliveryStatusLogObject.ChangedAt));
			AddParameter(cmd, pVarChar(DeliveryStatusLogBase.Property_IPAddress, 45, deliveryStatusLogObject.IPAddress));
			AddParameter(cmd, pNVarChar(DeliveryStatusLogBase.Property_UserAgent, 500, deliveryStatusLogObject.UserAgent));
			AddParameter(cmd, pBool(DeliveryStatusLogBase.Property_IsSystemGenerated, deliveryStatusLogObject.IsSystemGenerated));
			AddParameter(cmd, pNVarChar(DeliveryStatusLogBase.Property_SourceAction, 100, deliveryStatusLogObject.SourceAction));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts DeliveryStatusLog
        /// </summary>
        /// <param name="deliveryStatusLogObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(DeliveryStatusLogBase deliveryStatusLogObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTDELIVERYSTATUSLOG);
	
				AddParameter(cmd, pInt32Out(DeliveryStatusLogBase.Property_Id));
				AddCommonParams(cmd, deliveryStatusLogObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					deliveryStatusLogObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					deliveryStatusLogObject.Id = (Int32)GetOutParameter(cmd, DeliveryStatusLogBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(deliveryStatusLogObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates DeliveryStatusLog
        /// </summary>
        /// <param name="deliveryStatusLogObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(DeliveryStatusLogBase deliveryStatusLogObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEDELIVERYSTATUSLOG);
				
				AddParameter(cmd, pInt32(DeliveryStatusLogBase.Property_Id, deliveryStatusLogObject.Id));
				AddCommonParams(cmd, deliveryStatusLogObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					deliveryStatusLogObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(deliveryStatusLogObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes DeliveryStatusLog
        /// </summary>
        /// <param name="Id">Id of the DeliveryStatusLog object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEDELIVERYSTATUSLOG);	
				
				AddParameter(cmd, pInt32(DeliveryStatusLogBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(DeliveryStatusLog), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves DeliveryStatusLog object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the DeliveryStatusLog object to retrieve</param>
        /// <returns>DeliveryStatusLog object, null if not found</returns>
		public DeliveryStatusLog Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETDELIVERYSTATUSLOGBYID))
			{
				AddParameter( cmd, pInt32(DeliveryStatusLogBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all DeliveryStatusLog objects 
        /// </summary>
        /// <returns>A list of DeliveryStatusLog objects</returns>
		public DeliveryStatusLogList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLDELIVERYSTATUSLOG))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all DeliveryStatusLog objects by SalesOrderId
        /// </summary>
        /// <returns>A list of DeliveryStatusLog objects</returns>
		public DeliveryStatusLogList GetBySalesOrderId(Nullable<Int32> _SalesOrderId)
		{
			using( SqlCommand cmd = GetSPCommand(GETDELIVERYSTATUSLOGBYSALESORDERID))
			{
				
				AddParameter( cmd, pInt32(DeliveryStatusLogBase.Property_SalesOrderId, _SalesOrderId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all DeliveryStatusLog objects by PageRequest
        /// </summary>
        /// <returns>A list of DeliveryStatusLog objects</returns>
		public DeliveryStatusLogList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDDELIVERYSTATUSLOG))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				DeliveryStatusLogList _DeliveryStatusLogList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _DeliveryStatusLogList;
			}
		}
		
		/// <summary>
        /// Retrieves all DeliveryStatusLog objects by query String
        /// </summary>
        /// <returns>A list of DeliveryStatusLog objects</returns>
		public DeliveryStatusLogList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETDELIVERYSTATUSLOGBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get DeliveryStatusLog Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of DeliveryStatusLog
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETDELIVERYSTATUSLOGMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get DeliveryStatusLog Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of DeliveryStatusLog
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _DeliveryStatusLogRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETDELIVERYSTATUSLOGROWCOUNT))
			{
				SqlDataReader reader;
				_DeliveryStatusLogRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _DeliveryStatusLogRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills DeliveryStatusLog object
        /// </summary>
        /// <param name="deliveryStatusLogObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(DeliveryStatusLogBase deliveryStatusLogObject, SqlDataReader reader, int start)
		{
			
				deliveryStatusLogObject.Id = reader.GetInt32( start + 0 );			
				deliveryStatusLogObject.EntityType = reader.GetString( start + 1 );			
				deliveryStatusLogObject.EntityId = reader.GetInt32( start + 2 );			
				if(!reader.IsDBNull(3)) deliveryStatusLogObject.SalesOrderId = reader.GetInt32( start + 3 );			
				if(!reader.IsDBNull(4)) deliveryStatusLogObject.OldStatus = reader.GetString( start + 4 );			
				deliveryStatusLogObject.NewStatus = reader.GetString( start + 5 );			
				if(!reader.IsDBNull(6)) deliveryStatusLogObject.ChangeReason = reader.GetString( start + 6 );			
				if(!reader.IsDBNull(7)) deliveryStatusLogObject.OldConfirmed = reader.GetBoolean( start + 7 );			
				if(!reader.IsDBNull(8)) deliveryStatusLogObject.NewConfirmed = reader.GetBoolean( start + 8 );			
				if(!reader.IsDBNull(9)) deliveryStatusLogObject.TrackingNumber = reader.GetString( start + 9 );			
				deliveryStatusLogObject.ChangedBy = reader.GetString( start + 10 );			
				deliveryStatusLogObject.ChangedAt = reader.GetDateTime( start + 11 );			
				if(!reader.IsDBNull(12)) deliveryStatusLogObject.IPAddress = reader.GetString( start + 12 );			
				if(!reader.IsDBNull(13)) deliveryStatusLogObject.UserAgent = reader.GetString( start + 13 );			
				deliveryStatusLogObject.IsSystemGenerated = reader.GetBoolean( start + 14 );			
				if(!reader.IsDBNull(15)) deliveryStatusLogObject.SourceAction = reader.GetString( start + 15 );			
			FillBaseObject(deliveryStatusLogObject, reader, (start + 16));

			
			deliveryStatusLogObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills DeliveryStatusLog object
        /// </summary>
        /// <param name="deliveryStatusLogObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(DeliveryStatusLogBase deliveryStatusLogObject, SqlDataReader reader)
		{
			FillObject(deliveryStatusLogObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves DeliveryStatusLog object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>DeliveryStatusLog object</returns>
		private DeliveryStatusLog GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					DeliveryStatusLog deliveryStatusLogObject= new DeliveryStatusLog();
					FillObject(deliveryStatusLogObject, reader);
					return deliveryStatusLogObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of DeliveryStatusLog objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of DeliveryStatusLog objects</returns>
		private DeliveryStatusLogList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//DeliveryStatusLog list
			DeliveryStatusLogList list = new DeliveryStatusLogList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					DeliveryStatusLog deliveryStatusLogObject = new DeliveryStatusLog();
					FillObject(deliveryStatusLogObject, reader);

					list.Add(deliveryStatusLogObject);
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