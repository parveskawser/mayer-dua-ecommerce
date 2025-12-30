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
    /// Business logic processing for PoRequested.
    /// </summary>    
	public partial class PoRequestedManager
	{
	
		/// <summary>
        /// Update PoRequested Object.
        /// Data manipulation processing for: new, deleted, updated PoRequested
        /// </summary>
        /// <param name="poRequestedObject"></param>
        /// <returns></returns>
        public bool Update(PoRequested poRequestedObject)
        {
			bool success = false;
			
			success = UpdateBase(poRequestedObject);
		
			return success;
        }
		
		/// <summary>
        /// Fill External Childs of PoRequested Object.
        /// </summary>
        /// <param name="poRequestedObject"></param>
        /// <returns></returns>
        public void FillChilds(PoRequested poRequestedObject)
        {
			///Fill external information of Childs of PoRequestedObject
        }
	}	
}