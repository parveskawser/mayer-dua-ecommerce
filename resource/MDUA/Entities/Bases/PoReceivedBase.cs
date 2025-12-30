using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "PoReceivedBase", Namespace = "http://www.piistech.com//entities")]
	public class PoReceivedBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			PoRequestedId = 1,
			ReceivedQuantity = 2,
			BuyingPrice = 3,
			ReceivedDate = 4,
			CreatedBy = 5,
			CreatedAt = 6,
			UpdatedBy = 7,
			UpdatedAt = 8,
			Remarks = 9,
			InvoiceNo = 10
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_PoRequestedId = "PoRequestedId";		            
		public const string Property_ReceivedQuantity = "ReceivedQuantity";		            
		public const string Property_BuyingPrice = "BuyingPrice";		            
		public const string Property_ReceivedDate = "ReceivedDate";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		public const string Property_Remarks = "Remarks";		            
		public const string Property_InvoiceNo = "InvoiceNo";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _PoRequestedId;	            
		private Int32 _ReceivedQuantity;	            
		private Decimal _BuyingPrice;	            
		private DateTime _ReceivedDate;	            
		private String _CreatedBy;	            
		private DateTime _CreatedAt;	            
		private String _UpdatedBy;	            
		private Nullable<DateTime> _UpdatedAt;	            
		private String _Remarks;	            
		private String _InvoiceNo;	            
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
		public Int32 PoRequestedId
		{	
			get{ return _PoRequestedId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PoRequestedId, value, _PoRequestedId);
				if (PropertyChanging(args))
				{
					_PoRequestedId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 ReceivedQuantity
		{	
			get{ return _ReceivedQuantity; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ReceivedQuantity, value, _ReceivedQuantity);
				if (PropertyChanging(args))
				{
					_ReceivedQuantity = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Decimal BuyingPrice
		{	
			get{ return _BuyingPrice; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_BuyingPrice, value, _BuyingPrice);
				if (PropertyChanging(args))
				{
					_BuyingPrice = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime ReceivedDate
		{	
			get{ return _ReceivedDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ReceivedDate, value, _ReceivedDate);
				if (PropertyChanging(args))
				{
					_ReceivedDate = value;
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
		public String Remarks
		{	
			get{ return _Remarks; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Remarks, value, _Remarks);
				if (PropertyChanging(args))
				{
					_Remarks = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String InvoiceNo
		{	
			get{ return _InvoiceNo; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_InvoiceNo, value, _InvoiceNo);
				if (PropertyChanging(args))
				{
					_InvoiceNo = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  PoReceivedBase Clone()
		{
			PoReceivedBase newObj = new  PoReceivedBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.PoRequestedId = this.PoRequestedId;						
			newObj.ReceivedQuantity = this.ReceivedQuantity;						
			newObj.BuyingPrice = this.BuyingPrice;						
			newObj.ReceivedDate = this.ReceivedDate;						
			newObj.CreatedBy = this.CreatedBy;						
			newObj.CreatedAt = this.CreatedAt;						
			newObj.UpdatedBy = this.UpdatedBy;						
			newObj.UpdatedAt = this.UpdatedAt;						
			newObj.Remarks = this.Remarks;						
			newObj.InvoiceNo = this.InvoiceNo;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(PoReceivedBase.Property_Id, Id);				
			info.AddValue(PoReceivedBase.Property_PoRequestedId, PoRequestedId);				
			info.AddValue(PoReceivedBase.Property_ReceivedQuantity, ReceivedQuantity);				
			info.AddValue(PoReceivedBase.Property_BuyingPrice, BuyingPrice);				
			info.AddValue(PoReceivedBase.Property_ReceivedDate, ReceivedDate);				
			info.AddValue(PoReceivedBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(PoReceivedBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(PoReceivedBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(PoReceivedBase.Property_UpdatedAt, UpdatedAt);				
			info.AddValue(PoReceivedBase.Property_Remarks, Remarks);				
			info.AddValue(PoReceivedBase.Property_InvoiceNo, InvoiceNo);				
		}
		#endregion

		
	}
}