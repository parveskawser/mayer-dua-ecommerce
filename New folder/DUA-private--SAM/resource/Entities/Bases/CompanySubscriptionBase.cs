using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "CompanySubscriptionBase", Namespace = "http://www.piistech.com//entities")]
	public class CompanySubscriptionBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			CompanyId = 1,
			SubscriptionPlanId = 2,
			PlanNameSnapshot = 3,
			MaxProducts = 4,
			MaxOrders = 5,
			OrderCycle = 6,
			PriceCharged = 7,
			CurrencyCode = 8,
			StartDate = 9,
			EndDate = 10,
			NextBillingDate = 11,
			CycleAnchorDate = 12,
			Status = 13,
			CreatedBy = 14,
			CreatedAt = 15,
			UpdatedBy = 16,
			UpdatedAt = 17
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_CompanyId = "CompanyId";		            
		public const string Property_SubscriptionPlanId = "SubscriptionPlanId";		            
		public const string Property_PlanNameSnapshot = "PlanNameSnapshot";		            
		public const string Property_MaxProducts = "MaxProducts";		            
		public const string Property_MaxOrders = "MaxOrders";		            
		public const string Property_OrderCycle = "OrderCycle";		            
		public const string Property_PriceCharged = "PriceCharged";		            
		public const string Property_CurrencyCode = "CurrencyCode";		            
		public const string Property_StartDate = "StartDate";		            
		public const string Property_EndDate = "EndDate";		            
		public const string Property_NextBillingDate = "NextBillingDate";		            
		public const string Property_CycleAnchorDate = "CycleAnchorDate";		            
		public const string Property_Status = "Status";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _CompanyId;	            
		private Nullable<Int32> _SubscriptionPlanId;	            
		private String _PlanNameSnapshot;	            
		private Int32 _MaxProducts;	            
		private Int32 _MaxOrders;	            
		private String _OrderCycle;	            
		private Decimal _PriceCharged;	            
		private String _CurrencyCode;	            
		private DateTime _StartDate;	            
		private Nullable<DateTime> _EndDate;	            
		private Nullable<DateTime> _NextBillingDate;	            
		private Nullable<DateTime> _CycleAnchorDate;	            
		private String _Status;	            
		private String _CreatedBy;	            
		private DateTime _CreatedAt;	            
		private String _UpdatedBy;	            
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
		public Int32 CompanyId
		{	
			get{ return _CompanyId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CompanyId, value, _CompanyId);
				if (PropertyChanging(args))
				{
					_CompanyId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Int32> SubscriptionPlanId
		{	
			get{ return _SubscriptionPlanId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SubscriptionPlanId, value, _SubscriptionPlanId);
				if (PropertyChanging(args))
				{
					_SubscriptionPlanId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String PlanNameSnapshot
		{	
			get{ return _PlanNameSnapshot; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PlanNameSnapshot, value, _PlanNameSnapshot);
				if (PropertyChanging(args))
				{
					_PlanNameSnapshot = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 MaxProducts
		{	
			get{ return _MaxProducts; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_MaxProducts, value, _MaxProducts);
				if (PropertyChanging(args))
				{
					_MaxProducts = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 MaxOrders
		{	
			get{ return _MaxOrders; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_MaxOrders, value, _MaxOrders);
				if (PropertyChanging(args))
				{
					_MaxOrders = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String OrderCycle
		{	
			get{ return _OrderCycle; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_OrderCycle, value, _OrderCycle);
				if (PropertyChanging(args))
				{
					_OrderCycle = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Decimal PriceCharged
		{	
			get{ return _PriceCharged; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PriceCharged, value, _PriceCharged);
				if (PropertyChanging(args))
				{
					_PriceCharged = value;
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
		public DateTime StartDate
		{	
			get{ return _StartDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_StartDate, value, _StartDate);
				if (PropertyChanging(args))
				{
					_StartDate = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<DateTime> EndDate
		{	
			get{ return _EndDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_EndDate, value, _EndDate);
				if (PropertyChanging(args))
				{
					_EndDate = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<DateTime> NextBillingDate
		{	
			get{ return _NextBillingDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_NextBillingDate, value, _NextBillingDate);
				if (PropertyChanging(args))
				{
					_NextBillingDate = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<DateTime> CycleAnchorDate
		{	
			get{ return _CycleAnchorDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CycleAnchorDate, value, _CycleAnchorDate);
				if (PropertyChanging(args))
				{
					_CycleAnchorDate = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Status
		{	
			get{ return _Status; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Status, value, _Status);
				if (PropertyChanging(args))
				{
					_Status = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String CreatedBy
		{	
			get{ return _CreatedBy; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CreatedBy, value, _CreatedBy);
				if (PropertyChanging(args))
				{
					_CreatedBy = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime CreatedAt
		{	
			get{ return _CreatedAt; }			
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
		public String UpdatedBy
		{	
			get{ return _UpdatedBy; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_UpdatedBy, value, _UpdatedBy);
				if (PropertyChanging(args))
				{
					_UpdatedBy = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<DateTime> UpdatedAt
		{	
			get{ return _UpdatedAt; }			
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
		public  CompanySubscriptionBase Clone()
		{
			CompanySubscriptionBase newObj = new  CompanySubscriptionBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.CompanyId = this.CompanyId;						
			newObj.SubscriptionPlanId = this.SubscriptionPlanId;						
			newObj.PlanNameSnapshot = this.PlanNameSnapshot;						
			newObj.MaxProducts = this.MaxProducts;						
			newObj.MaxOrders = this.MaxOrders;						
			newObj.OrderCycle = this.OrderCycle;						
			newObj.PriceCharged = this.PriceCharged;						
			newObj.CurrencyCode = this.CurrencyCode;						
			newObj.StartDate = this.StartDate;						
			newObj.EndDate = this.EndDate;						
			newObj.NextBillingDate = this.NextBillingDate;						
			newObj.CycleAnchorDate = this.CycleAnchorDate;						
			newObj.Status = this.Status;						
			newObj.CreatedBy = this.CreatedBy;						
			newObj.CreatedAt = this.CreatedAt;						
			newObj.UpdatedBy = this.UpdatedBy;						
			newObj.UpdatedAt = this.UpdatedAt;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(CompanySubscriptionBase.Property_Id, Id);				
			info.AddValue(CompanySubscriptionBase.Property_CompanyId, CompanyId);				
			info.AddValue(CompanySubscriptionBase.Property_SubscriptionPlanId, SubscriptionPlanId);				
			info.AddValue(CompanySubscriptionBase.Property_PlanNameSnapshot, PlanNameSnapshot);				
			info.AddValue(CompanySubscriptionBase.Property_MaxProducts, MaxProducts);				
			info.AddValue(CompanySubscriptionBase.Property_MaxOrders, MaxOrders);				
			info.AddValue(CompanySubscriptionBase.Property_OrderCycle, OrderCycle);				
			info.AddValue(CompanySubscriptionBase.Property_PriceCharged, PriceCharged);				
			info.AddValue(CompanySubscriptionBase.Property_CurrencyCode, CurrencyCode);				
			info.AddValue(CompanySubscriptionBase.Property_StartDate, StartDate);				
			info.AddValue(CompanySubscriptionBase.Property_EndDate, EndDate);				
			info.AddValue(CompanySubscriptionBase.Property_NextBillingDate, NextBillingDate);				
			info.AddValue(CompanySubscriptionBase.Property_CycleAnchorDate, CycleAnchorDate);				
			info.AddValue(CompanySubscriptionBase.Property_Status, Status);				
			info.AddValue(CompanySubscriptionBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(CompanySubscriptionBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(CompanySubscriptionBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(CompanySubscriptionBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}