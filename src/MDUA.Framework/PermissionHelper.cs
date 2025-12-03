using Microsoft.Extensions.Configuration;
using MDUA.Framework.DataAccess;
using MDUA.Framework.Utils;
using System.Data.SqlClient;

namespace MDUA.Framework;

public class PermissionHelper : BaseDataAccess
{
    private readonly IConfiguration _configuration;
    
    public PermissionHelper(IConfiguration configuration) : base(configuration)
    {        
    }

    public static List<int> GetPermissionIds(string groupid, string CompanyId = "", string Conn = "", string UserId = "")
    { 
        string sqlQueryuser = "";
        string sqlQuery = @"select pgm.PermissionId from PermissionGroupMap pgm where pgm.PermissionGroupId={0} AND pgm.IsActive='1'";
        try
        {
            List<int> check = new List<int>();
            SqlConnection sqlConnection = new SqlConnection(Conn);
            if (!string.IsNullOrWhiteSpace(UserId))
            {
                sqlQueryuser = @"select pgm.PermissionId from PermissionGroupMap pgm where pgm.IsActive='1' AND UserId='{1}'";
            }
            string commandString = string.Format(sqlQuery, groupid, UserId);
            string commandStringUser = string.Format(sqlQueryuser, groupid, UserId);
            SqlCommand command = new SqlCommand(commandString, sqlConnection);
            SqlCommand commanduser = new SqlCommand(commandStringUser, sqlConnection);
            sqlConnection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    check.Add(reader.GetInt32(0));
                }
            }
            using (SqlDataReader reader = commanduser.ExecuteReader())
            {
                while (reader.Read())
                {
                    check.Add(reader.GetInt32(0));
                }
            }
            sqlConnection.Close();
            return check;
        }
        catch (Exception ex)
        {
            var catchmsg = ex.Message;
            return null;
        }
    }
    public static List<int> GetCompanyPackages(string comid)
    {
        List<int> check = new List<int>();
        string sqlQuery = @"
                                SELECT p.[Id]
                                  FROM [CompanyPackage] cp
                                  LEFT JOIN Package p on p.PackageId=cp.PackageId
                                WHERE cp.CompanyId='{0}'
                                AND cp.IsActive=1
                            ";
        try
        {
            sqlQuery = string.Format(sqlQuery, comid);
            SqlConnection sqlConnection = new SqlConnection(AppConfig.GetConnectionStringCompanyLite());
            SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);
            sqlConnection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    check.Add(reader.GetInt32(0));
                }
            }
            sqlConnection.Close();
            return check;
        }
        catch (Exception)
        {
            return null;
        }
    }
    public static bool IsPermitted(int pid, string UserId, string CompanyId, string Conn)
    {
        bool result = false;
        string connectionString = Conn;
        string sqlQuery = @"SELECT PermissionGroupId FROM UserPermission WHERE UserId = '{0}'";
        // AND CompanyId='{1}'
        try
        {
            bool recheck = false;
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            string commandString = String.Format(sqlQuery, UserId, CompanyId);
            SqlCommand sqlCommand = new SqlCommand(commandString, sqlConnection);
            sqlConnection.Open();
            string check = Convert.ToString(sqlCommand.ExecuteScalar());
            if (!string.IsNullOrWhiteSpace(check) && check != "0")
            {
                string sqlQuery2 = @"select IsActive from PermissionGroupMap where IsActive=1 AND PermissionGroupId={0} and PermissionId={1} AND CompanyId='{2}'";
                string commandString2 = String.Format(sqlQuery2, check, pid, CompanyId);
                SqlCommand sqlCommand2 = new SqlCommand(commandString2, sqlConnection);
                recheck = Convert.ToBoolean(sqlCommand2.ExecuteScalar());

            }
            sqlConnection.Close();
            return recheck;
        }
        catch (Exception ex)
        {
            var msg = ex.Message;
            return false;
        }
    }

    public static bool IsPermittedUser(int pid, string UserId, string CompanyId, string Conn)
    {
        bool result = false;
        string connectionString = Conn;
        string sqlQuery = @"SELECT PermissionGroupId FROM UserPermission WHERE UserId = '{0}'";
        // AND CompanyId='{1}'
        try
        {
            bool recheck = false;
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            string commandString = String.Format(sqlQuery, UserId, CompanyId);
            SqlCommand sqlCommand = new SqlCommand(commandString, sqlConnection);
            sqlConnection.Open();
            string check = Convert.ToString(sqlCommand.ExecuteScalar());
            if (!string.IsNullOrWhiteSpace(check) && check != "0")
            {
                string sqlQuery2 = @"select IsActive from PermissionGroupMap where PermissionGroupId={0} and PermissionId={1} AND CompanyId='{2}' AND UserId='{3}'";
                string commandString2 = String.Format(sqlQuery2, check, pid, CompanyId, UserId);
                SqlCommand sqlCommand2 = new SqlCommand(commandString2, sqlConnection);
                recheck = Convert.ToBoolean(sqlCommand2.ExecuteScalar());

            }
            sqlConnection.Close();
            return recheck;
        }
        catch (Exception ex)
        {
            var msg = ex.Message;
            return false;
        }
    }
    public static bool IsPermittedUserCustom(int pid, string UserId, string CompanyId, string Conn)
    {
        bool result = false;
        string connectionString = Conn;
        string sqlQuery = @"SELECT PermissionGroupId FROM UserPermission WHERE UserId = '{0}'";
        // AND CompanyId='{1}'
        try
        {
            bool recheck = false;
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            string commandString = String.Format(sqlQuery, UserId, CompanyId);
            SqlCommand sqlCommand = new SqlCommand(commandString, sqlConnection);
            sqlConnection.Open();
            string check = Convert.ToString(sqlCommand.ExecuteScalar());
            if (!string.IsNullOrWhiteSpace(check) && check != "0")
            {
                string sqlQuery2 = @"select IsActive from PermissionGroupMap where PermissionGroupId=0 and PermissionId={1} AND CompanyId='{2}' AND UserId='{3}'";
                string commandString2 = String.Format(sqlQuery2, check, pid, CompanyId, UserId);
                SqlCommand sqlCommand2 = new SqlCommand(commandString2, sqlConnection);
                recheck = Convert.ToBoolean(sqlCommand2.ExecuteScalar());

            }
            sqlConnection.Close();
            return recheck;
        }
        catch (Exception ex)
        {
            var msg = ex.Message;
            return false;
        }
    }
    public static bool IsAllowed(string CompanyId, string Key, string Value)
    {
        bool result = false;
        string connectionString = GetConnectionStringLite();
        string sqlQuery = @"SELECT IsActive FROM GlobalSetting WHERE CompanyId = '{0}' AND SearchKey='{1}' AND Value='{2}'";
        try
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            string commandString = String.Format(sqlQuery, CompanyId, Key, Value);
            SqlCommand sqlCommand = new SqlCommand(commandString, sqlConnection);
            sqlConnection.Open();
            result = Convert.ToBoolean(sqlCommand.ExecuteScalar());
            sqlConnection.Close();
            return result;
        }
        catch (Exception ex)
        {
            var msg = ex.Message;
            return false;
        }
    }
    public static string GlobalValue(string SearchKey, string CompanyId, string Conn)
    {
        string result = "";
        string sqlQuery = @"SELECT Value FROM GlobalSetting WHERE CompanyId ='{0}' and SearchKey='{1}' AND IsActive=1";
        try
        {
            SqlConnection sqlConnection = new SqlConnection(Conn);
            string commandString = String.Format(sqlQuery, CompanyId, SearchKey);
            SqlCommand sqlCommand = new SqlCommand(commandString, sqlConnection);
            sqlConnection.Open();
            result = Convert.ToString(sqlCommand.ExecuteScalar());
            sqlConnection.Close();
            return result;
        }
        catch (Exception ex)
        {
            var msg = ex.Message;
            return "";
        }
    }
    private static string GetConnectionStringLite()
    {
        string _connectionString = string.Empty;
        var configurationBuilder = new ConfigurationBuilder();
        var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        configurationBuilder.AddJsonFile(path, false);
        var root = configurationBuilder.Build();
        _connectionString = root.GetSection("ConnectionStrings").GetSection("DevConnectionPPOS").Value;
        return _connectionString;
    }
}
