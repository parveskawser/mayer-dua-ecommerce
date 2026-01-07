using System.Collections.Generic;
using MDUA.Entities;

namespace MDUA.Facade
{
    public interface IAttributeFacade
    {
        // --- READ ---
        List<AttributeName> GetAvailableAttributes(int companyId);
        
        /// <summary>
        /// Gets values. Set onlyActive=true for Dropdowns, false for Admin Grids.
        /// </summary>
        List<AttributeValue> GetValuesByAttributeId(int attributeId, bool onlyActive = true);
        
        AttributeName GetAttributeName(int id);

        // --- WRITE (Attributes) ---
        int SaveAttributeName(AttributeName attribute, int companyId);
        bool DeleteAttributeName(int id, int companyId);
        
        /// <summary>
        /// Toggles Active/Inactive. Enforces Company Ownership.
        /// </summary>
        void UpdateAttributeNameStatus(int id, bool isActive, int companyId);

        // --- WRITE (Values) ---
        int SaveAttributeValue(AttributeValue value, int companyId);
        bool DeleteAttributeValue(int valueId, int companyId);
        
        /// <summary>
        /// Toggles Value Status. Enforces Parent Attribute Ownership.
        /// </summary>
        void UpdateAttributeValueStatus(int id, bool isActive, int companyId);

        List<AttributeName> GetAllAttributesForManagement(int companyId);
    }
}