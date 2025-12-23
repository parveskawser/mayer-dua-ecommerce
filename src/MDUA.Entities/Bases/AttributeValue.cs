using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "AttributeValue", Namespace = "http://www.piistech.com//entities")]
	public partial class AttributeValue : AttributeValueBase
	{
		#region Exernal Properties
		private AttributeName _AttributeIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="AttributeName"/>.
		/// </summary>
		/// <value>The source AttributeName for _AttributeIdObject.</value>
		[DataMember]
		public AttributeName AttributeIdObject
      	{
            get { return this._AttributeIdObject; }
            set { this._AttributeIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(AttributeValue))
            {
                return false;
            }			
			
			 AttributeValue _paramObj = obj as AttributeValue;
            if (_paramObj != null)
            {			
                return (_paramObj.Id == this.Id && _paramObj.CustomPropertyMatch(this));
            }
            else
            {
                return base.Equals(obj);
            }
		}
		#endregion
		
		#region Orverride HashCode
		 public override int GetHashCode()
        {
            return base.Id.GetHashCode();
        }
		#endregion		
	}
}
