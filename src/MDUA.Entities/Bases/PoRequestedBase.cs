using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "PoRequestedBase", Namespace = "http://www.piistech.com//entities")]
	public class PoRequestedBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			VendorId = 1,
			Quantity = 2,
			RequestDate = 3,
			Status = 4,
			CreatedBy = 5,
			CreatedAt = 6,
			UpdatedBy = 7,
			UpdatedAt = 8,
			Remarks = 9,
			ReferenceNo = 10,
			ProductVariantId = 11,
			BulkPurchaseOrderId = 12
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_VendorId = "VendorId";		            
		public const string Property_Quantity = "Quantity";		            
		public const string Property_RequestDate = "RequestDate";		            
		public const string Property_Status = "Status";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		public const string Property_Remarks = "Remarks";		            
		public const string Property_ReferenceNo = "ReferenceNo";		            
		public const string Property_ProductVariantId = "ProductVariantId";		            
		public const string Property_BulkPurchaseOrderId = "BulkPurchaseOrderId";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _VendorId;	            
		private Int32 _Quantity;	            
		private DateTime _RequestDate;	            
		private String _Status;	            
		private String _CreatedBy;	            
		private DateTime _CreatedAt;	            
		private String _UpdatedBy;	            
		private Nullable<DateTime> _UpdatedAt;	            
		private String _Remarks;	            
		private String _ReferenceNo;	            
		private Int32 _ProductVariantId;	            
		private Nullable<Int32> _BulkPurchaseOrderId;	            
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
		public Int32 VendorId
		{	
			get{ return _VendorId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_VendorId, value, _VendorId);
				if (PropertyChanging(args))
				{
					_VendorId = value;
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

		[DataMember]
		public DateTime RequestDate
		{	
			get{ return _RequestDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_RequestDate, value, _RequestDate);
				if (PropertyChanging(args))
				{
					_RequestDate = value;
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
		public String ReferenceNo
		{	
			get{ return _ReferenceNo; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ReferenceNo, value, _ReferenceNo);
				if (PropertyChanging(args))
				{
					_ReferenceNo = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 ProductVariantId
		{	
			get{ return _ProductVariantId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ProductVariantId, value, _ProductVariantId);
				if (PropertyChanging(args))
				{
					_ProductVariantId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Int32> BulkPurchaseOrderId
		{	
			get{ return _BulkPurchaseOrderId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_BulkPurchaseOrderId, value, _BulkPurchaseOrderId);
				if (PropertyChanging(args))
				{
					_BulkPurchaseOrderId = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  PoRequestedBase Clone()
		{
			PoRequestedBase newObj = new  PoRequestedBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.VendorId = this.VendorId;						
			newObj.Quantity = this.Quantity;						
			newObj.RequestDate = this.RequestDate;						
			newObj.Status = this.Status;						
			newObj.CreatedBy = this.CreatedBy;						
			newObj.CreatedAt = this.CreatedAt;						
			newObj.UpdatedBy = this.UpdatedBy;						
			newObj.UpdatedAt = this.UpdatedAt;						
			newObj.Remarks = this.Remarks;						
			newObj.ReferenceNo = this.ReferenceNo;						
			newObj.ProductVariantId = this.ProductVariantId;						
			newObj.BulkPurchaseOrderId = this.BulkPurchaseOrderId;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(PoRequestedBase.Property_Id, Id);				
			info.AddValue(PoRequestedBase.Property_VendorId, VendorId);				
			info.AddValue(PoRequestedBase.Property_Quantity, Quantity);				
			info.AddValue(PoRequestedBase.Property_RequestDate, RequestDate);				
			info.AddValue(PoRequestedBase.Property_Status, Status);				
			info.AddValue(PoRequestedBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(PoRequestedBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(PoRequestedBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(PoRequestedBase.Property_UpdatedAt, UpdatedAt);				
			info.AddValue(PoRequestedBase.Property_Remarks, Remarks);				
			info.AddValue(PoRequestedBase.Property_ReferenceNo, ReferenceNo);				
			info.AddValue(PoRequestedBase.Property_ProductVariantId, ProductVariantId);				
			info.AddValue(PoRequestedBase.Property_BulkPurchaseOrderId, BulkPurchaseOrderId);				
		}
		#endregion

		
	}
}
