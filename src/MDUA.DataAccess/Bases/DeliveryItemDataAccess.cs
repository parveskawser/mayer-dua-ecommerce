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
	public partial class DeliveryItemDataAccess : BaseDataAccess, IDeliveryItemDataAccess
	{
		#region Constants
		private const string INSERTDELIVERYITEM = "InsertDeliveryItem";
		private const string UPDATEDELIVERYITEM = "UpdateDeliveryItem";
		private const string DELETEDELIVERYITEM = "DeleteDeliveryItem";
		private const string GETDELIVERYITEMBYID = "GetDeliveryItemById";
		private const string GETALLDELIVERYITEM = "GetAllDeliveryItem";
		private const string GETPAGEDDELIVERYITEM = "GetPagedDeliveryItem";
		private const string GETDELIVERYITEMBYDELIVERYID = "GetDeliveryItemByDeliveryId";
		private const string GETDELIVERYITEMBYSALESORDERDETAILID = "GetDeliveryItemBySalesOrderDetailId";
		private const string GETDELIVERYITEMMAXIMUMID = "GetDeliveryItemMaximumId";
		private const string GETDELIVERYITEMROWCOUNT = "GetDeliveryItemRowCount";	
		private const string GETDELIVERYITEMBYQUERY = "GetDeliveryItemByQuery";
		#endregion
		
		#region Constructors
		public DeliveryItemDataAccess(IConfiguration configuration) : base(configuration) { }
		public DeliveryItemDataAccess(ClientContext context) : base(context) { }
		public DeliveryItemDataAccess(SqlTransaction transaction) : base(transaction) { }
		public DeliveryItemDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="deliveryItemObject"></param>
		private void AddCommonParams(SqlCommand cmd, DeliveryItemBase deliveryItemObject)
		{	
			AddParameter(cmd, pInt32(DeliveryItemBase.Property_DeliveryId, deliveryItemObject.DeliveryId));
			AddParameter(cmd, pInt32(DeliveryItemBase.Property_SalesOrderDetailId, deliveryItemObject.SalesOrderDetailId));
			AddParameter(cmd, pInt32(DeliveryItemBase.Property_Quantity, deliveryItemObject.Quantity));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts DeliveryItem
        /// </summary>
        /// <param name="deliveryItemObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(DeliveryItemBase deliveryItemObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTDELIVERYITEM);
	
				AddParameter(cmd, pInt32Out(DeliveryItemBase.Property_Id));
				AddCommonParams(cmd, deliveryItemObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					deliveryItemObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					deliveryItemObject.Id = (Int32)GetOutParameter(cmd, DeliveryItemBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(deliveryItemObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates DeliveryItem
        /// </summary>
        /// <param name="deliveryItemObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(DeliveryItemBase deliveryItemObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEDELIVERYITEM);
				
				AddParameter(cmd, pInt32(DeliveryItemBase.Property_Id, deliveryItemObject.Id));
				AddCommonParams(cmd, deliveryItemObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					deliveryItemObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(deliveryItemObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes DeliveryItem
        /// </summary>
        /// <param name="Id">Id of the DeliveryItem object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEDELIVERYITEM);	
				
				AddParameter(cmd, pInt32(DeliveryItemBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(DeliveryItem), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves DeliveryItem object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the DeliveryItem object to retrieve</param>
        /// <returns>DeliveryItem object, null if not found</returns>
		public DeliveryItem Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETDELIVERYITEMBYID))
			{
				AddParameter( cmd, pInt32(DeliveryItemBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all DeliveryItem objects 
        /// </summary>
        /// <returns>A list of DeliveryItem objects</returns>
		public DeliveryItemList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLDELIVERYITEM))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all DeliveryItem objects by DeliveryId
        /// </summary>
        /// <returns>A list of DeliveryItem objects</returns>
		public DeliveryItemList GetByDeliveryId(Int32 _DeliveryId)
		{
			using( SqlCommand cmd = GetSPCommand(GETDELIVERYITEMBYDELIVERYID))
			{
				
				AddParameter( cmd, pInt32(DeliveryItemBase.Property_DeliveryId, _DeliveryId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all DeliveryItem objects by SalesOrderDetailId
        /// </summary>
        /// <returns>A list of DeliveryItem objects</returns>
		public DeliveryItemList GetBySalesOrderDetailId(Int32 _SalesOrderDetailId)
		{
			using( SqlCommand cmd = GetSPCommand(GETDELIVERYITEMBYSALESORDERDETAILID))
			{
				
				AddParameter( cmd, pInt32(DeliveryItemBase.Property_SalesOrderDetailId, _SalesOrderDetailId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all DeliveryItem objects by PageRequest
        /// </summary>
        /// <returns>A list of DeliveryItem objects</returns>
		public DeliveryItemList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDDELIVERYITEM))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				DeliveryItemList _DeliveryItemList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _DeliveryItemList;
			}
		}
		
		/// <summary>
        /// Retrieves all DeliveryItem objects by query String
        /// </summary>
        /// <returns>A list of DeliveryItem objects</returns>
		public DeliveryItemList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETDELIVERYITEMBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get DeliveryItem Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of DeliveryItem
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETDELIVERYITEMMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get DeliveryItem Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of DeliveryItem
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _DeliveryItemRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETDELIVERYITEMROWCOUNT))
			{
				SqlDataReader reader;
				_DeliveryItemRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _DeliveryItemRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills DeliveryItem object
        /// </summary>
        /// <param name="deliveryItemObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(DeliveryItemBase deliveryItemObject, SqlDataReader reader, int start)
		{
			
				deliveryItemObject.Id = reader.GetInt32( start + 0 );			
				deliveryItemObject.DeliveryId = reader.GetInt32( start + 1 );			
				deliveryItemObject.SalesOrderDetailId = reader.GetInt32( start + 2 );			
				deliveryItemObject.Quantity = reader.GetInt32( start + 3 );			
			FillBaseObject(deliveryItemObject, reader, (start + 4));

			
			deliveryItemObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills DeliveryItem object
        /// </summary>
        /// <param name="deliveryItemObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(DeliveryItemBase deliveryItemObject, SqlDataReader reader)
		{
			FillObject(deliveryItemObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves DeliveryItem object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>DeliveryItem object</returns>
		private DeliveryItem GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					DeliveryItem deliveryItemObject= new DeliveryItem();
					FillObject(deliveryItemObject, reader);
					return deliveryItemObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of DeliveryItem objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of DeliveryItem objects</returns>
		private DeliveryItemList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//DeliveryItem list
			DeliveryItemList list = new DeliveryItemList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					DeliveryItem deliveryItemObject = new DeliveryItem();
					FillObject(deliveryItemObject, reader);

					list.Add(deliveryItemObject);
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
