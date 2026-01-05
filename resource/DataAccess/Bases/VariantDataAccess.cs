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
	public partial class VariantDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTVARIANT = "InsertVariant";
		private const string UPDATEVARIANT = "UpdateVariant";
		private const string DELETEVARIANT = "DeleteVariant";
		private const string GETVARIANTBYID = "GetVariantById";
		private const string GETALLVARIANT = "GetAllVariant";
		private const string GETPAGEDVARIANT = "GetPagedVariant";
		private const string GETVARIANTBYPRODUCTID = "GetVariantByProductId";
		private const string GETVARIANTMAXIMUMID = "GetVariantMaximumId";
		private const string GETVARIANTROWCOUNT = "GetVariantRowCount";	
		private const string GETVARIANTBYQUERY = "GetVariantByQuery";
		#endregion
		
		#region Constructors
		public VariantDataAccess(IConfiguration configuration) : base(configuration) { }
		public VariantDataAccess(ClientContext context) : base(context) { }
		public VariantDataAccess(SqlTransaction transaction) : base(transaction) { }
		public VariantDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="variantObject"></param>
		private void AddCommonParams(SqlCommand cmd, VariantBase variantObject)
		{	
			AddParameter(cmd, pInt32(VariantBase.Property_ProductId, variantObject.ProductId));
			AddParameter(cmd, pNVarChar(VariantBase.Property_Sku, 100, variantObject.Sku));
			AddParameter(cmd, pNVarChar(VariantBase.Property_UpcBarcode, 64, variantObject.UpcBarcode));
			AddParameter(cmd, pNVarChar(VariantBase.Property_VariantKeyHash, 64, variantObject.VariantKeyHash));
			AddParameter(cmd, pBool(VariantBase.Property_IsActive, variantObject.IsActive));
			AddParameter(cmd, pDateTime(VariantBase.Property_CreatedAt, variantObject.CreatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts Variant
        /// </summary>
        /// <param name="variantObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(VariantBase variantObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTVARIANT);
	
				AddParameter(cmd, pInt32Out(VariantBase.Property_Id));
				AddCommonParams(cmd, variantObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					variantObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					variantObject.Id = (Int32)GetOutParameter(cmd, VariantBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(variantObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates Variant
        /// </summary>
        /// <param name="variantObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(VariantBase variantObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEVARIANT);
				
				AddParameter(cmd, pInt32(VariantBase.Property_Id, variantObject.Id));
				AddCommonParams(cmd, variantObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					variantObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(variantObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes Variant
        /// </summary>
        /// <param name="Id">Id of the Variant object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEVARIANT);	
				
				AddParameter(cmd, pInt32(VariantBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(Variant), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves Variant object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the Variant object to retrieve</param>
        /// <returns>Variant object, null if not found</returns>
		public Variant Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETVARIANTBYID))
			{
				AddParameter( cmd, pInt32(VariantBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all Variant objects 
        /// </summary>
        /// <returns>A list of Variant objects</returns>
		public VariantList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLVARIANT))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all Variant objects by ProductId
        /// </summary>
        /// <returns>A list of Variant objects</returns>
		public VariantList GetByProductId(Int32 _ProductId)
		{
			using( SqlCommand cmd = GetSPCommand(GETVARIANTBYPRODUCTID))
			{
				
				AddParameter( cmd, pInt32(VariantBase.Property_ProductId, _ProductId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all Variant objects by PageRequest
        /// </summary>
        /// <returns>A list of Variant objects</returns>
		public VariantList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDVARIANT))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				VariantList _VariantList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _VariantList;
			}
		}
		
		/// <summary>
        /// Retrieves all Variant objects by query String
        /// </summary>
        /// <returns>A list of Variant objects</returns>
		public VariantList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETVARIANTBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get Variant Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of Variant
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETVARIANTMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get Variant Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of Variant
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _VariantRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETVARIANTROWCOUNT))
			{
				SqlDataReader reader;
				_VariantRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _VariantRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills Variant object
        /// </summary>
        /// <param name="variantObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(VariantBase variantObject, SqlDataReader reader, int start)
		{
			
				variantObject.Id = reader.GetInt32( start + 0 );			
				variantObject.ProductId = reader.GetInt32( start + 1 );			
				if(!reader.IsDBNull(2)) variantObject.Sku = reader.GetString( start + 2 );			
				if(!reader.IsDBNull(3)) variantObject.UpcBarcode = reader.GetString( start + 3 );			
				variantObject.VariantKeyHash = reader.GetString( start + 4 );			
				variantObject.IsActive = reader.GetBoolean( start + 5 );			
				variantObject.CreatedAt = reader.GetDateTime( start + 6 );			
			FillBaseObject(variantObject, reader, (start + 7));

			
			variantObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills Variant object
        /// </summary>
        /// <param name="variantObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(VariantBase variantObject, SqlDataReader reader)
		{
			FillObject(variantObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves Variant object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>Variant object</returns>
		private Variant GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					Variant variantObject= new Variant();
					FillObject(variantObject, reader);
					return variantObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of Variant objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of Variant objects</returns>
		private VariantList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//Variant list
			VariantList list = new VariantList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					Variant variantObject = new Variant();
					FillObject(variantObject, reader);

					list.Add(variantObject);
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