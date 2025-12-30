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
	public partial class PoReceivedDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTPORECEIVED = "InsertPoReceived";
		private const string UPDATEPORECEIVED = "UpdatePoReceived";
		private const string DELETEPORECEIVED = "DeletePoReceived";
		private const string GETPORECEIVEDBYID = "GetPoReceivedById";
		private const string GETALLPORECEIVED = "GetAllPoReceived";
		private const string GETPAGEDPORECEIVED = "GetPagedPoReceived";
		private const string GETPORECEIVEDBYPOREQUESTEDID = "GetPoReceivedByPoRequestedId";
		private const string GETPORECEIVEDMAXIMUMID = "GetPoReceivedMaximumId";
		private const string GETPORECEIVEDROWCOUNT = "GetPoReceivedRowCount";	
		private const string GETPORECEIVEDBYQUERY = "GetPoReceivedByQuery";
		#endregion
		
		#region Constructors
		public PoReceivedDataAccess(IConfiguration configuration) : base(configuration) { }
		public PoReceivedDataAccess(ClientContext context) : base(context) { }
		public PoReceivedDataAccess(SqlTransaction transaction) : base(transaction) { }
		public PoReceivedDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="poReceivedObject"></param>
		private void AddCommonParams(SqlCommand cmd, PoReceivedBase poReceivedObject)
		{	
			AddParameter(cmd, pInt32(PoReceivedBase.Property_PoRequestedId, poReceivedObject.PoRequestedId));
			AddParameter(cmd, pInt32(PoReceivedBase.Property_ReceivedQuantity, poReceivedObject.ReceivedQuantity));
			AddParameter(cmd, pDecimal(PoReceivedBase.Property_BuyingPrice, 9, poReceivedObject.BuyingPrice));
			AddParameter(cmd, pDateTime(PoReceivedBase.Property_ReceivedDate, poReceivedObject.ReceivedDate));
			AddParameter(cmd, pNVarChar(PoReceivedBase.Property_CreatedBy, 100, poReceivedObject.CreatedBy));
			AddParameter(cmd, pDateTime(PoReceivedBase.Property_CreatedAt, poReceivedObject.CreatedAt));
			AddParameter(cmd, pNVarChar(PoReceivedBase.Property_UpdatedBy, 100, poReceivedObject.UpdatedBy));
			AddParameter(cmd, pDateTime(PoReceivedBase.Property_UpdatedAt, poReceivedObject.UpdatedAt));
			AddParameter(cmd, pNVarChar(PoReceivedBase.Property_Remarks, 500, poReceivedObject.Remarks));
			AddParameter(cmd, pNVarChar(PoReceivedBase.Property_InvoiceNo, 100, poReceivedObject.InvoiceNo));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts PoReceived
        /// </summary>
        /// <param name="poReceivedObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(PoReceivedBase poReceivedObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTPORECEIVED);
	
				AddParameter(cmd, pInt32Out(PoReceivedBase.Property_Id));
				AddCommonParams(cmd, poReceivedObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					poReceivedObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					poReceivedObject.Id = (Int32)GetOutParameter(cmd, PoReceivedBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(poReceivedObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates PoReceived
        /// </summary>
        /// <param name="poReceivedObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(PoReceivedBase poReceivedObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEPORECEIVED);
				
				AddParameter(cmd, pInt32(PoReceivedBase.Property_Id, poReceivedObject.Id));
				AddCommonParams(cmd, poReceivedObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					poReceivedObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(poReceivedObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes PoReceived
        /// </summary>
        /// <param name="Id">Id of the PoReceived object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEPORECEIVED);	
				
				AddParameter(cmd, pInt32(PoReceivedBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(PoReceived), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves PoReceived object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the PoReceived object to retrieve</param>
        /// <returns>PoReceived object, null if not found</returns>
		public PoReceived Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETPORECEIVEDBYID))
			{
				AddParameter( cmd, pInt32(PoReceivedBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all PoReceived objects 
        /// </summary>
        /// <returns>A list of PoReceived objects</returns>
		public PoReceivedList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLPORECEIVED))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all PoReceived objects by PoRequestedId
        /// </summary>
        /// <returns>A list of PoReceived objects</returns>
		public PoReceivedList GetByPoRequestedId(Int32 _PoRequestedId)
		{
			using( SqlCommand cmd = GetSPCommand(GETPORECEIVEDBYPOREQUESTEDID))
			{
				
				AddParameter( cmd, pInt32(PoReceivedBase.Property_PoRequestedId, _PoRequestedId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all PoReceived objects by PageRequest
        /// </summary>
        /// <returns>A list of PoReceived objects</returns>
		public PoReceivedList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDPORECEIVED))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				PoReceivedList _PoReceivedList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _PoReceivedList;
			}
		}
		
		/// <summary>
        /// Retrieves all PoReceived objects by query String
        /// </summary>
        /// <returns>A list of PoReceived objects</returns>
		public PoReceivedList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETPORECEIVEDBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get PoReceived Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of PoReceived
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPORECEIVEDMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get PoReceived Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of PoReceived
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _PoReceivedRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPORECEIVEDROWCOUNT))
			{
				SqlDataReader reader;
				_PoReceivedRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _PoReceivedRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills PoReceived object
        /// </summary>
        /// <param name="poReceivedObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(PoReceivedBase poReceivedObject, SqlDataReader reader, int start)
		{
			
				poReceivedObject.Id = reader.GetInt32( start + 0 );			
				poReceivedObject.PoRequestedId = reader.GetInt32( start + 1 );			
				poReceivedObject.ReceivedQuantity = reader.GetInt32( start + 2 );			
				poReceivedObject.BuyingPrice = reader.GetDecimal( start + 3 );			
				poReceivedObject.ReceivedDate = reader.GetDateTime( start + 4 );			
				if(!reader.IsDBNull(5)) poReceivedObject.CreatedBy = reader.GetString( start + 5 );			
				poReceivedObject.CreatedAt = reader.GetDateTime( start + 6 );			
				if(!reader.IsDBNull(7)) poReceivedObject.UpdatedBy = reader.GetString( start + 7 );			
				if(!reader.IsDBNull(8)) poReceivedObject.UpdatedAt = reader.GetDateTime( start + 8 );			
				if(!reader.IsDBNull(9)) poReceivedObject.Remarks = reader.GetString( start + 9 );			
				if(!reader.IsDBNull(10)) poReceivedObject.InvoiceNo = reader.GetString( start + 10 );			
			FillBaseObject(poReceivedObject, reader, (start + 11));

			
			poReceivedObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills PoReceived object
        /// </summary>
        /// <param name="poReceivedObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(PoReceivedBase poReceivedObject, SqlDataReader reader)
		{
			FillObject(poReceivedObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves PoReceived object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>PoReceived object</returns>
		private PoReceived GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					PoReceived poReceivedObject= new PoReceived();
					FillObject(poReceivedObject, reader);
					return poReceivedObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of PoReceived objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of PoReceived objects</returns>
		private PoReceivedList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//PoReceived list
			PoReceivedList list = new PoReceivedList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					PoReceived poReceivedObject = new PoReceived();
					FillObject(poReceivedObject, reader);

					list.Add(poReceivedObject);
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