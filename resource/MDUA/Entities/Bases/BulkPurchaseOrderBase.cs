using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "BulkPurchaseOrderBase", Namespace = "http://www.piistech.com//entities")]
	public class BulkPurchaseOrderBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			VendorId = 1,
			AgreementNumber = 2,
			Title = 3,
			AgreementDate = 4,
			ExpiryDate = 5,
			TotalTargetQuantity = 6,
			TotalTargetAmount = 7,
			Status = 8,
			Remarks = 9,
			CreatedBy = 10,
			CreatedAt = 11,
			UpdatedBy = 12,
			UpdatedAt = 13
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_VendorId = "VendorId";		            
		public const string Property_AgreementNumber = "AgreementNumber";		            
		public const string Property_Title = "Title";		            
		public const string Property_AgreementDate = "AgreementDate";		            
		public const string Property_ExpiryDate = "ExpiryDate";		            
		public const string Property_TotalTargetQuantity = "TotalTargetQuantity";		            
		public const string Property_TotalTargetAmount = "TotalTargetAmount";		            
		public const string Property_Status = "Status";		            
		public const string Property_Remarks = "Remarks";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _VendorId;	            
		private String _AgreementNumber;	            
		private String _Title;	            
		private DateTime _AgreementDate;	            
		private Nullable<DateTime> _ExpiryDate;	            
		private Nullable<Int32> _TotalTargetQuantity;	            
		private Nullable<Decimal> _TotalTargetAmount;	            
		private String _Status;	            
		private String _Remarks;	            
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
		public String AgreementNumber
		{	
			get{ return _AgreementNumber; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AgreementNumber, value, _AgreementNumber);
				if (PropertyChanging(args))
				{
					_AgreementNumber = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Title
		{	
			get{ return _Title; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Title, value, _Title);
				if (PropertyChanging(args))
				{
					_Title = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime AgreementDate
		{	
			get{ return _AgreementDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AgreementDate, value, _AgreementDate);
				if (PropertyChanging(args))
				{
					_AgreementDate = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<DateTime> ExpiryDate
		{	
			get{ return _ExpiryDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ExpiryDate, value, _ExpiryDate);
				if (PropertyChanging(args))
				{
					_ExpiryDate = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Int32> TotalTargetQuantity
		{	
			get{ return _TotalTargetQuantity; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_TotalTargetQuantity, value, _TotalTargetQuantity);
				if (PropertyChanging(args))
				{
					_TotalTargetQuantity = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Decimal> TotalTargetAmount
		{	
			get{ return _TotalTargetAmount; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_TotalTargetAmount, value, _TotalTargetAmount);
				if (PropertyChanging(args))
				{
					_TotalTargetAmount = value;
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
		public  BulkPurchaseOrderBase Clone()
		{
			BulkPurchaseOrderBase newObj = new  BulkPurchaseOrderBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.VendorId = this.VendorId;						
			newObj.AgreementNumber = this.AgreementNumber;						
			newObj.Title = this.Title;						
			newObj.AgreementDate = this.AgreementDate;						
			newObj.ExpiryDate = this.ExpiryDate;						
			newObj.TotalTargetQuantity = this.TotalTargetQuantity;						
			newObj.TotalTargetAmount = this.TotalTargetAmount;						
			newObj.Status = this.Status;						
			newObj.Remarks = this.Remarks;						
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
			info.AddValue(BulkPurchaseOrderBase.Property_Id, Id);				
			info.AddValue(BulkPurchaseOrderBase.Property_VendorId, VendorId);				
			info.AddValue(BulkPurchaseOrderBase.Property_AgreementNumber, AgreementNumber);				
			info.AddValue(BulkPurchaseOrderBase.Property_Title, Title);				
			info.AddValue(BulkPurchaseOrderBase.Property_AgreementDate, AgreementDate);				
			info.AddValue(BulkPurchaseOrderBase.Property_ExpiryDate, ExpiryDate);				
			info.AddValue(BulkPurchaseOrderBase.Property_TotalTargetQuantity, TotalTargetQuantity);				
			info.AddValue(BulkPurchaseOrderBase.Property_TotalTargetAmount, TotalTargetAmount);				
			info.AddValue(BulkPurchaseOrderBase.Property_Status, Status);				
			info.AddValue(BulkPurchaseOrderBase.Property_Remarks, Remarks);				
			info.AddValue(BulkPurchaseOrderBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(BulkPurchaseOrderBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(BulkPurchaseOrderBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(BulkPurchaseOrderBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}