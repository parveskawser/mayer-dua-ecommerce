using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "SubscriptionPlanBase", Namespace = "http://www.piistech.com//entities")]
	public class SubscriptionPlanBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			PlanCode = 1,
			PlanName = 2,
			DefaultsJSON = 3,
			BasePrice = 4,
			DiscountPrice = 5,
			CurrencyCode = 6,
			IsActive = 7,
			CreatedAt = 8,
			UpdatedAt = 9
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_PlanCode = "PlanCode";		            
		public const string Property_PlanName = "PlanName";		            
		public const string Property_DefaultsJSON = "DefaultsJSON";		            
		public const string Property_BasePrice = "BasePrice";		            
		public const string Property_DiscountPrice = "DiscountPrice";		            
		public const string Property_CurrencyCode = "CurrencyCode";		            
		public const string Property_IsActive = "IsActive";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _PlanCode;	            
		private String _PlanName;	            
		private String _DefaultsJSON;	            
		private Decimal _BasePrice;	            
		private Nullable<Decimal> _DiscountPrice;	            
		private String _CurrencyCode;	            
		private Boolean _IsActive;
        private DateTime _CreatedAt;
        private Nullable<DateTime> _UpdatedAt;
        #endregion

        #region Properties		
        [DataMember]
		public Int32 Id
		{	
			get{ return _Id; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Id, value, _Id);
				if (PropertyChanging(args))
				{
					_Id = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String PlanCode
		{	
			get{ return _PlanCode; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PlanCode, value, _PlanCode);
				if (PropertyChanging(args))
				{
					_PlanCode = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String PlanName
		{	
			get{ return _PlanName; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PlanName, value, _PlanName);
				if (PropertyChanging(args))
				{
					_PlanName = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String DefaultsJSON
		{	
			get{ return _DefaultsJSON; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_DefaultsJSON, value, _DefaultsJSON);
				if (PropertyChanging(args))
				{
					_DefaultsJSON = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Decimal BasePrice
		{	
			get{ return _BasePrice; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_BasePrice, value, _BasePrice);
				if (PropertyChanging(args))
				{
					_BasePrice = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Decimal> DiscountPrice
		{	
			get{ return _DiscountPrice; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_DiscountPrice, value, _DiscountPrice);
				if (PropertyChanging(args))
				{
					_DiscountPrice = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String CurrencyCode
		{	
			get{ return _CurrencyCode; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CurrencyCode, value, _CurrencyCode);
				if (PropertyChanging(args))
				{
					_CurrencyCode = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Boolean IsActive
		{	
			get{ return _IsActive; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_IsActive, value, _IsActive);
				if (PropertyChanging(args))
				{
					_IsActive = value;
					PropertyChanged(args);					
				}	
			}
        }

        [DataMember]
        public DateTime CreatedAt
        {
            get { return _CreatedAt; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CreatedAt, value, _CreatedAt);
                if (PropertyChanging(args))
                {
                    _CreatedAt = value;
                    PropertyChanged(args);
                }
            }
        }

        [DataMember]
        public Nullable<DateTime> UpdatedAt
        {
            get { return _UpdatedAt; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_UpdatedAt, value, _UpdatedAt);
                if (PropertyChanging(args))
                {
                    _UpdatedAt = value;
                    PropertyChanged(args);
                }
            }
        }

        #endregion

        #region Cloning Base Objects
        public  SubscriptionPlanBase Clone()
		{
			SubscriptionPlanBase newObj = new  SubscriptionPlanBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.PlanCode = this.PlanCode;						
			newObj.PlanName = this.PlanName;						
			newObj.DefaultsJSON = this.DefaultsJSON;						
			newObj.BasePrice = this.BasePrice;						
			newObj.DiscountPrice = this.DiscountPrice;						
			newObj.CurrencyCode = this.CurrencyCode;						
			newObj.IsActive = this.IsActive;						
			newObj.CreatedAt = this.CreatedAt;						
			newObj.UpdatedAt = this.UpdatedAt;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(SubscriptionPlanBase.Property_Id, Id);				
			info.AddValue(SubscriptionPlanBase.Property_PlanCode, PlanCode);				
			info.AddValue(SubscriptionPlanBase.Property_PlanName, PlanName);				
			info.AddValue(SubscriptionPlanBase.Property_DefaultsJSON, DefaultsJSON);				
			info.AddValue(SubscriptionPlanBase.Property_BasePrice, BasePrice);				
			info.AddValue(SubscriptionPlanBase.Property_DiscountPrice, DiscountPrice);				
			info.AddValue(SubscriptionPlanBase.Property_CurrencyCode, CurrencyCode);				
			info.AddValue(SubscriptionPlanBase.Property_IsActive, IsActive);				
			info.AddValue(SubscriptionPlanBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(SubscriptionPlanBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}