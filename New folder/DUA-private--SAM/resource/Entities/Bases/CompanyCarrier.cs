using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "CompanyCarrier", Namespace = "http://www.piistech.com//entities")]
	public partial class CompanyCarrier : CompanyCarrierBase
	{
		#region Exernal Properties
		private Carrier _CarrierIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="Carrier"/>.
		/// </summary>
		/// <value>The source Carrier for _CarrierIdObject.</value>
		[DataMember]
		public Carrier CarrierIdObject
      	{
            get { return this._CarrierIdObject; }
            set { this._CarrierIdObject = value; }
      	}
		
		private Company _CompanyIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="Company"/>.
		/// </summary>
		/// <value>The source Company for _CompanyIdObject.</value>
		[DataMember]
		public Company CompanyIdObject
      	{
            get { return this._CompanyIdObject; }
            set { this._CompanyIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(CompanyCarrier))
            {
                return false;
            }			
			
			 CompanyCarrier _paramObj = obj as CompanyCarrier;
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