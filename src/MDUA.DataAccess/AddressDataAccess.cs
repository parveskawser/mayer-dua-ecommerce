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
	public partial class AddressDataAccess
	{
        public Address CheckExistingAddress(int customerId, Address newAddress)
        {
            // WARNING: This query assumes no two people have the exact same address string.
            string SQLQuery = @"
        SELECT TOP 1 * FROM Address 
        WHERE CustomerId = @CustomerId
          AND Street = @Street
          AND City = @City
          AND Divison = @Divison
          AND PostalCode = @PostalCode
          -- Check ZipCode (NChar/Char[] equality requires special handling in C# or SQL)
          AND ZipCode = @ZipCode
          AND AddressType = @AddressType
        ORDER BY Id DESC";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                // Add all parameters (Street, City, Divison, PostalCode, ZipCode, AddressType)
                AddParameter(cmd, pInt32("CustomerId", customerId));
                AddParameter(cmd, pNVarChar("Street", 255, newAddress.Street));
                AddParameter(cmd, pNVarChar("City", 100, newAddress.City));
                AddParameter(cmd, pNVarChar("Divison", 100, newAddress.Divison));
                AddParameter(cmd, pVarChar("PostalCode", 20, newAddress.PostalCode));
                // NOTE: If ZipCode is passed as char[], you must convert it back to string/var for the parameter.
                AddParameter(cmd, pNVarChar("ZipCode", 50, new string(newAddress.ZipCode).Trim()));
                AddParameter(cmd, pNVarChar("AddressType", 50, newAddress.AddressType));

                return GetObject(cmd);
            }
        }
        public long InsertAddressSafe(Address address)
        {
            // ✅ BULLETPROOF METHOD: Inline SQL
            // We insert and immediately select the new ID.
            string SQLQuery = @"
                INSERT INTO [dbo].[Address]
                ([CustomerId], [Street], [City], [Divison], [PostalCode], [Country], 
                 [ZipCode], [AddressType], [CreatedBy], [CreatedAt])
                VALUES
                (@CustomerId, @Street, @City, @Divison, @PostalCode, @Country, 
                 @ZipCode, @AddressType, @CreatedBy, @CreatedAt);
                
                SELECT CONVERT(INT, SCOPE_IDENTITY());";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("CustomerId", address.CustomerId));
                AddParameter(cmd, pNVarChar("Street", 255, address.Street));
                AddParameter(cmd, pNVarChar("City", 100, address.City));
                AddParameter(cmd, pNVarChar("Divison", 100, address.Divison));
                AddParameter(cmd, pVarChar("PostalCode", 20, address.PostalCode));
                AddParameter(cmd, pNVarChar("Country", 100, address.Country));

                // Handle ZipCode char[] conversion
                string zipString = new string(address.ZipCode);
                AddParameter(cmd, pNVarChar("ZipCode", 50, zipString));

                AddParameter(cmd, pNVarChar("AddressType", 50, address.AddressType));
                AddParameter(cmd, pNVarChar("CreatedBy", 100, address.CreatedBy));
                AddParameter(cmd, pDateTime("CreatedAt", address.CreatedAt));

                // ✅ FIX: Use SelectRecords instead of ExecuteScalar
                // SelectRecords executes the command and gives us a reader to get the result (the new ID)
                SqlDataReader reader;
                SelectRecords(cmd, out reader);

                int newId = 0;
                using (reader)
                {
                    if (reader.Read() && !reader.IsDBNull(0))
                    {
                        newId = reader.GetInt32(0);
                    }
                    reader.Close();
                }

                return newId;
            }
        }
        public Address GetLatestByCustomerId(int customerId)
        {
            string SQLQuery = "SELECT TOP 1 * FROM Address WHERE CustomerId = @CustomerId ORDER BY Id DESC";

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))
            {
                AddParameter(cmd, pInt32("CustomerId", customerId));

                SqlDataReader reader;
                SelectRecords(cmd, out reader);

                using (reader)
                {
                    if (reader.Read())
                    {
                        Address obj = new Address();
                        // Call the overridden FillObject method
                        FillObject(obj, reader);
                        return obj;
                    }
                    reader.Close();
                }
                return null;
            }
        }

        private void FillObject(Address addressObject, SqlDataReader reader)
        {
            FillObject(addressObject, reader, 0);
        }
    }	
}
