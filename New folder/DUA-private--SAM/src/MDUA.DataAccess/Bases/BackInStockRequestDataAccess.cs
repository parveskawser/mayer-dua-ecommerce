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
	public partial class BackInStockRequestDataAccess : BaseDataAccess, IBackInStockRequestDataAccess
	{
		#region Constants
		private const string INSERTBACKINSTOCKREQUEST = "InsertBackInStockRequest";
		private const string UPDATEBACKINSTOCKREQUEST = "UpdateBackInStockRequest";
		private const string DELETEBACKINSTOCKREQUEST = "DeleteBackInStockRequest";
		private const string GETBACKINSTOCKREQUESTBYID = "GetBackInStockRequestById";
		private const string GETALLBACKINSTOCKREQUEST = "GetAllBackInStockRequest";
		private const string GETPAGEDBACKINSTOCKREQUEST = "GetPagedBackInStockRequest";
		private const string GETBACKINSTOCKREQUESTBYPRODUCTVARIANTID = "GetBackInStockRequestByProductVariantId";
		private const string GETBACKINSTOCKREQUESTMAXIMUMID = "GetBackInStockRequestMaximumId";
		private const string GETBACKINSTOCKREQUESTROWCOUNT = "GetBackInStockRequestRowCount";	
		private const string GETBACKINSTOCKREQUESTBYQUERY = "GetBackInStockRequestByQuery";
		#endregion
		
		#region Constructors
		public BackInStockRequestDataAccess(IConfiguration configuration) : base(configuration) { }
		public BackInStockRequestDataAccess(ClientContext context) : base(context) { }
		public BackInStockRequestDataAccess(SqlTransaction transaction) : base(transaction) { }
		public BackInStockRequestDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="backInStockRequestObject"></param>
		private void AddCommonParams(SqlCommand cmd, BackInStockRequestBase backInStockRequestObject)
		{	
			AddParameter(cmd, pInt32(BackInStockRequestBase.Property_ProductVariantId, backInStockRequestObject.ProductVariantId));
			AddParameter(cmd, pNVarChar(BackInStockRequestBase.Property_ContactNumber, 50, backInStockRequestObject.ContactNumber));
			AddParameter(cmd, pDateTime(BackInStockRequestBase.Property_RequestDate, backInStockRequestObject.RequestDate));
			AddParameter(cmd, pBool(BackInStockRequestBase.Property_IsNotified, backInStockRequestObject.IsNotified));
			AddParameter(cmd, pDateTime(BackInStockRequestBase.Property_NotifiedDate, backInStockRequestObject.NotifiedDate));
			AddParameter(cmd, pNVarChar(BackInStockRequestBase.Property_CreatedBy, 100, backInStockRequestObject.CreatedBy));
			AddParameter(cmd, pDateTime(BackInStockRequestBase.Property_CreatedAt, backInStockRequestObject.CreatedAt));
			AddParameter(cmd, pNVarChar(BackInStockRequestBase.Property_UpdatedBy, 100, backInStockRequestObject.UpdatedBy));
			AddParameter(cmd, pDateTime(BackInStockRequestBase.Property_UpdatedAt, backInStockRequestObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts BackInStockRequest
        /// </summary>
        /// <param name="backInStockRequestObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(BackInStockRequestBase backInStockRequestObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTBACKINSTOCKREQUEST);
	
				AddParameter(cmd, pInt32Out(BackInStockRequestBase.Property_Id));
				AddCommonParams(cmd, backInStockRequestObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					backInStockRequestObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					backInStockRequestObject.Id = (Int32)GetOutParameter(cmd, BackInStockRequestBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(backInStockRequestObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates BackInStockRequest
        /// </summary>
        /// <param name="backInStockRequestObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(BackInStockRequestBase backInStockRequestObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEBACKINSTOCKREQUEST);
				
				AddParameter(cmd, pInt32(BackInStockRequestBase.Property_Id, backInStockRequestObject.Id));
				AddCommonParams(cmd, backInStockRequestObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					backInStockRequestObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(backInStockRequestObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes BackInStockRequest
        /// </summary>
        /// <param name="Id">Id of the BackInStockRequest object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEBACKINSTOCKREQUEST);	
				
				AddParameter(cmd, pInt32(BackInStockRequestBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(BackInStockRequest), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves BackInStockRequest object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the BackInStockRequest object to retrieve</param>
        /// <returns>BackInStockRequest object, null if not found</returns>
		public BackInStockRequest Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETBACKINSTOCKREQUESTBYID))
			{
				AddParameter( cmd, pInt32(BackInStockRequestBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all BackInStockRequest objects 
        /// </summary>
        /// <returns>A list of BackInStockRequest objects</returns>
		public BackInStockRequestList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLBACKINSTOCKREQUEST))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all BackInStockRequest objects by ProductVariantId
        /// </summary>
        /// <returns>A list of BackInStockRequest objects</returns>
		public BackInStockRequestList GetByProductVariantId(Int32 _ProductVariantId)
		{
			using( SqlCommand cmd = GetSPCommand(GETBACKINSTOCKREQUESTBYPRODUCTVARIANTID))
			{
				
				AddParameter( cmd, pInt32(BackInStockRequestBase.Property_ProductVariantId, _ProductVariantId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all BackInStockRequest objects by PageRequest
        /// </summary>
        /// <returns>A list of BackInStockRequest objects</returns>
		public BackInStockRequestList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDBACKINSTOCKREQUEST))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				BackInStockRequestList _BackInStockRequestList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _BackInStockRequestList;
			}
		}
		
		/// <summary>
        /// Retrieves all BackInStockRequest objects by query String
        /// </summary>
        /// <returns>A list of BackInStockRequest objects</returns>
		public BackInStockRequestList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETBACKINSTOCKREQUESTBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get BackInStockRequest Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of BackInStockRequest
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETBACKINSTOCKREQUESTMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get BackInStockRequest Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of BackInStockRequest
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _BackInStockRequestRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETBACKINSTOCKREQUESTROWCOUNT))
			{
				SqlDataReader reader;
				_BackInStockRequestRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _BackInStockRequestRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills BackInStockRequest object
        /// </summary>
        /// <param name="backInStockRequestObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(BackInStockRequestBase backInStockRequestObject, SqlDataReader reader, int start)
		{
			
				backInStockRequestObject.Id = reader.GetInt32( start + 0 );			
				backInStockRequestObject.ProductVariantId = reader.GetInt32( start + 1 );			
				if(!reader.IsDBNull(2)) backInStockRequestObject.ContactNumber = reader.GetString( start + 2 );			
				backInStockRequestObject.RequestDate = reader.GetDateTime( start + 3 );			
				backInStockRequestObject.IsNotified = reader.GetBoolean( start + 4 );			
				if(!reader.IsDBNull(5)) backInStockRequestObject.NotifiedDate = reader.GetDateTime( start + 5 );			
				if(!reader.IsDBNull(6)) backInStockRequestObject.CreatedBy = reader.GetString( start + 6 );			
				backInStockRequestObject.CreatedAt = reader.GetDateTime( start + 7 );			
				if(!reader.IsDBNull(8)) backInStockRequestObject.UpdatedBy = reader.GetString( start + 8 );			
				if(!reader.IsDBNull(9)) backInStockRequestObject.UpdatedAt = reader.GetDateTime( start + 9 );			
			FillBaseObject(backInStockRequestObject, reader, (start + 10));

			
			backInStockRequestObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills BackInStockRequest object
        /// </summary>
        /// <param name="backInStockRequestObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(BackInStockRequestBase backInStockRequestObject, SqlDataReader reader)
		{
			FillObject(backInStockRequestObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves BackInStockRequest object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>BackInStockRequest object</returns>
		private BackInStockRequest GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					BackInStockRequest backInStockRequestObject= new BackInStockRequest();
					FillObject(backInStockRequestObject, reader);
					return backInStockRequestObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of BackInStockRequest objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of BackInStockRequest objects</returns>
		private BackInStockRequestList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//BackInStockRequest list
			BackInStockRequestList list = new BackInStockRequestList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					BackInStockRequest backInStockRequestObject = new BackInStockRequest();
					FillObject(backInStockRequestObject, reader);

					list.Add(backInStockRequestObject);
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
