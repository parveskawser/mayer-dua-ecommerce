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
	public partial class VariantImageDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTVARIANTIMAGE = "InsertVariantImage";
		private const string UPDATEVARIANTIMAGE = "UpdateVariantImage";
		private const string DELETEVARIANTIMAGE = "DeleteVariantImage";
		private const string GETVARIANTIMAGEBYID = "GetVariantImageById";
		private const string GETALLVARIANTIMAGE = "GetAllVariantImage";
		private const string GETPAGEDVARIANTIMAGE = "GetPagedVariantImage";
		private const string GETVARIANTIMAGEBYVARIANTID = "GetVariantImageByVariantId";
		private const string GETVARIANTIMAGEMAXIMUMID = "GetVariantImageMaximumId";
		private const string GETVARIANTIMAGEROWCOUNT = "GetVariantImageRowCount";	
		private const string GETVARIANTIMAGEBYQUERY = "GetVariantImageByQuery";
		#endregion
		
		#region Constructors
		public VariantImageDataAccess(IConfiguration configuration) : base(configuration) { }
		public VariantImageDataAccess(ClientContext context) : base(context) { }
		public VariantImageDataAccess(SqlTransaction transaction) : base(transaction) { }
		public VariantImageDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="variantImageObject"></param>
		private void AddCommonParams(SqlCommand cmd, VariantImageBase variantImageObject)
		{	
			AddParameter(cmd, pInt32(VariantImageBase.Property_VariantId, variantImageObject.VariantId));
			AddParameter(cmd, pNVarChar(VariantImageBase.Property_ImageUrl, 400, variantImageObject.ImageUrl));
			AddParameter(cmd, pNVarChar(VariantImageBase.Property_AltText, 200, variantImageObject.AltText));
			AddParameter(cmd, pInt32(VariantImageBase.Property_DisplayOrder, variantImageObject.DisplayOrder));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts VariantImage
        /// </summary>
        /// <param name="variantImageObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(VariantImageBase variantImageObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTVARIANTIMAGE);
	
				AddParameter(cmd, pInt32Out(VariantImageBase.Property_Id));
				AddCommonParams(cmd, variantImageObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					variantImageObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					variantImageObject.Id = (Int32)GetOutParameter(cmd, VariantImageBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(variantImageObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates VariantImage
        /// </summary>
        /// <param name="variantImageObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(VariantImageBase variantImageObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEVARIANTIMAGE);
				
				AddParameter(cmd, pInt32(VariantImageBase.Property_Id, variantImageObject.Id));
				AddCommonParams(cmd, variantImageObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					variantImageObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(variantImageObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes VariantImage
        /// </summary>
        /// <param name="Id">Id of the VariantImage object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEVARIANTIMAGE);	
				
				AddParameter(cmd, pInt32(VariantImageBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(VariantImage), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves VariantImage object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the VariantImage object to retrieve</param>
        /// <returns>VariantImage object, null if not found</returns>
		public VariantImage Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETVARIANTIMAGEBYID))
			{
				AddParameter( cmd, pInt32(VariantImageBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all VariantImage objects 
        /// </summary>
        /// <returns>A list of VariantImage objects</returns>
		public VariantImageList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLVARIANTIMAGE))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		/// <summary>
        /// Retrieves all VariantImage objects by VariantId
        /// </summary>
        /// <returns>A list of VariantImage objects</returns>
		public VariantImageList GetByVariantId(Int32 _VariantId)
		{
			using( SqlCommand cmd = GetSPCommand(GETVARIANTIMAGEBYVARIANTID))
			{
				
				AddParameter( cmd, pInt32(VariantImageBase.Property_VariantId, _VariantId));
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all VariantImage objects by PageRequest
        /// </summary>
        /// <returns>A list of VariantImage objects</returns>
		public VariantImageList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDVARIANTIMAGE))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				VariantImageList _VariantImageList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _VariantImageList;
			}
		}
		
		/// <summary>
        /// Retrieves all VariantImage objects by query String
        /// </summary>
        /// <returns>A list of VariantImage objects</returns>
		public VariantImageList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETVARIANTIMAGEBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get VariantImage Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of VariantImage
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETVARIANTIMAGEMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get VariantImage Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of VariantImage
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _VariantImageRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETVARIANTIMAGEROWCOUNT))
			{
				SqlDataReader reader;
				_VariantImageRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _VariantImageRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills VariantImage object
        /// </summary>
        /// <param name="variantImageObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(VariantImageBase variantImageObject, SqlDataReader reader, int start)
		{
			
				variantImageObject.Id = reader.GetInt32( start + 0 );			
				variantImageObject.VariantId = reader.GetInt32( start + 1 );			
				variantImageObject.ImageUrl = reader.GetString( start + 2 );			
				if(!reader.IsDBNull(3)) variantImageObject.AltText = reader.GetString( start + 3 );			
				variantImageObject.DisplayOrder = reader.GetInt32( start + 4 );			
			FillBaseObject(variantImageObject, reader, (start + 5));

			
			variantImageObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills VariantImage object
        /// </summary>
        /// <param name="variantImageObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(VariantImageBase variantImageObject, SqlDataReader reader)
		{
			FillObject(variantImageObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves VariantImage object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>VariantImage object</returns>
		private VariantImage GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					VariantImage variantImageObject= new VariantImage();
					FillObject(variantImageObject, reader);
					return variantImageObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of VariantImage objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of VariantImage objects</returns>
		private VariantImageList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//VariantImage list
			VariantImageList list = new VariantImageList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					VariantImage variantImageObject = new VariantImage();
					FillObject(variantImageObject, reader);

					list.Add(variantImageObject);
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