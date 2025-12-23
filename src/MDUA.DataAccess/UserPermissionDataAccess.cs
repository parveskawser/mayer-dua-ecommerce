using System;
using System.Data;
using System.Data.SqlClient;

using MDUA.Framework;
using MDUA.Framework.Exceptions;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.DataAccess.Interface;
using MDUA.Framework.DataAccess;
using Microsoft.Extensions.Configuration;

namespace MDUA.DataAccess
{
    public partial class UserPermissionDataAccess
    {
        public List<UserPermission> GetByUserIdForFacade(int userId)
        {
            using var cmd = GetSQLCommand("GetUserPermissionByUserId"); // single argument
            cmd.CommandType = CommandType.StoredProcedure;               // <-- set type here
            AddParameter(cmd, pInt32("UserId", userId));

            DataSet ds = GetDataSet(cmd);

            return ds.Tables[0].AsEnumerable()
                .Select(row => new UserPermission
                {
                    Id = Convert.ToInt32(row["Id"]),
                    UserId = Convert.ToInt32(row["UserId"]),
                    PermissionId = row.IsNull("PermissionId") ? null : (int?)Convert.ToInt32(row["PermissionId"]),
                    PermissionGroupId = row.IsNull("PermissionGroupId") ? null : (int?)Convert.ToInt32(row["PermissionGroupId"])
                }).ToList();
        }





    }


}
