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
    /// Business logic processing for Address.
    /// </summary>    
	public partial class AddressManager : BaseManager
	{
	
		#region Constructors
		public AddressManager(ClientContext context) : base(context) { }
		public AddressManager(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
		
		#region Insert Method		
		/// <summary>
        /// Insert new address.
        /// data manipulation for insertion of Address
        /// </summary>
        /// <param name="addressObject"></param>
        /// <returns></returns>
        private bool Insert(Address addressObject)
        {
            // new address
            using (AddressDataAccess data = new AddressDataAccess(ClientContext))
            {
                // insert to addressObject
                Int32 _Id = data.Insert(addressObject);
                // if successful, process
                if (_Id > 0)
                {
                    addressObject.Id = _Id;
                    return true;
                }
                else
                    return false;
            }
        }
		#endregion
		
		#region Update Method
		
		/// <summary>
        /// Update base of Address Object.
        /// Data manipulation processing for: new, deleted, updated Address
        /// </summary>
        /// <param name="addressObject"></param>
        /// <returns></returns>
        public bool UpdateBase(Address addressObject)
        {
            // use of switch for different types of DML
            switch (addressObject.RowState)
            {
                // insert new rows
                case BaseBusinessEntity.RowStateEnum.NewRow:
                    return Insert(addressObject);
                // delete rows
                case BaseBusinessEntity.RowStateEnum.DeletedRow:
                    return Delete(addressObject.Id);
            }
            // update rows
            using (AddressDataAccess data = new AddressDataAccess(ClientContext))
            {
                return (data.Update(addressObject) > 0);
            }
        }
		
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Delete operation for Address
        /// <param name="_Id"></param>
        /// <returns></returns>
        private bool Delete(Int32 _Id)
        {
            using (AddressDataAccess data = new AddressDataAccess(ClientContext))
            {
                // return if code > 0
                return (data.Delete(_Id) > 0);
            }
        }
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieve Address data using unique ID
        /// </summary>
        /// <param name="_Id"></param>
        /// <returns>Address Object</returns>
        public Address Get(Int32 _Id)
        {
            using (AddressDataAccess data = new AddressDataAccess(ClientContext))
            {
                return data.Get(_Id);
            }
        }
		
		
		/// <summary>
        /// Retrieve detail information for a Address .
        /// Detail child data includes:
        /// last updated on:
        /// change description:
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="fillChild"></param>
        /// <returns></returns>
        public Address Get(Int32 _Id, bool fillChild)
        {
            Address addressObject;
            addressObject = Get(_Id);
            
            if (addressObject != null && fillChild)
            {
                // populate child data for a addressObject
                FillAddressWithChilds(addressObject, fillChild);
            }

            return addressObject;
        }
		
		/// <summary>
        /// populates a Address with its child entities
        /// </summary>
        /// <param name="address"></param>
		/// <param name="fillChilds"></param>
        private void FillAddressWithChilds(Address addressObject, bool fillChilds)
        {
            // populate child data for a addressObject
            if (addressObject != null)
            {
				// Retrieve CustomerIdObject as Customer type for the Address using CustomerId
				using(CustomerManager customerManager = new CustomerManager(ClientContext))
				{
					addressObject.CustomerIdObject = customerManager.Get(addressObject.CustomerId, fillChilds);
				}
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieve list of Address.
        /// no parameters required to be passed in.
        /// </summary>
        /// <returns>List of Address</returns>
        public AddressList GetAll()
        {
            using (AddressDataAccess data = new AddressDataAccess(ClientContext))
            {
                return data.GetAll();
            }
        }
		
		/// <summary>
        /// Retrieve list of Address.
        /// </summary>
        /// <param name="fillChild"></param>
        /// <returns>List of Address</returns>
        public AddressList GetAll(bool fillChild)
        {
			AddressList addressList = new AddressList();
            using (AddressDataAccess data = new AddressDataAccess(ClientContext))
            {
                addressList = data.GetAll();
            }
			if (fillChild)
            {
				foreach (Address addressObject in addressList)
                {
					FillAddressWithChilds(addressObject, fillChild);
				}
			}
			return addressList;
        }
		
		/// <summary>
        /// Retrieve list of Address  by PageRequest.
        /// <param name="request"></param>
        /// </summary>
        /// <returns>List of Address</returns>
        public AddressList GetPaged(PagedRequest request)
        {
            using (AddressDataAccess data = new AddressDataAccess(ClientContext))
            {
                return data.GetPaged(request);
            }
        }
		
		/// <summary>
        /// Retrieve list of Address  by query String.
        /// <param name="query"></param>
        /// </summary>
        /// <returns>List of Address</returns>
        public AddressList GetByQuery(String query)
        {
            using (AddressDataAccess data = new AddressDataAccess(ClientContext))
            {
                return data.GetByQuery(query);
            }
        }
		#region Get Address Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of Address
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			using (AddressDataAccess data = new AddressDataAccess(ClientContext))
            {
                return data.GetMaxId();
            }
		}
		
		#endregion
		
		#region Get Address Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of Address
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			using (AddressDataAccess data = new AddressDataAccess(ClientContext))
            {
                return data.GetRowCount();
            }
		}
		
		#endregion
	
		/// <summary>
        /// Retrieve list of Address By CustomerId        
		/// <param name="_CustomerId"></param>
        /// </summary>
        /// <returns>List of Address</returns>
        public AddressList GetByCustomerId(Int32 _CustomerId)
        {
            using (AddressDataAccess data = new AddressDataAccess(ClientContext))
            {
                return data.GetByCustomerId(_CustomerId);
            }
        }
		
		
		#endregion
	}	
}