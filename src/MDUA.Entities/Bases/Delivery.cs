using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "Delivery", Namespace = "http://www.piistech.com//entities")]
	public partial class Delivery : DeliveryBase
	{
		#region Exernal Properties
		private SalesOrderHeader _SalesOrderIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="SalesOrderHeader"/>.
		/// </summary>
		/// <value>The source SalesOrderHeader for _SalesOrderIdObject.</value>
		[DataMember]
		public SalesOrderHeader SalesOrderIdObject
      	{
            get { return this._SalesOrderIdObject; }
            set { this._SalesOrderIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(Delivery))
            {
                return false;
            }			
			
			 Delivery _paramObj = obj as Delivery;
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
