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
	public partial class EmailTemplateDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTEMAILTEMPLATE = "InsertEmailTemplate";
		private const string UPDATEEMAILTEMPLATE = "UpdateEmailTemplate";
		private const string DELETEEMAILTEMPLATE = "DeleteEmailTemplate";
		private const string GETEMAILTEMPLATEBYID = "GetEmailTemplateById";
		private const string GETALLEMAILTEMPLATE = "GetAllEmailTemplate";
		private const string GETPAGEDEMAILTEMPLATE = "GetPagedEmailTemplate";
		private const string GETEMAILTEMPLATEMAXIMUMID = "GetEmailTemplateMaximumId";
		private const string GETEMAILTEMPLATEROWCOUNT = "GetEmailTemplateRowCount";	
		private const string GETEMAILTEMPLATEBYQUERY = "GetEmailTemplateByQuery";
		#endregion
		
		#region Constructors
		public EmailTemplateDataAccess(IConfiguration configuration) : base(configuration) { }
		public EmailTemplateDataAccess(ClientContext context) : base(context) { }
		public EmailTemplateDataAccess(SqlTransaction transaction) : base(transaction) { }
		public EmailTemplateDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="emailTemplateObject"></param>
		private void AddCommonParams(SqlCommand cmd, EmailTemplateBase emailTemplateObject)
		{	
			AddParameter(cmd, pNVarChar(EmailTemplateBase.Property_TemplateKey, 250, emailTemplateObject.TemplateKey));
			AddParameter(cmd, pNVarChar(EmailTemplateBase.Property_Name, 250, emailTemplateObject.Name));
			AddParameter(cmd, pNVarChar(EmailTemplateBase.Property_Description, 2050, emailTemplateObject.Description));
			AddParameter(cmd, pNVarChar(EmailTemplateBase.Property_ToEmail, 250, emailTemplateObject.ToEmail));
			AddParameter(cmd, pNVarChar(EmailTemplateBase.Property_CcEmail, 250, emailTemplateObject.CcEmail));
			AddParameter(cmd, pNVarChar(EmailTemplateBase.Property_BccEmail, 250, emailTemplateObject.BccEmail));
			AddParameter(cmd, pNVarChar(EmailTemplateBase.Property_FromEmail, 250, emailTemplateObject.FromEmail));
			AddParameter(cmd, pNVarChar(EmailTemplateBase.Property_FromName, 250, emailTemplateObject.FromName));
			AddParameter(cmd, pNVarChar(EmailTemplateBase.Property_ReplyEmail, 250, emailTemplateObject.ReplyEmail));
			AddParameter(cmd, pNVarChar(EmailTemplateBase.Property_Subject, 250, emailTemplateObject.Subject));
			AddParameter(cmd, pNVarChar(EmailTemplateBase.Property_BodyContent, emailTemplateObject.BodyContent));
			AddParameter(cmd, pNVarChar(EmailTemplateBase.Property_BodyFile, 250, emailTemplateObject.BodyFile));
			AddParameter(cmd, pBool(EmailTemplateBase.Property_IsActive, emailTemplateObject.IsActive));
			AddParameter(cmd, pNVarChar(EmailTemplateBase.Property_LastUpdatedBy, 250, emailTemplateObject.LastUpdatedBy));
			AddParameter(cmd, pDateTime(EmailTemplateBase.Property_LastUpdatedDate, emailTemplateObject.LastUpdatedDate));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts EmailTemplate
        /// </summary>
        /// <param name="emailTemplateObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(EmailTemplateBase emailTemplateObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTEMAILTEMPLATE);
	
				AddParameter(cmd, pInt32Out(EmailTemplateBase.Property_Id));
				AddCommonParams(cmd, emailTemplateObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					emailTemplateObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					emailTemplateObject.Id = (Int32)GetOutParameter(cmd, EmailTemplateBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(emailTemplateObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates EmailTemplate
        /// </summary>
        /// <param name="emailTemplateObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(EmailTemplateBase emailTemplateObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEEMAILTEMPLATE);
				
				AddParameter(cmd, pInt32(EmailTemplateBase.Property_Id, emailTemplateObject.Id));
				AddCommonParams(cmd, emailTemplateObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					emailTemplateObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(emailTemplateObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes EmailTemplate
        /// </summary>
        /// <param name="Id">Id of the EmailTemplate object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEEMAILTEMPLATE);	
				
				AddParameter(cmd, pInt32(EmailTemplateBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(EmailTemplate), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves EmailTemplate object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the EmailTemplate object to retrieve</param>
        /// <returns>EmailTemplate object, null if not found</returns>
		public EmailTemplate Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETEMAILTEMPLATEBYID))
			{
				AddParameter( cmd, pInt32(EmailTemplateBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all EmailTemplate objects 
        /// </summary>
        /// <returns>A list of EmailTemplate objects</returns>
		public EmailTemplateList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLEMAILTEMPLATE))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all EmailTemplate objects by PageRequest
        /// </summary>
        /// <returns>A list of EmailTemplate objects</returns>
		public EmailTemplateList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDEMAILTEMPLATE))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				EmailTemplateList _EmailTemplateList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _EmailTemplateList;
			}
		}
		
		/// <summary>
        /// Retrieves all EmailTemplate objects by query String
        /// </summary>
        /// <returns>A list of EmailTemplate objects</returns>
		public EmailTemplateList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETEMAILTEMPLATEBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get EmailTemplate Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of EmailTemplate
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETEMAILTEMPLATEMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get EmailTemplate Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of EmailTemplate
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _EmailTemplateRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETEMAILTEMPLATEROWCOUNT))
			{
				SqlDataReader reader;
				_EmailTemplateRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _EmailTemplateRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills EmailTemplate object
        /// </summary>
        /// <param name="emailTemplateObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(EmailTemplateBase emailTemplateObject, SqlDataReader reader, int start)
		{
			
				emailTemplateObject.Id = reader.GetInt32( start + 0 );			
				if(!reader.IsDBNull(1)) emailTemplateObject.TemplateKey = reader.GetString( start + 1 );			
				if(!reader.IsDBNull(2)) emailTemplateObject.Name = reader.GetString( start + 2 );			
				if(!reader.IsDBNull(3)) emailTemplateObject.Description = reader.GetString( start + 3 );			
				if(!reader.IsDBNull(4)) emailTemplateObject.ToEmail = reader.GetString( start + 4 );			
				if(!reader.IsDBNull(5)) emailTemplateObject.CcEmail = reader.GetString( start + 5 );			
				if(!reader.IsDBNull(6)) emailTemplateObject.BccEmail = reader.GetString( start + 6 );			
				if(!reader.IsDBNull(7)) emailTemplateObject.FromEmail = reader.GetString( start + 7 );			
				if(!reader.IsDBNull(8)) emailTemplateObject.FromName = reader.GetString( start + 8 );			
				if(!reader.IsDBNull(9)) emailTemplateObject.ReplyEmail = reader.GetString( start + 9 );			
				if(!reader.IsDBNull(10)) emailTemplateObject.Subject = reader.GetString( start + 10 );			
				if(!reader.IsDBNull(11)) emailTemplateObject.BodyContent = reader.GetString( start + 11 );			
				if(!reader.IsDBNull(12)) emailTemplateObject.BodyFile = reader.GetString( start + 12 );			
				if(!reader.IsDBNull(13)) emailTemplateObject.IsActive = reader.GetBoolean( start + 13 );			
				if(!reader.IsDBNull(14)) emailTemplateObject.LastUpdatedBy = reader.GetString( start + 14 );			
				if(!reader.IsDBNull(15)) emailTemplateObject.LastUpdatedDate = reader.GetDateTime( start + 15 );			
			FillBaseObject(emailTemplateObject, reader, (start + 16));

			
			emailTemplateObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills EmailTemplate object
        /// </summary>
        /// <param name="emailTemplateObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(EmailTemplateBase emailTemplateObject, SqlDataReader reader)
		{
			FillObject(emailTemplateObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves EmailTemplate object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>EmailTemplate object</returns>
		private EmailTemplate GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					EmailTemplate emailTemplateObject= new EmailTemplate();
					FillObject(emailTemplateObject, reader);
					return emailTemplateObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of EmailTemplate objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of EmailTemplate objects</returns>
		private EmailTemplateList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//EmailTemplate list
			EmailTemplateList list = new EmailTemplateList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					EmailTemplate emailTemplateObject = new EmailTemplate();
					FillObject(emailTemplateObject, reader);

					list.Add(emailTemplateObject);
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