using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "PaymentAllocationBase", Namespace = "http://www.piistech.com//entities")]
	public class PaymentAllocationBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			CustomerPaymentId = 1,
			SalesOrderId = 2,
			AllocatedAmount = 3,
			AllocatedDate = 4,
			Notes = 5,
			CreatedBy = 6,
			CreatedAt = 7,
			UpdatedBy = 8,
			UpdatedAt = 9
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_CustomerPaymentId = "CustomerPaymentId";		            
		public const string Property_SalesOrderId = "SalesOrderId";		            
		public const string Property_AllocatedAmount = "AllocatedAmount";		            
		public const string Property_AllocatedDate = "AllocatedDate";		            
		public const string Property_Notes = "Notes";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _CustomerPaymentId;	            
		private Nullable<Int32> _SalesOrderId;	            
		private Decimal _AllocatedAmount;	            
		private DateTime _AllocatedDate;	            
		private String _Notes;	            
		private String _CreatedBy;	            
		private Nullable<DateTime> _CreatedAt;	            
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
		public Int32 CustomerPaymentId
		{	
			get{ return _CustomerPaymentId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CustomerPaymentId, value, _CustomerPaymentId);
				if (PropertyChanging(args))
				{
					_CustomerPaymentId = value;
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
		public Decimal AllocatedAmount
		{	
			get{ return _AllocatedAmount; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AllocatedAmount, value, _AllocatedAmount);
				if (PropertyChanging(args))
				{
					_AllocatedAmount = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime AllocatedDate
		{	
			get{ return _AllocatedDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AllocatedDate, value, _AllocatedDate);
				if (PropertyChanging(args))
				{
					_AllocatedDate = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Notes
		{	
			get{ return _Notes; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Notes, value, _Notes);
				if (PropertyChanging(args))
				{
					_Notes = value;
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
		public Nullable<DateTime> CreatedAt
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
		public  PaymentAllocationBase Clone()
		{
			PaymentAllocationBase newObj = new  PaymentAllocationBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.CustomerPaymentId = this.CustomerPaymentId;						
			newObj.SalesOrderId = this.SalesOrderId;						
			newObj.AllocatedAmount = this.AllocatedAmount;						
			newObj.AllocatedDate = this.AllocatedDate;						
			newObj.Notes = this.Notes;						
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
			info.AddValue(PaymentAllocationBase.Property_Id, Id);				
			info.AddValue(PaymentAllocationBase.Property_CustomerPaymentId, CustomerPaymentId);				
			info.AddValue(PaymentAllocationBase.Property_SalesOrderId, SalesOrderId);				
			info.AddValue(PaymentAllocationBase.Property_AllocatedAmount, AllocatedAmount);				
			info.AddValue(PaymentAllocationBase.Property_AllocatedDate, AllocatedDate);				
			info.AddValue(PaymentAllocationBase.Property_Notes, Notes);				
			info.AddValue(PaymentAllocationBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(PaymentAllocationBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(PaymentAllocationBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(PaymentAllocationBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}
