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
	public partial class VariantAttributeValueDataAccess : BaseDataAccess, IVariantAttributeValueDataAccess
	{
		#region Constants
		private const string INSERTVARIANTATTRIBUTEVALUE = "InsertVariantAttributeValue";
		private const string UPDATEVARIANTATTRIBUTEVALUE = "UpdateVariantAttributeValue";
		private const string DELETEVARIANTATTRIBUTEVALUE = "DeleteVariantAttributeValue";
		private const string GETVARIANTATTRIBUTEVALUEBYID = "GetVariantAttributeValueById";
		private const string GETALLVARIANTATTRIBUTEVALUE = "GetAllVariantAttributeValue";
		private const string GETPAGEDVARIANTATTRIBUTEVALUE = "GetPagedVariantAttributeValue";
		private const string GETVARIANTATTRIBUTEVALUEBYVARIANTID = "GetVariantAttributeValueByVariantId";
		private const string GETVARIANTATTRIBUTEVALUEBYATTRIBUTEID = "GetVariantAttributeValueByAttributeId";
		private const string GETVARIANTATTRIBUTEVALUEBYATTRIBUTEVALUEID = "GetVariantAttributeValueByAttributeValueId";
		private const string GETVARIANTATTRIBUTEVALUEMAXIMUMID = "GetVariantAttributeValueMaximumId";
		private const string GETVARIANTATTRIBUTEVALUEROWCOUNT = "GetVariantAttributeValueRowCount";	
		private const string GETVARIANTATTRIBUTEVALUEBYQUERY = "GetVariantAttributeValueByQuery";
		#endregion
		
		#region Constructors
		public VariantAttributeValueDataAccess(IConfiguration configuration) : base(configuration) { }
		public VariantAttributeValueDataAccess(ClientContext context) : base(context) { }
		public VariantAttributeValueDataAccess(SqlTransaction transaction) : base(transaction) { }
		public VariantAttributeValueDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="variantAttributeValueObject"></param>
		private void AddCommonParams(SqlCommand cmd, VariantAttributeValueBase variantAttributeValueObject)
		{	
			AddParameter(cmd, pInt32(VariantAttributeValueBase.Property_VariantId, variantAttributeValueObject.VariantId));
			AddParameter(cmd, pInt32(VariantAttributeValueBase.Property_AttributeId, variantAttributeValueObject.AttributeId));
			AddParameter(cmd, pInt32(VariantAttributeValueBase.Property_AttributeValueId, variantAttributeValueObject.AttributeValueId));
			AddParameter(cmd, pInt32(VariantAttributeValueBase.Property_DisplayOrder, variantAttributeValueObject.DisplayOrder));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts VariantAttributeValue
        /// </summary>
        /// <param name="variantAttributeValueObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(VariantAttributeValueBase variantAttributeValueObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTVARIANTATTRIBUTEVALUE);
	
				AddParameter(cmd, pInt32Out(VariantAttributeValueBase.Property_Id));
				AddCommonParams(cmd, variantAttributeValueObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					variantAttributeValueObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					variantAttributeValueObject.Id = (Int32)GetOutParameter(cmd, VariantAttributeValueBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(variantAttributeValueObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates VariantAttributeValue
        /// </summary>
        /// <param name="variantAttributeValueObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(VariantAttributeValueBase variantAttributeValueObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEVARIANTATTRIBUTEVALUE);
				
				AddParameter(cmd, pInt32(VariantAttributeValueBase.Property_Id, variantAttributeValueObject.Id));
				AddCommonParams(cmd, variantAttributeValueObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					variantAttributeValueObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(variantAttributeValueObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes VariantAttributeValue
        /// </summary>
        /// <param name="Id">Id of the VariantAttributeValue object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEVARIANTATTRIBUTEVALUE);	
				
				AddParameter(cmd, pInt32(VariantAttributeValueBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(VariantAttributeValue), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves VariantAttributeValue object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the VariantAttributeValue object to retrieve</param>
        /// <returns>VariantAttributeValue object, null if not found</returns>
		public VariantAttributeValue Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETVARIANTATTRIBUTEVALUEBYID))
			{
				AddParameter( cmd, pInt32(VariantAttributeValueBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all VariantAttributeValue objects 
        /// </summary>
        /// <returns>A list of VariantAttributeValue objects</returns>
		public VariantAttributeValueList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLVARIANTATTRIBUTEVALUE))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all VariantAttributeValue objects by VariantId
        /// </summary>
        /// <returns>A list of VariantAttributeValue objects</returns>
		public VariantAttributeValueList GetByVariantId(Int32 _VariantId)
		{
			using( SqlCommand cmd = GetSPCommand(GETVARIANTATTRIBUTEVALUEBYVARIANTID))
			{
				
				AddParameter( cmd, pInt32(VariantAttributeValueBase.Property_VariantId, _VariantId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all VariantAttributeValue objects by AttributeId
        /// </summary>
        /// <returns>A list of VariantAttributeValue objects</returns>
		public VariantAttributeValueList GetByAttributeId(Int32 _AttributeId)
		{
			using( SqlCommand cmd = GetSPCommand(GETVARIANTATTRIBUTEVALUEBYATTRIBUTEID))
			{
				
				AddParameter( cmd, pInt32(VariantAttributeValueBase.Property_AttributeId, _AttributeId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all VariantAttributeValue objects by AttributeValueId
        /// </summary>
        /// <returns>A list of VariantAttributeValue objects</returns>
		public VariantAttributeValueList GetByAttributeValueId(Int32 _AttributeValueId)
		{
			using( SqlCommand cmd = GetSPCommand(GETVARIANTATTRIBUTEVALUEBYATTRIBUTEVALUEID))
			{
				
				AddParameter( cmd, pInt32(VariantAttributeValueBase.Property_AttributeValueId, _AttributeValueId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all VariantAttributeValue objects by PageRequest
        /// </summary>
        /// <returns>A list of VariantAttributeValue objects</returns>
		public VariantAttributeValueList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDVARIANTATTRIBUTEVALUE))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				VariantAttributeValueList _VariantAttributeValueList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _VariantAttributeValueList;
			}
		}
		
		/// <summary>
        /// Retrieves all VariantAttributeValue objects by query String
        /// </summary>
        /// <returns>A list of VariantAttributeValue objects</returns>
		public VariantAttributeValueList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETVARIANTATTRIBUTEVALUEBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get VariantAttributeValue Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of VariantAttributeValue
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETVARIANTATTRIBUTEVALUEMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get VariantAttributeValue Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of VariantAttributeValue
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _VariantAttributeValueRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETVARIANTATTRIBUTEVALUEROWCOUNT))
			{
				SqlDataReader reader;
				_VariantAttributeValueRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _VariantAttributeValueRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills VariantAttributeValue object
        /// </summary>
        /// <param name="variantAttributeValueObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(VariantAttributeValueBase variantAttributeValueObject, SqlDataReader reader, int start)
		{
			
				variantAttributeValueObject.Id = reader.GetInt32( start + 0 );			
				variantAttributeValueObject.VariantId = reader.GetInt32( start + 1 );			
				variantAttributeValueObject.AttributeId = reader.GetInt32( start + 2 );			
				variantAttributeValueObject.AttributeValueId = reader.GetInt32( start + 3 );			
				variantAttributeValueObject.DisplayOrder = reader.GetInt32( start + 4 );			
			FillBaseObject(variantAttributeValueObject, reader, (start + 5));

			
			variantAttributeValueObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills VariantAttributeValue object
        /// </summary>
        /// <param name="variantAttributeValueObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(VariantAttributeValueBase variantAttributeValueObject, SqlDataReader reader)
		{
			FillObject(variantAttributeValueObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves VariantAttributeValue object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>VariantAttributeValue object</returns>
		private VariantAttributeValue GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					VariantAttributeValue variantAttributeValueObject= new VariantAttributeValue();
					FillObject(variantAttributeValueObject, reader);
					return variantAttributeValueObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of VariantAttributeValue objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of VariantAttributeValue objects</returns>
		private VariantAttributeValueList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//VariantAttributeValue list
			VariantAttributeValueList list = new VariantAttributeValueList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					VariantAttributeValue variantAttributeValueObject = new VariantAttributeValue();
					FillObject(variantAttributeValueObject, reader);

					list.Add(variantAttributeValueObject);
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
