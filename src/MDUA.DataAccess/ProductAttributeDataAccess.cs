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
	public partial class ProductAttributeDataAccess
	{
        public int Insert(ProductAttribute attribute)
        {
            // Calls the Stored Procedure you provided
            using SqlCommand cmd = GetSPCommand("InsertProductAttribute");

            // OUTPUT
            AddParameter(cmd, pInt32Out(ProductAttributeBase.Property_Id));

            // PARAMS
            AddParameter(cmd, pInt32(ProductAttributeBase.Property_ProductId, attribute.ProductId));
            AddParameter(cmd, pInt32(ProductAttributeBase.Property_AttributeId, attribute.AttributeId));
            AddParameter(cmd, pInt32(ProductAttributeBase.Property_DisplayOrder, attribute.DisplayOrder));

            long result = InsertRecord(cmd);

            if (result > 0)
                return (int)GetOutParameter(cmd, ProductAttributeBase.Property_Id);

            return 0;
        }
    }	
}
