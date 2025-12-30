using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "VariantPriceStockBase", Namespace = "http://www.piistech.com//entities")]
	public class VariantPriceStockBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			Price = 1,
			CompareAtPrice = 2,
			CostPrice = 3,
			StockQty = 4,
			TrackInventory = 5,
			AllowBackorder = 6,
			WeightGrams = 7
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_Price = "Price";		            
		public const string Property_CompareAtPrice = "CompareAtPrice";		            
		public const string Property_CostPrice = "CostPrice";		            
		public const string Property_StockQty = "StockQty";		            
		public const string Property_TrackInventory = "TrackInventory";		            
		public const string Property_AllowBackorder = "AllowBackorder";		            
		public const string Property_WeightGrams = "WeightGrams";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Decimal _Price;	            
		private Nullable<Decimal> _CompareAtPrice;	            
		private Nullable<Decimal> _CostPrice;	            
		private Int32 _StockQty;	            
		private Boolean _TrackInventory;	            
		private Boolean _AllowBackorder;	            
		private Nullable<Int32> _WeightGrams;	            
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
		public Decimal Price
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
		public Nullable<Decimal> CompareAtPrice
		{	
			get{ return _CompareAtPrice; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CompareAtPrice, value, _CompareAtPrice);
				if (PropertyChanging(args))
				{
					_CompareAtPrice = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Decimal> CostPrice
		{	
			get{ return _CostPrice; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CostPrice, value, _CostPrice);
				if (PropertyChanging(args))
				{
					_CostPrice = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 StockQty
		{	
			get{ return _StockQty; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_StockQty, value, _StockQty);
				if (PropertyChanging(args))
				{
					_StockQty = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Boolean TrackInventory
		{	
			get{ return _TrackInventory; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_TrackInventory, value, _TrackInventory);
				if (PropertyChanging(args))
				{
					_TrackInventory = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Boolean AllowBackorder
		{	
			get{ return _AllowBackorder; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AllowBackorder, value, _AllowBackorder);
				if (PropertyChanging(args))
				{
					_AllowBackorder = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Int32> WeightGrams
		{	
			get{ return _WeightGrams; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_WeightGrams, value, _WeightGrams);
				if (PropertyChanging(args))
				{
					_WeightGrams = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  VariantPriceStockBase Clone()
		{
			VariantPriceStockBase newObj = new  VariantPriceStockBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.Price = this.Price;						
			newObj.CompareAtPrice = this.CompareAtPrice;						
			newObj.CostPrice = this.CostPrice;						
			newObj.StockQty = this.StockQty;						
			newObj.TrackInventory = this.TrackInventory;						
			newObj.AllowBackorder = this.AllowBackorder;						
			newObj.WeightGrams = this.WeightGrams;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(VariantPriceStockBase.Property_Id, Id);				
			info.AddValue(VariantPriceStockBase.Property_Price, Price);				
			info.AddValue(VariantPriceStockBase.Property_CompareAtPrice, CompareAtPrice);				
			info.AddValue(VariantPriceStockBase.Property_CostPrice, CostPrice);				
			info.AddValue(VariantPriceStockBase.Property_StockQty, StockQty);				
			info.AddValue(VariantPriceStockBase.Property_TrackInventory, TrackInventory);				
			info.AddValue(VariantPriceStockBase.Property_AllowBackorder, AllowBackorder);				
			info.AddValue(VariantPriceStockBase.Property_WeightGrams, WeightGrams);				
		}
		#endregion

		
	}
}