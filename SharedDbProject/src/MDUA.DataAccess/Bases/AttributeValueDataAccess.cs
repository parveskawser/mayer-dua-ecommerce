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
	public partial class AttributeValueDataAccess : BaseDataAccess, IAttributeValueDataAccess
	{
		#region Constants
		private const string INSERTATTRIBUTEVALUE = "InsertAttributeValue";
		private const string UPDATEATTRIBUTEVALUE = "UpdateAttributeValue";
		private const string DELETEATTRIBUTEVALUE = "DeleteAttributeValue";
		private const string GETATTRIBUTEVALUEBYID = "GetAttributeValueById";
		private const string GETALLATTRIBUTEVALUE = "GetAllAttributeValue";
		private const string GETPAGEDATTRIBUTEVALUE = "GetPagedAttributeValue";
		private const string GETATTRIBUTEVALUEBYATTRIBUTEID = "GetAttributeValueByAttributeId";
		private const string GETATTRIBUTEVALUEMAXIMUMID = "GetAttributeValueMaximumId";
		private const string GETATTRIBUTEVALUEROWCOUNT = "GetAttributeValueRowCount";	
		private const string GETATTRIBUTEVALUEBYQUERY = "GetAttributeValueByQuery";
		#endregion
		
		#region Constructors
		public AttributeValueDataAccess(IConfiguration configuration) : base(configuration) { }
		public AttributeValueDataAccess(ClientContext context) : base(context) { }
		public AttributeValueDataAccess(SqlTransaction transaction) : base(transaction) { }
		public AttributeValueDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="attributeValueObject"></param>
		private void AddCommonParams(SqlCommand cmd, AttributeValueBase attributeValueObject)
		{	
			AddParameter(cmd, pInt32(AttributeValueBase.Property_AttributeId, attributeValueObject.AttributeId));
			AddParameter(cmd, pNVarChar(AttributeValueBase.Property_Value, 100, attributeValueObject.Value));
			AddParameter(cmd, pInt32(AttributeValueBase.Property_DisplayOrder, attributeValueObject.DisplayOrder));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts AttributeValue
        /// </summary>
        /// <param name="attributeValueObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(AttributeValueBase attributeValueObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTATTRIBUTEVALUE);
	
				AddParameter(cmd, pInt32Out(AttributeValueBase.Property_Id));
				AddCommonParams(cmd, attributeValueObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					attributeValueObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					attributeValueObject.Id = (Int32)GetOutParameter(cmd, AttributeValueBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(attributeValueObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates AttributeValue
        /// </summary>
        /// <param name="attributeValueObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(AttributeValueBase attributeValueObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEATTRIBUTEVALUE);
				
				AddParameter(cmd, pInt32(AttributeValueBase.Property_Id, attributeValueObject.Id));
				AddCommonParams(cmd, attributeValueObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					attributeValueObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(attributeValueObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes AttributeValue
        /// </summary>
        /// <param name="Id">Id of the AttributeValue object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEATTRIBUTEVALUE);	
				
				AddParameter(cmd, pInt32(AttributeValueBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(AttributeValue), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves AttributeValue object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the AttributeValue object to retrieve</param>
        /// <returns>AttributeValue object, null if not found</returns>
		public AttributeValue Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETATTRIBUTEVALUEBYID))
			{
				AddParameter( cmd, pInt32(AttributeValueBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all AttributeValue objects 
        /// </summary>
        /// <returns>A list of AttributeValue objects</returns>
		public AttributeValueList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLATTRIBUTEVALUE))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all AttributeValue objects by AttributeId
        /// </summary>
        /// <returns>A list of AttributeValue objects</returns>
		public AttributeValueList GetByAttributeId(Int32 _AttributeId)
		{
			using( SqlCommand cmd = GetSPCommand(GETATTRIBUTEVALUEBYATTRIBUTEID))
			{
				
				AddParameter( cmd, pInt32(AttributeValueBase.Property_AttributeId, _AttributeId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all AttributeValue objects by PageRequest
        /// </summary>
        /// <returns>A list of AttributeValue objects</returns>
		public AttributeValueList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDATTRIBUTEVALUE))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				AttributeValueList _AttributeValueList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _AttributeValueList;
			}
		}
		
		/// <summary>
        /// Retrieves all AttributeValue objects by query String
        /// </summary>
        /// <returns>A list of AttributeValue objects</returns>
		public AttributeValueList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETATTRIBUTEVALUEBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get AttributeValue Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of AttributeValue
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETATTRIBUTEVALUEMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get AttributeValue Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of AttributeValue
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _AttributeValueRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETATTRIBUTEVALUEROWCOUNT))
			{
				SqlDataReader reader;
				_AttributeValueRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _AttributeValueRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills AttributeValue object
        /// </summary>
        /// <param name="attributeValueObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(AttributeValueBase attributeValueObject, SqlDataReader reader, int start)
		{
			
				attributeValueObject.Id = reader.GetInt32( start + 0 );			
				attributeValueObject.AttributeId = reader.GetInt32( start + 1 );			
				attributeValueObject.Value = reader.GetString( start + 2 );			
				attributeValueObject.DisplayOrder = reader.GetInt32( start + 3 );			
			FillBaseObject(attributeValueObject, reader, (start + 4));

			
			attributeValueObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills AttributeValue object
        /// </summary>
        /// <param name="attributeValueObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(AttributeValueBase attributeValueObject, SqlDataReader reader)
		{
			FillObject(attributeValueObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves AttributeValue object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>AttributeValue object</returns>
		private AttributeValue GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					AttributeValue attributeValueObject= new AttributeValue();
					FillObject(attributeValueObject, reader);
					return attributeValueObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of AttributeValue objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of AttributeValue objects</returns>
		private AttributeValueList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//AttributeValue list
			AttributeValueList list = new AttributeValueList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					AttributeValue attributeValueObject = new AttributeValue();
					FillObject(attributeValueObject, reader);

					list.Add(attributeValueObject);
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
