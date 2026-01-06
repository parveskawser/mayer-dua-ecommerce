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
	public partial class SalesOrderHeaderDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTSALESORDERHEADER = "InsertSalesOrderHeader";
		private const string UPDATESALESORDERHEADER = "UpdateSalesOrderHeader";
		private const string DELETESALESORDERHEADER = "DeleteSalesOrderHeader";
		private const string GETSALESORDERHEADERBYID = "GetSalesOrderHeaderById";
		private const string GETALLSALESORDERHEADER = "GetAllSalesOrderHeader";
		private const string GETPAGEDSALESORDERHEADER = "GetPagedSalesOrderHeader";
		private const string GETSALESORDERHEADERBYCOMPANYCUSTOMERID = "GetSalesOrderHeaderByCompanyCustomerId";
		private const string GETSALESORDERHEADERBYADDRESSID = "GetSalesOrderHeaderByAddressId";
		private const string GETSALESORDERHEADERBYSALESCHANNELID = "GetSalesOrderHeaderBySalesChannelId";
		private const string GETSALESORDERHEADERMAXIMUMID = "GetSalesOrderHeaderMaximumId";
		private const string GETSALESORDERHEADERROWCOUNT = "GetSalesOrderHeaderRowCount";	
		private const string GETSALESORDERHEADERBYQUERY = "GetSalesOrderHeaderByQuery";
		#endregion
		
		#region Constructors
		public SalesOrderHeaderDataAccess(IConfiguration configuration) : base(configuration) { }
		public SalesOrderHeaderDataAccess(ClientContext context) : base(context) { }
		public SalesOrderHeaderDataAccess(SqlTransaction transaction) : base(transaction) { }
		public SalesOrderHeaderDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="salesOrderHeaderObject"></param>
		private void AddCommonParams(SqlCommand cmd, SalesOrderHeaderBase salesOrderHeaderObject)
		{	
			AddParameter(cmd, pInt32(SalesOrderHeaderBase.Property_CompanyCustomerId, salesOrderHeaderObject.CompanyCustomerId));
			AddParameter(cmd, pInt32(SalesOrderHeaderBase.Property_AddressId, salesOrderHeaderObject.AddressId));
			AddParameter(cmd, pInt32(SalesOrderHeaderBase.Property_SalesChannelId, salesOrderHeaderObject.SalesChannelId));
			AddParameter(cmd, pVarChar(SalesOrderHeaderBase.Property_OnlineOrderId, 10, salesOrderHeaderObject.OnlineOrderId));
			AddParameter(cmd, pVarChar(SalesOrderHeaderBase.Property_DirectOrderId, 10, salesOrderHeaderObject.DirectOrderId));
			AddParameter(cmd, pDateTime(SalesOrderHeaderBase.Property_OrderDate, salesOrderHeaderObject.OrderDate));
			AddParameter(cmd, pDecimal(SalesOrderHeaderBase.Property_TotalAmount, 9, salesOrderHeaderObject.TotalAmount));
			AddParameter(cmd, pDecimal(SalesOrderHeaderBase.Property_DiscountAmount, 9, salesOrderHeaderObject.DiscountAmount));
			AddParameter(cmd, pDecimal(SalesOrderHeaderBase.Property_NetAmount, 9, salesOrderHeaderObject.NetAmount));
			AddParameter(cmd, pNVarChar(SalesOrderHeaderBase.Property_SessionId, 100, salesOrderHeaderObject.SessionId));
			AddParameter(cmd, pVarChar(SalesOrderHeaderBase.Property_IPAddress, 45, salesOrderHeaderObject.IPAddress));
			AddParameter(cmd, pNVarChar(SalesOrderHeaderBase.Property_Status, 30, salesOrderHeaderObject.Status));
			AddParameter(cmd, pBool(SalesOrderHeaderBase.Property_IsActive, salesOrderHeaderObject.IsActive));
			AddParameter(cmd, pBool(SalesOrderHeaderBase.Property_Confirmed, salesOrderHeaderObject.Confirmed));
			AddParameter(cmd, pNVarChar(SalesOrderHeaderBase.Property_CreatedBy, 100, salesOrderHeaderObject.CreatedBy));
			AddParameter(cmd, pDateTime(SalesOrderHeaderBase.Property_CreatedAt, salesOrderHeaderObject.CreatedAt));
			AddParameter(cmd, pNVarChar(SalesOrderHeaderBase.Property_UpdatedBy, 100, salesOrderHeaderObject.UpdatedBy));
			AddParameter(cmd, pDateTime(SalesOrderHeaderBase.Property_UpdatedAt, salesOrderHeaderObject.UpdatedAt));
			AddParameter(cmd, pVarChar(SalesOrderHeaderBase.Property_SalesOrderId, 10, salesOrderHeaderObject.SalesOrderId));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts SalesOrderHeader
        /// </summary>
        /// <param name="salesOrderHeaderObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(SalesOrderHeaderBase salesOrderHeaderObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTSALESORDERHEADER);
	
				AddParameter(cmd, pInt32Out(SalesOrderHeaderBase.Property_Id));
				AddCommonParams(cmd, salesOrderHeaderObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					salesOrderHeaderObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					salesOrderHeaderObject.Id = (Int32)GetOutParameter(cmd, SalesOrderHeaderBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(salesOrderHeaderObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates SalesOrderHeader
        /// </summary>
        /// <param name="salesOrderHeaderObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(SalesOrderHeaderBase salesOrderHeaderObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATESALESORDERHEADER);
				
				AddParameter(cmd, pInt32(SalesOrderHeaderBase.Property_Id, salesOrderHeaderObject.Id));
				AddCommonParams(cmd, salesOrderHeaderObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					salesOrderHeaderObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(salesOrderHeaderObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes SalesOrderHeader
        /// </summary>
        /// <param name="Id">Id of the SalesOrderHeader object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETESALESORDERHEADER);	
				
				AddParameter(cmd, pInt32(SalesOrderHeaderBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(SalesOrderHeader), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves SalesOrderHeader object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the SalesOrderHeader object to retrieve</param>
        /// <returns>SalesOrderHeader object, null if not found</returns>
		public SalesOrderHeader Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETSALESORDERHEADERBYID))
			{
				AddParameter( cmd, pInt32(SalesOrderHeaderBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all SalesOrderHeader objects 
        /// </summary>
        /// <returns>A list of SalesOrderHeader objects</returns>
		public SalesOrderHeaderList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLSALESORDERHEADER))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all SalesOrderHeader objects by CompanyCustomerId
        /// </summary>
        /// <returns>A list of SalesOrderHeader objects</returns>
		public SalesOrderHeaderList GetByCompanyCustomerId(Int32 _CompanyCustomerId)
		{
			using( SqlCommand cmd = GetSPCommand(GETSALESORDERHEADERBYCOMPANYCUSTOMERID))
			{
				
				AddParameter( cmd, pInt32(SalesOrderHeaderBase.Property_CompanyCustomerId, _CompanyCustomerId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all SalesOrderHeader objects by AddressId
        /// </summary>
        /// <returns>A list of SalesOrderHeader objects</returns>
		public SalesOrderHeaderList GetByAddressId(Int32 _AddressId)
		{
			using( SqlCommand cmd = GetSPCommand(GETSALESORDERHEADERBYADDRESSID))
			{
				
				AddParameter( cmd, pInt32(SalesOrderHeaderBase.Property_AddressId, _AddressId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all SalesOrderHeader objects by SalesChannelId
        /// </summary>
        /// <returns>A list of SalesOrderHeader objects</returns>
		public SalesOrderHeaderList GetBySalesChannelId(Int32 _SalesChannelId)
		{
			using( SqlCommand cmd = GetSPCommand(GETSALESORDERHEADERBYSALESCHANNELID))
			{
				
				AddParameter( cmd, pInt32(SalesOrderHeaderBase.Property_SalesChannelId, _SalesChannelId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all SalesOrderHeader objects by PageRequest
        /// </summary>
        /// <returns>A list of SalesOrderHeader objects</returns>
		public SalesOrderHeaderList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDSALESORDERHEADER))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				SalesOrderHeaderList _SalesOrderHeaderList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _SalesOrderHeaderList;
			}
		}
		
		/// <summary>
        /// Retrieves all SalesOrderHeader objects by query String
        /// </summary>
        /// <returns>A list of SalesOrderHeader objects</returns>
		public SalesOrderHeaderList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETSALESORDERHEADERBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get SalesOrderHeader Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of SalesOrderHeader
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETSALESORDERHEADERMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get SalesOrderHeader Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of SalesOrderHeader
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _SalesOrderHeaderRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETSALESORDERHEADERROWCOUNT))
			{
				SqlDataReader reader;
				_SalesOrderHeaderRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _SalesOrderHeaderRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills SalesOrderHeader object
        /// </summary>
        /// <param name="salesOrderHeaderObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(SalesOrderHeaderBase salesOrderHeaderObject, SqlDataReader reader, int start)
		{
			
				salesOrderHeaderObject.Id = reader.GetInt32( start + 0 );			
				salesOrderHeaderObject.CompanyCustomerId = reader.GetInt32( start + 1 );			
				salesOrderHeaderObject.AddressId = reader.GetInt32( start + 2 );			
				salesOrderHeaderObject.SalesChannelId = reader.GetInt32( start + 3 );			
				if(!reader.IsDBNull(4)) salesOrderHeaderObject.OnlineOrderId = reader.GetString( start + 4 );			
				if(!reader.IsDBNull(5)) salesOrderHeaderObject.DirectOrderId = reader.GetString( start + 5 );			
				salesOrderHeaderObject.OrderDate = reader.GetDateTime( start + 6 );			
				salesOrderHeaderObject.TotalAmount = reader.GetDecimal( start + 7 );			
				salesOrderHeaderObject.DiscountAmount = reader.GetDecimal( start + 8 );			
				if(!reader.IsDBNull(9)) salesOrderHeaderObject.NetAmount = reader.GetDecimal( start + 9 );			
				if(!reader.IsDBNull(10)) salesOrderHeaderObject.SessionId = reader.GetString( start + 10 );			
				if(!reader.IsDBNull(11)) salesOrderHeaderObject.IPAddress = reader.GetString( start + 11 );			
				salesOrderHeaderObject.Status = reader.GetString( start + 12 );			
				salesOrderHeaderObject.IsActive = reader.GetBoolean( start + 13 );			
				salesOrderHeaderObject.Confirmed = reader.GetBoolean( start + 14 );			
				if(!reader.IsDBNull(15)) salesOrderHeaderObject.CreatedBy = reader.GetString( start + 15 );			
				salesOrderHeaderObject.CreatedAt = reader.GetDateTime( start + 16 );			
				if(!reader.IsDBNull(17)) salesOrderHeaderObject.UpdatedBy = reader.GetString( start + 17 );			
				if(!reader.IsDBNull(18)) salesOrderHeaderObject.UpdatedAt = reader.GetDateTime( start + 18 );			
				if(!reader.IsDBNull(19)) salesOrderHeaderObject.SalesOrderId = reader.GetString( start + 19 );			
			FillBaseObject(salesOrderHeaderObject, reader, (start + 20));

			
			salesOrderHeaderObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills SalesOrderHeader object
        /// </summary>
        /// <param name="salesOrderHeaderObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(SalesOrderHeaderBase salesOrderHeaderObject, SqlDataReader reader)
		{
			FillObject(salesOrderHeaderObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves SalesOrderHeader object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>SalesOrderHeader object</returns>
		private SalesOrderHeader GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					SalesOrderHeader salesOrderHeaderObject= new SalesOrderHeader();
					FillObject(salesOrderHeaderObject, reader);
					return salesOrderHeaderObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of SalesOrderHeader objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of SalesOrderHeader objects</returns>
		private SalesOrderHeaderList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//SalesOrderHeader list
			SalesOrderHeaderList list = new SalesOrderHeaderList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					SalesOrderHeader salesOrderHeaderObject = new SalesOrderHeader();
					FillObject(salesOrderHeaderObject, reader);

					list.Add(salesOrderHeaderObject);
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