using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml;
using System.Collections.Generic;
using MDUA.Framework.Objects;

namespace MDUA.Framework
{
    /// <summary>
    /// Class PropertyChangingEventArgs
    /// extends EventArgs
    /// provides advanced and property specific event handling mechanisms
    /// </summary>
    public class PropertyChangingEventArgs : EventArgs
    {
        private string _PropertyName;
        private object _Value;
        private object _OldValue;

        public object OldValue
        {
            get
            {
                return _OldValue;
            }
            set
            {
                _OldValue = value;
            }
        } 
        /// <summary>
        /// gets or sets the property name
        /// </summary>
        public string PropertyName
        {
            get
            {
                return _PropertyName;
            }
            set
            {
                _PropertyName = value;
            }
        } 
        /// <summary>
        /// gets or sets the value of the changing property
        /// </summary>
        public object Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }

        /// <summary>
        /// The constructor : PropertyChangingEventArgs
        /// </summary>
        /// <param name="propertyName">a string that represents a property name</param>
        /// <param name="value">an object the holds the value</param>
        public PropertyChangingEventArgs(string propertyName, object value, object oldValue)
        {
            PropertyName = propertyName;
            Value = value;
            OldValue = oldValue;
        }
    }

    /// <summary>
    /// Delegate: PropertyChangingDelegate
    /// returns a bool value that depends on the success of the operation spcified
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public delegate bool PropertyChangingDelegate(object sender, PropertyChangingEventArgs e);
    /// <summary>
    /// delegate: PropertyChangingDelegate
    /// dosen't return any value
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PropertyChangedDelegate(object sender, PropertyChangingEventArgs e);

    /// <summary>
    /// abstract class BaseBusinessEntity
    /// extends IDisposable
    /// provides basic funtionalities for business entities
    /// </summary>
    [Serializable]
    [DataContract]
    public abstract class BaseBusinessEntity : IDisposable//, ISerializable
    {
        private Dictionary<string, object> _InitialValues = new Dictionary<string, object>();
        private Dictionary<string,object> _Changes = new Dictionary<string,object>();

        public PropertyLog[] GetInitialLogs()
        {
            List<PropertyLog> logs = new List<PropertyLog>();
            foreach (KeyValuePair<string, object> pair in _InitialValues)
            {
                PropertyChangingEventArgs args = (PropertyChangingEventArgs)pair.Value;

                if (args.OldValue == null || args.OldValue.Equals(args.Value) == false)
                {
                    logs.Add(new PropertyLog()
                             {
                                 Name = this.GetType().Name + "." + args.PropertyName,
                                 OldValue = args.OldValue,
                                 NewValue = args.Value
                             });
                }
            }

            return logs.ToArray();
        }

        public PropertyLog[] GetChangeLogs()
        {
            List<PropertyLog> logs = new List<PropertyLog>();
            foreach (KeyValuePair<string, object> pair in _Changes)
            {
                PropertyChangingEventArgs args = (PropertyChangingEventArgs)pair.Value;

                if (args.OldValue == null || args.OldValue.Equals(args.Value) == false)
                {
                    logs.Add(new PropertyLog() {
                                 Name = this.GetType().Name + "." + args.PropertyName,
                                 OldValue = args.OldValue,
                                 NewValue = args.Value
                             });
                }
            }

            return logs.ToArray();
        }
        
        private void SetHistory(string propertyName, object newValue)
        {
            if (_Changes == null)
                return;

            if (_Changes.ContainsKey(propertyName))
            {
                PropertyChangingEventArgs oldArgs = (PropertyChangingEventArgs)_Changes[propertyName];
                    
                if (newValue is PropertyChangingEventArgs)
                {
                    oldArgs.Value = ((PropertyChangingEventArgs)newValue).Value;
                }
            }
            else
            {
                PropertyChangingEventArgs args = (PropertyChangingEventArgs)newValue;
                if (args.OldValue != null)
                {
                    if (!args.OldValue.Equals(args.Value))
                        _Changes.Add(propertyName, newValue);
                }
                else
                {
                    if (args.Value != null)
                        _Changes.Add(propertyName, newValue);
                }
            }
        }

        /// <summary>
        /// Enumarates the Row States
        /// </summary>
        public enum RowStateEnum
        {
            NormalRow = 0,
            NewRow = 1,
            UpdatedRow = 2,
            DeletedRow = 3
        }

        //The common fields in the database
        //public const string Property_CreatorID = "CreatorID";
        //public const string Property_CreatedTimeStamp = "CreatedTimeStamp";
        //public const string Property_ModifierID = "ModifierID";
        //public const string Property_ModifiedTimeStamp = "ModifiedTimeStamp";
        //public const string Property_IsDeleted = "IsDeleted";

        //the private variables
        private RowStateEnum _RowState = RowStateEnum.NewRow;
        //private long _CreatorID;
        //private DateTime _CreatedTimeStamp;
        //private long _ModifierID;
        //private DateTime _ModifiedTimeStamp;
        //private bool _IsDeleted;
        private CustomProperties _CustomProperties;

        [field: NonSerialized]
        public event PropertyChangingDelegate OnPropertyChanging;
        [field: NonSerialized]
        public event PropertyChangingDelegate OnPropertyChanged;

        /// <summary>
        ///gets or sets the row state of an object 
        /// </summary>
        [DataMember]
        public RowStateEnum RowState
        {
            get
            {
                return _RowState;
            }
            set
            {
                _RowState = value;
            }
        }

        ///// <summary>
        ///// gets or sets the ID of the creator
        ///// </summary>
        //public long CreatorID
        //{
        //    get { return _CreatorID; }
        //    set { _CreatorID = value; }
        //}

        ///// <summary>
        ///// gets or sets the time when the object was created
        ///// </summary>
        //public DateTime CreatedTimeStamp
        //{
        //    get { return _CreatedTimeStamp; }
        //    set { _CreatedTimeStamp = value; }
        //}

        ///// <summary>
        ///// gets or sets the ID of the modifier
        ///// </summary>
        //public long ModifierID
        //{
        //    get { return _ModifierID; }
        //    set { _ModifierID = value; }
        //}

        ///// <summary>
        ///// gets or sets the time when the object was last modified 
        ///// </summary>
        //public DateTime ModifiedTimeStamp
        //{
        //    get { return _ModifiedTimeStamp; }
        //    set { _ModifiedTimeStamp = value; }
        //}

        ///// <summary>
        ///// has the entry been marked as deleted
        ///// </summary>
        //public bool IsDeleted
        //{
        //    get
        //    {
        //        if (_RowState == RowStateEnum.DeletedRow)
        //            return true;

        //        return false;
        //    }
        //    //get { return _IsDeleted; }
        //    //set { _IsDeleted = value; }
        //}

        /// <summary>
        /// gets or sets the CustomProperties
        /// </summary>
        public CustomProperties CustomProperties
        {
            get
            {
                return _CustomProperties;
            }
            set
            {
                _CustomProperties = value;
            }
        }

        /// <summary>
        /// the constructor
        /// </summary>
        protected BaseBusinessEntity()
        {
            _RowState = RowStateEnum.NewRow;
        }

        //protected void ChangeProperty(string propName, object value)
        //{
        //    PropertyChangingEventArgs args = new PropertyChangingEventArgs(propName, value);

        //    if (PropertyChanging(args))
        //    {
        //        SetPropertyValue(args);
        //        PropertyChanged(args);
        //    }
        //}

        //protected void ChangeProperty(string propName, ref object container, object value)
        //{
        //    PropertyChangingEventArgs args = new PropertyChangingEventArgs(propName, value);

        //    if (PropertyChanging(args))
        //    {
        //        container = value;
        //        PropertyChanged(args);
        //    }
        //}

        //protected void SetPropertyValue(PropertyChangingEventArgs args)
        //{
        //    Type objType = this.GetType();
        //    System.Reflection.PropertyInfo propInfo = objType.GetProperty(args.PropertyName);
        //    propInfo.SetValue(this, args.Value, null);
        //}

        /// <summary>
        /// virtual method: PropertyChanging
        /// if the OnPropertyChanging is not null then calls the delegate
        /// </summary>
        /// <param name="args">an instance of type PropertyChangingEventArgs</param>
        /// <returns></returns>
        public virtual bool PropertyChanging(PropertyChangingEventArgs args)
        {
            if (OnPropertyChanging != null)
                return OnPropertyChanging(this, args);

            return true;
        }

        /// <summary>
        /// virtual method: PropertyChanged
        /// it sets the row state to Updates row if the previous state was normal row
        /// and if OnPropertyChanged dlegate is not empty, then calls the delegate
        /// </summary>
        /// <param name="args"></param>
        public virtual void PropertyChanged(PropertyChangingEventArgs args)
        {
            if (RowState == RowStateEnum.NormalRow || RowState == RowStateEnum.UpdatedRow)
            {
                RowState = RowStateEnum.UpdatedRow;

                SetHistory(args.PropertyName, args);
            }
            else
            {
                if (_InitialValues.ContainsKey(args.PropertyName))
                {
                    PropertyChangingEventArgs oldArgs = (PropertyChangingEventArgs)_InitialValues[args.PropertyName];
                    oldArgs.Value = args.Value;
                    oldArgs.OldValue = args.OldValue;
                }
                else
                    _InitialValues.Add(args.PropertyName, args);
            }

            if (OnPropertyChanged != null)
                OnPropertyChanged(this, args);
        }

        /// <summary>
        /// Method: UpdatedPredicate 
        /// checks if some object is updated or not
        /// </summary>
        /// <param name="obj">an instance of BaseBusinessEntity</param>
        /// <returns></returns>
        public bool UpdatedPredicate(BaseBusinessEntity obj)
        {
            return (obj.RowState == RowStateEnum.UpdatedRow);
        }

        /// <summary>
        /// virtual method Dispose
        /// </summary>
        public virtual void Dispose()
        {
            //GC.SuppressFinalize(this);
        }


        /// <summary>
        /// method CloneBase
        /// makes a clone of the base business entity properties
        /// </summary>
        /// <param name="newObj"></param>
        protected void CloneBase(BaseBusinessEntity newObj)
        {
            //newObj.CreatorID = this.CreatorID;
            //newObj.CreatedTimeStamp = this.CreatedTimeStamp;
            //newObj.ModifierID = this.ModifierID;
            //newObj.ModifiedTimeStamp = this.ModifiedTimeStamp;
            //newObj.IsDeleted = this.IsDeleted;
        }

        /// <summary>
        /// method GetObjectData
        /// gets the data of this current object..
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //info.AddValue(BaseBusinessEntity.Property_CreatorID, this.CreatorID);
            //info.AddValue(BaseBusinessEntity.Property_CreatedTimeStamp, this.CreatedTimeStamp);
            //info.AddValue(BaseBusinessEntity.Property_ModifierID, this.ModifierID);
            //info.AddValue(BaseBusinessEntity.Property_ModifiedTimeStamp, this.ModifiedTimeStamp);
            //info.AddValue(BaseBusinessEntity.Property_IsDeleted, this.IsDeleted);
        }

        public bool CustomPropertyMatch(object obj)
        {
            BaseBusinessEntity temp = obj as BaseBusinessEntity;
            if (temp != null)
            {
                if (this.CustomProperties != null)
                {
                    foreach (CustomProperty actualProp in this.CustomProperties)
                    {
                        if (temp.CustomProperties[actualProp.Key] == null ||
                            actualProp.Value != temp.CustomProperties[actualProp.Key])
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

    }
}
