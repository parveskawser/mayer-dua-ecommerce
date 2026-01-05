using System;
using System.Collections.Generic;
using MDUA.DataAccess.Interface;
using MDUA.Entities;
using MDUA.Entities.List;

namespace MDUA.Facade
{
    public interface IAttributeFacade
    {
        AttributeNameList GetAllAttributes();
        AttributeName GetAttribute(int id);
        long SaveAttribute(AttributeName attribute);
        long DeleteAttribute(int id);

        AttributeValueList GetValuesByAttributeId(int attributeId);
        AttributeValue GetValue(int id);
        long SaveAttributeValue(AttributeValue attrValue);
        long DeleteAttributeValue(int id);
    }

    public class AttributeFacade : IAttributeFacade
    {
        private readonly IAttributeNameDataAccess _attributeNameDA;
        private readonly IAttributeValueDataAccess _attributeValueDA;

        public AttributeFacade(IAttributeNameDataAccess attributeNameDA, IAttributeValueDataAccess attributeValueDA)
        {
            _attributeNameDA = attributeNameDA;
            _attributeValueDA = attributeValueDA;
        }

        #region Attribute Name Methods
        public AttributeNameList GetAllAttributes()
        {
            // Uses generated method from BaseAttributeNameDataAccess
            return _attributeNameDA.GetAll();
        }

        public AttributeName GetAttribute(int id)
        {
            return _attributeNameDA.Get(id);
        }

        public long SaveAttribute(AttributeName attribute)
        {
            if (attribute.Id > 0)
                return _attributeNameDA.Update(attribute); //
            else
                return _attributeNameDA.Insert(attribute); //
        }

        public long DeleteAttribute(int id)
        {
            // Note: You might want to check for dependencies in AttributeValue or ProductAttribute before deleting
            return _attributeNameDA.Delete(id);
        }
        #endregion

        #region Attribute Value Methods
        public AttributeValueList GetValuesByAttributeId(int attributeId)
        {
            // Uses generated method from BaseAttributeValueDataAccess
            return _attributeValueDA.GetByAttributeId(attributeId);
        }

        public AttributeValue GetValue(int id)
        {
            return _attributeValueDA.Get(id);
        }

        public long SaveAttributeValue(AttributeValue attrValue)
        {
            if (attrValue.Id > 0)
                return _attributeValueDA.Update(attrValue); //
            else
                return _attributeValueDA.Insert(attrValue); //
        }

        public long DeleteAttributeValue(int id)
        {
            return _attributeValueDA.Delete(id);
        }
        #endregion
    }
}