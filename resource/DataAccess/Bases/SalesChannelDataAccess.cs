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
	public partial class SalesChannelDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTSALESCHANNEL = "InsertSalesChannel";
		private const string UPDATESALESCHANNEL = "UpdateSalesChannel";
		private const string DELETESALESCHANNEL = "DeleteSalesChannel";
		private const string GETSALESCHANNELBYID = "GetSalesChannelById";
		private const string GETALLSALESCHANNEL = "GetAllSalesChannel";
		private const string GETPAGEDSALESCHANNEL = "GetPagedSalesChannel";
		private const string GETSALESCHANNELMAXIMUMID = "GetSalesChannelMaximumId";
		private const string GETSALESCHANNELROWCOUNT = "GetSalesChannelRowCount";	
		private const string GETSALESCHANNELBYQUERY = "GetSalesChannelByQuery";
		#endregion
		
		#region Constructors
		public SalesChannelDataAccess(IConfiguration configuration) : base(configuration) { }
		public SalesChannelDataAccess(ClientContext context) : base(context) { }
		public SalesChannelDataAccess(SqlTransaction transaction) : base(transaction) { }
		public SalesChannelDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="salesChannelObject"></param>
		private void AddCommonParams(SqlCommand cmd, SalesChannelBase salesChannelObject)
		{	
			AddParameter(cmd, pNVarChar(SalesChannelBase.Property_Name, 50, salesChannelObject.Name));
			AddParameter(cmd, pNVarChar(SalesChannelBase.Property_CreatedBy, 100, salesChannelObject.CreatedBy));
			AddParameter(cmd, pDateTime(SalesChannelBase.Property_CreatedAt, salesChannelObject.CreatedAt));
			AddParameter(cmd, pNVarChar(SalesChannelBase.Property_UpdatedBy, 100, salesChannelObject.UpdatedBy));
			AddParameter(cmd, pDateTime(SalesChannelBase.Property_UpdatedAt, salesChannelObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts SalesChannel
        /// </summary>
        /// <param name="salesChannelObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(SalesChannelBase salesChannelObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTSALESCHANNEL);
	
				AddParameter(cmd, pInt32Out(SalesChannelBase.Property_Id));
				AddCommonParams(cmd, salesChannelObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					salesChannelObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					salesChannelObject.Id = (Int32)GetOutParameter(cmd, SalesChannelBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(salesChannelObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates SalesChannel
        /// </summary>
        /// <param name="salesChannelObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(SalesChannelBase salesChannelObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATESALESCHANNEL);
				
				AddParameter(cmd, pInt32(SalesChannelBase.Property_Id, salesChannelObject.Id));
				AddCommonParams(cmd, salesChannelObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					salesChannelObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(salesChannelObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes SalesChannel
        /// </summary>
        /// <param name="Id">Id of the SalesChannel object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETESALESCHANNEL);	
				
				AddParameter(cmd, pInt32(SalesChannelBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(SalesChannel), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves SalesChannel object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the SalesChannel object to retrieve</param>
        /// <returns>SalesChannel object, null if not found</returns>
		public SalesChannel Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETSALESCHANNELBYID))
			{
				AddParameter( cmd, pInt32(SalesChannelBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all SalesChannel objects 
        /// </summary>
        /// <returns>A list of SalesChannel objects</returns>
		public SalesChannelList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLSALESCHANNEL))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all SalesChannel objects by PageRequest
        /// </summary>
        /// <returns>A list of SalesChannel objects</returns>
		public SalesChannelList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDSALESCHANNEL))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				SalesChannelList _SalesChannelList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _SalesChannelList;
			}
		}
		
		/// <summary>
        /// Retrieves all SalesChannel objects by query String
        /// </summary>
        /// <returns>A list of SalesChannel objects</returns>
		public SalesChannelList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETSALESCHANNELBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get SalesChannel Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of SalesChannel
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETSALESCHANNELMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get SalesChannel Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of SalesChannel
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _SalesChannelRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETSALESCHANNELROWCOUNT))
			{
				SqlDataReader reader;
				_SalesChannelRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _SalesChannelRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills SalesChannel object
        /// </summary>
        /// <param name="salesChannelObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(SalesChannelBase salesChannelObject, SqlDataReader reader, int start)
		{
			
				salesChannelObject.Id = reader.GetInt32( start + 0 );			
				salesChannelObject.Name = reader.GetString( start + 1 );			
				if(!reader.IsDBNull(2)) salesChannelObject.CreatedBy = reader.GetString( start + 2 );			
				if(!reader.IsDBNull(3)) salesChannelObject.CreatedAt = reader.GetDateTime( start + 3 );			
				if(!reader.IsDBNull(4)) salesChannelObject.UpdatedBy = reader.GetString( start + 4 );			
				if(!reader.IsDBNull(5)) salesChannelObject.UpdatedAt = reader.GetDateTime( start + 5 );			
			FillBaseObject(salesChannelObject, reader, (start + 6));

			
			salesChannelObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills SalesChannel object
        /// </summary>
        /// <param name="salesChannelObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(SalesChannelBase salesChannelObject, SqlDataReader reader)
		{
			FillObject(salesChannelObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves SalesChannel object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>SalesChannel object</returns>
		private SalesChannel GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					SalesChannel salesChannelObject= new SalesChannel();
					FillObject(salesChannelObject, reader);
					return salesChannelObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of SalesChannel objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of SalesChannel objects</returns>
		private SalesChannelList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//SalesChannel list
			SalesChannelList list = new SalesChannelList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					SalesChannel salesChannelObject = new SalesChannel();
					FillObject(salesChannelObject, reader);

					list.Add(salesChannelObject);
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