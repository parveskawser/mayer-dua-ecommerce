using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace MDUA.Framework
{
    /// <summary>
    /// Some properties are not mapped while retrieving from database and populating objects, 
    /// but can be still retrieved. And this class holds the values as Key Value pairs.
    /// This is usually needed for retrieving data from views.
    /// </summary>
    [Serializable]
    [DataContract]
    public class CustomProperty
    {
        private object _Key;
        private object _Value;
        [DataMember]
        public object Key
        {
            get { return _Key; }
            set { _Key = value; }
        }
        [DataMember]
        public object Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public CustomProperty() { }

        public CustomProperty(object key, object value)
        {
            _Key = key;
            _Value = value;
        }
    }

    /// <summary>
    /// List of Custom Properties. As non-mapped items retrieved from database could be more than one
    /// </summary>
    [Serializable]
    [CollectionDataContract]
    public class CustomProperties : List<CustomProperty>
    {
        /// <summary>
        /// Finds a value using key of non-mapped [with object/entity] data item
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CustomProperty FindByKey(object key)
        {
            return Find(delegate(CustomProperty prop) { return prop.Key.Equals(key); });
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[object key]
        {
            get
            {
                CustomProperty prop = FindByKey(key);
                if (prop == null)
                    return null;
                else
                    return prop.Value;
            }
            set
            {
                CustomProperty prop = FindByKey(key);
                if (prop == null)
                    Add(new CustomProperty(key, value));
                else
                    prop.Value = value;
            }
        }
    }
}
