using System;
using System.Data;
using System.Data.SqlClient;

using MDUA.Framework;
using MDUA.Framework.Exceptions;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.DataAccess.Interface;

namespace MDUA.DataAccess
{
	public partial class PermissionGroupMapDataAccess : IPermissionGroupMapDataAccess
    {

        public List<Permission> GetPermissionsByGroupId(int groupId)
        {
            List<Permission> permissions = new List<Permission>();

            // Call stored procedure that returns PermissionGroupMap rows
            string query = "EXEC GetPermissionGroupMapByPermissionGroupId @PermissionGroupId";
            using var cmd = GetSQLCommand(query);
            AddParameter(cmd, pInt32("PermissionGroupId", groupId));

            DataSet ds = GetDataSet(cmd);
            if (ds != null && ds.Tables.Count > 0)
            {
                permissions = ds.Tables[0].AsEnumerable()
                    .SelectMany(row =>
                    {
                        int permissionId = row.IsNull("PermissionId") ? 0 : Convert.ToInt32(row["PermissionId"]);

                        using var permCmd = GetSQLCommand("EXEC GetPermissionById @Id");
                        AddParameter(permCmd, pInt32("Id", permissionId));
                        DataSet permDs = GetDataSet(permCmd);

                        if (permDs != null && permDs.Tables.Count > 0 && permDs.Tables[0].Rows.Count > 0)
                        {
                            var permRow = permDs.Tables[0].Rows[0];
                            return new List<Permission>
                            {
                new Permission
                {
                    Id = Convert.ToInt32(permRow["Id"]),
                    Name = permRow["Name"]?.ToString(),
                    ActionName = permRow["Name"]?.ToString()
                }
                            };
                        }

                        return new List<Permission>();
                    })
                    .ToList();

            }

            return permissions;
        }

    }
}
