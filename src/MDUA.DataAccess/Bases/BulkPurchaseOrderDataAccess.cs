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
	public partial class BulkPurchaseOrderDataAccess : BaseDataAccess, IBulkPurchaseOrderDataAccess
	{
		#region Constants
		private const string INSERTBULKPURCHASEORDER = "InsertBulkPurchaseOrder";
		private const string UPDATEBULKPURCHASEORDER = "UpdateBulkPurchaseOrder";
		private const string DELETEBULKPURCHASEORDER = "DeleteBulkPurchaseOrder";
		private const string GETBULKPURCHASEORDERBYID = "GetBulkPurchaseOrderById";
		private const string GETALLBULKPURCHASEORDER = "GetAllBulkPurchaseOrder";
		private const string GETPAGEDBULKPURCHASEORDER = "GetPagedBulkPurchaseOrder";
		private const string GETBULKPURCHASEORDERBYVENDORID = "GetBulkPurchaseOrderByVendorId";
		private const string GETBULKPURCHASEORDERMAXIMUMID = "GetBulkPurchaseOrderMaximumId";
		private const string GETBULKPURCHASEORDERROWCOUNT = "GetBulkPurchaseOrderRowCount";	
		private const string GETBULKPURCHASEORDERBYQUERY = "GetBulkPurchaseOrderByQuery";
		#endregion
		
		#region Constructors
		public BulkPurchaseOrderDataAccess(IConfiguration configuration) : base(configuration) { }
		public BulkPurchaseOrderDataAccess(ClientContext context) : base(context) { }
		public BulkPurchaseOrderDataAccess(SqlTransaction transaction) : base(transaction) { }
		public BulkPurchaseOrderDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="bulkPurchaseOrderObject"></param>
		private void AddCommonParams(SqlCommand cmd, BulkPurchaseOrderBase bulkPurchaseOrderObject)
		{	
			AddParameter(cmd, pInt32(BulkPurchaseOrderBase.Property_VendorId, bulkPurchaseOrderObject.VendorId));
			AddParameter(cmd, pNVarChar(BulkPurchaseOrderBase.Property_AgreementNumber, 50, bulkPurchaseOrderObject.AgreementNumber));
			AddParameter(cmd, pNVarChar(BulkPurchaseOrderBase.Property_Title, 200, bulkPurchaseOrderObject.Title));
			AddParameter(cmd, pDateTime(BulkPurchaseOrderBase.Property_AgreementDate, bulkPurchaseOrderObject.AgreementDate));
			AddParameter(cmd, pDateTime(BulkPurchaseOrderBase.Property_ExpiryDate, bulkPurchaseOrderObject.ExpiryDate));
			AddParameter(cmd, pInt32(BulkPurchaseOrderBase.Property_TotalTargetQuantity, bulkPurchaseOrderObject.TotalTargetQuantity));
			AddParameter(cmd, pDecimal(BulkPurchaseOrderBase.Property_TotalTargetAmount, 9, bulkPurchaseOrderObject.TotalTargetAmount));
			AddParameter(cmd, pNVarChar(BulkPurchaseOrderBase.Property_Status, 20, bulkPurchaseOrderObject.Status));
			AddParameter(cmd, pNVarChar(BulkPurchaseOrderBase.Property_Remarks, 500, bulkPurchaseOrderObject.Remarks));
			AddParameter(cmd, pNVarChar(BulkPurchaseOrderBase.Property_CreatedBy, 100, bulkPurchaseOrderObject.CreatedBy));
			AddParameter(cmd, pDateTime(BulkPurchaseOrderBase.Property_CreatedAt, bulkPurchaseOrderObject.CreatedAt));
			AddParameter(cmd, pNVarChar(BulkPurchaseOrderBase.Property_UpdatedBy, 100, bulkPurchaseOrderObject.UpdatedBy));
			AddParameter(cmd, pDateTime(BulkPurchaseOrderBase.Property_UpdatedAt, bulkPurchaseOrderObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts BulkPurchaseOrder
        /// </summary>
        /// <param name="bulkPurchaseOrderObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(BulkPurchaseOrderBase bulkPurchaseOrderObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTBULKPURCHASEORDER);
	
				AddParameter(cmd, pInt32Out(BulkPurchaseOrderBase.Property_Id));
				AddCommonParams(cmd, bulkPurchaseOrderObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					bulkPurchaseOrderObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					bulkPurchaseOrderObject.Id = (Int32)GetOutParameter(cmd, BulkPurchaseOrderBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(bulkPurchaseOrderObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates BulkPurchaseOrder
        /// </summary>
        /// <param name="bulkPurchaseOrderObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(BulkPurchaseOrderBase bulkPurchaseOrderObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEBULKPURCHASEORDER);
				
				AddParameter(cmd, pInt32(BulkPurchaseOrderBase.Property_Id, bulkPurchaseOrderObject.Id));
				AddCommonParams(cmd, bulkPurchaseOrderObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					bulkPurchaseOrderObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(bulkPurchaseOrderObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes BulkPurchaseOrder
        /// </summary>
        /// <param name="Id">Id of the BulkPurchaseOrder object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEBULKPURCHASEORDER);	
				
				AddParameter(cmd, pInt32(BulkPurchaseOrderBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(BulkPurchaseOrder), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves BulkPurchaseOrder object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the BulkPurchaseOrder object to retrieve</param>
        /// <returns>BulkPurchaseOrder object, null if not found</returns>
		public BulkPurchaseOrder Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETBULKPURCHASEORDERBYID))
			{
				AddParameter( cmd, pInt32(BulkPurchaseOrderBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all BulkPurchaseOrder objects 
        /// </summary>
        /// <returns>A list of BulkPurchaseOrder objects</returns>
		public BulkPurchaseOrderList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLBULKPURCHASEORDER))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all BulkPurchaseOrder objects by VendorId
        /// </summary>
        /// <returns>A list of BulkPurchaseOrder objects</returns>
		public BulkPurchaseOrderList GetByVendorId(Int32 _VendorId)
		{
			using( SqlCommand cmd = GetSPCommand(GETBULKPURCHASEORDERBYVENDORID))
			{
				
				AddParameter( cmd, pInt32(BulkPurchaseOrderBase.Property_VendorId, _VendorId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all BulkPurchaseOrder objects by PageRequest
        /// </summary>
        /// <returns>A list of BulkPurchaseOrder objects</returns>
		public BulkPurchaseOrderList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDBULKPURCHASEORDER))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				BulkPurchaseOrderList _BulkPurchaseOrderList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _BulkPurchaseOrderList;
			}
		}
		
		/// <summary>
        /// Retrieves all BulkPurchaseOrder objects by query String
        /// </summary>
        /// <returns>A list of BulkPurchaseOrder objects</returns>
		public BulkPurchaseOrderList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETBULKPURCHASEORDERBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get BulkPurchaseOrder Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of BulkPurchaseOrder
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETBULKPURCHASEORDERMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get BulkPurchaseOrder Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of BulkPurchaseOrder
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _BulkPurchaseOrderRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETBULKPURCHASEORDERROWCOUNT))
			{
				SqlDataReader reader;
				_BulkPurchaseOrderRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _BulkPurchaseOrderRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills BulkPurchaseOrder object
        /// </summary>
        /// <param name="bulkPurchaseOrderObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(BulkPurchaseOrderBase bulkPurchaseOrderObject, SqlDataReader reader, int start)
		{
			
				bulkPurchaseOrderObject.Id = reader.GetInt32( start + 0 );			
				bulkPurchaseOrderObject.VendorId = reader.GetInt32( start + 1 );			
				bulkPurchaseOrderObject.AgreementNumber = reader.GetString( start + 2 );			
				if(!reader.IsDBNull(3)) bulkPurchaseOrderObject.Title = reader.GetString( start + 3 );			
				bulkPurchaseOrderObject.AgreementDate = reader.GetDateTime( start + 4 );			
				if(!reader.IsDBNull(5)) bulkPurchaseOrderObject.ExpiryDate = reader.GetDateTime( start + 5 );			
				if(!reader.IsDBNull(6)) bulkPurchaseOrderObject.TotalTargetQuantity = reader.GetInt32( start + 6 );			
				if(!reader.IsDBNull(7)) bulkPurchaseOrderObject.TotalTargetAmount = reader.GetDecimal( start + 7 );			
				bulkPurchaseOrderObject.Status = reader.GetString( start + 8 );			
				if(!reader.IsDBNull(9)) bulkPurchaseOrderObject.Remarks = reader.GetString( start + 9 );			
				if(!reader.IsDBNull(10)) bulkPurchaseOrderObject.CreatedBy = reader.GetString( start + 10 );			
				bulkPurchaseOrderObject.CreatedAt = reader.GetDateTime( start + 11 );			
				if(!reader.IsDBNull(12)) bulkPurchaseOrderObject.UpdatedBy = reader.GetString( start + 12 );			
				if(!reader.IsDBNull(13)) bulkPurchaseOrderObject.UpdatedAt = reader.GetDateTime( start + 13 );			
			FillBaseObject(bulkPurchaseOrderObject, reader, (start + 14));

			
			bulkPurchaseOrderObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills BulkPurchaseOrder object
        /// </summary>
        /// <param name="bulkPurchaseOrderObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(BulkPurchaseOrderBase bulkPurchaseOrderObject, SqlDataReader reader)
		{
			FillObject(bulkPurchaseOrderObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves BulkPurchaseOrder object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>BulkPurchaseOrder object</returns>
		private BulkPurchaseOrder GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					BulkPurchaseOrder bulkPurchaseOrderObject= new BulkPurchaseOrder();
					FillObject(bulkPurchaseOrderObject, reader);
					return bulkPurchaseOrderObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of BulkPurchaseOrder objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of BulkPurchaseOrder objects</returns>
		private BulkPurchaseOrderList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//BulkPurchaseOrder list
			BulkPurchaseOrderList list = new BulkPurchaseOrderList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					BulkPurchaseOrder bulkPurchaseOrderObject = new BulkPurchaseOrder();
					FillObject(bulkPurchaseOrderObject, reader);

					list.Add(bulkPurchaseOrderObject);
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
