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
	public partial class ChatSessionDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTCHATSESSION = "InsertChatSession";
		private const string UPDATECHATSESSION = "UpdateChatSession";
		private const string DELETECHATSESSION = "DeleteChatSession";
		private const string GETCHATSESSIONBYID = "GetChatSessionById";
		private const string GETALLCHATSESSION = "GetAllChatSession";
		private const string GETPAGEDCHATSESSION = "GetPagedChatSession";
		private const string GETCHATSESSIONBYUSERLOGINID = "GetChatSessionByUserLoginId";
		private const string GETCHATSESSIONMAXIMUMID = "GetChatSessionMaximumId";
		private const string GETCHATSESSIONROWCOUNT = "GetChatSessionRowCount";	
		private const string GETCHATSESSIONBYQUERY = "GetChatSessionByQuery";
		#endregion
		
		#region Constructors
		public ChatSessionDataAccess(IConfiguration configuration) : base(configuration) { }
		public ChatSessionDataAccess(ClientContext context) : base(context) { }
		public ChatSessionDataAccess(SqlTransaction transaction) : base(transaction) { }
		public ChatSessionDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="chatSessionObject"></param>
		private void AddCommonParams(SqlCommand cmd, ChatSessionBase chatSessionObject)
		{	
			AddParameter(cmd, pGuid(ChatSessionBase.Property_SessionGuid, chatSessionObject.SessionGuid));
			AddParameter(cmd, pInt32(ChatSessionBase.Property_UserLoginId, chatSessionObject.UserLoginId));
			AddParameter(cmd, pNVarChar(ChatSessionBase.Property_GuestName, 100, chatSessionObject.GuestName));
			AddParameter(cmd, pNVarChar(ChatSessionBase.Property_Status, 20, chatSessionObject.Status));
			AddParameter(cmd, pDateTime(ChatSessionBase.Property_StartedAt, chatSessionObject.StartedAt));
			AddParameter(cmd, pDateTime(ChatSessionBase.Property_LastMessageAt, chatSessionObject.LastMessageAt));
			AddParameter(cmd, pBool(ChatSessionBase.Property_IsActive, chatSessionObject.IsActive));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts ChatSession
        /// </summary>
        /// <param name="chatSessionObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(ChatSessionBase chatSessionObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTCHATSESSION);
	
				AddParameter(cmd, pInt32Out(ChatSessionBase.Property_Id));
				AddCommonParams(cmd, chatSessionObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					chatSessionObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					chatSessionObject.Id = (Int32)GetOutParameter(cmd, ChatSessionBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(chatSessionObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates ChatSession
        /// </summary>
        /// <param name="chatSessionObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(ChatSessionBase chatSessionObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATECHATSESSION);
				
				AddParameter(cmd, pInt32(ChatSessionBase.Property_Id, chatSessionObject.Id));
				AddCommonParams(cmd, chatSessionObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					chatSessionObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(chatSessionObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes ChatSession
        /// </summary>
        /// <param name="Id">Id of the ChatSession object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETECHATSESSION);	
				
				AddParameter(cmd, pInt32(ChatSessionBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(ChatSession), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves ChatSession object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the ChatSession object to retrieve</param>
        /// <returns>ChatSession object, null if not found</returns>
		public ChatSession Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETCHATSESSIONBYID))
			{
				AddParameter( cmd, pInt32(ChatSessionBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all ChatSession objects 
        /// </summary>
        /// <returns>A list of ChatSession objects</returns>
		public ChatSessionList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLCHATSESSION))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all ChatSession objects by UserLoginId
        /// </summary>
        /// <returns>A list of ChatSession objects</returns>
		public ChatSessionList GetByUserLoginId(Nullable<Int32> _UserLoginId)
		{
			using( SqlCommand cmd = GetSPCommand(GETCHATSESSIONBYUSERLOGINID))
			{
				
				AddParameter( cmd, pInt32(ChatSessionBase.Property_UserLoginId, _UserLoginId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all ChatSession objects by PageRequest
        /// </summary>
        /// <returns>A list of ChatSession objects</returns>
		public ChatSessionList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDCHATSESSION))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				ChatSessionList _ChatSessionList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _ChatSessionList;
			}
		}
		
		/// <summary>
        /// Retrieves all ChatSession objects by query String
        /// </summary>
        /// <returns>A list of ChatSession objects</returns>
		public ChatSessionList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETCHATSESSIONBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get ChatSession Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of ChatSession
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCHATSESSIONMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get ChatSession Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of ChatSession
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _ChatSessionRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCHATSESSIONROWCOUNT))
			{
				SqlDataReader reader;
				_ChatSessionRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _ChatSessionRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills ChatSession object
        /// </summary>
        /// <param name="chatSessionObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(ChatSessionBase chatSessionObject, SqlDataReader reader, int start)
		{
			
				chatSessionObject.Id = reader.GetInt32( start + 0 );			
				chatSessionObject.SessionGuid = reader.GetGuid( start + 1 );			
				if(!reader.IsDBNull(2)) chatSessionObject.UserLoginId = reader.GetInt32( start + 2 );			
				if(!reader.IsDBNull(3)) chatSessionObject.GuestName = reader.GetString( start + 3 );			
				chatSessionObject.Status = reader.GetString( start + 4 );			
				chatSessionObject.StartedAt = reader.GetDateTime( start + 5 );			
				chatSessionObject.LastMessageAt = reader.GetDateTime( start + 6 );			
				chatSessionObject.IsActive = reader.GetBoolean( start + 7 );			
			FillBaseObject(chatSessionObject, reader, (start + 8));

			
			chatSessionObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills ChatSession object
        /// </summary>
        /// <param name="chatSessionObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(ChatSessionBase chatSessionObject, SqlDataReader reader)
		{
			FillObject(chatSessionObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves ChatSession object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>ChatSession object</returns>
		private ChatSession GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					ChatSession chatSessionObject= new ChatSession();
					FillObject(chatSessionObject, reader);
					return chatSessionObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of ChatSession objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of ChatSession objects</returns>
		private ChatSessionList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//ChatSession list
			ChatSessionList list = new ChatSessionList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					ChatSession chatSessionObject = new ChatSession();
					FillObject(chatSessionObject, reader);

					list.Add(chatSessionObject);
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