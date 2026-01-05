using System;
using MDUA.Entities;
using MDUA.Entities.List;

namespace MDUA.Facade.Interface
{
    public interface IAttributeFacade
    {
        // Attribute Name Methods
        AttributeNameList GetAllAttributes();
        AttributeName GetAttribute(int id);
        long SaveAttribute(AttributeName attribute);
        long DeleteAttribute(int id);

        // Attribute Value Methods
        AttributeValueList GetValuesByAttributeId(int attributeId);
        AttributeValue GetValue(int id);
        long SaveAttributeValue(AttributeValue attrValue);
        long DeleteAttributeValue(int id);
    }
}