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

        //change

        public long InsertAddressSafe(Address address)

        {

            // ✅ SQL: Insert and return ID immediately using SCOPE_IDENTITY

            string SQLQuery = @"

              INSERT INTO [dbo].[Address]

              ([CustomerId], [Street], [City], [Divison], [Thana], [SubOffice], 

               [PostalCode], [ZipCode], [Country], [AddressType], 

               [CreatedBy], [CreatedAt])

              VALUES

              (@CustomerId, @Street, @City, @Divison, @Thana, @SubOffice, 

               @PostalCode, @ZipCode, @Country, @AddressType, 

               @CreatedBy, @CreatedAt);

              SELECT CAST(SCOPE_IDENTITY() AS INT);";

            // GetSQLCommand AUTOMATICALLY handles the transaction linkage

            using (SqlCommand cmd = GetSQLCommand(SQLQuery))

            {

                AddParameter(cmd, pInt32("CustomerId", address.CustomerId));

                AddParameter(cmd, pNVarChar("Street", 255, address.Street));

                AddParameter(cmd, pNVarChar("City", 100, address.City));

                AddParameter(cmd, pNVarChar("Divison", 100, address.Divison));

                AddParameter(cmd, pNVarChar("Thana", 100, address.Thana ?? ""));

                AddParameter(cmd, pNVarChar("SubOffice", 100, address.SubOffice ?? ""));

                AddParameter(cmd, pVarChar("PostalCode", 20, address.PostalCode));

                AddParameter(cmd, pNVarChar("Country", 100, address.Country));

                // Handle ZipCode char[] conversion

                string zipString = address.ZipCode != null ? new string(address.ZipCode).Trim() : "0000";

                AddParameter(cmd, pNVarChar("ZipCode", 50, zipString));

                AddParameter(cmd, pNVarChar("AddressType", 50, address.AddressType));

                AddParameter(cmd, pNVarChar("CreatedBy", 100, address.CreatedBy));

                AddParameter(cmd, pDateTime("CreatedAt", address.CreatedAt));

                // Ensure connection is open (Transaction connection might be closed by previous op)

                if (cmd.Connection.State != ConnectionState.Open)

                    cmd.Connection.Open();

                // ✅ EXECUTE AND GET ID

                object result = cmd.ExecuteScalar();

                int newId = 0;

                if (result != null && result != DBNull.Value)

                {

                    newId = Convert.ToInt32(result);

                }

                // Populate the object so Facade can use it

                address.Id = newId;

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
