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
			TrackingNumber = 2,
			ShipDate = 3,
			EstimatedArrival = 4,
			ActualDeliveryDate = 5,
			Status = 6,
			ShippingCost = 7,
			CreatedBy = 8,
			CreatedAt = 9,
			UpdatedBy = 10,
			UpdatedAt = 11,
			CarrierCharge = 12,
			PackageWeightGrams = 13,
			CarrierResponse = 14,
			ConsignmentId = 15,
			CarrierId = 16
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_SalesOrderId = "SalesOrderId";		            
		public const string Property_TrackingNumber = "TrackingNumber";		            
		public const string Property_ShipDate = "ShipDate";		            
		public const string Property_EstimatedArrival = "EstimatedArrival";		            
		public const string Property_ActualDeliveryDate = "ActualDeliveryDate";		            
		public const string Property_Status = "Status";		            
		public const string Property_ShippingCost = "ShippingCost";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		public const string Property_CarrierCharge = "CarrierCharge";		            
		public const string Property_PackageWeightGrams = "PackageWeightGrams";		            
		public const string Property_CarrierResponse = "CarrierResponse";		            
		public const string Property_ConsignmentId = "ConsignmentId";		            
		public const string Property_CarrierId = "CarrierId";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _SalesOrderId;	            
		private String _TrackingNumber;	            
		private Nullable<DateTime> _ShipDate;	            
		private Nullable<DateTime> _EstimatedArrival;	            
		private Nullable<DateTime> _ActualDeliveryDate;	            
		private String _Status;	            
		private Nullable<Decimal> _ShippingCost;	            
		private String _CreatedBy;	            
		private DateTime _CreatedAt;	            
		private String _UpdatedBy;	            
		private Nullable<DateTime> _UpdatedAt;	            
		private Nullable<Decimal> _CarrierCharge;	            
		private Nullable<Int32> _PackageWeightGrams;	            
		private String _CarrierResponse;	            
		private String _ConsignmentId;	            
		private Nullable<Int32> _CarrierId;	            
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
		public Nullable<DateTime> ShipDate
		{	
			get{ return _ShipDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ShipDate, value, _ShipDate);
				if (PropertyChanging(args))
				{
					_ShipDate = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<DateTime> EstimatedArrival
		{	
			get{ return _EstimatedArrival; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_EstimatedArrival, value, _EstimatedArrival);
				if (PropertyChanging(args))
				{
					_EstimatedArrival = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<DateTime> ActualDeliveryDate
		{	
			get{ return _ActualDeliveryDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ActualDeliveryDate, value, _ActualDeliveryDate);
				if (PropertyChanging(args))
				{
					_ActualDeliveryDate = value;
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
		public Nullable<Decimal> ShippingCost
		{	
			get{ return _ShippingCost; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ShippingCost, value, _ShippingCost);
				if (PropertyChanging(args))
				{
					_ShippingCost = value;
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

		[DataMember]
		public Nullable<Decimal> CarrierCharge
		{	
			get{ return _CarrierCharge; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CarrierCharge, value, _CarrierCharge);
				if (PropertyChanging(args))
				{
					_CarrierCharge = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Int32> PackageWeightGrams
		{	
			get{ return _PackageWeightGrams; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PackageWeightGrams, value, _PackageWeightGrams);
				if (PropertyChanging(args))
				{
					_PackageWeightGrams = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String CarrierResponse
		{	
			get{ return _CarrierResponse; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CarrierResponse, value, _CarrierResponse);
				if (PropertyChanging(args))
				{
					_CarrierResponse = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String ConsignmentId
		{	
			get{ return _ConsignmentId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ConsignmentId, value, _ConsignmentId);
				if (PropertyChanging(args))
				{
					_ConsignmentId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Int32> CarrierId
		{	
			get{ return _CarrierId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CarrierId, value, _CarrierId);
				if (PropertyChanging(args))
				{
					_CarrierId = value;
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
			newObj.TrackingNumber = this.TrackingNumber;						
			newObj.ShipDate = this.ShipDate;						
			newObj.EstimatedArrival = this.EstimatedArrival;						
			newObj.ActualDeliveryDate = this.ActualDeliveryDate;						
			newObj.Status = this.Status;						
			newObj.ShippingCost = this.ShippingCost;						
			newObj.CreatedBy = this.CreatedBy;						
			newObj.CreatedAt = this.CreatedAt;						
			newObj.UpdatedBy = this.UpdatedBy;						
			newObj.UpdatedAt = this.UpdatedAt;						
			newObj.CarrierCharge = this.CarrierCharge;						
			newObj.PackageWeightGrams = this.PackageWeightGrams;						
			newObj.CarrierResponse = this.CarrierResponse;						
			newObj.ConsignmentId = this.ConsignmentId;						
			newObj.CarrierId = this.CarrierId;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(DeliveryBase.Property_Id, Id);				
			info.AddValue(DeliveryBase.Property_SalesOrderId, SalesOrderId);				
			info.AddValue(DeliveryBase.Property_TrackingNumber, TrackingNumber);				
			info.AddValue(DeliveryBase.Property_ShipDate, ShipDate);				
			info.AddValue(DeliveryBase.Property_EstimatedArrival, EstimatedArrival);				
			info.AddValue(DeliveryBase.Property_ActualDeliveryDate, ActualDeliveryDate);				
			info.AddValue(DeliveryBase.Property_Status, Status);				
			info.AddValue(DeliveryBase.Property_ShippingCost, ShippingCost);				
			info.AddValue(DeliveryBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(DeliveryBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(DeliveryBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(DeliveryBase.Property_UpdatedAt, UpdatedAt);				
			info.AddValue(DeliveryBase.Property_CarrierCharge, CarrierCharge);				
			info.AddValue(DeliveryBase.Property_PackageWeightGrams, PackageWeightGrams);				
			info.AddValue(DeliveryBase.Property_CarrierResponse, CarrierResponse);				
			info.AddValue(DeliveryBase.Property_ConsignmentId, ConsignmentId);				
			info.AddValue(DeliveryBase.Property_CarrierId, CarrierId);				
		}
		#endregion

		
	}
}