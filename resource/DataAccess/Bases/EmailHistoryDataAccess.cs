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
	public partial class EmailHistoryDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTEMAILHISTORY = "InsertEmailHistory";
		private const string UPDATEEMAILHISTORY = "UpdateEmailHistory";
		private const string DELETEEMAILHISTORY = "DeleteEmailHistory";
		private const string GETEMAILHISTORYBYID = "GetEmailHistoryById";
		private const string GETALLEMAILHISTORY = "GetAllEmailHistory";
		private const string GETPAGEDEMAILHISTORY = "GetPagedEmailHistory";
		private const string GETEMAILHISTORYMAXIMUMID = "GetEmailHistoryMaximumId";
		private const string GETEMAILHISTORYROWCOUNT = "GetEmailHistoryRowCount";	
		private const string GETEMAILHISTORYBYQUERY = "GetEmailHistoryByQuery";
		#endregion
		
		#region Constructors
		public EmailHistoryDataAccess(IConfiguration configuration) : base(configuration) { }
		public EmailHistoryDataAccess(ClientContext context) : base(context) { }
		public EmailHistoryDataAccess(SqlTransaction transaction) : base(transaction) { }
		public EmailHistoryDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="emailHistoryObject"></param>
		private void AddCommonParams(SqlCommand cmd, EmailHistoryBase emailHistoryObject)
		{	
			AddParameter(cmd, pNVarChar(EmailHistoryBase.Property_TemplateKey, 250, emailHistoryObject.TemplateKey));
			AddParameter(cmd, pNVarChar(EmailHistoryBase.Property_ToEmail, 250, emailHistoryObject.ToEmail));
			AddParameter(cmd, pNVarChar(EmailHistoryBase.Property_CcEmail, 250, emailHistoryObject.CcEmail));
			AddParameter(cmd, pNVarChar(EmailHistoryBase.Property_BccEmail, 250, emailHistoryObject.BccEmail));
			AddParameter(cmd, pNVarChar(EmailHistoryBase.Property_FromEmail, 250, emailHistoryObject.FromEmail));
			AddParameter(cmd, pNVarChar(EmailHistoryBase.Property_FromName, 250, emailHistoryObject.FromName));
			AddParameter(cmd, pNVarChar(EmailHistoryBase.Property_EmailSubject, 250, emailHistoryObject.EmailSubject));
			AddParameter(cmd, pNVarChar(EmailHistoryBase.Property_EmailBodyContent, emailHistoryObject.EmailBodyContent));
			AddParameter(cmd, pDateTime(EmailHistoryBase.Property_EmailSentDate, emailHistoryObject.EmailSentDate));
			AddParameter(cmd, pBool(EmailHistoryBase.Property_IsSystemAutoSent, emailHistoryObject.IsSystemAutoSent));
			AddParameter(cmd, pBool(EmailHistoryBase.Property_IsRead, emailHistoryObject.IsRead));
			AddParameter(cmd, pInt32(EmailHistoryBase.Property_ReadCount, emailHistoryObject.ReadCount));
			AddParameter(cmd, pDateTime(EmailHistoryBase.Property_LastUpdatedDate, emailHistoryObject.LastUpdatedDate));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts EmailHistory
        /// </summary>
        /// <param name="emailHistoryObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(EmailHistoryBase emailHistoryObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTEMAILHISTORY);
	
				AddParameter(cmd, pInt32Out(EmailHistoryBase.Property_Id));
				AddCommonParams(cmd, emailHistoryObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					emailHistoryObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					emailHistoryObject.Id = (Int32)GetOutParameter(cmd, EmailHistoryBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(emailHistoryObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates EmailHistory
        /// </summary>
        /// <param name="emailHistoryObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(EmailHistoryBase emailHistoryObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEEMAILHISTORY);
				
				AddParameter(cmd, pInt32(EmailHistoryBase.Property_Id, emailHistoryObject.Id));
				AddCommonParams(cmd, emailHistoryObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					emailHistoryObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(emailHistoryObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes EmailHistory
        /// </summary>
        /// <param name="Id">Id of the EmailHistory object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEEMAILHISTORY);	
				
				AddParameter(cmd, pInt32(EmailHistoryBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(EmailHistory), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves EmailHistory object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the EmailHistory object to retrieve</param>
        /// <returns>EmailHistory object, null if not found</returns>
		public EmailHistory Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETEMAILHISTORYBYID))
			{
				AddParameter( cmd, pInt32(EmailHistoryBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all EmailHistory objects 
        /// </summary>
        /// <returns>A list of EmailHistory objects</returns>
		public EmailHistoryList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLEMAILHISTORY))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all EmailHistory objects by PageRequest
        /// </summary>
        /// <returns>A list of EmailHistory objects</returns>
		public EmailHistoryList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDEMAILHISTORY))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				EmailHistoryList _EmailHistoryList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _EmailHistoryList;
			}
		}
		
		/// <summary>
        /// Retrieves all EmailHistory objects by query String
        /// </summary>
        /// <returns>A list of EmailHistory objects</returns>
		public EmailHistoryList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETEMAILHISTORYBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get EmailHistory Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of EmailHistory
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETEMAILHISTORYMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get EmailHistory Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of EmailHistory
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _EmailHistoryRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETEMAILHISTORYROWCOUNT))
			{
				SqlDataReader reader;
				_EmailHistoryRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _EmailHistoryRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills EmailHistory object
        /// </summary>
        /// <param name="emailHistoryObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(EmailHistoryBase emailHistoryObject, SqlDataReader reader, int start)
		{
			
				emailHistoryObject.Id = reader.GetInt32( start + 0 );			
				if(!reader.IsDBNull(1)) emailHistoryObject.TemplateKey = reader.GetString( start + 1 );			
				if(!reader.IsDBNull(2)) emailHistoryObject.ToEmail = reader.GetString( start + 2 );			
				if(!reader.IsDBNull(3)) emailHistoryObject.CcEmail = reader.GetString( start + 3 );			
				if(!reader.IsDBNull(4)) emailHistoryObject.BccEmail = reader.GetString( start + 4 );			
				if(!reader.IsDBNull(5)) emailHistoryObject.FromEmail = reader.GetString( start + 5 );			
				if(!reader.IsDBNull(6)) emailHistoryObject.FromName = reader.GetString( start + 6 );			
				if(!reader.IsDBNull(7)) emailHistoryObject.EmailSubject = reader.GetString( start + 7 );			
				if(!reader.IsDBNull(8)) emailHistoryObject.EmailBodyContent = reader.GetString( start + 8 );			
				if(!reader.IsDBNull(9)) emailHistoryObject.EmailSentDate = reader.GetDateTime( start + 9 );			
				if(!reader.IsDBNull(10)) emailHistoryObject.IsSystemAutoSent = reader.GetBoolean( start + 10 );			
				if(!reader.IsDBNull(11)) emailHistoryObject.IsRead = reader.GetBoolean( start + 11 );			
				if(!reader.IsDBNull(12)) emailHistoryObject.ReadCount = reader.GetInt32( start + 12 );			
				if(!reader.IsDBNull(13)) emailHistoryObject.LastUpdatedDate = reader.GetDateTime( start + 13 );			
			FillBaseObject(emailHistoryObject, reader, (start + 14));

			
			emailHistoryObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills EmailHistory object
        /// </summary>
        /// <param name="emailHistoryObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(EmailHistoryBase emailHistoryObject, SqlDataReader reader)
		{
			FillObject(emailHistoryObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves EmailHistory object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>EmailHistory object</returns>
		private EmailHistory GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					EmailHistory emailHistoryObject= new EmailHistory();
					FillObject(emailHistoryObject, reader);
					return emailHistoryObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of EmailHistory objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of EmailHistory objects</returns>
		private EmailHistoryList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//EmailHistory list
			EmailHistoryList list = new EmailHistoryList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					EmailHistory emailHistoryObject = new EmailHistory();
					FillObject(emailHistoryObject, reader);

					list.Add(emailHistoryObject);
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