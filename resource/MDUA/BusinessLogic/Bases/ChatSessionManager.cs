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
    /// Business logic processing for ChatSession.
    /// </summary>    
	public partial class ChatSessionManager : BaseManager
	{
	
		#region Constructors
		public ChatSessionManager(ClientContext context) : base(context) { }
		public ChatSessionManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new chatSession.
        /// data manipulation for insertion of ChatSession
        /// </summary>
        /// <param name="chatSessionObject"></param>
        /// <returns></returns>
        private bool Insert(ChatSession chatSessionObject)
        {
            // new chatSession
            using (ChatSessionDataAccess data = new ChatSessionDataAccess(ClientContext))
            {
                // insert to chatSessionObject
                Int32 _Id = data.Insert(chatSessionObject);
                // if successful, process
                if (_Id > 0)
                {
                    chatSessionObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of ChatSession Object.
        /// Data manipulation processing for: new, deleted, updated ChatSession
        /// </summary>
        /// <param name="chatSessionObject"></param>
        /// <returns></returns>
        public bool UpdateBase(ChatSession chatSessionObject)
        {
            // use of switch for different types of DML
            switch (chatSessionObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(chatSessionObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(chatSessionObject.Id);
            }
            // update rows
            using (ChatSessionDataAccess data = new ChatSessionDataAccess(ClientContext))
            {
                return (data.Update(chatSessionObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for ChatSession
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (ChatSessionDataAccess data = new ChatSessionDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve ChatSession data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>ChatSession Object</returns>
        public ChatSession Get(Int32 _Id)
        {
            using (ChatSessionDataAccess data = new ChatSessionDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a ChatSession .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public ChatSession Get(Int32 _Id, bool fillChild)
        {
            ChatSession chatSessionObject;
            chatSessionObject = Get(_Id);
            
            if (chatSessionObject != null && fillChild)
            {
                // populate child data for a chatSessionObject
                FillChatSessionWithChilds(chatSessionObject, fillChild);
            }

            return chatSessionObject;
        }
		
		/// <summary>
        /// populates a ChatSession with its child entities
        /// </summary>
        /// <param name="chatSession"></param>
		/// <param name="fillChilds"></param>
        private void FillChatSessionWithChilds(ChatSession chatSessionObject, bool fillChilds)
        {
            // populate child data for a chatSessionObject
            if (chatSessionObject != null)
            {
				// Retrieve UserLoginIdObject as UserLogin type for the ChatSession using UserLoginId
				using(UserLoginManager userLoginManager = new UserLoginManager(ClientContext))
				{
					chatSessionObject.UserLoginIdObject = userLoginManager.Get(chatSessionObject.UserLoginId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of ChatSession.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of ChatSession</returns>
        public ChatSessionList GetAll()
        {
            using (ChatSessionDataAccess data = new ChatSessionDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of ChatSession.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of ChatSession</returns>
        public ChatSessionList GetAll(bool fillChild)
        {
			ChatSessionList chatSessionList = new ChatSessionList();
            using (ChatSessionDataAccess data = new ChatSessionDataAccess(ClientContext))
            {
                chatSessionList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (ChatSession chatSessionObject in chatSessionList)
                {
					FillChatSessionWithChilds(chatSessionObject, fillChild);
				}
			}
			return chatSessionList;
        }
		
		/// <summary>
        /// Retrieve list of ChatSession  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of ChatSession</returns>
        public ChatSessionList GetPaged(PagedRequest request)
        {
            using (ChatSessionDataAccess data = new ChatSessionDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of ChatSession  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of ChatSession</returns>
        public ChatSessionList GetByQuery(String query)
        {
            using (ChatSessionDataAccess data = new ChatSessionDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get ChatSession Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of ChatSession
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (ChatSessionDataAccess data = new ChatSessionDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get ChatSession Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of ChatSession
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (ChatSessionDataAccess data = new ChatSessionDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of ChatSession By UserLoginId        
		/// <param name="_UserLoginId"></param>
        /// </summary>
        /// <returns>List of ChatSession</returns>
        public ChatSessionList GetByUserLoginId(Nullable<Int32> _UserLoginId)
        {
            using (ChatSessionDataAccess data = new ChatSessionDataAccess(ClientContext))
            {
                return data.GetByUserLoginId(_UserLoginId);
            }
        }
		
		
		#endregion
	}	
}