using Microsoft.Extensions.Configuration;
using MDUA.Framework.Exceptions;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MDUA.Framework.DataAccess
{
    /// <summary>
    /// Basic data access block for Sql Server. The class exposes basic method to connect to database
    /// and fetch and store data.
    /// </summary>
    public abstract class BaseDataAccess : IDisposable
    {
        //private readonly string KEY_SUFFIX_VIEW = "VIEW";
        //private readonly string KEY_SUFFIX_INSERT = "INSERT";
        //private readonly string KEY_SUFFIX_UPDATE = "UPDATE";
        //private readonly string KEY_SUFFIX_DELETE = "DELETE";

        // private string ConnectionString = ConfigurationBlock.MasterConnectionString;
        private SqlConnection _DBConnection;
        private SqlTransaction _Transaction;
        private const string ReturnValue = "@Return_Value";
        protected const long ALL_AVAILABLE_RECORDS = -1;
        public BaseDataAccess(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            _DBConnection = new SqlConnection(connectionString);
        }
        public BaseDataAccess(ClientContext context)
        {
            _DBConnection = new SqlConnection(ConfigurationBlock.MasterConnectionString);
        }
        public BaseDataAccess(SqlTransaction transaction)
        {
            if (transaction != null)
            {
                _Transaction = transaction;
                if (transaction.Connection != null)
                    _DBConnection = transaction.Connection;
            }
        }
        public BaseDataAccess(SqlTransaction transaction, ClientContext context)
        {
            if (transaction != null)
            {
                _Transaction = transaction;
                if (transaction.Connection != null)
                    _DBConnection = transaction.Connection;
            }
        }

        /// <summary>
        /// Adds parameters which are common both for insert and update for the sql command object
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="baseObject"></param>
        private void AddBaseParametersForCommonOperation(SqlCommand cmd, BaseBusinessEntity baseObject)
        {
            //baseObject.ModifierID = (UserContext != null) ? UserContext.UserID : 1;
            //baseObject.ModifiedTimeStamp = DateTime.Now;
            //AddParameter(cmd, pInt64(BaseBusinessEntity.Property_ModifierID, baseObject.ModifierID));
            //AddParameter(cmd, pDateTime(BaseBusinessEntity.Property_ModifiedTimeStamp, baseObject.ModifiedTimeStamp));
            //AddParameter(cmd, pBool(BaseBusinessEntity.Property_IsDeleted, baseObject.IsDeleted));
        }

        /// <summary>
        /// Adds parameters custom for insert.  During insert we need not have the modifier id and timestamp
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="baseObject"></param>
        protected void AddBaseParametersForInsert(SqlCommand cmd, BaseBusinessEntity baseObject)
        {
            //baseObject.CreatorID = (UserContext != null) ? UserContext.UserID : 1;
            //baseObject.CreatedTimeStamp = DateTime.Now;
            //AddParameter(cmd, pInt64(BaseBusinessEntity.Property_CreatorID, baseObject.CreatorID));
            //AddParameter(cmd, pDateTime(BaseBusinessEntity.Property_CreatedTimeStamp, baseObject.CreatedTimeStamp));
            AddBaseParametersForCommonOperation(cmd, baseObject);
        }

        /// <summary>
        /// Adds parameters for update.  During update we need to update the modifier id and timestamp but
        /// keep the creator as earlier.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="baseObject"></param>
        protected void AddBaseParametersForUpdate(SqlCommand cmd, BaseBusinessEntity baseObject)
        {
            //baseObject.CreatorID = (UserContext != null) ? UserContext.UserID : 1;
            //if (baseObject.CreatedTimeStamp == DateTime.MinValue)
            //    baseObject.CreatedTimeStamp = DateTime.Now;
            //AddParameter(cmd, pInt64(BaseBusinessEntity.Property_CreatorID, baseObject.CreatorID));
            //AddParameter(cmd, pDateTime(BaseBusinessEntity.Property_CreatedTimeStamp, baseObject.CreatedTimeStamp));
            AddBaseParametersForCommonOperation(cmd, baseObject);
        }

        /// <summary>
        /// Fills the base object with base datas that are defined in the framework
        /// </summary>
        /// <param name="baseObject"></param>
        /// <param name="reader"></param>
        /// <param name="start"></param>
        protected void FillBaseObject(BaseBusinessEntity baseObject, SqlDataReader reader, int start)
        {
            //baseObject.CreatorID = reader.GetInt64(start + 0);
            //baseObject.CreatedTimeStamp = reader.GetDateTime(start + 1);
            //if (!reader.IsDBNull(start + 2))
            //    baseObject.ModifierID = reader.GetInt64(start + 2);
            //if (!reader.IsDBNull(start + 3))
            //    baseObject.ModifiedTimeStamp = reader.GetDateTime(start + 3);
            //if (reader.GetBoolean(start + 4))
            //{
            //    baseObject.RowState = BaseBusinessEntity.RowStateEnum.DeletedRow;
            //}
            //if (start + 5 < reader.FieldCount)
            //{
            //    baseObject.CustomProperties = new CustomProperties();
            //}
            //for (int i = start + 5; i < reader.FieldCount; i++)
            //    baseObject.CustomProperties[reader.GetName(i)] = reader.GetValue(i);
            if (start < reader.FieldCount)
            {
                baseObject.CustomProperties = new CustomProperties();
            }
            for (int i = start; i < reader.FieldCount; i++)
                baseObject.CustomProperties[reader.GetName(i)] = reader.GetValue(i);

            baseObject.RowState = BaseBusinessEntity.RowStateEnum.NormalRow;
        }

        /// <summary>
        /// gets Database Connection Object
        /// </summary>
        protected SqlConnection DBConnection
        {
            get
            {
                if (_DBConnection == null)
                {
                    _DBConnection = new SqlConnection(ConfigurationBlock.MasterConnectionString);
                }
                return _DBConnection;
            }
        }

        /// <summary>
        /// Start a transaction
        /// </summary>
        /// <param name="iso"></param>
        /// <returns></returns>
        public static SqlTransaction BeginTransaction(string connectionString, IsolationLevel iso)
        {
            // CHANGE: Use the passed connection string instead of the static ConfigurationBlock
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                conn.Open();
            }
            catch (SqlException ex)
            {
                throw new DatabaseConnectionException(ex);
            }

            return conn.BeginTransaction(iso);
        }
        /// <summary>
        /// Start a transaction with default Isolation Level
        /// </summary>
        /// <returns></returns>
        public static SqlTransaction BeginTransaction(string connectionString)
        {
            // CHANGE: Pass the connection string along
            return BeginTransaction(connectionString, System.Data.IsolationLevel.ReadUncommitted);
        }
        /// <summary>
        /// Ends a transaction either commits or rollsback depending upon the success parameter
        /// </summary>
        /// <param name="success"></param>
        /// <param name="transaction"></param>
        public static void CloseTransaction(bool success, SqlTransaction transaction)
        {
            if (transaction != null)
            {
                //if all the transactions gone well, only then success is passed as true, so should commit
                if (success)
                    transaction.Commit();
                else
                    transaction.Rollback();
                //make sure that connection is closed, resource management, you know..
                if (transaction.Connection != null)
                {
                    transaction.Connection.Close();
                    transaction.Connection.Dispose();
                }
                transaction.Dispose();
            }
        }

        /// <summary>
        /// method OpenConnection
        /// opens database connection
        /// </summary>
        protected void OpenConnection()
        {
            if (DBConnection.State != ConnectionState.Open)
            {
                try
                {
                    DBConnection.Open();
                }
                catch (SqlException ex)
                {

                    new DatabaseConnectionException(ex);
                }
            }
        }

        /// <summary>
        /// method CloseConnection
        /// used to close a database connection
        /// </summary>
        protected void CloseConnection()
        {
            //trying to close the connection, but need to know whether connection exists at all or not
            if (DBConnection != null)
                if (DBConnection.State != ConnectionState.Closed)//yes, connection exists. but, was it already closed?
                    if (_Transaction == null)//connection is not closed, but if the transaction is null, connection need not closed
                        DBConnection.Close();
        }

        /// <summary>
        /// Wrapper method to execute a query command and return a field value
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected object SelectScaler(SqlCommand command)
        {
            OpenConnection();
            return command.ExecuteScalar();
        }

        /// <summary>
        /// Wrapper method to execute a query command and returns the row count and sets the
        /// output parameter reader with mutliple rows
        /// </summary>
        /// <param name="command"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected Int64 SelectRecords(SqlCommand command, out SqlDataReader reader)
        {
            AddReturnParameter(command);
            reader = GetDataReader(command);
            return GetReturnParameter(command);
        }

        /// <summary>
        /// Wrapper method to execute a query command and returns the row count and sets the
        /// output parameter reader with single row
        /// </summary>
        /// <param name="command"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected Int64 SelectRecord(SqlCommand command, out SqlDataReader reader)
        {
            AddReturnParameter(command);
            reader = GetSingleRow(command);
            return GetReturnParameter(command);
        }

        /// <summary>
        /// gets the SqlDataReader for the provided SqlCommand
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected SqlDataReader GetDataReader(SqlCommand command)
        {
            OpenConnection();

            if (_Transaction == null)


                return command.ExecuteReader(CommandBehavior.CloseConnection);
            return command.ExecuteReader();
        }

        /// <summary>
        /// gets a single row from database
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected SqlDataReader GetSingleRow(SqlCommand command)
        {
            OpenConnection();

            if (_Transaction == null)
                return command.ExecuteReader(CommandBehavior.SingleRow | CommandBehavior.CloseConnection);

            return command.ExecuteReader(CommandBehavior.SingleRow);
        }

        /// <summary>
        /// Wrapper method to execute an insert operation
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected Int64 InsertRecord(SqlCommand command)
        {
            AddReturnParameter(command);
            ExecuteCommand(command);
            return GetReturnParameter(command);
        }

        /// <summary>
        /// Wrapper method to execute an update operation
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected Int64 UpdateRecordAndReturn(SqlCommand command)
        {
            AddReturnParameter(command);
            return ExecuteCommand(command);
        }
        /// <summary>
        /// Wrapper method to execute an update operation
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected Int64 UpdateRecord(SqlCommand command)
        {
            AddReturnParameter(command);
            ExecuteCommand(command);
            return GetReturnParameter(command);
        }

        /// <summary>
        /// Wrapper method to execute a delete operation
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns> 
        protected Int64 DeleteRecord(SqlCommand command)
        {
            AddReturnParameter(command);
            ExecuteCommand(command);
            return GetReturnParameter(command);
        }

        /// <summary>
        /// Executes a command and returns the value returned by the ExecuteNonQuery method
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected Int64 ExecuteCommand(SqlCommand command)
        {
            OpenConnection();
            int ReturnValue = -1;

            ReturnValue = command.ExecuteNonQuery();

            CloseConnection();

            return ReturnValue;
        }

        /// <summary>
        /// Adds return parameter to a command object
        /// </summary>
        /// <param name="command"></param>
        protected void AddReturnParameter(SqlCommand command)
        {
            SqlParameter param = new SqlParameter(ReturnValue, SqlDbType.BigInt);
            param.Direction = ParameterDirection.ReturnValue;
            command.Parameters.Add(param);
        }

        /// <summary>
        /// Retrieves the return parameter of the command object passed.
        /// This expects the return parameter to be of type Int64
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected Int64 GetReturnParameter(SqlCommand command)
        {
            if (command.Parameters[ReturnValue] != null)
            {
                if (command.Parameters[ReturnValue].Value != null)
                    return Convert.ToInt64(command.Parameters[ReturnValue].Value);
                else
                    return 0;
            }
            else
                return 0;
        }

        /// <summary>
        /// Retrieves the value of an output parameter in the command object
        /// </summary>
        /// <param name="command"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        protected object GetOutParameter(SqlCommand command, string paramName)
        {
            SqlParameter idParameter = command.Parameters["@" + paramName];
            if (idParameter != null)
                return (object)idParameter.Value;
            else
                return 0;
        }

        /// <summary>
        /// Fills a dataset using a given command and names the datatable inside the set as default
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns> 
        protected DataSet GetDataSet(SqlCommand command)
        {
            return GetDataSet(command, "Default");
        }

        /// <summary>
        /// Fills a dataset using a given command and names the datatable inside the set
        /// with the parameter passed as tablename
        /// </summary>
        /// <param name="command"></param>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public DataSet GetDataSet(SqlCommand command, string tablename)
        {
            DataSet dataset = new DataSet();
            dataset.Tables.Add(new DataTable(tablename));

            SqlDataAdapter dataadapter = new SqlDataAdapter(command);
            dataadapter.Fill(dataset, tablename);
            return dataset;
        }

        /// <summary>
        /// Prepares and returns an sql command object to access based on a SP
        /// </summary>
        /// <param name="StoredProcedureName"></param>
        /// <returns></returns>

        public SqlCommand GetSPCommand(string StoredProcedureName, bool resetdabase = false)
        {
            if (resetdabase)
            {
                _DBConnection = null;
            }
            SqlCommand command;
            if (_Transaction == null)
                command = new SqlCommand(StoredProcedureName, DBConnection);
            else
                command = new SqlCommand(StoredProcedureName, _Transaction.Connection, _Transaction);

            command.CommandType = CommandType.StoredProcedure;

            return command;
        }

        /// <summary>
        /// Prepares and returns an sql command object to access based on a sql query
        /// </summary>
        /// <param name="QueryString"></param>
        /// <returns></returns>
        protected SqlCommand GetSQLCommand(string QueryString)
        {
            SqlCommand command;
            if (_Transaction == null)
                command = new SqlCommand(QueryString, DBConnection);
            else
                command = new SqlCommand(QueryString, _Transaction.Connection, _Transaction);

            command.CommandType = CommandType.Text;

            return command;
        }

        /// <summary>
        /// Prepares and returns an sql command object to access based on a table name
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        protected SqlCommand GetTableCommand(string TableName)
        {
            SqlCommand command;
            if (_Transaction == null)
                command = new SqlCommand(TableName, DBConnection);
            else
                command = new SqlCommand(TableName, _Transaction.Connection, _Transaction);

            command.CommandType = CommandType.TableDirect;

            return command;
        }

        /// <summary>
        /// Method to add SqlParameter to SqlCommand Object
        /// </summary>
        /// <param name="command"></param>
        /// <param name="param"></param>
        public void AddParameter(SqlCommand command, SqlParameter param)
        {
            if (param != null)
                command.Parameters.Add(param);
        }

        /// <summary>
        /// Method to add multiple SqlParameters to SqlCommand Object
        /// </summary>
        /// <param name="command"></param>
        /// <param name="Parameters"></param>

        public void AddParameters(SqlCommand command, params SqlParameter[] Parameters)
        {
            foreach (SqlParameter param in Parameters)
            {
                AddParameter(command, param);
            }
        }

        #region Integer Type Parameters

        /// <summary>
        /// Int Type SqlParameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected SqlParameter pInt32(string paramName, int value)
        {
            return pInt32(paramName, value, ParameterDirection.Input);
        }

        /// <summary>
        /// Int Type SqlParameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected SqlParameter pInt32(string paramName, int? value)
        {
            return pInt32(paramName, value, ParameterDirection.Input);
        }
        protected SqlParameter pDouble(string paramName, double? value)
        {
            return pDouble(paramName, value, ParameterDirection.Input);
        }

        /// <summary>
        /// Int Type Output SqlParameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        protected SqlParameter pInt32Out(string paramName)
        {
            return pInt32(paramName, Int32.MinValue, ParameterDirection.Output);
        }

        /// <summary>
        /// Int Type SqlParamter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pInt32(string paramName, int? value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.Int);
            if (!value.HasValue)
                param.Value = DBNull.Value;
            else
                param.Value = value;
            param.Direction = direction;
            return param;
        }
        protected SqlParameter pDouble(string paramName, double? value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.Float);
            if (!value.HasValue)
                param.Value = DBNull.Value;
            else
                param.Value = value;
            param.Direction = direction;
            return param;
        }
        /// <summary>
        /// Big Integer type parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public SqlParameter pInt64(string paramName, Int64? value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.BigInt);
            if (!value.HasValue)
                param.Value = DBNull.Value;
            else
                param.Value = value;
            param.Direction = direction;
            return param;
        }

        /// <summary>
        /// Big integer type parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlParameter pInt64(string paramName, Int64? value)
        {
            return pInt64(paramName, value, ParameterDirection.Input);
        }

        /// <summary>
        /// out parameter of Big integer
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        protected SqlParameter pInt64Out(string paramName)
        {
            return pInt64(paramName, int.MinValue, ParameterDirection.Output);
        }

        /// <summary>
        /// out parameter of type Big integer
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected SqlParameter pInt64Out(string paramName, Int64? value)
        {
            return pInt64(paramName, value, ParameterDirection.Output);
        }

        /// <summary>
        /// short int type parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public SqlParameter pInt16(string paramName, Int16 value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.TinyInt);
            param.Value = value;
            param.Direction = direction;
            return param;
        }

        /// <summary>
        /// adds short int type parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlParameter pInt16(string paramName, Int16 value)
        {
            return pInt16(paramName, value, ParameterDirection.Input);
        }

        /// <summary>
        /// adds out parameter of type short int 
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        protected SqlParameter pInt16Out(string paramName)
        {
            return pInt16(paramName, short.MinValue, ParameterDirection.Output);
        }

        /// <summary>
        /// adds out parameter of type short int
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected SqlParameter pInt16Out(string paramName, Int16 value)
        {
            return pInt16(paramName, value, ParameterDirection.Output);
        }

        /// <summary>
        /// adds parameter of type byte
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pByte(string paramName, byte[] value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.Image);
            param.Value = value;
            param.Direction = direction;
            return param;
        }

        /// <summary>
        /// adds parameter of type Bytes and sets value
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected SqlParameter pByte(string paramName, byte[] value)
        {
            return pByte(paramName, value, ParameterDirection.Input);
            //SqlParameter param = new SqlParameter ( "@" + paramName, value);
            //return param;
        }

        /// <summary>
        /// adds parameter of type byte
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pVarBinary(string paramName, byte[] value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.VarBinary);
            if (value == null)
                param.Value = DBNull.Value;
            else
                param.Value = value;
            param.Direction = direction;
            return param;
        }

        /// <summary>
        /// adds parameter of type Bytes and sets value
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected SqlParameter pVarBinary(string paramName, byte[] value)
        {
            return pVarBinary(paramName, value, ParameterDirection.Input);
            //SqlParameter param = new SqlParameter ( "@" + paramName, value);
            //return param;
        }

        #endregion

        #region Floting Point Type Parameters

        /// <summary>
        /// adds Float (SqlDbType.Float) as parameter with parameter direction set to 'input'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pFloat(string paramName, float value)
        {
            return pFloat(paramName, value, ParameterDirection.Input);
        }

        /// <summary>
        /// adds Float (SqlDbType.Float) as parameter with parameter direction set to 'output'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pFloatOut(string paramName)
        {
            return pFloat(paramName, float.MinValue, ParameterDirection.Output);
        }

        /// <summary>
        /// adds Float (SqlDbType.Float) as parameter with parameter direction set to 'direction'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pFloat(string paramName, float value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.Float);
            param.Value = value;
            param.Direction = direction;
            return param;
        }

        /// <summary>
        /// adds Decimal (SqlDbType.Decimal) as parameter with parameter direction set to 'input'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pDecimal(string paramName, Decimal value)
        {
            return pDecimal(paramName, value, ParameterDirection.Input);
        }

        /// <summary>
        /// adds Decimal (SqlDbType.Decimal) as parameter with parameter direction set to 'output'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pDecimalOut(string paramName)
        {
            return pDecimal(paramName, decimal.MinValue, ParameterDirection.Output);
        }

        /// <summary>
        /// adds Decimal (SqlDbType.Decimal) as parameter with parameter direction set to 'direction'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pDecimal(string paramName, Decimal value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.Decimal);
            param.Value = value;
            param.Direction = direction;
            return param;
        }
        protected SqlParameter pDecimal(string paramName, Decimal? value)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.Decimal);
            if (!value.HasValue)
                param.Value = DBNull.Value;
            else
                param.Value = value;
            return param;
        }

        protected SqlParameter pDecimal(string paramName, int count, Decimal? value)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.Decimal);
            if (!value.HasValue)
                param.Value = DBNull.Value;
            else
                param.Value = value;
            return param;
        }
        /// <summary>
        /// adds Real (SqlDbType.Real) as parameter with parameter direction set to 'input'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pReal(string paramName, Single? value)
        {
            if (value.HasValue)
                return pReal(paramName, value.Value, ParameterDirection.Input);
            else
                return pReal(paramName, ParameterDirection.Input);
        }

        /// <summary>
        /// adds Real (SqlDbType.Real) as parameter with parameter direction set to 'input'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pReal(string paramName, Single value)
        {
            return pReal(paramName, value, ParameterDirection.Input);
        }

        /// <summary>
        /// adds Real (SqlDbType.Real) as parameter with parameter direction set to 'output'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pRealOut(string paramName)
        {
            return pReal(paramName, Single.MinValue, ParameterDirection.Output);
        }

        /// <summary>
        /// adds Real (SqlDbType.Real) as parameter with parameter direction set to 'direction'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pReal(string paramName, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.Real);
            param.Value = DBNull.Value;
            param.Direction = direction;
            return param;
        }

        /// <summary>
        /// adds Real (SqlDbType.Real) as parameter with parameter direction set to 'direction'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pReal(string paramName, Single value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.Real);
            param.Value = value;
            param.Direction = direction;
            return param;
        }

        /// <summary>
        /// adds Double (SqlDbType.Real) as parameter with parameter direction set to 'input'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pDouble(string paramName, double value)
        {
            return pDouble(paramName, value, ParameterDirection.Input);
        }

        /// <summary>
        /// adds Double (SqlDbType.Real) as parameter with parameter direction set to 'output'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pDouble(string paramName)
        {
            return pDouble(paramName, double.MinValue, ParameterDirection.Output);
        }

        /// <summary>
        /// adds Double (SqlDbType.Real) as parameter with parameter direction set to 'direction'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pDouble(string paramName, double value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.Real);
            param.Value = value;
            param.Direction = direction;
            return param;
        }

        /// <summary>
        /// adds Decimal (SqlDbType.Money) as parameter with parameter direction set to 'input'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pMoney(string paramName, decimal value)
        {
            return pMoney(paramName, value, ParameterDirection.Input);
        }

        /// <summary>
        /// adds Decimal (SqlDbType.Money) as parameter with parameter direction set to 'output'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pMoneyOut(string paramName)
        {
            return pMoney(paramName, decimal.MinValue, ParameterDirection.Output);
        }

        /// <summary>
        /// adds Decimal (SqlDbType.Money) as parameter with parameter direction set to 'direction'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pMoney(string paramName, decimal value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.Money);
            param.Value = value;
            param.Direction = direction;
            return param;
        }

        #endregion

        #region Text and String Parameters

        /// <summary>
        /// adds String (VarChar) as parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public SqlParameter pNChar(string paramName, int size, char[] value)
        {
            return pNChar(paramName, size, value, ParameterDirection.Input);
        }
        /// <summary>
        /// adds String (VarChar) as parameter with parameter direction set to 'out'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pNCharOut(string paramName)
        {
            return pNChar(paramName, 255, null, ParameterDirection.Output);
        }

        /// <summary>
        /// adds String (VarChar) as parameter with parameter direction set to 'direction'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pNChar(string paramName, int size, char[] value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.NChar);
            param.Size = size;
            if (value == null)
                param.Value = DBNull.Value;
            else
                param.Value = value;
            param.Direction = direction;
            return param;
        }
        //protected SqlParameter pNChar(string paramName, int size, Nullable<char[]> value, ParameterDirection direction)
        //{
        //    SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.NChar);
        //    param.Size = size;
        //    if (!value.HasValue)
        //        param.Value = DBNull.Value;
        //    else
        //        param.Value = value.Value;
        //    param.Direction = direction;
        //    return param;
        //}

        /// <summary>
        /// adds String (VarChar) as parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public SqlParameter pVarChar(string paramName, string value)
        {
            return pVarChar(paramName, 50, value, ParameterDirection.Input);
        }

        /// <summary>
        /// adds String (VarChar) as parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public SqlParameter pVarChar(string paramName, int size, string value)
        {
            return pVarChar(paramName, size, value, ParameterDirection.Input);
        }

        /// <summary>
        /// adds String (VarChar) as parameter with parameter direction set to 'out'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pVarCharOut(string paramName)
        {
            return pVarChar(paramName, 255, string.Empty, ParameterDirection.Output);
        }

        /// <summary>
        /// adds String (VarChar) as parameter with parameter direction set to 'direction'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pVarChar(string paramName, int size, string value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.VarChar);
            param.Size = size;
            if (String.IsNullOrEmpty(value))
                param.Value = "";
            else
                param.Value = value;
            param.Direction = direction;
            return param;
        }

        /// <summary>
        /// adds String (VarChar) as parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public SqlParameter pNVarChar(string paramName, string value)
        {
            return pNVarChar(paramName, -1, value, ParameterDirection.Input);
        }

        /// <summary>
        /// adds String (VarChar) as parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public SqlParameter pNVarChar(string paramName, int size, string value)
        {
            return pNVarChar(paramName, size, value, ParameterDirection.Input);
        }

        /// <summary>
        /// adds String (VarChar) as parameter with parameter direction set to 'out'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pNVarCharOut(string paramName)
        {
            return pNVarChar(paramName, 255, string.Empty, ParameterDirection.Output);
        }

        /// <summary>
        /// adds String (VarChar) as parameter with parameter direction set to 'direction'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pNVarChar(string paramName, int size, string value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.NVarChar);
            param.Size = size;
            if (value == null)
                param.Value = "";
            else
                param.Value = value;
            param.Direction = direction;
            return param;
        }

        /// <summary>
        /// adds String (Text) as parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pText(string paramName, String value)
        {
            return pText(paramName, value, ParameterDirection.Input);
        }

        /// <summary>
        /// adds String (Text) as parameter with parameter direction set to 'out'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pTextOut(string paramName)
        {
            return pText(paramName, string.Empty, ParameterDirection.Output);
        }

        /// <summary>
        /// adds String (Text) as parameter with parameter direction set to 'direction'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pText(string paramName, String value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.Text);
            if (String.IsNullOrEmpty(value))
                param.Value = DBNull.Value;
            else
                param.Value = value;
            param.Direction = direction;
            return param;
        }

        /// <summary>
        /// adds String (NText) as parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pNText(string paramName, String value)
        {
            return pNText(paramName, value, ParameterDirection.Input);
        }

        /// <summary>
        /// adds String (NText) as parameter with parameter direction set to 'out'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pNTextOut(string paramName)
        {
            return pNText(paramName, string.Empty, ParameterDirection.Output);
        }

        /// <summary>
        /// adds String (NText) as parameter with parameter direction set to 'direction'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pNText(string paramName, String value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.NText);
            param.Value = value;
            param.Direction = direction;
            return param;
        }

        #endregion

        #region Bit and Boolean Parameters

        /// <summary>
        /// adds boolean (bit) parameters with parameter direction set to 'direction' 
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pBit(string paramName, bool value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.Bit);
            param.Value = value;
            param.Direction = direction;
            return param;
        }

        /// <summary>
        /// adds boolean (bit) parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pBit(string paramName, bool value)
        {
            return pBit(paramName, value, ParameterDirection.Input);
        }

        /// <summary>
        /// adds boolean (bit) parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pBool(string paramName, bool value)
        {
            return pBool(paramName, value, ParameterDirection.Input);
        }

        /// <summary>
        /// adds boolean (bit) parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pBool(string paramName, bool? value)
        {
            return pBool(paramName, value.HasValue ? value.Value : false, ParameterDirection.Input);
        }

        /// <summary>
        /// adds boolean (bit) parameters with parameter direction set to 'out' 
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pBoolOut(string paramName)
        {
            return pBool(paramName, false, ParameterDirection.Output);
        }

        /// <summary>
        /// adds boolean (bit) parameters with parameter direction set to 'direction' 
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pBool(string paramName, bool value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.Bit);
            param.Value = value;
            param.Direction = direction;
            return param;
        }

        #endregion

        #region Other Parameters

        /// <summary>
        /// adds DateTimeOffset type parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pDateTimeOffset(string paramName, DateTimeOffset? value)
        {
            return pDateTimeOffset(paramName, value, ParameterDirection.Input);
        }

        /// <summary>
        /// adds DateTimeOffset type parameter with parameter direction set to 'out'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pDateTimeOffsetOut(string paramName)
        {
            return pDateTimeOffset(paramName, DateTime.MinValue, ParameterDirection.Output);
        }

        /// <summary>
        /// adds DateTimeOffset type parameter with parameter direction set to 'direction'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pDateTimeOffset(string paramName, DateTimeOffset? value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.DateTimeOffset);
            if (!value.HasValue)
                param.Value = DBNull.Value;
            else if (value.Value == DateTimeOffset.MinValue && direction == ParameterDirection.Input)
                param.Value = DBNull.Value;
            else
                param.Value = value;
            param.Direction = direction;
            return param;
        }

        /// <summary>
        /// adds Time type parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pTime(string paramName, TimeSpan? value)
        {
            return pTime(paramName, value, ParameterDirection.Input);
        }

        /// <summary>
        /// adds Time type parameter with parameter direction set to 'out'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pTimeOut(string paramName)
        {
            return pTime(paramName, TimeSpan.MinValue, ParameterDirection.Output);
        }

        /// <summary>
        /// adds Time type parameter with parameter direction set to 'direction'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pTime(string paramName, TimeSpan? value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.Time);
            if (!value.HasValue)
                param.Value = DBNull.Value;
            else if (value.Value == TimeSpan.MinValue && direction == ParameterDirection.Input)
                param.Value = DBNull.Value;
            else
                param.Value = value;
            param.Direction = direction;
            return param;
        }

        /// <summary>
        /// adds DateTime type parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pDateTime(string paramName, DateTime? value)
        {
            return pDateTime(paramName, value, ParameterDirection.Input);
        }

        /// <summary>
        /// adds DateTime type parameter with parameter direction set to 'out'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pDateTimeOut(string paramName)
        {
            return pDateTime(paramName, DateTime.MinValue, ParameterDirection.Output);
        }

        /// <summary>
        /// adds DateTime type parameter with parameter direction set to 'direction'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pDateTime(string paramName, DateTime? value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.DateTime);
            if (!value.HasValue)
                param.Value = DBNull.Value;
            else if (value.Value == DateTime.MinValue && direction == ParameterDirection.Input)
                param.Value = DBNull.Value;
            else
                param.Value = value;
            param.Direction = direction;
            return param;
        }

        /// <summary>
        /// adds Guid as parameter with parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pGuid(string paramName, Guid value)
        {
            return pGuid(paramName, value, ParameterDirection.Input);
        }
        
        /// <summary>
        /// adds Guid as parameter with parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pGuid(string paramName, Guid? value)
        {
            return pGuid(paramName, value, ParameterDirection.Input);
        }

        /// <summary>
        /// adds Guid as parameter with parameter direction set to 'out'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pGuidOut(string paramName)
        {
            return pGuid(paramName, Guid.Empty, ParameterDirection.Output);
        }

        /// <summary>
        /// adds Guid as parameter with parameter direction set to 'direction'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pGuid(string paramName, Guid value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.UniqueIdentifier);
            param.Value = value;
            param.Direction = direction;
            return param;
        }
        
        /// <summary>
        /// adds Guid as parameter with parameter direction set to 'direction'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pGuid(string paramName, Guid? value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.UniqueIdentifier);
            if (!value.HasValue)
                param.Value = DBNull.Value;
            else
                param.Value = value;
            param.Direction = direction;
            return param;
        }

        /// <summary>
        /// adds image represented as an array of bytes as parameter 
        /// with Parameter Direction set to 'direction'
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        protected SqlParameter pImage(string paramName, byte[] value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter("@" + paramName, SqlDbType.Image);
            param.Value = value;
            param.Direction = direction;
            return param;
        }

        /// <summary>
        /// adds image represented as an array of bytes as parameter
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected SqlParameter pImage(string paramName, byte[] value)
        {
            return pImage(paramName, value, ParameterDirection.Input);
        }

        #endregion

        /// <summary>
        /// Method Dispose
        /// does the clean up jobs
        /// </summary>

        public void Dispose()
        {
            CloseConnection();
            if (_Transaction == null)
                DBConnection.Dispose();
            //			GC.Collect();
        }
    }
}