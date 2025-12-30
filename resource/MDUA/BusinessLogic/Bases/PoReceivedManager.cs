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
    /// Business logic processing for PoReceived.
    /// </summary>    
	public partial class PoReceivedManager : BaseManager
	{
	
		#region Constructors
		public PoReceivedManager(ClientContext context) : base(context) { }
		public PoReceivedManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new poReceived.
        /// data manipulation for insertion of PoReceived
        /// </summary>
        /// <param name="poReceivedObject"></param>
        /// <returns></returns>
        private bool Insert(PoReceived poReceivedObject)
        {
            // new poReceived
            using (PoReceivedDataAccess data = new PoReceivedDataAccess(ClientContext))
            {
                // insert to poReceivedObject
                Int32 _Id = data.Insert(poReceivedObject);
                // if successful, process
                if (_Id > 0)
                {
                    poReceivedObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of PoReceived Object.
        /// Data manipulation processing for: new, deleted, updated PoReceived
        /// </summary>
        /// <param name="poReceivedObject"></param>
        /// <returns></returns>
        public bool UpdateBase(PoReceived poReceivedObject)
        {
            // use of switch for different types of DML
            switch (poReceivedObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(poReceivedObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(poReceivedObject.Id);
            }
            // update rows
            using (PoReceivedDataAccess data = new PoReceivedDataAccess(ClientContext))
            {
                return (data.Update(poReceivedObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for PoReceived
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (PoReceivedDataAccess data = new PoReceivedDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve PoReceived data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>PoReceived Object</returns>
        public PoReceived Get(Int32 _Id)
        {
            using (PoReceivedDataAccess data = new PoReceivedDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a PoReceived .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public PoReceived Get(Int32 _Id, bool fillChild)
        {
            PoReceived poReceivedObject;
            poReceivedObject = Get(_Id);
            
            if (poReceivedObject != null && fillChild)
            {
                // populate child data for a poReceivedObject
                FillPoReceivedWithChilds(poReceivedObject, fillChild);
            }

            return poReceivedObject;
        }
		
		/// <summary>
        /// populates a PoReceived with its child entities
        /// </summary>
        /// <param name="poReceived"></param>
		/// <param name="fillChilds"></param>
        private void FillPoReceivedWithChilds(PoReceived poReceivedObject, bool fillChilds)
        {
            // populate child data for a poReceivedObject
            if (poReceivedObject != null)
            {
				// Retrieve PoRequestedIdObject as PoRequested type for the PoReceived using PoRequestedId
				using(PoRequestedManager poRequestedManager = new PoRequestedManager(ClientContext))
				{
					poReceivedObject.PoRequestedIdObject = poRequestedManager.Get(poReceivedObject.PoRequestedId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of PoReceived.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of PoReceived</returns>
        public PoReceivedList GetAll()
        {
            using (PoReceivedDataAccess data = new PoReceivedDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of PoReceived.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of PoReceived</returns>
        public PoReceivedList GetAll(bool fillChild)
        {
			PoReceivedList poReceivedList = new PoReceivedList();
            using (PoReceivedDataAccess data = new PoReceivedDataAccess(ClientContext))
            {
                poReceivedList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (PoReceived poReceivedObject in poReceivedList)
                {
					FillPoReceivedWithChilds(poReceivedObject, fillChild);
				}
			}
			return poReceivedList;
        }
		
		/// <summary>
        /// Retrieve list of PoReceived  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of PoReceived</returns>
        public PoReceivedList GetPaged(PagedRequest request)
        {
            using (PoReceivedDataAccess data = new PoReceivedDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of PoReceived  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of PoReceived</returns>
        public PoReceivedList GetByQuery(String query)
        {
            using (PoReceivedDataAccess data = new PoReceivedDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get PoReceived Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of PoReceived
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (PoReceivedDataAccess data = new PoReceivedDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get PoReceived Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of PoReceived
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (PoReceivedDataAccess data = new PoReceivedDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of PoReceived By PoRequestedId        
		/// <param name="_PoRequestedId"></param>
        /// </summary>
        /// <returns>List of PoReceived</returns>
        public PoReceivedList GetByPoRequestedId(Int32 _PoRequestedId)
        {
            using (PoReceivedDataAccess data = new PoReceivedDataAccess(ClientContext))
            {
                return data.GetByPoRequestedId(_PoRequestedId);
            }
        }
		
		
		#endregion
	}	
}