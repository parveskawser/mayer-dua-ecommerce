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
	public partial class AttributeNameDataAccess : BaseDataAccess, IAttributeNameDataAccess
	{
		#region Constants
		private const string INSERTATTRIBUTENAME = "InsertAttributeName";
		private const string UPDATEATTRIBUTENAME = "UpdateAttributeName";
		private const string DELETEATTRIBUTENAME = "DeleteAttributeName";
		private const string GETATTRIBUTENAMEBYID = "GetAttributeNameById";
		private const string GETALLATTRIBUTENAME = "GetAllAttributeName";
		private const string GETPAGEDATTRIBUTENAME = "GetPagedAttributeName";
		private const string GETATTRIBUTENAMEMAXIMUMID = "GetAttributeNameMaximumId";
		private const string GETATTRIBUTENAMEROWCOUNT = "GetAttributeNameRowCount";	
		private const string GETATTRIBUTENAMEBYQUERY = "GetAttributeNameByQuery";
		#endregion
		
		#region Constructors
		public AttributeNameDataAccess(IConfiguration configuration) : base(configuration) { }
		public AttributeNameDataAccess(ClientContext context) : base(context) { }
		public AttributeNameDataAccess(SqlTransaction transaction) : base(transaction) { }
		public AttributeNameDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="attributeNameObject"></param>
		private void AddCommonParams(SqlCommand cmd, AttributeNameBase attributeNameObject)
		{	
			AddParameter(cmd, pNVarChar(AttributeNameBase.Property_Name, 100, attributeNameObject.Name));
			AddParameter(cmd, pInt32(AttributeNameBase.Property_DisplayOrder, attributeNameObject.DisplayOrder));
			AddParameter(cmd, pBool(AttributeNameBase.Property_IsVariantAffecting, attributeNameObject.IsVariantAffecting));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts AttributeName
        /// </summary>
        /// <param name="attributeNameObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(AttributeNameBase attributeNameObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTATTRIBUTENAME);
	
				AddParameter(cmd, pInt32Out(AttributeNameBase.Property_Id));
				AddCommonParams(cmd, attributeNameObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					attributeNameObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					attributeNameObject.Id = (Int32)GetOutParameter(cmd, AttributeNameBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(attributeNameObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates AttributeName
        /// </summary>
        /// <param name="attributeNameObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(AttributeNameBase attributeNameObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEATTRIBUTENAME);
				
				AddParameter(cmd, pInt32(AttributeNameBase.Property_Id, attributeNameObject.Id));
				AddCommonParams(cmd, attributeNameObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					attributeNameObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(attributeNameObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes AttributeName
        /// </summary>
        /// <param name="Id">Id of the AttributeName object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEATTRIBUTENAME);	
				
				AddParameter(cmd, pInt32(AttributeNameBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(AttributeName), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves AttributeName object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the AttributeName object to retrieve</param>
        /// <returns>AttributeName object, null if not found</returns>
		public AttributeName Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETATTRIBUTENAMEBYID))
			{
				AddParameter( cmd, pInt32(AttributeNameBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all AttributeName objects 
        /// </summary>
        /// <returns>A list of AttributeName objects</returns>
		public AttributeNameList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLATTRIBUTENAME))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all AttributeName objects by PageRequest
        /// </summary>
        /// <returns>A list of AttributeName objects</returns>
		public AttributeNameList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDATTRIBUTENAME))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				AttributeNameList _AttributeNameList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _AttributeNameList;
			}
		}
		
		/// <summary>
        /// Retrieves all AttributeName objects by query String
        /// </summary>
        /// <returns>A list of AttributeName objects</returns>
		public AttributeNameList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETATTRIBUTENAMEBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get AttributeName Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of AttributeName
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETATTRIBUTENAMEMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get AttributeName Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of AttributeName
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _AttributeNameRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETATTRIBUTENAMEROWCOUNT))
			{
				SqlDataReader reader;
				_AttributeNameRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _AttributeNameRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills AttributeName object
        /// </summary>
        /// <param name="attributeNameObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(AttributeNameBase attributeNameObject, SqlDataReader reader, int start)
		{
			
				attributeNameObject.Id = reader.GetInt32( start + 0 );			
				attributeNameObject.Name = reader.GetString( start + 1 );			
				attributeNameObject.DisplayOrder = reader.GetInt32( start + 2 );			
				attributeNameObject.IsVariantAffecting = reader.GetBoolean( start + 3 );			
			FillBaseObject(attributeNameObject, reader, (start + 4));

			
			attributeNameObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills AttributeName object
        /// </summary>
        /// <param name="attributeNameObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(AttributeNameBase attributeNameObject, SqlDataReader reader)
		{
			FillObject(attributeNameObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves AttributeName object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>AttributeName object</returns>
		private AttributeName GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					AttributeName attributeNameObject= new AttributeName();
					FillObject(attributeNameObject, reader);
					return attributeNameObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of AttributeName objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of AttributeName objects</returns>
		private AttributeNameList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//AttributeName list
			AttributeNameList list = new AttributeNameList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					AttributeName attributeNameObject = new AttributeName();
					FillObject(attributeNameObject, reader);

					list.Add(attributeNameObject);
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
