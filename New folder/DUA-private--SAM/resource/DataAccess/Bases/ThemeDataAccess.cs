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
	public partial class ThemeDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTTHEME = "InsertTheme";
		private const string UPDATETHEME = "UpdateTheme";
		private const string DELETETHEME = "DeleteTheme";
		private const string GETTHEMEBYID = "GetThemeById";
		private const string GETALLTHEME = "GetAllTheme";
		private const string GETPAGEDTHEME = "GetPagedTheme";
		private const string GETTHEMEMAXIMUMID = "GetThemeMaximumId";
		private const string GETTHEMEROWCOUNT = "GetThemeRowCount";	
		private const string GETTHEMEBYQUERY = "GetThemeByQuery";
		#endregion
		
		#region Constructors
		public ThemeDataAccess(IConfiguration configuration) : base(configuration) { }
		public ThemeDataAccess(ClientContext context) : base(context) { }
		public ThemeDataAccess(SqlTransaction transaction) : base(transaction) { }
		public ThemeDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="themeObject"></param>
		private void AddCommonParams(SqlCommand cmd, ThemeBase themeObject)
		{	
			AddParameter(cmd, pNVarChar(ThemeBase.Property_ThemeKey, 50, themeObject.ThemeKey));
			AddParameter(cmd, pNVarChar(ThemeBase.Property_DisplayName, 100, themeObject.DisplayName));
			AddParameter(cmd, pNVarChar(ThemeBase.Property_PreviewImageUrl, 255, themeObject.PreviewImageUrl));
			AddParameter(cmd, pBool(ThemeBase.Property_IsActive, themeObject.IsActive));
			AddParameter(cmd, pNVarChar(ThemeBase.Property_CreatedBy, 100, themeObject.CreatedBy));
			AddParameter(cmd, pDateTime(ThemeBase.Property_CreatedAt, themeObject.CreatedAt));
			AddParameter(cmd, pNVarChar(ThemeBase.Property_UpdatedBy, 100, themeObject.UpdatedBy));
			AddParameter(cmd, pDateTime(ThemeBase.Property_UpdatedAt, themeObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts Theme
        /// </summary>
        /// <param name="themeObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(ThemeBase themeObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTTHEME);
	
				AddParameter(cmd, pInt32Out(ThemeBase.Property_Id));
				AddCommonParams(cmd, themeObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					themeObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					themeObject.Id = (Int32)GetOutParameter(cmd, ThemeBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(themeObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates Theme
        /// </summary>
        /// <param name="themeObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(ThemeBase themeObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATETHEME);
				
				AddParameter(cmd, pInt32(ThemeBase.Property_Id, themeObject.Id));
				AddCommonParams(cmd, themeObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					themeObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(themeObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes Theme
        /// </summary>
        /// <param name="Id">Id of the Theme object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETETHEME);	
				
				AddParameter(cmd, pInt32(ThemeBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(Theme), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves Theme object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the Theme object to retrieve</param>
        /// <returns>Theme object, null if not found</returns>
		public Theme Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETTHEMEBYID))
			{
				AddParameter( cmd, pInt32(ThemeBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all Theme objects 
        /// </summary>
        /// <returns>A list of Theme objects</returns>
		public ThemeList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLTHEME))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all Theme objects by PageRequest
        /// </summary>
        /// <returns>A list of Theme objects</returns>
		public ThemeList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDTHEME))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				ThemeList _ThemeList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _ThemeList;
			}
		}
		
		/// <summary>
        /// Retrieves all Theme objects by query String
        /// </summary>
        /// <returns>A list of Theme objects</returns>
		public ThemeList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETTHEMEBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get Theme Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of Theme
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETTHEMEMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get Theme Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of Theme
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _ThemeRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETTHEMEROWCOUNT))
			{
				SqlDataReader reader;
				_ThemeRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _ThemeRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills Theme object
        /// </summary>
        /// <param name="themeObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(ThemeBase themeObject, SqlDataReader reader, int start)
		{
			
				themeObject.Id = reader.GetInt32( start + 0 );			
				themeObject.ThemeKey = reader.GetString( start + 1 );			
				themeObject.DisplayName = reader.GetString( start + 2 );			
				if(!reader.IsDBNull(3)) themeObject.PreviewImageUrl = reader.GetString( start + 3 );			
				themeObject.IsActive = reader.GetBoolean( start + 4 );			
				themeObject.CreatedBy = reader.GetString( start + 5 );			
				themeObject.CreatedAt = reader.GetDateTime( start + 6 );			
				if(!reader.IsDBNull(7)) themeObject.UpdatedBy = reader.GetString( start + 7 );			
				if(!reader.IsDBNull(8)) themeObject.UpdatedAt = reader.GetDateTime( start + 8 );			
			FillBaseObject(themeObject, reader, (start + 9));

			
			themeObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills Theme object
        /// </summary>
        /// <param name="themeObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(ThemeBase themeObject, SqlDataReader reader)
		{
			FillObject(themeObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves Theme object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>Theme object</returns>
		private Theme GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					Theme themeObject= new Theme();
					FillObject(themeObject, reader);
					return themeObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of Theme objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of Theme objects</returns>
		private ThemeList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//Theme list
			ThemeList list = new ThemeList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					Theme themeObject = new Theme();
					FillObject(themeObject, reader);

					list.Add(themeObject);
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