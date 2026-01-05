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
	public partial class JournalEntryDetailDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTJOURNALENTRYDETAIL = "InsertJournalEntryDetail";
		private const string UPDATEJOURNALENTRYDETAIL = "UpdateJournalEntryDetail";
		private const string DELETEJOURNALENTRYDETAIL = "DeleteJournalEntryDetail";
		private const string GETJOURNALENTRYDETAILBYID = "GetJournalEntryDetailById";
		private const string GETALLJOURNALENTRYDETAIL = "GetAllJournalEntryDetail";
		private const string GETPAGEDJOURNALENTRYDETAIL = "GetPagedJournalEntryDetail";
		private const string GETJOURNALENTRYDETAILBYJOURNALENTRYID = "GetJournalEntryDetailByJournalEntryId";
		private const string GETJOURNALENTRYDETAILBYACCOUNTID = "GetJournalEntryDetailByAccountId";
		private const string GETJOURNALENTRYDETAILMAXIMUMID = "GetJournalEntryDetailMaximumId";
		private const string GETJOURNALENTRYDETAILROWCOUNT = "GetJournalEntryDetailRowCount";	
		private const string GETJOURNALENTRYDETAILBYQUERY = "GetJournalEntryDetailByQuery";
		#endregion
		
		#region Constructors
		public JournalEntryDetailDataAccess(IConfiguration configuration) : base(configuration) { }
		public JournalEntryDetailDataAccess(ClientContext context) : base(context) { }
		public JournalEntryDetailDataAccess(SqlTransaction transaction) : base(transaction) { }
		public JournalEntryDetailDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="journalEntryDetailObject"></param>
		private void AddCommonParams(SqlCommand cmd, JournalEntryDetailBase journalEntryDetailObject)
		{	
			AddParameter(cmd, pInt32(JournalEntryDetailBase.Property_JournalEntryId, journalEntryDetailObject.JournalEntryId));
			AddParameter(cmd, pInt32(JournalEntryDetailBase.Property_AccountId, journalEntryDetailObject.AccountId));
			AddParameter(cmd, pDecimal(JournalEntryDetailBase.Property_Debit, 9, journalEntryDetailObject.Debit));
			AddParameter(cmd, pDecimal(JournalEntryDetailBase.Property_Credit, 9, journalEntryDetailObject.Credit));
			AddParameter(cmd, pNVarChar(JournalEntryDetailBase.Property_CreatedBy, 100, journalEntryDetailObject.CreatedBy));
			AddParameter(cmd, pDateTime(JournalEntryDetailBase.Property_CreatedAt, journalEntryDetailObject.CreatedAt));
			AddParameter(cmd, pNVarChar(JournalEntryDetailBase.Property_UpdatedBy, 100, journalEntryDetailObject.UpdatedBy));
			AddParameter(cmd, pDateTime(JournalEntryDetailBase.Property_UpdatedAt, journalEntryDetailObject.UpdatedAt));
			AddParameter(cmd, pNVarChar(JournalEntryDetailBase.Property_LineDescription, 255, journalEntryDetailObject.LineDescription));
			AddParameter(cmd, pNVarChar(JournalEntryDetailBase.Property_CurrencyCode, 10, journalEntryDetailObject.CurrencyCode));
			AddParameter(cmd, pDecimal(JournalEntryDetailBase.Property_ExchangeRate, 9, journalEntryDetailObject.ExchangeRate));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts JournalEntryDetail
        /// </summary>
        /// <param name="journalEntryDetailObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(JournalEntryDetailBase journalEntryDetailObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTJOURNALENTRYDETAIL);
	
				AddParameter(cmd, pInt32Out(JournalEntryDetailBase.Property_Id));
				AddCommonParams(cmd, journalEntryDetailObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					journalEntryDetailObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					journalEntryDetailObject.Id = (Int32)GetOutParameter(cmd, JournalEntryDetailBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(journalEntryDetailObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates JournalEntryDetail
        /// </summary>
        /// <param name="journalEntryDetailObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(JournalEntryDetailBase journalEntryDetailObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEJOURNALENTRYDETAIL);
				
				AddParameter(cmd, pInt32(JournalEntryDetailBase.Property_Id, journalEntryDetailObject.Id));
				AddCommonParams(cmd, journalEntryDetailObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					journalEntryDetailObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(journalEntryDetailObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes JournalEntryDetail
        /// </summary>
        /// <param name="Id">Id of the JournalEntryDetail object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEJOURNALENTRYDETAIL);	
				
				AddParameter(cmd, pInt32(JournalEntryDetailBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(JournalEntryDetail), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves JournalEntryDetail object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the JournalEntryDetail object to retrieve</param>
        /// <returns>JournalEntryDetail object, null if not found</returns>
		public JournalEntryDetail Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETJOURNALENTRYDETAILBYID))
			{
				AddParameter( cmd, pInt32(JournalEntryDetailBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all JournalEntryDetail objects 
        /// </summary>
        /// <returns>A list of JournalEntryDetail objects</returns>
		public JournalEntryDetailList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLJOURNALENTRYDETAIL))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all JournalEntryDetail objects by JournalEntryId
        /// </summary>
        /// <returns>A list of JournalEntryDetail objects</returns>
		public JournalEntryDetailList GetByJournalEntryId(Int32 _JournalEntryId)
		{
			using( SqlCommand cmd = GetSPCommand(GETJOURNALENTRYDETAILBYJOURNALENTRYID))
			{
				
				AddParameter( cmd, pInt32(JournalEntryDetailBase.Property_JournalEntryId, _JournalEntryId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all JournalEntryDetail objects by AccountId
        /// </summary>
        /// <returns>A list of JournalEntryDetail objects</returns>
		public JournalEntryDetailList GetByAccountId(Int32 _AccountId)
		{
			using( SqlCommand cmd = GetSPCommand(GETJOURNALENTRYDETAILBYACCOUNTID))
			{
				
				AddParameter( cmd, pInt32(JournalEntryDetailBase.Property_AccountId, _AccountId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all JournalEntryDetail objects by PageRequest
        /// </summary>
        /// <returns>A list of JournalEntryDetail objects</returns>
		public JournalEntryDetailList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDJOURNALENTRYDETAIL))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				JournalEntryDetailList _JournalEntryDetailList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _JournalEntryDetailList;
			}
		}
		
		/// <summary>
        /// Retrieves all JournalEntryDetail objects by query String
        /// </summary>
        /// <returns>A list of JournalEntryDetail objects</returns>
		public JournalEntryDetailList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETJOURNALENTRYDETAILBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get JournalEntryDetail Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of JournalEntryDetail
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETJOURNALENTRYDETAILMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get JournalEntryDetail Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of JournalEntryDetail
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _JournalEntryDetailRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETJOURNALENTRYDETAILROWCOUNT))
			{
				SqlDataReader reader;
				_JournalEntryDetailRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _JournalEntryDetailRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills JournalEntryDetail object
        /// </summary>
        /// <param name="journalEntryDetailObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(JournalEntryDetailBase journalEntryDetailObject, SqlDataReader reader, int start)
		{
			
				journalEntryDetailObject.Id = reader.GetInt32( start + 0 );			
				journalEntryDetailObject.JournalEntryId = reader.GetInt32( start + 1 );			
				journalEntryDetailObject.AccountId = reader.GetInt32( start + 2 );			
				journalEntryDetailObject.Debit = reader.GetDecimal( start + 3 );			
				journalEntryDetailObject.Credit = reader.GetDecimal( start + 4 );			
				if(!reader.IsDBNull(5)) journalEntryDetailObject.CreatedBy = reader.GetString( start + 5 );			
				journalEntryDetailObject.CreatedAt = reader.GetDateTime( start + 6 );			
				if(!reader.IsDBNull(7)) journalEntryDetailObject.UpdatedBy = reader.GetString( start + 7 );			
				if(!reader.IsDBNull(8)) journalEntryDetailObject.UpdatedAt = reader.GetDateTime( start + 8 );			
				if(!reader.IsDBNull(9)) journalEntryDetailObject.LineDescription = reader.GetString( start + 9 );			
				if(!reader.IsDBNull(10)) journalEntryDetailObject.CurrencyCode = reader.GetString( start + 10 );			
				if(!reader.IsDBNull(11)) journalEntryDetailObject.ExchangeRate = reader.GetDecimal( start + 11 );			
			FillBaseObject(journalEntryDetailObject, reader, (start + 12));

			
			journalEntryDetailObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills JournalEntryDetail object
        /// </summary>
        /// <param name="journalEntryDetailObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(JournalEntryDetailBase journalEntryDetailObject, SqlDataReader reader)
		{
			FillObject(journalEntryDetailObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves JournalEntryDetail object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>JournalEntryDetail object</returns>
		private JournalEntryDetail GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					JournalEntryDetail journalEntryDetailObject= new JournalEntryDetail();
					FillObject(journalEntryDetailObject, reader);
					return journalEntryDetailObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of JournalEntryDetail objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of JournalEntryDetail objects</returns>
		private JournalEntryDetailList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//JournalEntryDetail list
			JournalEntryDetailList list = new JournalEntryDetailList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					JournalEntryDetail journalEntryDetailObject = new JournalEntryDetail();
					FillObject(journalEntryDetailObject, reader);

					list.Add(journalEntryDetailObject);
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