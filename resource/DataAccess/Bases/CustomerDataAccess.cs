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
	public partial class CustomerDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTCUSTOMER = "InsertCustomer";
		private const string UPDATECUSTOMER = "UpdateCustomer";
		private const string DELETECUSTOMER = "DeleteCustomer";
		private const string GETCUSTOMERBYID = "GetCustomerById";
		private const string GETALLCUSTOMER = "GetAllCustomer";
		private const string GETPAGEDCUSTOMER = "GetPagedCustomer";
		private const string GETCUSTOMERMAXIMUMID = "GetCustomerMaximumId";
		private const string GETCUSTOMERROWCOUNT = "GetCustomerRowCount";	
		private const string GETCUSTOMERBYQUERY = "GetCustomerByQuery";
		#endregion
		
		#region Constructors
		public CustomerDataAccess(IConfiguration configuration) : base(configuration) { }
		public CustomerDataAccess(ClientContext context) : base(context) { }
		public CustomerDataAccess(SqlTransaction transaction) : base(transaction) { }
		public CustomerDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="customerObject"></param>
		private void AddCommonParams(SqlCommand cmd, CustomerBase customerObject)
		{	
			AddParameter(cmd, pNVarChar(CustomerBase.Property_CustomerName, 150, customerObject.CustomerName));
			AddParameter(cmd, pNVarChar(CustomerBase.Property_Email, 255, customerObject.Email));
			AddParameter(cmd, pVarChar(CustomerBase.Property_Phone, 20, customerObject.Phone));
			AddParameter(cmd, pBool(CustomerBase.Property_IsActive, customerObject.IsActive));
			AddParameter(cmd, pDateTime(CustomerBase.Property_DateOfBirth, customerObject.DateOfBirth));
			AddParameter(cmd, pNVarChar(CustomerBase.Property_Gender, 10, customerObject.Gender));
			AddParameter(cmd, pNVarChar(CustomerBase.Property_Notes, 500, customerObject.Notes));
			AddParameter(cmd, pNVarChar(CustomerBase.Property_CreatedBy, 100, customerObject.CreatedBy));
			AddParameter(cmd, pDateTime(CustomerBase.Property_CreatedAt, customerObject.CreatedAt));
			AddParameter(cmd, pNVarChar(CustomerBase.Property_UpdatedBy, 100, customerObject.UpdatedBy));
			AddParameter(cmd, pDateTime(CustomerBase.Property_UpdatedAt, customerObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts Customer
        /// </summary>
        /// <param name="customerObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(CustomerBase customerObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTCUSTOMER);
	
				AddParameter(cmd, pInt32Out(CustomerBase.Property_Id));
				AddCommonParams(cmd, customerObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					customerObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					customerObject.Id = (Int32)GetOutParameter(cmd, CustomerBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(customerObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates Customer
        /// </summary>
        /// <param name="customerObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(CustomerBase customerObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATECUSTOMER);
				
				AddParameter(cmd, pInt32(CustomerBase.Property_Id, customerObject.Id));
				AddCommonParams(cmd, customerObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					customerObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(customerObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes Customer
        /// </summary>
        /// <param name="Id">Id of the Customer object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETECUSTOMER);	
				
				AddParameter(cmd, pInt32(CustomerBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(Customer), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves Customer object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the Customer object to retrieve</param>
        /// <returns>Customer object, null if not found</returns>
		public Customer Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETCUSTOMERBYID))
			{
				AddParameter( cmd, pInt32(CustomerBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all Customer objects 
        /// </summary>
        /// <returns>A list of Customer objects</returns>
		public CustomerList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLCUSTOMER))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all Customer objects by PageRequest
        /// </summary>
        /// <returns>A list of Customer objects</returns>
		public CustomerList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDCUSTOMER))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				CustomerList _CustomerList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _CustomerList;
			}
		}
		
		/// <summary>
        /// Retrieves all Customer objects by query String
        /// </summary>
        /// <returns>A list of Customer objects</returns>
		public CustomerList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETCUSTOMERBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get Customer Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of Customer
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCUSTOMERMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get Customer Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of Customer
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _CustomerRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCUSTOMERROWCOUNT))
			{
				SqlDataReader reader;
				_CustomerRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _CustomerRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills Customer object
        /// </summary>
        /// <param name="customerObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(CustomerBase customerObject, SqlDataReader reader, int start)
		{
			
				customerObject.Id = reader.GetInt32( start + 0 );			
				customerObject.CustomerName = reader.GetString( start + 1 );			
				customerObject.Email = reader.GetString( start + 2 );			
				customerObject.Phone = reader.GetString( start + 3 );			
				customerObject.IsActive = reader.GetBoolean( start + 4 );			
				if(!reader.IsDBNull(5)) customerObject.DateOfBirth = reader.GetDateTime( start + 5 );			
				if(!reader.IsDBNull(6)) customerObject.Gender = reader.GetString( start + 6 );			
				if(!reader.IsDBNull(7)) customerObject.Notes = reader.GetString( start + 7 );			
				customerObject.CreatedBy = reader.GetString( start + 8 );			
				customerObject.CreatedAt = reader.GetDateTime( start + 9 );			
				if(!reader.IsDBNull(10)) customerObject.UpdatedBy = reader.GetString( start + 10 );			
				if(!reader.IsDBNull(11)) customerObject.UpdatedAt = reader.GetDateTime( start + 11 );			
			FillBaseObject(customerObject, reader, (start + 12));

			
			customerObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills Customer object
        /// </summary>
        /// <param name="customerObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(CustomerBase customerObject, SqlDataReader reader)
		{
			FillObject(customerObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves Customer object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>Customer object</returns>
		private Customer GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					Customer customerObject= new Customer();
					FillObject(customerObject, reader);
					return customerObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of Customer objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of Customer objects</returns>
		private CustomerList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//Customer list
			CustomerList list = new CustomerList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					Customer customerObject = new Customer();
					FillObject(customerObject, reader);

					list.Add(customerObject);
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