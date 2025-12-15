using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "DeliveryItemBase", Namespace = "http://www.piistech.com//entities")]
	public class DeliveryItemBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			DeliveryId = 1,
			SalesOrderDetailId = 2,
			Quantity = 3
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_DeliveryId = "DeliveryId";		            
		public const string Property_SalesOrderDetailId = "SalesOrderDetailId";		            
		public const string Property_Quantity = "Quantity";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _DeliveryId;	            
		private Int32 _SalesOrderDetailId;	            
		private Int32 _Quantity;	            
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
		public Int32 DeliveryId
		{	
			get{ return _DeliveryId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_DeliveryId, value, _DeliveryId);
				if (PropertyChanging(args))
				{
					_DeliveryId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 SalesOrderDetailId
		{	
			get{ return _SalesOrderDetailId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SalesOrderDetailId, value, _SalesOrderDetailId);
				if (PropertyChanging(args))
				{
					_SalesOrderDetailId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 Quantity
		{	
			get{ return _Quantity; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Quantity, value, _Quantity);
				if (PropertyChanging(args))
				{
					_Quantity = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  DeliveryItemBase Clone()
		{
			DeliveryItemBase newObj = new  DeliveryItemBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.DeliveryId = this.DeliveryId;						
			newObj.SalesOrderDetailId = this.SalesOrderDetailId;						
			newObj.Quantity = this.Quantity;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(DeliveryItemBase.Property_Id, Id);				
			info.AddValue(DeliveryItemBase.Property_DeliveryId, DeliveryId);				
			info.AddValue(DeliveryItemBase.Property_SalesOrderDetailId, SalesOrderDetailId);				
			info.AddValue(DeliveryItemBase.Property_Quantity, Quantity);				
		}
		#endregion

		
	}
}
