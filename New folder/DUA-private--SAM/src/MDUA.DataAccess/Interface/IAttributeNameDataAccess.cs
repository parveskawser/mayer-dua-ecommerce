using System;
using System.Collections.Generic;
using MDUA.Entities;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.DataAccess.Interface
{
    public interface IAttributeNameDataAccess : ICommonDataAccess<AttributeName, AttributeNameList, AttributeNameBase>
    {
        // Existing Methods
        List<AttributeValue> GetValuesByAttributeId(int attributeId);
        List<AttributeName> GetByProductId(int productId);
        List<AttributeName> GetMissingAttributesForVariant(int productId, int variantId);
        string GetValueName(int valueId);
        
        // ✅ Restored Method
        Dictionary<string, List<string>> GetSpecificationsByProductId(int productId);

        // ✅ NEW: Multi-Tenancy Support (Option B)
        List<AttributeName> GetAvailableAttributes(int companyId);
        AttributeName GetPrivateAttribute(string name, int companyId);
        int CloneGlobalToPrivate(int globalAttributeId, int targetCompanyId);

        void UpdateStatus(int id, bool isActive);
        List<AttributeName> GetAllAttributesForManagement(int companyId);
        List<AttributeName> GetAttributeNamesByProductId(int productId);
    }
}