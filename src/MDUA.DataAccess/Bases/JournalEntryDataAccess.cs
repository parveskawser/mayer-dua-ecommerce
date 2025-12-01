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
	public partial class JournalEntryDataAccess : BaseDataAccess, IJournalEntryDataAccess
	{
		#region Constants
		private const string INSERTJOURNALENTRY = "InsertJournalEntry";
		private const string UPDATEJOURNALENTRY = "UpdateJournalEntry";
		private const string DELETEJOURNALENTRY = "DeleteJournalEntry";
		private const string GETJOURNALENTRYBYID = "GetJournalEntryById";
		private const string GETALLJOURNALENTRY = "GetAllJournalEntry";
		private const string GETPAGEDJOURNALENTRY = "GetPagedJournalEntry";
		private const string GETJOURNALENTRYMAXIMUMID = "GetJournalEntryMaximumId";
		private const string GETJOURNALENTRYROWCOUNT = "GetJournalEntryRowCount";	
		private const string GETJOURNALENTRYBYQUERY = "GetJournalEntryByQuery";
		#endregion
		
		#region Constructors
		public JournalEntryDataAccess(IConfiguration configuration) : base(configuration) { }
		public JournalEntryDataAccess(ClientContext context) : base(context) { }
		public JournalEntryDataAccess(SqlTransaction transaction) : base(transaction) { }
		public JournalEntryDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="journalEntryObject"></param>
		private void AddCommonParams(SqlCommand cmd, JournalEntryBase journalEntryObject)
		{	
			AddParameter(cmd, pNVarChar(JournalEntryBase.Property_ReferenceType, 50, journalEntryObject.ReferenceType));
			AddParameter(cmd, pInt32(JournalEntryBase.Property_ReferenceId, journalEntryObject.ReferenceId));
			AddParameter(cmd, pDateTime(JournalEntryBase.Property_EntryDate, journalEntryObject.EntryDate));
			AddParameter(cmd, pNVarChar(JournalEntryBase.Property_Description, 255, journalEntryObject.Description));
			AddParameter(cmd, pNVarChar(JournalEntryBase.Property_CreatedBy, 100, journalEntryObject.CreatedBy));
			AddParameter(cmd, pDateTime(JournalEntryBase.Property_CreatedAt, journalEntryObject.CreatedAt));
			AddParameter(cmd, pNVarChar(JournalEntryBase.Property_UpdatedBy, 100, journalEntryObject.UpdatedBy));
			AddParameter(cmd, pDateTime(JournalEntryBase.Property_UpdatedAt, journalEntryObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts JournalEntry
        /// </summary>
        /// <param name="journalEntryObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(JournalEntryBase journalEntryObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTJOURNALENTRY);
	
				AddParameter(cmd, pInt32Out(JournalEntryBase.Property_Id));
				AddCommonParams(cmd, journalEntryObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					journalEntryObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					journalEntryObject.Id = (Int32)GetOutParameter(cmd, JournalEntryBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(journalEntryObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates JournalEntry
        /// </summary>
        /// <param name="journalEntryObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(JournalEntryBase journalEntryObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEJOURNALENTRY);
				
				AddParameter(cmd, pInt32(JournalEntryBase.Property_Id, journalEntryObject.Id));
				AddCommonParams(cmd, journalEntryObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					journalEntryObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(journalEntryObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes JournalEntry
        /// </summary>
        /// <param name="Id">Id of the JournalEntry object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEJOURNALENTRY);	
				
				AddParameter(cmd, pInt32(JournalEntryBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(JournalEntry), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves JournalEntry object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the JournalEntry object to retrieve</param>
        /// <returns>JournalEntry object, null if not found</returns>
		public JournalEntry Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETJOURNALENTRYBYID))
			{
				AddParameter( cmd, pInt32(JournalEntryBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all JournalEntry objects 
        /// </summary>
        /// <returns>A list of JournalEntry objects</returns>
		public JournalEntryList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLJOURNALENTRY))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all JournalEntry objects by PageRequest
        /// </summary>
        /// <returns>A list of JournalEntry objects</returns>
		public JournalEntryList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDJOURNALENTRY))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				JournalEntryList _JournalEntryList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _JournalEntryList;
			}
		}
		
		/// <summary>
        /// Retrieves all JournalEntry objects by query String
        /// </summary>
        /// <returns>A list of JournalEntry objects</returns>
		public JournalEntryList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETJOURNALENTRYBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get JournalEntry Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of JournalEntry
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETJOURNALENTRYMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get JournalEntry Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of JournalEntry
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _JournalEntryRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETJOURNALENTRYROWCOUNT))
			{
				SqlDataReader reader;
				_JournalEntryRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _JournalEntryRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills JournalEntry object
        /// </summary>
        /// <param name="journalEntryObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(JournalEntryBase journalEntryObject, SqlDataReader reader, int start)
		{
			
				journalEntryObject.Id = reader.GetInt32( start + 0 );			
				if(!reader.IsDBNull(1)) journalEntryObject.ReferenceType = reader.GetString( start + 1 );			
				if(!reader.IsDBNull(2)) journalEntryObject.ReferenceId = reader.GetInt32( start + 2 );			
				journalEntryObject.EntryDate = reader.GetDateTime( start + 3 );			
				if(!reader.IsDBNull(4)) journalEntryObject.Description = reader.GetString( start + 4 );			
				if(!reader.IsDBNull(5)) journalEntryObject.CreatedBy = reader.GetString( start + 5 );			
				journalEntryObject.CreatedAt = reader.GetDateTime( start + 6 );			
				if(!reader.IsDBNull(7)) journalEntryObject.UpdatedBy = reader.GetString( start + 7 );			
				if(!reader.IsDBNull(8)) journalEntryObject.UpdatedAt = reader.GetDateTime( start + 8 );			
			FillBaseObject(journalEntryObject, reader, (start + 9));

			
			journalEntryObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills JournalEntry object
        /// </summary>
        /// <param name="journalEntryObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(JournalEntryBase journalEntryObject, SqlDataReader reader)
		{
			FillObject(journalEntryObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves JournalEntry object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>JournalEntry object</returns>
		private JournalEntry GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					JournalEntry journalEntryObject= new JournalEntry();
					FillObject(journalEntryObject, reader);
					return journalEntryObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of JournalEntry objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of JournalEntry objects</returns>
		private JournalEntryList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//JournalEntry list
			JournalEntryList list = new JournalEntryList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					JournalEntry journalEntryObject = new JournalEntry();
					FillObject(journalEntryObject, reader);

					list.Add(journalEntryObject);
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
