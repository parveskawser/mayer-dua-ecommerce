using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "SalesReturn", Namespace = "http://www.piistech.com//entities")]
	public partial class SalesReturn : SalesReturnBase
	{
		#region Exernal Properties
		private SalesOrderDetail _SalesOrderDetailIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="SalesOrderDetail"/>.
		/// </summary>
		/// <value>The source SalesOrderDetail for _SalesOrderDetailIdObject.</value>
		[DataMember]
		public SalesOrderDetail SalesOrderDetailIdObject
      	{
            get { return this._SalesOrderDetailIdObject; }
            set { this._SalesOrderDetailIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(SalesReturn))
            {
                return false;
            }			
			
			 SalesReturn _paramObj = obj as SalesReturn;
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
