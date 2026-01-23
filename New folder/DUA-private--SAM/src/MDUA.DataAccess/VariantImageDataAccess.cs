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
    public partial class VariantImageDataAccess
    {
        public VariantImageList GetImagesForVariant(int variantId)
        {
            // Use the existing Stored Procedure name
            using (SqlCommand cmd = GetSPCommand("GetVariantImageByVariantId"))
            {
                AddParameter(cmd, pInt32("VariantId", variantId));

                // ✅ THE FIX: We pass -1 (Infinite) or 10000 instead of the broken constant
                // We can access 'GetList' because we are part of the same class!
                return GetList(cmd, -1);
            }
        }
    }
}
