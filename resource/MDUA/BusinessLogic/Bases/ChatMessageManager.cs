using System;
using System.Data.SqlClient;

using MDUA.Framework;
using MDUA.Framework.Exceptions;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.DataAccess;

namespace MDUA.BusinessLogic
{
	/// <summary>
    /// Business logic processing for ChatMessage.
    /// </summary>    
	public partial class ChatMessageManager : BaseManager
	{
	
		#region Constructors
		public ChatMessageManager(ClientContext context) : base(context) { }
		public ChatMessageManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new chatMessage.
        /// data manipulation for insertion of ChatMessage
        /// </summary>
        /// <param name="chatMessageObject"></param>
        /// <returns></returns>
        private bool Insert(ChatMessage chatMessageObject)
        {
            // new chatMessage
            using (ChatMessageDataAccess data = new ChatMessageDataAccess(ClientContext))
            {
                // insert to chatMessageObject
                Int32 _Id = data.Insert(chatMessageObject);
                // if successful, process
                if (_Id > 0)
                {
                    chatMessageObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of ChatMessage Object.
        /// Data manipulation processing for: new, deleted, updated ChatMessage
        /// </summary>
        /// <param name="chatMessageObject"></param>
        /// <returns></returns>
        public bool UpdateBase(ChatMessage chatMessageObject)
        {
            // use of switch for different types of DML
            switch (chatMessageObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(chatMessageObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(chatMessageObject.Id);
            }
            // update rows
            using (ChatMessageDataAccess data = new ChatMessageDataAccess(ClientContext))
            {
                return (data.Update(chatMessageObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for ChatMessage
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (ChatMessageDataAccess data = new ChatMessageDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve ChatMessage data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>ChatMessage Object</returns>
        public ChatMessage Get(Int32 _Id)
        {
            using (ChatMessageDataAccess data = new ChatMessageDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a ChatMessage .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public ChatMessage Get(Int32 _Id, bool fillChild)
        {
            ChatMessage chatMessageObject;
            chatMessageObject = Get(_Id);
            
            if (chatMessageObject != null && fillChild)
            {
                // populate child data for a chatMessageObject
                FillChatMessageWithChilds(chatMessageObject, fillChild);
            }

            return chatMessageObject;
        }
		
		/// <summary>
        /// populates a ChatMessage with its child entities
        /// </summary>
        /// <param name="chatMessage"></param>
		/// <param name="fillChilds"></param>
        private void FillChatMessageWithChilds(ChatMessage chatMessageObject, bool fillChilds)
        {
            // populate child data for a chatMessageObject
            if (chatMessageObject != null)
            {
				// Retrieve ChatSessionIdObject as ChatSession type for the ChatMessage using ChatSessionId
				using(ChatSessionManager chatSessionManager = new ChatSessionManager(ClientContext))
				{
					chatMessageObject.ChatSessionIdObject = chatSessionManager.Get(chatMessageObject.ChatSessionId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of ChatMessage.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of ChatMessage</returns>
        public ChatMessageList GetAll()
        {
            using (ChatMessageDataAccess data = new ChatMessageDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of ChatMessage.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of ChatMessage</returns>
        public ChatMessageList GetAll(bool fillChild)
        {
			ChatMessageList chatMessageList = new ChatMessageList();
            using (ChatMessageDataAccess data = new ChatMessageDataAccess(ClientContext))
            {
                chatMessageList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (ChatMessage chatMessageObject in chatMessageList)
                {
					FillChatMessageWithChilds(chatMessageObject, fillChild);
				}
			}
			return chatMessageList;
        }
		
		/// <summary>
        /// Retrieve list of ChatMessage  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of ChatMessage</returns>
        public ChatMessageList GetPaged(PagedRequest request)
        {
            using (ChatMessageDataAccess data = new ChatMessageDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of ChatMessage  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of ChatMessage</returns>
        public ChatMessageList GetByQuery(String query)
        {
            using (ChatMessageDataAccess data = new ChatMessageDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get ChatMessage Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of ChatMessage
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (ChatMessageDataAccess data = new ChatMessageDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get ChatMessage Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of ChatMessage
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (ChatMessageDataAccess data = new ChatMessageDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of ChatMessage By ChatSessionId        
		/// <param name="_ChatSessionId"></param>
        /// </summary>
        /// <returns>List of ChatMessage</returns>
        public ChatMessageList GetByChatSessionId(Int32 _ChatSessionId)
        {
            using (ChatMessageDataAccess data = new ChatMessageDataAccess(ClientContext))
            {
                return data.GetByChatSessionId(_ChatSessionId);
            }
        }
		
		
		#endregion
	}	
}