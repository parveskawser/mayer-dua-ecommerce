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
	public partial class ChatMessageDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTCHATMESSAGE = "InsertChatMessage";
		private const string UPDATECHATMESSAGE = "UpdateChatMessage";
		private const string DELETECHATMESSAGE = "DeleteChatMessage";
		private const string GETCHATMESSAGEBYID = "GetChatMessageById";
		private const string GETALLCHATMESSAGE = "GetAllChatMessage";
		private const string GETPAGEDCHATMESSAGE = "GetPagedChatMessage";
		private const string GETCHATMESSAGEBYCHATSESSIONID = "GetChatMessageByChatSessionId";
		private const string GETCHATMESSAGEMAXIMUMID = "GetChatMessageMaximumId";
		private const string GETCHATMESSAGEROWCOUNT = "GetChatMessageRowCount";	
		private const string GETCHATMESSAGEBYQUERY = "GetChatMessageByQuery";
		#endregion
		
		#region Constructors
		public ChatMessageDataAccess(IConfiguration configuration) : base(configuration) { }
		public ChatMessageDataAccess(ClientContext context) : base(context) { }
		public ChatMessageDataAccess(SqlTransaction transaction) : base(transaction) { }
		public ChatMessageDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="chatMessageObject"></param>
		private void AddCommonParams(SqlCommand cmd, ChatMessageBase chatMessageObject)
		{	
			AddParameter(cmd, pInt32(ChatMessageBase.Property_ChatSessionId, chatMessageObject.ChatSessionId));
			AddParameter(cmd, pInt32(ChatMessageBase.Property_SenderId, chatMessageObject.SenderId));
			AddParameter(cmd, pNVarChar(ChatMessageBase.Property_SenderName, 100, chatMessageObject.SenderName));
			AddParameter(cmd, pNVarChar(ChatMessageBase.Property_MessageText, chatMessageObject.MessageText));
			AddParameter(cmd, pBool(ChatMessageBase.Property_IsFromAdmin, chatMessageObject.IsFromAdmin));
			AddParameter(cmd, pBool(ChatMessageBase.Property_IsRead, chatMessageObject.IsRead));
			AddParameter(cmd, pDateTime(ChatMessageBase.Property_SentAt, chatMessageObject.SentAt));
			AddParameter(cmd, pNVarChar(ChatMessageBase.Property_SenderType, 20, chatMessageObject.SenderType));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts ChatMessage
        /// </summary>
        /// <param name="chatMessageObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(ChatMessageBase chatMessageObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTCHATMESSAGE);
	
				AddParameter(cmd, pInt32Out(ChatMessageBase.Property_Id));
				AddCommonParams(cmd, chatMessageObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					chatMessageObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					chatMessageObject.Id = (Int32)GetOutParameter(cmd, ChatMessageBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(chatMessageObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates ChatMessage
        /// </summary>
        /// <param name="chatMessageObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(ChatMessageBase chatMessageObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATECHATMESSAGE);
				
				AddParameter(cmd, pInt32(ChatMessageBase.Property_Id, chatMessageObject.Id));
				AddCommonParams(cmd, chatMessageObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					chatMessageObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(chatMessageObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes ChatMessage
        /// </summary>
        /// <param name="Id">Id of the ChatMessage object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETECHATMESSAGE);	
				
				AddParameter(cmd, pInt32(ChatMessageBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(ChatMessage), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves ChatMessage object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the ChatMessage object to retrieve</param>
        /// <returns>ChatMessage object, null if not found</returns>
		public ChatMessage Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETCHATMESSAGEBYID))
			{
				AddParameter( cmd, pInt32(ChatMessageBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all ChatMessage objects 
        /// </summary>
        /// <returns>A list of ChatMessage objects</returns>
		public ChatMessageList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLCHATMESSAGE))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all ChatMessage objects by ChatSessionId
        /// </summary>
        /// <returns>A list of ChatMessage objects</returns>
		public ChatMessageList GetByChatSessionId(Int32 _ChatSessionId)
		{
			using( SqlCommand cmd = GetSPCommand(GETCHATMESSAGEBYCHATSESSIONID))
			{
				
				AddParameter( cmd, pInt32(ChatMessageBase.Property_ChatSessionId, _ChatSessionId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all ChatMessage objects by PageRequest
        /// </summary>
        /// <returns>A list of ChatMessage objects</returns>
		public ChatMessageList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDCHATMESSAGE))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				ChatMessageList _ChatMessageList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _ChatMessageList;
			}
		}
		
		/// <summary>
        /// Retrieves all ChatMessage objects by query String
        /// </summary>
        /// <returns>A list of ChatMessage objects</returns>
		public ChatMessageList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETCHATMESSAGEBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get ChatMessage Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of ChatMessage
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCHATMESSAGEMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get ChatMessage Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of ChatMessage
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _ChatMessageRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCHATMESSAGEROWCOUNT))
			{
				SqlDataReader reader;
				_ChatMessageRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _ChatMessageRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills ChatMessage object
        /// </summary>
        /// <param name="chatMessageObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(ChatMessageBase chatMessageObject, SqlDataReader reader, int start)
		{
			
				chatMessageObject.Id = reader.GetInt32( start + 0 );			
				chatMessageObject.ChatSessionId = reader.GetInt32( start + 1 );			
				if(!reader.IsDBNull(2)) chatMessageObject.SenderId = reader.GetInt32( start + 2 );			
				chatMessageObject.SenderName = reader.GetString( start + 3 );			
				chatMessageObject.MessageText = reader.GetString( start + 4 );			
				chatMessageObject.IsFromAdmin = reader.GetBoolean( start + 5 );			
				chatMessageObject.IsRead = reader.GetBoolean( start + 6 );			
				chatMessageObject.SentAt = reader.GetDateTime( start + 7 );			
				if(!reader.IsDBNull(8)) chatMessageObject.SenderType = reader.GetString( start + 8 );			
			FillBaseObject(chatMessageObject, reader, (start + 9));

			
			chatMessageObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills ChatMessage object
        /// </summary>
        /// <param name="chatMessageObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(ChatMessageBase chatMessageObject, SqlDataReader reader)
		{
			FillObject(chatMessageObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves ChatMessage object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>ChatMessage object</returns>
		private ChatMessage GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					ChatMessage chatMessageObject= new ChatMessage();
					FillObject(chatMessageObject, reader);
					return chatMessageObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of ChatMessage objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of ChatMessage objects</returns>
		private ChatMessageList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//ChatMessage list
			ChatMessageList list = new ChatMessageList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					ChatMessage chatMessageObject = new ChatMessage();
					FillObject(chatMessageObject, reader);

					list.Add(chatMessageObject);
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