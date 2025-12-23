using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "DeliveryBase", Namespace = "http://www.piistech.com//entities")]
	public class DeliveryBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			SalesOrderId = 1,
			DeliveryDate = 2,
			TrackingNumber = 3,
			Status = 4,
			CreatedBy = 5,
			CreatedAt = 6,
			UpdatedBy = 7,
			UpdatedAt = 8
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_SalesOrderId = "SalesOrderId";		            
		public const string Property_DeliveryDate = "DeliveryDate";		            
		public const string Property_TrackingNumber = "TrackingNumber";		            
		public const string Property_Status = "Status";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _SalesOrderId;	            
		private DateTime _DeliveryDate;	            
		private String _TrackingNumber;	            
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
		public Int32 SalesOrderId
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
		public DateTime DeliveryDate
		{	
			get{ return _DeliveryDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_DeliveryDate, value, _DeliveryDate);
				if (PropertyChanging(args))
				{
					_DeliveryDate = value;
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
		public  DeliveryBase Clone()
		{
			DeliveryBase newObj = new  DeliveryBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.SalesOrderId = this.SalesOrderId;						
			newObj.DeliveryDate = this.DeliveryDate;						
			newObj.TrackingNumber = this.TrackingNumber;						
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
			info.AddValue(DeliveryBase.Property_Id, Id);				
			info.AddValue(DeliveryBase.Property_SalesOrderId, SalesOrderId);				
			info.AddValue(DeliveryBase.Property_DeliveryDate, DeliveryDate);				
			info.AddValue(DeliveryBase.Property_TrackingNumber, TrackingNumber);				
			info.AddValue(DeliveryBase.Property_Status, Status);				
			info.AddValue(DeliveryBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(DeliveryBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(DeliveryBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(DeliveryBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}
