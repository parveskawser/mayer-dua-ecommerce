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
	public partial class CarrierDataAccess : BaseDataAccess
	{
		#region Constants
		private const string INSERTCARRIER = "InsertCarrier";
		private const string UPDATECARRIER = "UpdateCarrier";
		private const string DELETECARRIER = "DeleteCarrier";
		private const string GETCARRIERBYID = "GetCarrierById";
		private const string GETALLCARRIER = "GetAllCarrier";
		private const string GETPAGEDCARRIER = "GetPagedCarrier";
		private const string GETCARRIERMAXIMUMID = "GetCarrierMaximumId";
		private const string GETCARRIERROWCOUNT = "GetCarrierRowCount";	
		private const string GETCARRIERBYQUERY = "GetCarrierByQuery";
		#endregion
		
		#region Constructors
		public CarrierDataAccess(IConfiguration configuration) : base(configuration) { }
		public CarrierDataAccess(ClientContext context) : base(context) { }
		public CarrierDataAccess(SqlTransaction transaction) : base(transaction) { }
		public CarrierDataAccess(SqlTransaction transaction, ClientContext context) : base(transaction, context) { }
        #endregion
				
		#region AddCommonParams Method
        /// <summary>
        /// Add common parameters before calling a procedure
        /// </summary>
        /// <param name="cmd">command object, where parameters will be added</param>
        /// <param name="carrierObject"></param>
		private void AddCommonParams(SqlCommand cmd, CarrierBase carrierObject)
		{	
			AddParameter(cmd, pNVarChar(CarrierBase.Property_CarrierName, 100, carrierObject.CarrierName));
			AddParameter(cmd, pNVarChar(CarrierBase.Property_ApiEndpoint, 500, carrierObject.ApiEndpoint));
			AddParameter(cmd, pBool(CarrierBase.Property_RequiresApi, carrierObject.RequiresApi));
			AddParameter(cmd, pBool(CarrierBase.Property_IsActive, carrierObject.IsActive));
		}
		#endregion
		
		#region Insert Method
		/// <summary>
        /// Inserts Carrier
        /// </summary>
        /// <param name="carrierObject">Object to be inserted</param>
        /// <returns>Number of rows affected</returns>
		public long Insert(CarrierBase carrierObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(INSERTCARRIER);
	
				AddParameter(cmd, pInt32Out(CarrierBase.Property_Id));
				AddCommonParams(cmd, carrierObject);
			
				long result = InsertRecord(cmd);
				if (result > 0)
				{
					carrierObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
					carrierObject.Id = (Int32)GetOutParameter(cmd, CarrierBase.Property_Id);
				}
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectInsertException(carrierObject, x);
			}
		}
		#endregion
		
		#region Update Method
		/// <summary>
        /// Updates Carrier
        /// </summary>
        /// <param name="carrierObject">Object to be updated</param>
        /// <returns>Number of rows affected</returns>
		public long Update(CarrierBase carrierObject)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(UPDATECARRIER);
				
				AddParameter(cmd, pInt32(CarrierBase.Property_Id, carrierObject.Id));
				AddCommonParams(cmd, carrierObject);
	
				long result = UpdateRecord(cmd);
				if (result > 0)
					carrierObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
				return result;
			}
			catch(SqlException x)
			{
				throw new ObjectUpdateException(carrierObject, x);
			}
		}
		#endregion
		
		#region Delete Method
		/// <summary>
        /// Deletes Carrier
        /// </summary>
        /// <param name="Id">Id of the Carrier object that will be deleted</param>
        /// <returns>Number of rows affected</returns>
		public long Delete(Int32 _Id)
		{
			try
			{
				SqlCommand cmd = GetSPCommand(DELETECARRIER);	
				
				AddParameter(cmd, pInt32(CarrierBase.Property_Id, _Id));
				 
				return DeleteRecord(cmd);
			}
			catch(SqlException x)
			{
				throw new ObjectDeleteException(typeof(Carrier), _Id, x);
			}
			
		}
		#endregion
		
		#region Get By Id Method
		/// <summary>
        /// Retrieves Carrier object using it's Id
        /// </summary>
        /// <param name="Id">The Id of the Carrier object to retrieve</param>
        /// <returns>Carrier object, null if not found</returns>
		public Carrier Get(Int32 _Id)
		{
			using( SqlCommand cmd = GetSPCommand(GETCARRIERBYID))
			{
				AddParameter( cmd, pInt32(CarrierBase.Property_Id, _Id));

				return GetObject(cmd);
			}
		}
		#endregion
		
		#region GetAll Method
		/// <summary>
        /// Retrieves all Carrier objects 
        /// </summary>
        /// <returns>A list of Carrier objects</returns>
		public CarrierList GetAll()
		{
			using( SqlCommand cmd = GetSPCommand(GETALLCARRIER))
			{
				return GetList(cmd, ALL_AVAILABLE_RECORDS);
			}
		}
		
		
		/// <summary>
        /// Retrieves all Carrier objects by PageRequest
        /// </summary>
        /// <returns>A list of Carrier objects</returns>
		public CarrierList GetPaged(PagedRequest request)
		{
			using( SqlCommand cmd = GetSPCommand(GETPAGEDCARRIER))
			{
				AddParameter( cmd, pInt32Out("TotalRows") );
			 	AddParameter( cmd, pInt32("PageIndex", request.PageIndex) );
				AddParameter( cmd, pInt32("RowPerPage", request.RowPerPage) );
				AddParameter(cmd, pNVarChar("WhereClause", 4000, request.WhereClause) );
				AddParameter(cmd, pNVarChar("SortColumn", 128, request.SortColumn) );
				AddParameter(cmd, pNVarChar("SortOrder", 4, request.SortOrder) );
				
				CarrierList _CarrierList = GetList(cmd, ALL_AVAILABLE_RECORDS);
				request.TotalRows = Convert.ToInt32(GetOutParameter(cmd, "TotalRows"));
				return _CarrierList;
			}
		}
		
		/// <summary>
        /// Retrieves all Carrier objects by query String
        /// </summary>
        /// <returns>A list of Carrier objects</returns>
		public CarrierList GetByQuery(String query)
		{
			using( SqlCommand cmd = GetSPCommand(GETCARRIERBYQUERY))
			{
				AddParameter(cmd, pNVarChar("Query", 4000, query) );
				return GetList(cmd, ALL_AVAILABLE_RECORDS);;
			}
		}
		
		#endregion
		
		
		#region Get Carrier Maximum Id Method
		/// <summary>
        /// Retrieves Get Maximum Id of Carrier
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetMaxId()
		{
			Int32 _MaximumId = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCARRIERMAXIMUMID))
			{
				SqlDataReader reader;
				_MaximumId = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _MaximumId;
		}
		
		#endregion
		
		#region Get Carrier Row Count Method
		/// <summary>
        /// Retrieves Get Total Rows of Carrier
        /// </summary>
        /// <returns>Int32 type object</returns>
		public Int32 GetRowCount()
		{
			Int32 _CarrierRowCount = 0; 
			using( SqlCommand cmd = GetSPCommand(GETCARRIERROWCOUNT))
			{
				SqlDataReader reader;
				_CarrierRowCount = (Int32) SelectRecords(cmd, out reader);
				reader.Close();
				reader.Dispose();
			}
			return _CarrierRowCount;
		}
		
		#endregion
	
		#region Fill Methods
		/// <summary>
        /// Fills Carrier object
        /// </summary>
        /// <param name="carrierObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
        /// <param name="start">The ordinal position from which to start reading the reader</param>
		protected void FillObject(CarrierBase carrierObject, SqlDataReader reader, int start)
		{
			
				carrierObject.Id = reader.GetInt32( start + 0 );			
				carrierObject.CarrierName = reader.GetString( start + 1 );			
				if(!reader.IsDBNull(2)) carrierObject.ApiEndpoint = reader.GetString( start + 2 );			
				carrierObject.RequiresApi = reader.GetBoolean( start + 3 );			
				carrierObject.IsActive = reader.GetBoolean( start + 4 );			
			FillBaseObject(carrierObject, reader, (start + 5));

			
			carrierObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;	
		}
		
		/// <summary>
        /// Fills Carrier object
        /// </summary>
        /// <param name="carrierObject">The object to be filled</param>
        /// <param name="reader">The reader to use to fill a single object</param>
		protected void FillObject(CarrierBase carrierObject, SqlDataReader reader)
		{
			FillObject(carrierObject, reader, 0);
		}
		
		/// <summary>
        /// Retrieves Carrier object from SqlCommand, after database query
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <returns>Carrier object</returns>
		private Carrier GetObject(SqlCommand cmd)
		{
			SqlDataReader reader;
			long rows = SelectRecords(cmd, out reader);

			using(reader)
			{
				if(reader.Read())
				{
					Carrier carrierObject= new Carrier();
					FillObject(carrierObject, reader);
					return carrierObject;
				}
				else
				{
					return null;
				}				
			}
		}
		
		/// <summary>
        /// Retrieves list of Carrier objects from SqlCommand, after database query
        /// number of rows retrieved and returned depends upon the rows field value
        /// </summary>
        /// <param name="cmd">The command object to use for query</param>
        /// <param name="rows">Number of rows to process</param>
        /// <returns>A list of Carrier objects</returns>
		private CarrierList GetList(SqlCommand cmd, long rows)
		{
			// Select multiple records
			SqlDataReader reader;
			long result = SelectRecords(cmd, out reader);

			//Carrier list
			CarrierList list = new CarrierList();

			using( reader )
			{
				// Read rows until end of result or number of rows specified is reached
				while( reader.Read() && rows-- != 0 )
				{
					Carrier carrierObject = new Carrier();
					FillObject(carrierObject, reader);

					list.Add(carrierObject);
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