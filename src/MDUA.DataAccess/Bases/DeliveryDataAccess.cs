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
	public partial class DeliveryDataAccess : BaseDataAccess, IDeliveryDataAccess
	{
		#region Constants
		private const string INSERTDELIVERY = "InsertDelivery";
		private const string UPDATEDELIVERY = "UpdateDelivery";
		private const string DELETEDELIVERY = "DeleteDelivery";
		private const string GETDELIVERYBYID = "GetDeliveryById";
		private const string GETALLDELIVERY = "GetAllDelivery";
		private const string GETPAGEDDELIVERY = "GetPagedDelivery";
		private const string GETDELIVERYBYSALESORDERID = "GetDeliveryBySalesOrderId";
		private const string GETDELIVERYMAXIMUMID = "GetDeliveryMaximumId";
		private const string GETDELIVERYROWCOUNT = "GetDeliveryRowCount";	
		private const string GETDELIVERYBYQUERY = "GetDeliveryByQuery";
		#endregion
		
		#region Constructors
		public DeliveryDataAccess(IConfiguration configuration) : base(configuration) { }
		public DeliveryDataAccess(ClientContext context) : base(context) { }
		public DeliveryDataAccess(SqlTransaction transaction) : base(transaction) { }
		public DeliveryDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="deliveryObject"></param>
		private void AddCommonParams(SqlCommand cmd, DeliveryBase deliveryObject)
		{	
			AddParameter(cmd, pInt32(DeliveryBase.Property_SalesOrderId, deliveryObject.SalesOrderId));
			AddParameter(cmd, pDateTime(DeliveryBase.Property_DeliveryDate, deliveryObject.DeliveryDate));
			AddParameter(cmd, pNVarChar(DeliveryBase.Property_TrackingNumber, 100, deliveryObject.TrackingNumber));
			AddParameter(cmd, pNVarChar(DeliveryBase.Property_Status, 50, deliveryObject.Status));
			AddParameter(cmd, pNVarChar(DeliveryBase.Property_CreatedBy, 100, deliveryObject.CreatedBy));
			AddParameter(cmd, pDateTime(DeliveryBase.Property_CreatedAt, deliveryObject.CreatedAt));
			AddParameter(cmd, pNVarChar(DeliveryBase.Property_UpdatedBy, 100, deliveryObject.UpdatedBy));
			AddParameter(cmd, pDateTime(DeliveryBase.Property_UpdatedAt, deliveryObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts Delivery
        /// </summary>
        /// <param name="deliveryObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(DeliveryBase deliveryObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTDELIVERY);
	
				AddParameter(cmd, pInt32Out(DeliveryBase.Property_Id));
				AddCommonParams(cmd, deliveryObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					deliveryObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					deliveryObject.Id = (Int32)GetOutParameter(cmd, DeliveryBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(deliveryObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates Delivery
        /// </summary>
        /// <param name="deliveryObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(DeliveryBase deliveryObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEDELIVERY);
				
				AddParameter(cmd, pInt32(DeliveryBase.Property_Id, deliveryObject.Id));
				AddCommonParams(cmd, deliveryObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					deliveryObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(deliveryObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes Delivery
        /// </summary>
        /// <param name="Id">Id of the Delivery object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEDELIVERY);	
				
				AddParameter(cmd, pInt32(DeliveryBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(Delivery), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves Delivery object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the Delivery object to retrieve</param>
        /// <returns>Delivery object, null if not found</returns>
		public Delivery Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETDELIVERYBYID))
			{
				AddParameter( cmd, pInt32(DeliveryBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all Delivery objects 
        /// </summary>
        /// <returns>A list of Delivery objects</returns>
		public DeliveryList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLDELIVERY))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all Delivery objects by SalesOrderId
        /// </summary>
        /// <returns>A list of Delivery objects</returns>
		public DeliveryList GetBySalesOrderId(Int32 _SalesOrderId)
		{
			using( SqlCommand cmd = GetSPCommand(GETDELIVERYBYSALESORDERID))
			{
				
				AddParameter( cmd, pInt32(DeliveryBase.Property_SalesOrderId, _SalesOrderId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all Delivery objects by PageRequest
        /// </summary>
        /// <returns>A list of Delivery objects</returns>
		public DeliveryList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDDELIVERY))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				DeliveryList _DeliveryList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _DeliveryList;
			}
		}
		
		/// <summary>
        /// Retrieves all Delivery objects by query String
        /// </summary>
        /// <returns>A list of Delivery objects</returns>
		public DeliveryList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETDELIVERYBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get Delivery Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of Delivery
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETDELIVERYMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get Delivery Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of Delivery
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _DeliveryRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETDELIVERYROWCOUNT))
			{
				SqlDataReader reader;
				_DeliveryRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _DeliveryRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills Delivery object
        /// </summary>
        /// <param name="deliveryObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(DeliveryBase deliveryObject, SqlDataReader reader, int start)
		{
			
				deliveryObject.Id = reader.GetInt32( start + 0 );			
				deliveryObject.SalesOrderId = reader.GetInt32( start + 1 );			
				deliveryObject.DeliveryDate = reader.GetDateTime( start + 2 );			
				if(!reader.IsDBNull(3)) deliveryObject.TrackingNumber = reader.GetString( start + 3 );			
				deliveryObject.Status = reader.GetString( start + 4 );			
				deliveryObject.CreatedBy = reader.GetString( start + 5 );			
				deliveryObject.CreatedAt = reader.GetDateTime( start + 6 );			
				if(!reader.IsDBNull(7)) deliveryObject.UpdatedBy = reader.GetString( start + 7 );			
				if(!reader.IsDBNull(8)) deliveryObject.UpdatedAt = reader.GetDateTime( start + 8 );			
			FillBaseObject(deliveryObject, reader, (start + 9));

			
			deliveryObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills Delivery object
        /// </summary>
        /// <param name="deliveryObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(DeliveryBase deliveryObject, SqlDataReader reader)
		{
			FillObject(deliveryObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves Delivery object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>Delivery object</returns>
		private Delivery GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					Delivery deliveryObject= new Delivery();
					FillObject(deliveryObject, reader);
					return deliveryObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of Delivery objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of Delivery objects</returns>
		private DeliveryList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//Delivery list
			DeliveryList list = new DeliveryList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					Delivery deliveryObject = new Delivery();
					FillObject(deliveryObject, reader);

					list.Add(deliveryObject);
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
