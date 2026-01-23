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
	public partial class PostalCodesDataAccess : BaseDataAccess, IPostalCodesDataAccess
	{
		#region Constants
		private const string INSERTPOSTALCODES = "InsertPostalCodes";
		private const string UPDATEPOSTALCODES = "UpdatePostalCodes";
		private const string DELETEPOSTALCODES = "DeletePostalCodes";
		private const string GETPOSTALCODESBYID = "GetPostalCodesById";
		private const string GETALLPOSTALCODES = "GetAllPostalCodes";
		private const string GETPAGEDPOSTALCODES = "GetPagedPostalCodes";
		private const string GETPOSTALCODESMAXIMUMID = "GetPostalCodesMaximumId";
		private const string GETPOSTALCODESROWCOUNT = "GetPostalCodesRowCount";	
		private const string GETPOSTALCODESBYQUERY = "GetPostalCodesByQuery";
		#endregion
		
		#region Constructors
		public PostalCodesDataAccess(IConfiguration configuration) : base(configuration) { }
		public PostalCodesDataAccess(ClientContext context) : base(context) { }
		public PostalCodesDataAccess(SqlTransaction transaction) : base(transaction) { }
		public PostalCodesDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="postalCodesObject"></param>
		private void AddCommonParams(SqlCommand cmd, PostalCodesBase postalCodesObject)
		{	
			AddParameter(cmd, pNVarChar(PostalCodesBase.Property_PostCode, 10, postalCodesObject.PostCode));
			AddParameter(cmd, pNVarChar(PostalCodesBase.Property_SubOfficeEn, 100, postalCodesObject.SubOfficeEn));
			AddParameter(cmd, pNVarChar(PostalCodesBase.Property_SubOfficeBn, 100, postalCodesObject.SubOfficeBn));
			AddParameter(cmd, pNVarChar(PostalCodesBase.Property_ThanaEn, 100, postalCodesObject.ThanaEn));
			AddParameter(cmd, pNVarChar(PostalCodesBase.Property_ThanaBn, 100, postalCodesObject.ThanaBn));
			AddParameter(cmd, pNVarChar(PostalCodesBase.Property_DistrictBn, 100, postalCodesObject.DistrictBn));
			AddParameter(cmd, pNVarChar(PostalCodesBase.Property_DistrictEn, 100, postalCodesObject.DistrictEn));
			AddParameter(cmd, pNVarChar(PostalCodesBase.Property_DivisionBn, 100, postalCodesObject.DivisionBn));
			AddParameter(cmd, pNVarChar(PostalCodesBase.Property_DivisionEn, 100, postalCodesObject.DivisionEn));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts PostalCodes
        /// </summary>
        /// <param name="postalCodesObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(PostalCodesBase postalCodesObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTPOSTALCODES);
	
				AddParameter(cmd, pInt32Out(PostalCodesBase.Property_Id));
				AddCommonParams(cmd, postalCodesObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					postalCodesObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					postalCodesObject.Id = (Int32)GetOutParameter(cmd, PostalCodesBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(postalCodesObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates PostalCodes
        /// </summary>
        /// <param name="postalCodesObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(PostalCodesBase postalCodesObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEPOSTALCODES);
				
				AddParameter(cmd, pInt32(PostalCodesBase.Property_Id, postalCodesObject.Id));
				AddCommonParams(cmd, postalCodesObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					postalCodesObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(postalCodesObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes PostalCodes
        /// </summary>
        /// <param name="Id">Id of the PostalCodes object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEPOSTALCODES);	
				
				AddParameter(cmd, pInt32(PostalCodesBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(PostalCodes), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves PostalCodes object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the PostalCodes object to retrieve</param>
        /// <returns>PostalCodes object, null if not found</returns>
		public PostalCodes Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETPOSTALCODESBYID))
			{
				AddParameter( cmd, pInt32(PostalCodesBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all PostalCodes objects 
        /// </summary>
        /// <returns>A list of PostalCodes objects</returns>
		public PostalCodesList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLPOSTALCODES))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all PostalCodes objects by PageRequest
        /// </summary>
        /// <returns>A list of PostalCodes objects</returns>
		public PostalCodesList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDPOSTALCODES))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				PostalCodesList _PostalCodesList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _PostalCodesList;
			}
		}
		
		/// <summary>
        /// Retrieves all PostalCodes objects by query String
        /// </summary>
        /// <returns>A list of PostalCodes objects</returns>
		public PostalCodesList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETPOSTALCODESBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get PostalCodes Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of PostalCodes
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPOSTALCODESMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get PostalCodes Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of PostalCodes
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _PostalCodesRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETPOSTALCODESROWCOUNT))
			{
				SqlDataReader reader;
				_PostalCodesRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _PostalCodesRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills PostalCodes object
        /// </summary>
        /// <param name="postalCodesObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(PostalCodesBase postalCodesObject, SqlDataReader reader, int start)
		{
			
				postalCodesObject.Id = reader.GetInt32( start + 0 );			
				postalCodesObject.PostCode = reader.GetString( start + 1 );			
				postalCodesObject.SubOfficeEn = reader.GetString( start + 2 );			
				postalCodesObject.SubOfficeBn = reader.GetString( start + 3 );			
				postalCodesObject.ThanaEn = reader.GetString( start + 4 );			
				postalCodesObject.ThanaBn = reader.GetString( start + 5 );			
				postalCodesObject.DistrictBn = reader.GetString( start + 6 );			
				postalCodesObject.DistrictEn = reader.GetString( start + 7 );			
				postalCodesObject.DivisionBn = reader.GetString( start + 8 );			
				postalCodesObject.DivisionEn = reader.GetString( start + 9 );			
			FillBaseObject(postalCodesObject, reader, (start + 10));

			
			postalCodesObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills PostalCodes object
        /// </summary>
        /// <param name="postalCodesObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(PostalCodesBase postalCodesObject, SqlDataReader reader)
		{
			FillObject(postalCodesObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves PostalCodes object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>PostalCodes object</returns>
		private PostalCodes GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					PostalCodes postalCodesObject= new PostalCodes();
					FillObject(postalCodesObject, reader);
					return postalCodesObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of PostalCodes objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of PostalCodes objects</returns>
		private PostalCodesList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//PostalCodes list
			PostalCodesList list = new PostalCodesList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					PostalCodes postalCodesObject = new PostalCodes();
					FillObject(postalCodesObject, reader);

					list.Add(postalCodesObject);
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
