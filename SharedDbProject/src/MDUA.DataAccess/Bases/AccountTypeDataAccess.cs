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
	public partial class AccountTypeDataAccess : BaseDataAccess, IAccountTypeDataAccess
	{
		#region Constants
		private const string INSERTACCOUNTTYPE = "InsertAccountType";
		private const string UPDATEACCOUNTTYPE = "UpdateAccountType";
		private const string DELETEACCOUNTTYPE = "DeleteAccountType";
		private const string GETACCOUNTTYPEBYID = "GetAccountTypeById";
		private const string GETALLACCOUNTTYPE = "GetAllAccountType";
		private const string GETPAGEDACCOUNTTYPE = "GetPagedAccountType";
		private const string GETACCOUNTTYPEMAXIMUMID = "GetAccountTypeMaximumId";
		private const string GETACCOUNTTYPEROWCOUNT = "GetAccountTypeRowCount";	
		private const string GETACCOUNTTYPEBYQUERY = "GetAccountTypeByQuery";
		#endregion
		
		#region Constructors
		public AccountTypeDataAccess(IConfiguration configuration) : base(configuration) { }
		public AccountTypeDataAccess(ClientContext context) : base(context) { }
		public AccountTypeDataAccess(SqlTransaction transaction) : base(transaction) { }
		public AccountTypeDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="accountTypeObject"></param>
		private void AddCommonParams(SqlCommand cmd, AccountTypeBase accountTypeObject)
		{	
			AddParameter(cmd, pNVarChar(AccountTypeBase.Property_Name, 50, accountTypeObject.Name));
			AddParameter(cmd, pNVarChar(AccountTypeBase.Property_Description, 200, accountTypeObject.Description));
			AddParameter(cmd, pNVarChar(AccountTypeBase.Property_CreatedBy, 100, accountTypeObject.CreatedBy));
			AddParameter(cmd, pDateTime(AccountTypeBase.Property_CreatedAt, accountTypeObject.CreatedAt));
			AddParameter(cmd, pNVarChar(AccountTypeBase.Property_UpdatedBy, 100, accountTypeObject.UpdatedBy));
			AddParameter(cmd, pDateTime(AccountTypeBase.Property_UpdatedAt, accountTypeObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts AccountType
        /// </summary>
        /// <param name="accountTypeObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(AccountTypeBase accountTypeObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTACCOUNTTYPE);
	
				AddParameter(cmd, pInt32Out(AccountTypeBase.Property_Id));
				AddCommonParams(cmd, accountTypeObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					accountTypeObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					accountTypeObject.Id = (Int32)GetOutParameter(cmd, AccountTypeBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(accountTypeObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates AccountType
        /// </summary>
        /// <param name="accountTypeObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(AccountTypeBase accountTypeObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEACCOUNTTYPE);
				
				AddParameter(cmd, pInt32(AccountTypeBase.Property_Id, accountTypeObject.Id));
				AddCommonParams(cmd, accountTypeObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					accountTypeObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(accountTypeObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes AccountType
        /// </summary>
        /// <param name="Id">Id of the AccountType object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEACCOUNTTYPE);	
				
				AddParameter(cmd, pInt32(AccountTypeBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(AccountType), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves AccountType object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the AccountType object to retrieve</param>
        /// <returns>AccountType object, null if not found</returns>
		public AccountType Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETACCOUNTTYPEBYID))
			{
				AddParameter( cmd, pInt32(AccountTypeBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all AccountType objects 
        /// </summary>
        /// <returns>A list of AccountType objects</returns>
		public AccountTypeList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLACCOUNTTYPE))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all AccountType objects by PageRequest
        /// </summary>
        /// <returns>A list of AccountType objects</returns>
		public AccountTypeList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDACCOUNTTYPE))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				AccountTypeList _AccountTypeList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _AccountTypeList;
			}
		}
		
		/// <summary>
        /// Retrieves all AccountType objects by query String
        /// </summary>
        /// <returns>A list of AccountType objects</returns>
		public AccountTypeList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETACCOUNTTYPEBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get AccountType Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of AccountType
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETACCOUNTTYPEMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get AccountType Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of AccountType
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _AccountTypeRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETACCOUNTTYPEROWCOUNT))
			{
				SqlDataReader reader;
				_AccountTypeRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _AccountTypeRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills AccountType object
        /// </summary>
        /// <param name="accountTypeObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(AccountTypeBase accountTypeObject, SqlDataReader reader, int start)
		{
			
				accountTypeObject.Id = reader.GetInt32( start + 0 );			
				accountTypeObject.Name = reader.GetString( start + 1 );			
				if(!reader.IsDBNull(2)) accountTypeObject.Description = reader.GetString( start + 2 );			
				accountTypeObject.CreatedBy = reader.GetString( start + 3 );			
				accountTypeObject.CreatedAt = reader.GetDateTime( start + 4 );			
				if(!reader.IsDBNull(5)) accountTypeObject.UpdatedBy = reader.GetString( start + 5 );			
				if(!reader.IsDBNull(6)) accountTypeObject.UpdatedAt = reader.GetDateTime( start + 6 );			
			FillBaseObject(accountTypeObject, reader, (start + 7));

			
			accountTypeObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills AccountType object
        /// </summary>
        /// <param name="accountTypeObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(AccountTypeBase accountTypeObject, SqlDataReader reader)
		{
			FillObject(accountTypeObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves AccountType object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>AccountType object</returns>
		private AccountType GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					AccountType accountTypeObject= new AccountType();
					FillObject(accountTypeObject, reader);
					return accountTypeObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of AccountType objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of AccountType objects</returns>
		private AccountTypeList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//AccountType list
			AccountTypeList list = new AccountTypeList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					AccountType accountTypeObject = new AccountType();
					FillObject(accountTypeObject, reader);

					list.Add(accountTypeObject);
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
