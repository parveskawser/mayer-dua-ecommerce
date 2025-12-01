using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "InventoryTransactionBase", Namespace = "http://www.piistech.com//entities")]
	public class InventoryTransactionBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			SalesOrderDetailId = 1,
			PoReceivedId = 2,
			ProductId = 3,
			InOut = 4,
			Date = 5,
			Price = 6,
			Quantity = 7,
			CreatedBy = 8,
			CreatedAt = 9,
			UpdatedBy = 10,
			UpdatedAt = 11,
			Remarks = 12
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_SalesOrderDetailId = "SalesOrderDetailId";		            
		public const string Property_PoReceivedId = "PoReceivedId";		            
		public const string Property_ProductId = "ProductId";		            
		public const string Property_InOut = "InOut";		            
		public const string Property_Date = "Date";		            
		public const string Property_Price = "Price";		            
		public const string Property_Quantity = "Quantity";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		public const string Property_Remarks = "Remarks";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Nullable<Int32> _SalesOrderDetailId;	            
		private Nullable<Int32> _PoReceivedId;	            
		private Int32 _ProductId;	            
		private String _InOut;	            
		private DateTime _Date;	            
		private Nullable<Decimal> _Price;	            
		private Int32 _Quantity;	            
		private String _CreatedBy;	            
		private DateTime _CreatedAt;	            
		private String _UpdatedBy;	            
		private Nullable<DateTime> _UpdatedAt;	            
		private String _Remarks;	            
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
		public Nullable<Int32> SalesOrderDetailId
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
		public Nullable<Int32> PoReceivedId
		{	
			get{ return _PoReceivedId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PoReceivedId, value, _PoReceivedId);
				if (PropertyChanging(args))
				{
					_PoReceivedId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 ProductId
		{	
			get{ return _ProductId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ProductId, value, _ProductId);
				if (PropertyChanging(args))
				{
					_ProductId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String InOut
		{	
			get{ return _InOut; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_InOut, value, _InOut);
				if (PropertyChanging(args))
				{
					_InOut = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime Date
		{	
			get{ return _Date; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Date, value, _Date);
				if (PropertyChanging(args))
				{
					_Date = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Decimal> Price
		{	
			get{ return _Price; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Price, value, _Price);
				if (PropertyChanging(args))
				{
					_Price = value;
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

		#endregion
		
		#region Cloning Base Objects
		public  InventoryTransactionBase Clone()
		{
			InventoryTransactionBase newObj = new  InventoryTransactionBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.SalesOrderDetailId = this.SalesOrderDetailId;						
			newObj.PoReceivedId = this.PoReceivedId;						
			newObj.ProductId = this.ProductId;						
			newObj.InOut = this.InOut;						
			newObj.Date = this.Date;						
			newObj.Price = this.Price;						
			newObj.Quantity = this.Quantity;						
			newObj.CreatedBy = this.CreatedBy;						
			newObj.CreatedAt = this.CreatedAt;						
			newObj.UpdatedBy = this.UpdatedBy;						
			newObj.UpdatedAt = this.UpdatedAt;						
			newObj.Remarks = this.Remarks;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(InventoryTransactionBase.Property_Id, Id);				
			info.AddValue(InventoryTransactionBase.Property_SalesOrderDetailId, SalesOrderDetailId);				
			info.AddValue(InventoryTransactionBase.Property_PoReceivedId, PoReceivedId);				
			info.AddValue(InventoryTransactionBase.Property_ProductId, ProductId);				
			info.AddValue(InventoryTransactionBase.Property_InOut, InOut);				
			info.AddValue(InventoryTransactionBase.Property_Date, Date);				
			info.AddValue(InventoryTransactionBase.Property_Price, Price);				
			info.AddValue(InventoryTransactionBase.Property_Quantity, Quantity);				
			info.AddValue(InventoryTransactionBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(InventoryTransactionBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(InventoryTransactionBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(InventoryTransactionBase.Property_UpdatedAt, UpdatedAt);				
			info.AddValue(InventoryTransactionBase.Property_Remarks, Remarks);				
		}
		#endregion

		
	}
}
