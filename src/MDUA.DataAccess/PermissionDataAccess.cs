using System;
using System.Data;
using System.Data.SqlClient;

using MDUA.Framework;
using MDUA.Framework.Exceptions;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess
{
	public partial class PermissionDataAccess
	{
        public List<Permission> GetPermissionsByGroupId(int groupId)
        {
            List<Permission> permissions = new List<Permission>();

            // Step 1: Get mapping rows
            string spMapping = "GetPermissionGroupMapByPermissionGroupId";
            using var cmdMapping = GetSQLCommand(spMapping);
            cmdMapping.CommandType = CommandType.StoredProcedure;
            AddParameter(cmdMapping, pInt32("PermissionGroupId", groupId));

            DataSet dsMapping = GetDataSet(cmdMapping);

            if (dsMapping == null || dsMapping.Tables.Count == 0) return permissions;

            var permissionIds = dsMapping.Tables[0].AsEnumerable()
                                    .Where(r => !r.IsNull("PermissionId"))
                                    .Select(r => Convert.ToInt32(r["PermissionId"]))
                                    .ToList();

            if (!permissionIds.Any()) return permissions;

            // Step 2: Get each Permission detail via existing SP
            foreach (var permissionId in permissionIds)
            {
                string spPermission = "GetPermissionById";
                using var cmdPermission = GetSQLCommand(spPermission);
                cmdPermission.CommandType = CommandType.StoredProcedure;
                AddParameter(cmdPermission, pInt32("Id", permissionId));

                DataSet dsPermission = GetDataSet(cmdPermission);

                if (dsPermission != null && dsPermission.Tables.Count > 0 && dsPermission.Tables[0].Rows.Count > 0)
                {
                    var row = dsPermission.Tables[0].Rows[0];
                    permissions.Add(new Permission
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Name = row["Name"].ToString(),       // convert to string
                        ActionName = row["Name"].ToString()  // convert to string
                    });
                }
            }

            return permissions;
        }

        public List<Permission> GetByIds(List<int> ids)
        {
            List<Permission> permissions = new List<Permission>();
            if (ids == null || !ids.Any())
                return permissions;

            // Create a comma-separated list for SQL IN clause
            string idList = string.Join(",", ids);

            string query = $"SELECT * FROM Permission WHERE Id IN ({idList})";

            using var cmd = GetSQLCommand(query);
            DataSet ds = GetDataSet(cmd);

            if (ds != null && ds.Tables.Count > 0)
            {
                permissions = ds.Tables[0].AsEnumerable()
                    .Select(row => new Permission
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Name = row["Name"].ToString(),
                        ActionName = row["Name"].ToString() // or another column if needed
                    }).ToList();
            }

            return permissions;
        }
    }
}
