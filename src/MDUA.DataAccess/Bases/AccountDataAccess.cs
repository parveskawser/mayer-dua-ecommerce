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
	public partial class AccountDataAccess : BaseDataAccess, IAccountDataAccess
	{
		#region Constants
		private const string INSERTACCOUNT = "InsertAccount";
		private const string UPDATEACCOUNT = "UpdateAccount";
		private const string DELETEACCOUNT = "DeleteAccount";
		private const string GETACCOUNTBYID = "GetAccountById";
		private const string GETALLACCOUNT = "GetAllAccount";
		private const string GETPAGEDACCOUNT = "GetPagedAccount";
		private const string GETACCOUNTBYACCOUNTTYPEID = "GetAccountByAccountTypeId";
		private const string GETACCOUNTMAXIMUMID = "GetAccountMaximumId";
		private const string GETACCOUNTROWCOUNT = "GetAccountRowCount";	
		private const string GETACCOUNTBYQUERY = "GetAccountByQuery";
		#endregion
		
		#region Constructors
		public AccountDataAccess(IConfiguration configuration) : base(configuration) { }
		public AccountDataAccess(ClientContext context) : base(context) { }
		public AccountDataAccess(SqlTransaction transaction) : base(transaction) { }
		public AccountDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="accountObject"></param>
		private void AddCommonParams(SqlCommand cmd, AccountBase accountObject)
		{	
			AddParameter(cmd, pNVarChar(AccountBase.Property_AccountCode, 20, accountObject.AccountCode));
			AddParameter(cmd, pNVarChar(AccountBase.Property_AccountName, 100, accountObject.AccountName));
			AddParameter(cmd, pInt32(AccountBase.Property_AccountTypeId, accountObject.AccountTypeId));
			AddParameter(cmd, pBool(AccountBase.Property_IsActive, accountObject.IsActive));
			AddParameter(cmd, pNVarChar(AccountBase.Property_Description, 255, accountObject.Description));
			AddParameter(cmd, pDecimal(AccountBase.Property_Balance, 9, accountObject.Balance));
			AddParameter(cmd, pNVarChar(AccountBase.Property_CurrencyCode, 10, accountObject.CurrencyCode));
			AddParameter(cmd, pInt32(AccountBase.Property_ParentAccountId, accountObject.ParentAccountId));
			AddParameter(cmd, pNVarChar(AccountBase.Property_CreatedBy, 100, accountObject.CreatedBy));
			AddParameter(cmd, pDateTime(AccountBase.Property_CreatedAt, accountObject.CreatedAt));
			AddParameter(cmd, pNVarChar(AccountBase.Property_UpdatedBy, 100, accountObject.UpdatedBy));
			AddParameter(cmd, pDateTime(AccountBase.Property_UpdatedAt, accountObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts Account
        /// </summary>
        /// <param name="accountObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(AccountBase accountObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTACCOUNT);
	
				AddParameter(cmd, pInt32Out(AccountBase.Property_Id));
				AddCommonParams(cmd, accountObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					accountObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					accountObject.Id = (Int32)GetOutParameter(cmd, AccountBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(accountObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates Account
        /// </summary>
        /// <param name="accountObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(AccountBase accountObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEACCOUNT);
				
				AddParameter(cmd, pInt32(AccountBase.Property_Id, accountObject.Id));
				AddCommonParams(cmd, accountObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					accountObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(accountObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes Account
        /// </summary>
        /// <param name="Id">Id of the Account object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEACCOUNT);	
				
				AddParameter(cmd, pInt32(AccountBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(Account), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves Account object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the Account object to retrieve</param>
        /// <returns>Account object, null if not found</returns>
		public Account Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETACCOUNTBYID))
			{
				AddParameter( cmd, pInt32(AccountBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all Account objects 
        /// </summary>
        /// <returns>A list of Account objects</returns>
		public AccountList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLACCOUNT))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all Account objects by AccountTypeId
        /// </summary>
        /// <returns>A list of Account objects</returns>
		public AccountList GetByAccountTypeId(Int32 _AccountTypeId)
		{
			using( SqlCommand cmd = GetSPCommand(GETACCOUNTBYACCOUNTTYPEID))
			{
				
				AddParameter( cmd, pInt32(AccountBase.Property_AccountTypeId, _AccountTypeId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all Account objects by PageRequest
        /// </summary>
        /// <returns>A list of Account objects</returns>
		public AccountList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDACCOUNT))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				AccountList _AccountList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _AccountList;
			}
		}
		
		/// <summary>
        /// Retrieves all Account objects by query String
        /// </summary>
        /// <returns>A list of Account objects</returns>
		public AccountList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETACCOUNTBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get Account Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of Account
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETACCOUNTMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get Account Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of Account
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _AccountRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETACCOUNTROWCOUNT))
			{
				SqlDataReader reader;
				_AccountRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _AccountRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills Account object
        /// </summary>
        /// <param name="accountObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(AccountBase accountObject, SqlDataReader reader, int start)
		{
			
				accountObject.Id = reader.GetInt32( start + 0 );			
				accountObject.AccountCode = reader.GetString( start + 1 );			
				accountObject.AccountName = reader.GetString( start + 2 );			
				accountObject.AccountTypeId = reader.GetInt32( start + 3 );			
				accountObject.IsActive = reader.GetBoolean( start + 4 );			
				if(!reader.IsDBNull(5)) accountObject.Description = reader.GetString( start + 5 );			
				if(!reader.IsDBNull(6)) accountObject.Balance = reader.GetDecimal( start + 6 );			
				if(!reader.IsDBNull(7)) accountObject.CurrencyCode = reader.GetString( start + 7 );			
				if(!reader.IsDBNull(8)) accountObject.ParentAccountId = reader.GetInt32( start + 8 );			
				accountObject.CreatedBy = reader.GetString( start + 9 );			
				accountObject.CreatedAt = reader.GetDateTime( start + 10 );			
				if(!reader.IsDBNull(11)) accountObject.UpdatedBy = reader.GetString( start + 11 );			
				if(!reader.IsDBNull(12)) accountObject.UpdatedAt = reader.GetDateTime( start + 12 );			
			FillBaseObject(accountObject, reader, (start + 13));

			
			accountObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills Account object
        /// </summary>
        /// <param name="accountObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(AccountBase accountObject, SqlDataReader reader)
		{
			FillObject(accountObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves Account object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>Account object</returns>
		private Account GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					Account accountObject= new Account();
					FillObject(accountObject, reader);
					return accountObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of Account objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of Account objects</returns>
		private AccountList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//Account list
			AccountList list = new AccountList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					Account accountObject = new Account();
					FillObject(accountObject, reader);

					list.Add(accountObject);
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
