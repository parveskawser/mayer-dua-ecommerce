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
	public partial class AddressDataAccess : BaseDataAccess, IAddressDataAccess
	{
		#region Constants
		private const string INSERTADDRESS = "InsertAddress";
		private const string UPDATEADDRESS = "UpdateAddress";
		private const string DELETEADDRESS = "DeleteAddress";
		private const string GETADDRESSBYID = "GetAddressById";
		private const string GETALLADDRESS = "GetAllAddress";
		private const string GETPAGEDADDRESS = "GetPagedAddress";
		private const string GETADDRESSBYCUSTOMERID = "GetAddressByCustomerId";
		private const string GETADDRESSMAXIMUMID = "GetAddressMaximumId";
		private const string GETADDRESSROWCOUNT = "GetAddressRowCount";	
		private const string GETADDRESSBYQUERY = "GetAddressByQuery";
		#endregion
		
		#region Constructors
		public AddressDataAccess(IConfiguration configuration) : base(configuration) { }
		public AddressDataAccess(ClientContext context) : base(context) { }
		public AddressDataAccess(SqlTransaction transaction) : base(transaction) { }
		public AddressDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="addressObject"></param>
		private void AddCommonParams(SqlCommand cmd, AddressBase addressObject)
		{	
			AddParameter(cmd, pInt32(AddressBase.Property_CustomerId, addressObject.CustomerId));
			AddParameter(cmd, pNVarChar(AddressBase.Property_Street, 255, addressObject.Street));
			AddParameter(cmd, pNVarChar(AddressBase.Property_City, 100, addressObject.City));
			AddParameter(cmd, pNVarChar(AddressBase.Property_Divison, 100, addressObject.Divison));
			AddParameter(cmd, pVarChar(AddressBase.Property_PostalCode, 20, addressObject.PostalCode));
			AddParameter(cmd, pNChar(AddressBase.Property_ZipCode, 50, addressObject.ZipCode));
			AddParameter(cmd, pNVarChar(AddressBase.Property_Country, 100, addressObject.Country));
			AddParameter(cmd, pNVarChar(AddressBase.Property_AddressType, 50, addressObject.AddressType));
			AddParameter(cmd, pNVarChar(AddressBase.Property_CreatedBy, 100, addressObject.CreatedBy));
			AddParameter(cmd, pDateTime(AddressBase.Property_CreatedAt, addressObject.CreatedAt));
			AddParameter(cmd, pNVarChar(AddressBase.Property_UpdatedBy, 100, addressObject.UpdatedBy));
			AddParameter(cmd, pDateTime(AddressBase.Property_UpdatedAt, addressObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts Address
        /// </summary>
        /// <param name="addressObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(AddressBase addressObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTADDRESS);
	
				AddParameter(cmd, pInt32Out(AddressBase.Property_Id));
				AddCommonParams(cmd, addressObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					addressObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					addressObject.Id = (Int32)GetOutParameter(cmd, AddressBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(addressObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates Address
        /// </summary>
        /// <param name="addressObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(AddressBase addressObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEADDRESS);
				
				AddParameter(cmd, pInt32(AddressBase.Property_Id, addressObject.Id));
				AddCommonParams(cmd, addressObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					addressObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(addressObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes Address
        /// </summary>
        /// <param name="Id">Id of the Address object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEADDRESS);	
				
				AddParameter(cmd, pInt32(AddressBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(Address), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves Address object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the Address object to retrieve</param>
        /// <returns>Address object, null if not found</returns>
		public Address Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETADDRESSBYID))
			{
				AddParameter( cmd, pInt32(AddressBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all Address objects 
        /// </summary>
        /// <returns>A list of Address objects</returns>
		public AddressList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLADDRESS))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all Address objects by CustomerId
        /// </summary>
        /// <returns>A list of Address objects</returns>
		public AddressList GetByCustomerId(Int32 _CustomerId)
		{
			using( SqlCommand cmd = GetSPCommand(GETADDRESSBYCUSTOMERID))
			{
				
				AddParameter( cmd, pInt32(AddressBase.Property_CustomerId, _CustomerId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all Address objects by PageRequest
        /// </summary>
        /// <returns>A list of Address objects</returns>
		public AddressList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDADDRESS))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				AddressList _AddressList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _AddressList;
			}
		}
		
		/// <summary>
        /// Retrieves all Address objects by query String
        /// </summary>
        /// <returns>A list of Address objects</returns>
		public AddressList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETADDRESSBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get Address Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of Address
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETADDRESSMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get Address Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of Address
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _AddressRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETADDRESSROWCOUNT))
			{
				SqlDataReader reader;
				_AddressRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _AddressRowCount;
		}

        #endregion

        #region Fill Methods
        /// <summary>
        /// Fills Address object
        /// </summary>
        /// <param name="addressObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
        protected void FillObject(AddressBase addressObject, SqlDataReader reader, int start)
        {
            // IMPORTANT: This mapping MUST match the order of columns in your latest SQL definition.
            // SQL ORDER: Id, CustomerId, Country, Divison, City, Thana, SubOffice, Street, PostalCode, ZipCode, AddressType...

            addressObject.Id = reader.GetInt32(start + 0); // Id
            addressObject.CustomerId = reader.GetInt32(start + 1); // CustomerId

            // NOTE: Your database schema changed order starting here!

            if (!reader.IsDBNull(start + 2)) addressObject.Country = reader.GetString(start + 2); // Country
            if (!reader.IsDBNull(start + 3)) addressObject.Divison = reader.GetString(start + 3); // Divison
            addressObject.City = reader.GetString(start + 4); // City
            if (!reader.IsDBNull(start + 5)) addressObject.Thana = reader.GetString(start + 5); // Thana (NEW)
            if (!reader.IsDBNull(start + 6)) addressObject.SubOffice = reader.GetString(start + 6); // SubOffice (NEW)
            addressObject.Street = reader.GetString(start + 7); // Street

            // Handle Char Array conversions and strings
            addressObject.PostalCode = reader.GetString(start + 8); // PostalCode (varchar/string)

            // ZipCode (nchar/char[]) is at index 9 now
            // We use GetString(index).ToCharArray() for the C# Entity (assuming it's char[])
            addressObject.ZipCode = reader.GetString(start + 9).ToCharArray();

            if (!reader.IsDBNull(start + 10)) addressObject.AddressType = reader.GetString(start + 10); // AddressType
            addressObject.CreatedBy = reader.GetString(start + 11); // CreatedBy
            addressObject.CreatedAt = reader.GetDateTime(start + 12); // CreatedAt

            if (!reader.IsDBNull(start + 13)) addressObject.UpdatedBy = reader.GetString(start + 13);
            if (!reader.IsDBNull(start + 14)) addressObject.UpdatedAt = reader.GetDateTime(start + 14);

            FillBaseObject(addressObject, reader, (start + 15)); // Adjust start index if needed

            addressObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
        }
        /// <summary>
        /// Fills Address object
        /// </summary>
        /// <param name="addressObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        protected void FillObject(AddressBase addressObject, SqlDataReader reader)
		{
			FillObject(addressObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves Address object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>Address object</returns>
		private Address GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					Address addressObject= new Address();
					FillObject(addressObject, reader);
					return addressObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of Address objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of Address objects</returns>
		private AddressList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//Address list
			AddressList list = new AddressList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					Address addressObject = new Address();
					FillObject(addressObject, reader);

					list.Add(addressObject);
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
