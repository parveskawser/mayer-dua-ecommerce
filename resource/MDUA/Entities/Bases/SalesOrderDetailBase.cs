using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "SalesOrderDetailBase", Namespace = "http://www.piistech.com//entities")]
	public class SalesOrderDetailBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			SalesOrderId = 1,
			ProductId = 2,
			Quantity = 3,
			UnitPrice = 4,
			LineTotal = 5,
			ProfitAmount = 6,
			CreatedBy = 7,
			CreatedAt = 8,
			UpdatedBy = 9,
			UpdatedAt = 10
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_SalesOrderId = "SalesOrderId";		            
		public const string Property_ProductId = "ProductId";		            
		public const string Property_Quantity = "Quantity";		            
		public const string Property_UnitPrice = "UnitPrice";		            
		public const string Property_LineTotal = "LineTotal";		            
		public const string Property_ProfitAmount = "ProfitAmount";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _SalesOrderId;	            
		private Int32 _ProductId;	            
		private Int32 _Quantity;	            
		private Decimal _UnitPrice;	            
		private Nullable<Decimal> _LineTotal;	            
		private Nullable<Decimal> _ProfitAmount;	            
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
		public Decimal UnitPrice
		{	
			get{ return _UnitPrice; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_UnitPrice, value, _UnitPrice);
				if (PropertyChanging(args))
				{
					_UnitPrice = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Decimal> LineTotal
		{	
			get{ return _LineTotal; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_LineTotal, value, _LineTotal);
				if (PropertyChanging(args))
				{
					_LineTotal = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Decimal> ProfitAmount
		{	
			get{ return _ProfitAmount; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ProfitAmount, value, _ProfitAmount);
				if (PropertyChanging(args))
				{
					_ProfitAmount = value;
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
		public  SalesOrderDetailBase Clone()
		{
			SalesOrderDetailBase newObj = new  SalesOrderDetailBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.SalesOrderId = this.SalesOrderId;						
			newObj.ProductId = this.ProductId;						
			newObj.Quantity = this.Quantity;						
			newObj.UnitPrice = this.UnitPrice;						
			newObj.LineTotal = this.LineTotal;						
			newObj.ProfitAmount = this.ProfitAmount;						
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
			info.AddValue(SalesOrderDetailBase.Property_Id, Id);				
			info.AddValue(SalesOrderDetailBase.Property_SalesOrderId, SalesOrderId);				
			info.AddValue(SalesOrderDetailBase.Property_ProductId, ProductId);				
			info.AddValue(SalesOrderDetailBase.Property_Quantity, Quantity);				
			info.AddValue(SalesOrderDetailBase.Property_UnitPrice, UnitPrice);				
			info.AddValue(SalesOrderDetailBase.Property_LineTotal, LineTotal);				
			info.AddValue(SalesOrderDetailBase.Property_ProfitAmount, ProfitAmount);				
			info.AddValue(SalesOrderDetailBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(SalesOrderDetailBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(SalesOrderDetailBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(SalesOrderDetailBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}