using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "ProductVariantBase", Namespace = "http://www.piistech.com//entities")]
	public class ProductVariantBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			ProductId = 1,
			VariantName = 2,
			SKU = 3,
			Barcode = 4,
			VariantPrice = 5,
			IsActive = 6,
			CreatedBy = 7,
			CreatedAt = 8,
			UpdatedBy = 9,
			UpdatedAt = 10
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_ProductId = "ProductId";		            
		public const string Property_VariantName = "VariantName";		            
		public const string Property_SKU = "SKU";		            
		public const string Property_Barcode = "Barcode";		            
		public const string Property_VariantPrice = "VariantPrice";		            
		public const string Property_IsActive = "IsActive";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _ProductId;	            
		private String _VariantName;	            
		private String _SKU;	            
		private String _Barcode;	            
		private Nullable<Decimal> _VariantPrice;	            
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
		public String VariantName
		{	
			get{ return _VariantName; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_VariantName, value, _VariantName);
				if (PropertyChanging(args))
				{
					_VariantName = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String SKU
		{	
			get{ return _SKU; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SKU, value, _SKU);
				if (PropertyChanging(args))
				{
					_SKU = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Barcode
		{	
			get{ return _Barcode; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Barcode, value, _Barcode);
				if (PropertyChanging(args))
				{
					_Barcode = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Decimal> VariantPrice
		{	
			get{ return _VariantPrice; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_VariantPrice, value, _VariantPrice);
				if (PropertyChanging(args))
				{
					_VariantPrice = value;
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
		public  ProductVariantBase Clone()
		{
			ProductVariantBase newObj = new  ProductVariantBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.ProductId = this.ProductId;						
			newObj.VariantName = this.VariantName;						
			newObj.SKU = this.SKU;						
			newObj.Barcode = this.Barcode;						
			newObj.VariantPrice = this.VariantPrice;						
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
			info.AddValue(ProductVariantBase.Property_Id, Id);				
			info.AddValue(ProductVariantBase.Property_ProductId, ProductId);				
			info.AddValue(ProductVariantBase.Property_VariantName, VariantName);				
			info.AddValue(ProductVariantBase.Property_SKU, SKU);				
			info.AddValue(ProductVariantBase.Property_Barcode, Barcode);				
			info.AddValue(ProductVariantBase.Property_VariantPrice, VariantPrice);				
			info.AddValue(ProductVariantBase.Property_IsActive, IsActive);				
			info.AddValue(ProductVariantBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(ProductVariantBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(ProductVariantBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(ProductVariantBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}