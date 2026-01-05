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
	public partial class VendorDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTVENDOR = "InsertVendor";
		private const string UPDATEVENDOR = "UpdateVendor";
		private const string DELETEVENDOR = "DeleteVendor";
		private const string GETVENDORBYID = "GetVendorById";
		private const string GETALLVENDOR = "GetAllVendor";
		private const string GETPAGEDVENDOR = "GetPagedVendor";
		private const string GETVENDORMAXIMUMID = "GetVendorMaximumId";
		private const string GETVENDORROWCOUNT = "GetVendorRowCount";	
		private const string GETVENDORBYQUERY = "GetVendorByQuery";
		#endregion
		
		#region Constructors
		public VendorDataAccess(IConfiguration configuration) : base(configuration) { }
		public VendorDataAccess(ClientContext context) : base(context) { }
		public VendorDataAccess(SqlTransaction transaction) : base(transaction) { }
		public VendorDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="vendorObject"></param>
		private void AddCommonParams(SqlCommand cmd, VendorBase vendorObject)
		{	
			AddParameter(cmd, pNVarChar(VendorBase.Property_VendorName, 150, vendorObject.VendorName));
			AddParameter(cmd, pNVarChar(VendorBase.Property_Email, 255, vendorObject.Email));
			AddParameter(cmd, pVarChar(VendorBase.Property_Phone, 20, vendorObject.Phone));
			AddParameter(cmd, pNVarChar(VendorBase.Property_CreatedBy, 100, vendorObject.CreatedBy));
			AddParameter(cmd, pDateTime(VendorBase.Property_CreatedAt, vendorObject.CreatedAt));
			AddParameter(cmd, pNVarChar(VendorBase.Property_UpdatedBy, 100, vendorObject.UpdatedBy));
			AddParameter(cmd, pDateTime(VendorBase.Property_UpdatedAt, vendorObject.UpdatedAt));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts Vendor
        /// </summary>
        /// <param name="vendorObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(VendorBase vendorObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTVENDOR);
	
				AddParameter(cmd, pInt32Out(VendorBase.Property_Id));
				AddCommonParams(cmd, vendorObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					vendorObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					vendorObject.Id = (Int32)GetOutParameter(cmd, VendorBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(vendorObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates Vendor
        /// </summary>
        /// <param name="vendorObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(VendorBase vendorObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATEVENDOR);
				
				AddParameter(cmd, pInt32(VendorBase.Property_Id, vendorObject.Id));
				AddCommonParams(cmd, vendorObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					vendorObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(vendorObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes Vendor
        /// </summary>
        /// <param name="Id">Id of the Vendor object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETEVENDOR);	
				
				AddParameter(cmd, pInt32(VendorBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(Vendor), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves Vendor object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the Vendor object to retrieve</param>
        /// <returns>Vendor object, null if not found</returns>
		public Vendor Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETVENDORBYID))
			{
				AddParameter( cmd, pInt32(VendorBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all Vendor objects 
        /// </summary>
        /// <returns>A list of Vendor objects</returns>
		public VendorList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLVENDOR))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all Vendor objects by PageRequest
        /// </summary>
        /// <returns>A list of Vendor objects</returns>
		public VendorList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDVENDOR))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				VendorList _VendorList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _VendorList;
			}
		}
		
		/// <summary>
        /// Retrieves all Vendor objects by query String
        /// </summary>
        /// <returns>A list of Vendor objects</returns>
		public VendorList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETVENDORBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get Vendor Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of Vendor
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETVENDORMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get Vendor Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of Vendor
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _VendorRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETVENDORROWCOUNT))
			{
				SqlDataReader reader;
				_VendorRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _VendorRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills Vendor object
        /// </summary>
        /// <param name="vendorObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(VendorBase vendorObject, SqlDataReader reader, int start)
		{
			
				vendorObject.Id = reader.GetInt32( start + 0 );			
				vendorObject.VendorName = reader.GetString( start + 1 );			
				if(!reader.IsDBNull(2)) vendorObject.Email = reader.GetString( start + 2 );			
				if(!reader.IsDBNull(3)) vendorObject.Phone = reader.GetString( start + 3 );			
				if(!reader.IsDBNull(4)) vendorObject.CreatedBy = reader.GetString( start + 4 );			
				if(!reader.IsDBNull(5)) vendorObject.CreatedAt = reader.GetDateTime( start + 5 );			
				if(!reader.IsDBNull(6)) vendorObject.UpdatedBy = reader.GetString( start + 6 );			
				if(!reader.IsDBNull(7)) vendorObject.UpdatedAt = reader.GetDateTime( start + 7 );			
			FillBaseObject(vendorObject, reader, (start + 8));

			
			vendorObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills Vendor object
        /// </summary>
        /// <param name="vendorObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(VendorBase vendorObject, SqlDataReader reader)
		{
			FillObject(vendorObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves Vendor object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>Vendor object</returns>
		private Vendor GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					Vendor vendorObject= new Vendor();
					FillObject(vendorObject, reader);
					return vendorObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of Vendor objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of Vendor objects</returns>
		private VendorList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//Vendor list
			VendorList list = new VendorList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					Vendor vendorObject = new Vendor();
					FillObject(vendorObject, reader);

					list.Add(vendorObject);
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