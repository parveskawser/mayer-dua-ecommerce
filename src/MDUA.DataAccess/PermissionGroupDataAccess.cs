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
	public partial class PermissionGroupDataAccess
	{
        public PermissionGroup GetById(int id)
        {
            string query = "SELECT * FROM PermissionGroup WHERE Id = @Id";
            using var cmd = GetSQLCommand(query);
            AddParameter(cmd, pInt32("Id", id));

            DataSet ds = GetDataSet(cmd);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var row = ds.Tables[0].Rows[0];
                return new PermissionGroup
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString()
                    // map other fields if any
                };
            }
            return null;
        }
    }	
}
