using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "SalesReturnBase", Namespace = "http://www.piistech.com//entities")]
	public class SalesReturnBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			SalesOrderDetailId = 1,
			ReturnDate = 2,
			Quantity = 3,
			Reason = 4,
			RestockToInventory = 5,
			RefundAmount = 6,
			Status = 7,
			Remarks = 8,
			CreatedBy = 9,
			CreatedAt = 10,
			UpdatedBy = 11,
			UpdatedAt = 12
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_SalesOrderDetailId = "SalesOrderDetailId";		            
		public const string Property_ReturnDate = "ReturnDate";		            
		public const string Property_Quantity = "Quantity";		            
		public const string Property_Reason = "Reason";		            
		public const string Property_RestockToInventory = "RestockToInventory";		            
		public const string Property_RefundAmount = "RefundAmount";		            
		public const string Property_Status = "Status";		            
		public const string Property_Remarks = "Remarks";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _SalesOrderDetailId;	            
		private DateTime _ReturnDate;	            
		private Int32 _Quantity;	            
		private String _Reason;	            
		private Boolean _RestockToInventory;	            
		private Decimal _RefundAmount;	            
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
		public DateTime ReturnDate
		{	
			get{ return _ReturnDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ReturnDate, value, _ReturnDate);
				if (PropertyChanging(args))
				{
					_ReturnDate = value;
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
		public String Reason
		{	
			get{ return _Reason; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Reason, value, _Reason);
				if (PropertyChanging(args))
				{
					_Reason = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Boolean RestockToInventory
		{	
			get{ return _RestockToInventory; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_RestockToInventory, value, _RestockToInventory);
				if (PropertyChanging(args))
				{
					_RestockToInventory = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Decimal RefundAmount
		{	
			get{ return _RefundAmount; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_RefundAmount, value, _RefundAmount);
				if (PropertyChanging(args))
				{
					_RefundAmount = value;
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
		public  SalesReturnBase Clone()
		{
			SalesReturnBase newObj = new  SalesReturnBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.SalesOrderDetailId = this.SalesOrderDetailId;						
			newObj.ReturnDate = this.ReturnDate;						
			newObj.Quantity = this.Quantity;						
			newObj.Reason = this.Reason;						
			newObj.RestockToInventory = this.RestockToInventory;						
			newObj.RefundAmount = this.RefundAmount;						
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
			info.AddValue(SalesReturnBase.Property_Id, Id);				
			info.AddValue(SalesReturnBase.Property_SalesOrderDetailId, SalesOrderDetailId);				
			info.AddValue(SalesReturnBase.Property_ReturnDate, ReturnDate);				
			info.AddValue(SalesReturnBase.Property_Quantity, Quantity);				
			info.AddValue(SalesReturnBase.Property_Reason, Reason);				
			info.AddValue(SalesReturnBase.Property_RestockToInventory, RestockToInventory);				
			info.AddValue(SalesReturnBase.Property_RefundAmount, RefundAmount);				
			info.AddValue(SalesReturnBase.Property_Status, Status);				
			info.AddValue(SalesReturnBase.Property_Remarks, Remarks);				
			info.AddValue(SalesReturnBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(SalesReturnBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(SalesReturnBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(SalesReturnBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}
