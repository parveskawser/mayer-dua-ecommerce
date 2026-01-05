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
	public partial class AuditLogDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTAUDITLOG = "InsertAuditLog";
		private const string UPDATEAUDITLOG = "UpdateAuditLog";
		private const string DELETEAUDITLOG = "DeleteAuditLog";
		private const string GETAUDITLOGBYID = "GetAuditLogById";
		private const string GETALLAUDITLOG = "GetAllAuditLog";
		private const string GETPAGEDAUDITLOG = "GetPagedAuditLog";
		private const string GETAUDITLOGMAXIMUMID = "GetAuditLogMaximumId";
		private const string GETAUDITLOGROWCOUNT = "GetAuditLogRowCount";	
		private const string GETAUDITLOGBYQUERY = "GetAuditLogByQuery";
		#endregion
		
		#region Constructors
		public AuditLogDataAccess(IConfiguration configuration) : base(configuration) { }
		public AuditLogDataAccess(ClientContext context) : base(context) { }
		public AuditLogDataAccess(SqlTransaction transaction) : base(transaction) { }
		public AuditLogDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="auditLogObject"></param>
		private void AddCommonParams(SqlCommand cmd, AuditLogBase auditLogObject)
		{	
			AddParameter(cmd, pNVarChar(AuditLogBase.Property_TableName, 128, auditLogObject.TableName));
			AddParameter(cmd, pNVarChar(AuditLogBase.Property_Operation, 10, auditLogObject.Operation));
			AddParameter(cmd, pInt32(AuditLogBase.Property_PrimaryKeyValue, auditLogObject.PrimaryKeyValue));
			AddParameter(cmd, pNVarChar(AuditLogBase.Property_OldValues, auditLogObject.OldValues));
			AddParameter(cmd, pNVarChar(AuditLogBase.Property_NewValues, auditLogObject.NewValues));
			AddParameter(cmd, pNVarChar(AuditLogBase.Property_ChangedBy, 100, auditLogObject.ChangedBy));
			AddParameter(cmd, pDateTime(AuditLogBase.Property_ChangeDate, auditLogObject.ChangeDate));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts AuditLog
        /// </summary>
        /// <param name="auditLogObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(AuditLogBase auditLogObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTAUDITLOG);
	
				AddParameter(cmd, pInt32Out(AuditLogBase.Property_Id));
				AddCommonParams(cmd, auditLogObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					auditLogObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					auditLogObject.Id = (Int32)GetOutParameter(cmd, AuditLogBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(auditLogObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates AuditLog
        /// </summary>
        /// <param name="auditLogObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(AuditLogBase auditLogObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEAUDITLOG);
				
				AddParameter(cmd, pInt32(AuditLogBase.Property_Id, auditLogObject.Id));
				AddCommonParams(cmd, auditLogObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					auditLogObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(auditLogObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes AuditLog
        /// </summary>
        /// <param name="Id">Id of the AuditLog object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEAUDITLOG);	
				
				AddParameter(cmd, pInt32(AuditLogBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(AuditLog), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves AuditLog object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the AuditLog object to retrieve</param>
        /// <returns>AuditLog object, null if not found</returns>
		public AuditLog Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETAUDITLOGBYID))
			{
				AddParameter( cmd, pInt32(AuditLogBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all AuditLog objects 
        /// </summary>
        /// <returns>A list of AuditLog objects</returns>
		public AuditLogList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLAUDITLOG))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all AuditLog objects by PageRequest
        /// </summary>
        /// <returns>A list of AuditLog objects</returns>
		public AuditLogList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDAUDITLOG))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				AuditLogList _AuditLogList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _AuditLogList;
			}
		}
		
		/// <summary>
        /// Retrieves all AuditLog objects by query String
        /// </summary>
        /// <returns>A list of AuditLog objects</returns>
		public AuditLogList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETAUDITLOGBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get AuditLog Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of AuditLog
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETAUDITLOGMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get AuditLog Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of AuditLog
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _AuditLogRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETAUDITLOGROWCOUNT))
			{
				SqlDataReader reader;
				_AuditLogRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _AuditLogRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills AuditLog object
        /// </summary>
        /// <param name="auditLogObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(AuditLogBase auditLogObject, SqlDataReader reader, int start)
		{
			
				auditLogObject.Id = reader.GetInt32( start + 0 );			
				auditLogObject.TableName = reader.GetString( start + 1 );			
				auditLogObject.Operation = reader.GetString( start + 2 );			
				auditLogObject.PrimaryKeyValue = reader.GetInt32( start + 3 );			
				if(!reader.IsDBNull(4)) auditLogObject.OldValues = reader.GetString( start + 4 );			
				if(!reader.IsDBNull(5)) auditLogObject.NewValues = reader.GetString( start + 5 );			
				if(!reader.IsDBNull(6)) auditLogObject.ChangedBy = reader.GetString( start + 6 );			
				auditLogObject.ChangeDate = reader.GetDateTime( start + 7 );			
			FillBaseObject(auditLogObject, reader, (start + 8));

			
			auditLogObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills AuditLog object
        /// </summary>
        /// <param name="auditLogObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(AuditLogBase auditLogObject, SqlDataReader reader)
		{
			FillObject(auditLogObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves AuditLog object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>AuditLog object</returns>
		private AuditLog GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					AuditLog auditLogObject= new AuditLog();
					FillObject(auditLogObject, reader);
					return auditLogObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of AuditLog objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of AuditLog objects</returns>
		private AuditLogList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//AuditLog list
			AuditLogList list = new AuditLogList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					AuditLog auditLogObject = new AuditLog();
					FillObject(auditLogObject, reader);

					list.Add(auditLogObject);
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