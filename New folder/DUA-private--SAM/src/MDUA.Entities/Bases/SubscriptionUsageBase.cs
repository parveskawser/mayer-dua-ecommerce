using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "SubscriptionUsageBase", Namespace = "http://www.piistech.com//entities")]
	public class SubscriptionUsageBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			SubscriptionId = 1,
			CycleStart = 2,
			CycleEnd = 3,
			OrdersProcessed = 4,
			CreatedAt = 5,
			UpdatedAt = 6
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_SubscriptionId = "SubscriptionId";		            
		public const string Property_CycleStart = "CycleStart";		            
		public const string Property_CycleEnd = "CycleEnd";		            
		public const string Property_OrdersProcessed = "OrdersProcessed";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _SubscriptionId;	            
		private DateTime _CycleStart;	            
		private DateTime _CycleEnd;	            
		private Int32 _OrdersProcessed;	            
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
		public Int32 SubscriptionId
		{	
			get{ return _SubscriptionId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SubscriptionId, value, _SubscriptionId);
				if (PropertyChanging(args))
				{
					_SubscriptionId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime CycleStart
		{	
			get{ return _CycleStart; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CycleStart, value, _CycleStart);
				if (PropertyChanging(args))
				{
					_CycleStart = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime CycleEnd
		{	
			get{ return _CycleEnd; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CycleEnd, value, _CycleEnd);
				if (PropertyChanging(args))
				{
					_CycleEnd = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 OrdersProcessed
		{	
			get{ return _OrdersProcessed; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_OrdersProcessed, value, _OrdersProcessed);
				if (PropertyChanging(args))
				{
					_OrdersProcessed = value;
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
		public  SubscriptionUsageBase Clone()
		{
			SubscriptionUsageBase newObj = new  SubscriptionUsageBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.SubscriptionId = this.SubscriptionId;						
			newObj.CycleStart = this.CycleStart;						
			newObj.CycleEnd = this.CycleEnd;						
			newObj.OrdersProcessed = this.OrdersProcessed;						
			newObj.CreatedAt = this.CreatedAt;						
			newObj.UpdatedAt = this.UpdatedAt;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(SubscriptionUsageBase.Property_Id, Id);				
			info.AddValue(SubscriptionUsageBase.Property_SubscriptionId, SubscriptionId);				
			info.AddValue(SubscriptionUsageBase.Property_CycleStart, CycleStart);				
			info.AddValue(SubscriptionUsageBase.Property_CycleEnd, CycleEnd);				
			info.AddValue(SubscriptionUsageBase.Property_OrdersProcessed, OrdersProcessed);				
			info.AddValue(SubscriptionUsageBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(SubscriptionUsageBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}