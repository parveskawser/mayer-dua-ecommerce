using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "ProductDiscountBase", Namespace = "http://www.piistech.com//entities")]
	public class ProductDiscountBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			ProductId = 1,
			DiscountType = 2,
			DiscountValue = 3,
			MinQuantity = 4,
			EffectiveFrom = 5,
			EffectiveTo = 6,
			IsActive = 7,
			CreatedBy = 8,
			CreatedAt = 9,
			UpdatedBy = 10,
			UpdatedAt = 11
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_ProductId = "ProductId";		            
		public const string Property_DiscountType = "DiscountType";		            
		public const string Property_DiscountValue = "DiscountValue";		            
		public const string Property_MinQuantity = "MinQuantity";		            
		public const string Property_EffectiveFrom = "EffectiveFrom";		            
		public const string Property_EffectiveTo = "EffectiveTo";		            
		public const string Property_IsActive = "IsActive";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _ProductId;	            
		private String _DiscountType;	            
		private Decimal _DiscountValue;	            
		private Nullable<Int32> _MinQuantity;	            
		private DateTime _EffectiveFrom;	            
		private Nullable<DateTime> _EffectiveTo;	            
		private Boolean _IsActive;	            
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
		public String DiscountType
		{	
			get{ return _DiscountType; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_DiscountType, value, _DiscountType);
				if (PropertyChanging(args))
				{
					_DiscountType = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Decimal DiscountValue
		{	
			get{ return _DiscountValue; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_DiscountValue, value, _DiscountValue);
				if (PropertyChanging(args))
				{
					_DiscountValue = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Int32> MinQuantity
		{	
			get{ return _MinQuantity; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_MinQuantity, value, _MinQuantity);
				if (PropertyChanging(args))
				{
					_MinQuantity = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime EffectiveFrom
		{	
			get{ return _EffectiveFrom; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_EffectiveFrom, value, _EffectiveFrom);
				if (PropertyChanging(args))
				{
					_EffectiveFrom = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<DateTime> EffectiveTo
		{	
			get{ return _EffectiveTo; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_EffectiveTo, value, _EffectiveTo);
				if (PropertyChanging(args))
				{
					_EffectiveTo = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Boolean IsActive
		{	
			get{ return _IsActive; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_IsActive, value, _IsActive);
				if (PropertyChanging(args))
				{
					_IsActive = value;
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
		public  ProductDiscountBase Clone()
		{
			ProductDiscountBase newObj = new  ProductDiscountBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.ProductId = this.ProductId;						
			newObj.DiscountType = this.DiscountType;						
			newObj.DiscountValue = this.DiscountValue;						
			newObj.MinQuantity = this.MinQuantity;						
			newObj.EffectiveFrom = this.EffectiveFrom;						
			newObj.EffectiveTo = this.EffectiveTo;						
			newObj.IsActive = this.IsActive;						
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
			info.AddValue(ProductDiscountBase.Property_Id, Id);				
			info.AddValue(ProductDiscountBase.Property_ProductId, ProductId);				
			info.AddValue(ProductDiscountBase.Property_DiscountType, DiscountType);				
			info.AddValue(ProductDiscountBase.Property_DiscountValue, DiscountValue);				
			info.AddValue(ProductDiscountBase.Property_MinQuantity, MinQuantity);				
			info.AddValue(ProductDiscountBase.Property_EffectiveFrom, EffectiveFrom);				
			info.AddValue(ProductDiscountBase.Property_EffectiveTo, EffectiveTo);				
			info.AddValue(ProductDiscountBase.Property_IsActive, IsActive);				
			info.AddValue(ProductDiscountBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(ProductDiscountBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(ProductDiscountBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(ProductDiscountBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}