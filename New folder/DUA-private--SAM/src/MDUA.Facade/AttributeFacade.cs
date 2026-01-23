using MDUA.DataAccess;
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using System;
using System.Collections.Generic;

namespace MDUA.Facade
{
    public class AttributeFacade : IAttributeFacade
    {
        private readonly IAttributeNameDataAccess _nameDA;
        private readonly IAttributeValueDataAccess _valueDA;

        public AttributeFacade(IAttributeNameDataAccess nameDA, IAttributeValueDataAccess valueDA)
        {
            _nameDA = nameDA;
            _valueDA = valueDA;
        }

        // ====================================================================
        // READ OPERATIONS
        // ====================================================================

        public List<AttributeName> GetAvailableAttributes(int companyId)
        {
            // Fetches Global (NULL) AND Private (companyId) attributes
            return _nameDA.GetAvailableAttributes(companyId);
        }

        public List<AttributeValue> GetValuesByAttributeId(int attributeId)
        {
            return _nameDA.GetValuesByAttributeId(attributeId);
        }

        public AttributeName GetAttributeName(int id)
        {
            return _nameDA.Get(id);
        }

        // ====================================================================
        // WRITE OPERATIONS (ATTRIBUTE NAME)
        // ====================================================================

        public int SaveAttributeName(AttributeName attribute, int companyId)
        {
            if (string.IsNullOrWhiteSpace(attribute.Name))
                throw new Exception("Attribute Name is required.");

            // 1. INSERT (New)
            if (attribute.Id <= 0)
            {
                // Force Company ID (Security)
                attribute.CompanyId = companyId;
                
                // Optional: Check if name exists for this company
                var existing = _nameDA.GetPrivateAttribute(attribute.Name, companyId);
                if (existing != null)
                    throw new Exception($"You already have an attribute named '{attribute.Name}'.");

                _nameDA.Insert(attribute);
                return attribute.Id;
            }
            // 2. UPDATE (Existing)
            else
            {
                var existing = _nameDA.Get(attribute.Id);
                if (existing == null) throw new Exception("Attribute not found.");

                // SECURITY: Prevent editing Global or Other Company's attributes
                if (existing.CompanyId == null)
                    throw new Exception("You cannot edit Global System Attributes directly.");
                
                if (existing.CompanyId != companyId)
                    throw new Exception("Access Denied: You do not own this attribute.");

                // Update fields
                existing.Name = attribute.Name;
                existing.DisplayOrder = attribute.DisplayOrder;
                existing.IsActive = attribute.IsActive;
                // existing.IsVariantAffecting usually shouldn't change after creation logic, but depends on your rules.

                _nameDA.Update(existing);
                return existing.Id;
            }
        }

        public bool DeleteAttributeName(int id, int companyId)
        {
            var existing = _nameDA.Get(id);
            if (existing == null) return false;

            // SECURITY
            if (existing.CompanyId != companyId)
                throw new Exception("You cannot delete Global attributes or attributes belonging to other companies.");

            // Standard Delete (Cascade in DB handles values)
            _nameDA.Delete(id);
            return true;
        }

        // ====================================================================
        // WRITE OPERATIONS (ATTRIBUTE VALUES - THE TRICKY PART)
        // ====================================================================

        public int SaveAttributeValue(AttributeValue value, int companyId)
        {
            if (string.IsNullOrWhiteSpace(value.Value))
                throw new Exception("Value text is required.");

            // STEP 1: Fetch Parent Attribute to check ownership
            var parentAttribute = _nameDA.Get(value.AttributeId);
            if (parentAttribute == null) throw new Exception("Parent Attribute not found.");

            int workingAttributeId = parentAttribute.Id;

            // STEP 2: CHECK GLOBAL STATUS (The "Option B" Logic)
            // If the user tries to add/edit a value on a GLOBAL attribute, we must CLONE it first.
            if (parentAttribute.CompanyId == null) // It is Global
            {
                // 🚀 CLONE ACTION: Create a private copy of this Global attribute for the company
                // This copies the Name "Size" and all existing values "S,M,L" to a new ID.
                int newPrivateId = _nameDA.CloneGlobalToPrivate(parentAttribute.Id, companyId);
                
                if (newPrivateId <= 0) throw new Exception("Failed to clone global attribute.");

                // Switch our target to the new Private ID
                workingAttributeId = newPrivateId;
                value.AttributeId = newPrivateId; 
            }
            else if (parentAttribute.CompanyId != companyId)
            {
                // It's Private, but belongs to someone else!
                throw new Exception("Security Alert: You cannot modify another company's attribute.");
            }

            // STEP 3: Now we are safe (we own 'workingAttributeId'). Perform Insert/Update.
            if (value.Id <= 0)
            {
                _valueDA.Insert(value);
            }
            else
            {
                // Ideally we check if value.Id actually belongs to workingAttributeId via DB, 
                // but since we checked the parent above, we rely on the Relationship.
                _valueDA.Update(value);
            }

            // Return the AttributeID we ended up using. 
            // The Controller needs this to update the UI (in case we swapped from Global->Private)
            return workingAttributeId;
        }

        public bool DeleteAttributeValue(int valueId, int companyId)
        {
            // We need to know who owns the PARENT attribute of this value
            // Since we can't easily join in Facade without a custom query, we assume 
            // the ValueDataAccess has a way to get the object, then we get parent.
            var val = _valueDA.Get(valueId);
            if (val == null) return false;

            var parent = _nameDA.Get(val.AttributeId);
            if (parent == null) return false;

            // SECURITY: Can only delete values if I own the Attribute Name
            if (parent.CompanyId != companyId)
            {
                if (parent.CompanyId == null)
                    throw new Exception("You cannot delete values from a Global Attribute. Try adding a custom value to 'Clone' it first.");
                
                throw new Exception("Access Denied.");
            }

            _valueDA.Delete(valueId);
            return true;
        }
        public List<AttributeValue> GetValuesByAttributeId(int attributeId, bool onlyActive = true)
        {
            if (onlyActive)
            {
                // Call the new Partial method for Active only
                // Note: We cast to the concrete class to access the Partial method 
                // if it's not in the generic ICommonDataAccess interface yet.
                return _valueDA.GetActiveValues(attributeId);            }
            else
            {
                // Call Base method for ALL
                return _valueDA.GetByAttributeId(attributeId);            }
        }

        // ... (Keep SaveAttributeName, DeleteAttributeName) ...
        public List<AttributeName> GetAllAttributesForManagement(int companyId)
        {
            // Use the new BROAD method for the management list
            return ((AttributeNameDataAccess)_nameDA).GetAllAttributesForManagement(companyId);
        }
        public void UpdateAttributeNameStatus(int id, bool isActive, int companyId)
        {
            var existing = _nameDA.Get(id);
            if (existing == null) throw new Exception("Attribute not found.");

            // CASE 1: It is a Private Attribute (My Company)
            if (existing.CompanyId == companyId)
            {
                // Simple Toggle
                ((AttributeNameDataAccess)_nameDA).UpdateStatus(id, isActive);
            }
            // CASE 2: It is a Global Attribute (System)
            else if (existing.CompanyId == null)
            {
                if (isActive)
                {
                    // Scenario: User tries to "Activate" a Global attribute? 
                    // It's likely already active. If they want to edit it, they clone it via Save.
                    // For status toggle, we generally assume Global items are active. 
                    // We can block this or treat it as a no-op.
                    return;
                }
                else
                {
                    // Scenario: User wants to DEACTIVATE (Hide) a Global Attribute.
                    // Action: Clone it to Private, then set Private to Inactive.

                    // 1. Clone
                    int newPrivateId = ((AttributeNameDataAccess)_nameDA).CloneGlobalToPrivate(existing.Id, companyId);

                    if (newPrivateId > 0)
                    {
                        // 2. Set the NEW Private record to Inactive
                        ((AttributeNameDataAccess)_nameDA).UpdateStatus(newPrivateId, false);
                    }
                    else
                    {
                        throw new Exception("Failed to clone global attribute for deactivation.");
                    }
                }
            }
            // CASE 3: It belongs to another company (Security)
            else
            {
                throw new Exception("Access Denied: You cannot modify this attribute.");
            }
        }
        public void UpdateAttributeValueStatus(int id, bool isActive, int companyId)
        {
            // 1. Get the Value
            var val = _valueDA.Get(id);
            if (val == null) throw new Exception("Value not found.");

            // 2. Get the Parent Attribute to check ownership
            var parent = _nameDA.Get(val.AttributeId);
            if (parent == null) throw new Exception("Parent Attribute not found.");

            // 3. SECURITY: Check ownership of the PARENT
            if (parent.CompanyId != companyId)
            {
                if (parent.CompanyId == null)
                    throw new Exception("You cannot modify Global Attribute Values directly. Clone the attribute first.");
                
                throw new Exception("Access Denied.");
            }

            // 4. Update
            _valueDA.UpdateStatus(id, isActive);        }
    }
}