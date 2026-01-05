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
	public partial class LanguageDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTLANGUAGE = "InsertLanguage";
		private const string UPDATELANGUAGE = "UpdateLanguage";
		private const string DELETELANGUAGE = "DeleteLanguage";
		private const string GETLANGUAGEBYID = "GetLanguageById";
		private const string GETALLLANGUAGE = "GetAllLanguage";
		private const string GETPAGEDLANGUAGE = "GetPagedLanguage";
		private const string GETLANGUAGEMAXIMUMID = "GetLanguageMaximumId";
		private const string GETLANGUAGEROWCOUNT = "GetLanguageRowCount";	
		private const string GETLANGUAGEBYQUERY = "GetLanguageByQuery";
		#endregion
		
		#region Constructors
		public LanguageDataAccess(IConfiguration configuration) : base(configuration) { }
		public LanguageDataAccess(ClientContext context) : base(context) { }
		public LanguageDataAccess(SqlTransaction transaction) : base(transaction) { }
		public LanguageDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="languageObject"></param>
		private void AddCommonParams(SqlCommand cmd, LanguageBase languageObject)
		{	
			AddParameter(cmd, pNVarChar(LanguageBase.Property_Name, 100, languageObject.Name));
			AddParameter(cmd, pVarChar(LanguageBase.Property_Code, 10, languageObject.Code));
			AddParameter(cmd, pBool(LanguageBase.Property_IsActive, languageObject.IsActive));
			AddParameter(cmd, pNVarChar(LanguageBase.Property_CreatedBy, 100, languageObject.CreatedBy));
			AddParameter(cmd, pDateTime(LanguageBase.Property_CreatedAt, languageObject.CreatedAt));
			AddParameter(cmd, pNVarChar(LanguageBase.Property_UpdatedBy, 100, languageObject.UpdatedBy));
			AddParameter(cmd, pDateTime(LanguageBase.Property_UpdatedAt, languageObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts Language
        /// </summary>
        /// <param name="languageObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(LanguageBase languageObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTLANGUAGE);
	
				AddParameter(cmd, pInt32Out(LanguageBase.Property_Id));
				AddCommonParams(cmd, languageObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					languageObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					languageObject.Id = (Int32)GetOutParameter(cmd, LanguageBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(languageObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates Language
        /// </summary>
        /// <param name="languageObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(LanguageBase languageObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATELANGUAGE);
				
				AddParameter(cmd, pInt32(LanguageBase.Property_Id, languageObject.Id));
				AddCommonParams(cmd, languageObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					languageObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(languageObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes Language
        /// </summary>
        /// <param name="Id">Id of the Language object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETELANGUAGE);	
				
				AddParameter(cmd, pInt32(LanguageBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(Language), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves Language object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the Language object to retrieve</param>
        /// <returns>Language object, null if not found</returns>
		public Language Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETLANGUAGEBYID))
			{
				AddParameter( cmd, pInt32(LanguageBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all Language objects 
        /// </summary>
        /// <returns>A list of Language objects</returns>
		public LanguageList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLLANGUAGE))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all Language objects by PageRequest
        /// </summary>
        /// <returns>A list of Language objects</returns>
		public LanguageList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDLANGUAGE))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				LanguageList _LanguageList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _LanguageList;
			}
		}
		
		/// <summary>
        /// Retrieves all Language objects by query String
        /// </summary>
        /// <returns>A list of Language objects</returns>
		public LanguageList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETLANGUAGEBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get Language Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of Language
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETLANGUAGEMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get Language Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of Language
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _LanguageRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETLANGUAGEROWCOUNT))
			{
				SqlDataReader reader;
				_LanguageRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _LanguageRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills Language object
        /// </summary>
        /// <param name="languageObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(LanguageBase languageObject, SqlDataReader reader, int start)
		{
			
				languageObject.Id = reader.GetInt32( start + 0 );			
				languageObject.Name = reader.GetString( start + 1 );			
				languageObject.Code = reader.GetString( start + 2 );			
				languageObject.IsActive = reader.GetBoolean( start + 3 );			
				if(!reader.IsDBNull(4)) languageObject.CreatedBy = reader.GetString( start + 4 );			
				languageObject.CreatedAt = reader.GetDateTime( start + 5 );			
				if(!reader.IsDBNull(6)) languageObject.UpdatedBy = reader.GetString( start + 6 );			
				if(!reader.IsDBNull(7)) languageObject.UpdatedAt = reader.GetDateTime( start + 7 );			
			FillBaseObject(languageObject, reader, (start + 8));

			
			languageObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills Language object
        /// </summary>
        /// <param name="languageObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(LanguageBase languageObject, SqlDataReader reader)
		{
			FillObject(languageObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves Language object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>Language object</returns>
		private Language GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					Language languageObject= new Language();
					FillObject(languageObject, reader);
					return languageObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of Language objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of Language objects</returns>
		private LanguageList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//Language list
			LanguageList list = new LanguageList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					Language languageObject = new Language();
					FillObject(languageObject, reader);

					list.Add(languageObject);
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