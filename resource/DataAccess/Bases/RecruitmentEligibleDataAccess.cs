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
	public partial class RecruitmentEligibleDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTRECRUITMENTELIGIBLE = "InsertRecruitmentEligible";
		private const string UPDATERECRUITMENTELIGIBLE = "UpdateRecruitmentEligible";
		private const string DELETERECRUITMENTELIGIBLE = "DeleteRecruitmentEligible";
		private const string GETRECRUITMENTELIGIBLEBYID = "GetRecruitmentEligibleById";
		private const string GETALLRECRUITMENTELIGIBLE = "GetAllRecruitmentEligible";
		private const string GETPAGEDRECRUITMENTELIGIBLE = "GetPagedRecruitmentEligible";
		private const string GETRECRUITMENTELIGIBLEMAXIMUMID = "GetRecruitmentEligibleMaximumId";
		private const string GETRECRUITMENTELIGIBLEROWCOUNT = "GetRecruitmentEligibleRowCount";	
		private const string GETRECRUITMENTELIGIBLEBYQUERY = "GetRecruitmentEligibleByQuery";
		#endregion
		
		#region Constructors
		public RecruitmentEligibleDataAccess(IConfiguration configuration) : base(configuration) { }
		public RecruitmentEligibleDataAccess(ClientContext context) : base(context) { }
		public RecruitmentEligibleDataAccess(SqlTransaction transaction) : base(transaction) { }
		public RecruitmentEligibleDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="recruitmentEligibleObject"></param>
		private void AddCommonParams(SqlCommand cmd, RecruitmentEligibleBase recruitmentEligibleObject)
		{	
			AddParameter(cmd, pGuid(RecruitmentEligibleBase.Property_CompanyId, recruitmentEligibleObject.CompanyId));
			AddParameter(cmd, pGuid(RecruitmentEligibleBase.Property_RecruitmentEligibleId, recruitmentEligibleObject.RecruitmentEligibleId));
			AddParameter(cmd, pDecimal(RecruitmentEligibleBase.Property_TestResultMark, 9, recruitmentEligibleObject.TestResultMark));
			AddParameter(cmd, pDecimal(RecruitmentEligibleBase.Property_InterViewResultMark, 9, recruitmentEligibleObject.InterViewResultMark));
			AddParameter(cmd, pBool(RecruitmentEligibleBase.Property_Status, recruitmentEligibleObject.Status));
			AddParameter(cmd, pGuid(RecruitmentEligibleBase.Property_CreatedBy, recruitmentEligibleObject.CreatedBy));
			AddParameter(cmd, pDateTime(RecruitmentEligibleBase.Property_CreatedDate, recruitmentEligibleObject.CreatedDate));
			AddParameter(cmd, pGuid(RecruitmentEligibleBase.Property_LastUpdatedBy, recruitmentEligibleObject.LastUpdatedBy));
			AddParameter(cmd, pDateTime(RecruitmentEligibleBase.Property_LastUpdatedDate, recruitmentEligibleObject.LastUpdatedDate));
			AddParameter(cmd, pGuid(RecruitmentEligibleBase.Property_CandidateId, recruitmentEligibleObject.CandidateId));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts RecruitmentEligible
        /// </summary>
        /// <param name="recruitmentEligibleObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(RecruitmentEligibleBase recruitmentEligibleObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTRECRUITMENTELIGIBLE);
	
				AddParameter(cmd, pInt32Out(RecruitmentEligibleBase.Property_Id));
				AddCommonParams(cmd, recruitmentEligibleObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					recruitmentEligibleObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					recruitmentEligibleObject.Id = (Int32)GetOutParameter(cmd, RecruitmentEligibleBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(recruitmentEligibleObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates RecruitmentEligible
        /// </summary>
        /// <param name="recruitmentEligibleObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(RecruitmentEligibleBase recruitmentEligibleObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATERECRUITMENTELIGIBLE);
				
				AddParameter(cmd, pInt32(RecruitmentEligibleBase.Property_Id, recruitmentEligibleObject.Id));
				AddCommonParams(cmd, recruitmentEligibleObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					recruitmentEligibleObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(recruitmentEligibleObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes RecruitmentEligible
        /// </summary>
        /// <param name="Id">Id of the RecruitmentEligible object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETERECRUITMENTELIGIBLE);	
				
				AddParameter(cmd, pInt32(RecruitmentEligibleBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(RecruitmentEligible), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves RecruitmentEligible object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the RecruitmentEligible object to retrieve</param>
        /// <returns>RecruitmentEligible object, null if not found</returns>
		public RecruitmentEligible Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETRECRUITMENTELIGIBLEBYID))
			{
				AddParameter( cmd, pInt32(RecruitmentEligibleBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all RecruitmentEligible objects 
        /// </summary>
        /// <returns>A list of RecruitmentEligible objects</returns>
		public RecruitmentEligibleList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLRECRUITMENTELIGIBLE))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all RecruitmentEligible objects by PageRequest
        /// </summary>
        /// <returns>A list of RecruitmentEligible objects</returns>
		public RecruitmentEligibleList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDRECRUITMENTELIGIBLE))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				RecruitmentEligibleList _RecruitmentEligibleList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _RecruitmentEligibleList;
			}
		}
		
		/// <summary>
        /// Retrieves all RecruitmentEligible objects by query String
        /// </summary>
        /// <returns>A list of RecruitmentEligible objects</returns>
		public RecruitmentEligibleList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETRECRUITMENTELIGIBLEBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get RecruitmentEligible Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of RecruitmentEligible
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETRECRUITMENTELIGIBLEMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get RecruitmentEligible Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of RecruitmentEligible
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _RecruitmentEligibleRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETRECRUITMENTELIGIBLEROWCOUNT))
			{
				SqlDataReader reader;
				_RecruitmentEligibleRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _RecruitmentEligibleRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills RecruitmentEligible object
        /// </summary>
        /// <param name="recruitmentEligibleObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(RecruitmentEligibleBase recruitmentEligibleObject, SqlDataReader reader, int start)
		{
			
				recruitmentEligibleObject.Id = reader.GetInt32( start + 0 );			
				recruitmentEligibleObject.CompanyId = reader.GetGuid( start + 1 );			
				recruitmentEligibleObject.RecruitmentEligibleId = reader.GetGuid( start + 2 );			
				if(!reader.IsDBNull(3)) recruitmentEligibleObject.TestResultMark = reader.GetDecimal( start + 3 );			
				if(!reader.IsDBNull(4)) recruitmentEligibleObject.InterViewResultMark = reader.GetDecimal( start + 4 );			
				if(!reader.IsDBNull(5)) recruitmentEligibleObject.Status = reader.GetBoolean( start + 5 );			
				recruitmentEligibleObject.CreatedBy = reader.GetGuid( start + 6 );			
				if(!reader.IsDBNull(7)) recruitmentEligibleObject.CreatedDate = reader.GetDateTime( start + 7 );			
				recruitmentEligibleObject.LastUpdatedBy = reader.GetGuid( start + 8 );			
				if(!reader.IsDBNull(9)) recruitmentEligibleObject.LastUpdatedDate = reader.GetDateTime( start + 9 );			
				recruitmentEligibleObject.CandidateId = reader.GetGuid( start + 10 );			
			FillBaseObject(recruitmentEligibleObject, reader, (start + 11));

			
			recruitmentEligibleObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills RecruitmentEligible object
        /// </summary>
        /// <param name="recruitmentEligibleObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(RecruitmentEligibleBase recruitmentEligibleObject, SqlDataReader reader)
		{
			FillObject(recruitmentEligibleObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves RecruitmentEligible object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>RecruitmentEligible object</returns>
		private RecruitmentEligible GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					RecruitmentEligible recruitmentEligibleObject= new RecruitmentEligible();
					FillObject(recruitmentEligibleObject, reader);
					return recruitmentEligibleObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of RecruitmentEligible objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of RecruitmentEligible objects</returns>
		private RecruitmentEligibleList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//RecruitmentEligible list
			RecruitmentEligibleList list = new RecruitmentEligibleList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					RecruitmentEligible recruitmentEligibleObject = new RecruitmentEligible();
					FillObject(recruitmentEligibleObject, reader);

					list.Add(recruitmentEligibleObject);
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