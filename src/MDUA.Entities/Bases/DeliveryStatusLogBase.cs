using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "DeliveryStatusLogBase", Namespace = "http://www.piistech.com//entities")]
	public class DeliveryStatusLogBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			EntityType = 1,
			EntityId = 2,
			SalesOrderId = 3,
			OldStatus = 4,
			NewStatus = 5,
			ChangeReason = 6,
			OldConfirmed = 7,
			NewConfirmed = 8,
			TrackingNumber = 9,
			ChangedBy = 10,
			ChangedAt = 11,
			IPAddress = 12,
			UserAgent = 13,
			IsSystemGenerated = 14,
			SourceAction = 15
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_EntityType = "EntityType";		            
		public const string Property_EntityId = "EntityId";		            
		public const string Property_SalesOrderId = "SalesOrderId";		            
		public const string Property_OldStatus = "OldStatus";		            
		public const string Property_NewStatus = "NewStatus";		            
		public const string Property_ChangeReason = "ChangeReason";		            
		public const string Property_OldConfirmed = "OldConfirmed";		            
		public const string Property_NewConfirmed = "NewConfirmed";		            
		public const string Property_TrackingNumber = "TrackingNumber";		            
		public const string Property_ChangedBy = "ChangedBy";		            
		public const string Property_ChangedAt = "ChangedAt";		            
		public const string Property_IPAddress = "IPAddress";		            
		public const string Property_UserAgent = "UserAgent";		            
		public const string Property_IsSystemGenerated = "IsSystemGenerated";		            
		public const string Property_SourceAction = "SourceAction";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _EntityType;	            
		private Int32 _EntityId;	            
		private Nullable<Int32> _SalesOrderId;	            
		private String _OldStatus;	            
		private String _NewStatus;	            
		private String _ChangeReason;	            
		private Nullable<Boolean> _OldConfirmed;	            
		private Nullable<Boolean> _NewConfirmed;	            
		private String _TrackingNumber;	            
		private String _ChangedBy;	            
		private DateTime _ChangedAt;	            
		private String _IPAddress;	            
		private String _UserAgent;	            
		private Boolean _IsSystemGenerated;	            
		private String _SourceAction;	            
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
		public String EntityType
		{	
			get{ return _EntityType; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_EntityType, value, _EntityType);
				if (PropertyChanging(args))
				{
					_EntityType = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 EntityId
		{	
			get{ return _EntityId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_EntityId, value, _EntityId);
				if (PropertyChanging(args))
				{
					_EntityId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Int32> SalesOrderId
		{	
			get{ return _SalesOrderId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SalesOrderId, value, _SalesOrderId);
				if (PropertyChanging(args))
				{
					_SalesOrderId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String OldStatus
		{	
			get{ return _OldStatus; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_OldStatus, value, _OldStatus);
				if (PropertyChanging(args))
				{
					_OldStatus = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String NewStatus
		{	
			get{ return _NewStatus; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_NewStatus, value, _NewStatus);
				if (PropertyChanging(args))
				{
					_NewStatus = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String ChangeReason
		{	
			get{ return _ChangeReason; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ChangeReason, value, _ChangeReason);
				if (PropertyChanging(args))
				{
					_ChangeReason = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Boolean> OldConfirmed
		{	
			get{ return _OldConfirmed; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_OldConfirmed, value, _OldConfirmed);
				if (PropertyChanging(args))
				{
					_OldConfirmed = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Boolean> NewConfirmed
		{	
			get{ return _NewConfirmed; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_NewConfirmed, value, _NewConfirmed);
				if (PropertyChanging(args))
				{
					_NewConfirmed = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String TrackingNumber
		{	
			get{ return _TrackingNumber; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_TrackingNumber, value, _TrackingNumber);
				if (PropertyChanging(args))
				{
					_TrackingNumber = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String ChangedBy
		{	
			get{ return _ChangedBy; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ChangedBy, value, _ChangedBy);
				if (PropertyChanging(args))
				{
					_ChangedBy = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime ChangedAt
		{	
			get{ return _ChangedAt; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ChangedAt, value, _ChangedAt);
				if (PropertyChanging(args))
				{
					_ChangedAt = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String IPAddress
		{	
			get{ return _IPAddress; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_IPAddress, value, _IPAddress);
				if (PropertyChanging(args))
				{
					_IPAddress = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String UserAgent
		{	
			get{ return _UserAgent; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_UserAgent, value, _UserAgent);
				if (PropertyChanging(args))
				{
					_UserAgent = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Boolean IsSystemGenerated
		{	
			get{ return _IsSystemGenerated; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_IsSystemGenerated, value, _IsSystemGenerated);
				if (PropertyChanging(args))
				{
					_IsSystemGenerated = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String SourceAction
		{	
			get{ return _SourceAction; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SourceAction, value, _SourceAction);
				if (PropertyChanging(args))
				{
					_SourceAction = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  DeliveryStatusLogBase Clone()
		{
			DeliveryStatusLogBase newObj = new  DeliveryStatusLogBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.EntityType = this.EntityType;						
			newObj.EntityId = this.EntityId;						
			newObj.SalesOrderId = this.SalesOrderId;						
			newObj.OldStatus = this.OldStatus;						
			newObj.NewStatus = this.NewStatus;						
			newObj.ChangeReason = this.ChangeReason;						
			newObj.OldConfirmed = this.OldConfirmed;						
			newObj.NewConfirmed = this.NewConfirmed;						
			newObj.TrackingNumber = this.TrackingNumber;						
			newObj.ChangedBy = this.ChangedBy;						
			newObj.ChangedAt = this.ChangedAt;						
			newObj.IPAddress = this.IPAddress;						
			newObj.UserAgent = this.UserAgent;						
			newObj.IsSystemGenerated = this.IsSystemGenerated;						
			newObj.SourceAction = this.SourceAction;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(DeliveryStatusLogBase.Property_Id, Id);				
			info.AddValue(DeliveryStatusLogBase.Property_EntityType, EntityType);				
			info.AddValue(DeliveryStatusLogBase.Property_EntityId, EntityId);				
			info.AddValue(DeliveryStatusLogBase.Property_SalesOrderId, SalesOrderId);				
			info.AddValue(DeliveryStatusLogBase.Property_OldStatus, OldStatus);				
			info.AddValue(DeliveryStatusLogBase.Property_NewStatus, NewStatus);				
			info.AddValue(DeliveryStatusLogBase.Property_ChangeReason, ChangeReason);				
			info.AddValue(DeliveryStatusLogBase.Property_OldConfirmed, OldConfirmed);				
			info.AddValue(DeliveryStatusLogBase.Property_NewConfirmed, NewConfirmed);				
			info.AddValue(DeliveryStatusLogBase.Property_TrackingNumber, TrackingNumber);				
			info.AddValue(DeliveryStatusLogBase.Property_ChangedBy, ChangedBy);				
			info.AddValue(DeliveryStatusLogBase.Property_ChangedAt, ChangedAt);				
			info.AddValue(DeliveryStatusLogBase.Property_IPAddress, IPAddress);				
			info.AddValue(DeliveryStatusLogBase.Property_UserAgent, UserAgent);				
			info.AddValue(DeliveryStatusLogBase.Property_IsSystemGenerated, IsSystemGenerated);				
			info.AddValue(DeliveryStatusLogBase.Property_SourceAction, SourceAction);				
		}
		#endregion

		
	}
}