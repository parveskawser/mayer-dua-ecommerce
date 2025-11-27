using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "ProductBase", Namespace = "http://www.piistech.com//entities")]
	public class ProductBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			CompanyId = 1,
			ProductName = 2,
			ReorderLevel = 3,
			Barcode = 4,
			CategoryId = 5,
			Description = 6,
			Slug = 7,
			BasePrice = 8,
			IsVariantBased = 9,
			IsActive = 10,
			CreatedBy = 11,
			CreatedAt = 12,
			UpdatedBy = 13,
			UpdatedAt = 14
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_CompanyId = "CompanyId";		            
		public const string Property_ProductName = "ProductName";		            
		public const string Property_ReorderLevel = "ReorderLevel";		            
		public const string Property_Barcode = "Barcode";		            
		public const string Property_CategoryId = "CategoryId";		            
		public const string Property_Description = "Description";		            
		public const string Property_Slug = "Slug";		            
		public const string Property_BasePrice = "BasePrice";		            
		public const string Property_IsVariantBased = "IsVariantBased";		            
		public const string Property_IsActive = "IsActive";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _CompanyId;	            
		private String _ProductName;	            
		private Int32 _ReorderLevel;	            
		private String _Barcode;	            
		private Nullable<Int32> _CategoryId;	            
		private String _Description;	            
		private String _Slug;	            
		private Nullable<Decimal> _BasePrice;	            
		private Nullable<Boolean> _IsVariantBased;	            
		private Boolean _IsActive;	            
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
		public Int32 CompanyId
		{	
			get{ return _CompanyId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CompanyId, value, _CompanyId);
				if (PropertyChanging(args))
				{
					_CompanyId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String ProductName
		{	
			get{ return _ProductName; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ProductName, value, _ProductName);
				if (PropertyChanging(args))
				{
					_ProductName = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 ReorderLevel
		{	
			get{ return _ReorderLevel; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ReorderLevel, value, _ReorderLevel);
				if (PropertyChanging(args))
				{
					_ReorderLevel = value;
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
		public Nullable<Int32> CategoryId
		{	
			get{ return _CategoryId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CategoryId, value, _CategoryId);
				if (PropertyChanging(args))
				{
					_CategoryId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Description
		{	
			get{ return _Description; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Description, value, _Description);
				if (PropertyChanging(args))
				{
					_Description = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Slug
		{	
			get{ return _Slug; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Slug, value, _Slug);
				if (PropertyChanging(args))
				{
					_Slug = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Decimal> BasePrice
		{	
			get{ return _BasePrice; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_BasePrice, value, _BasePrice);
				if (PropertyChanging(args))
				{
					_BasePrice = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Boolean> IsVariantBased
		{	
			get{ return _IsVariantBased; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_IsVariantBased, value, _IsVariantBased);
				if (PropertyChanging(args))
				{
					_IsVariantBased = value;
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
		public  ProductBase Clone()
		{
			ProductBase newObj = new  ProductBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.CompanyId = this.CompanyId;						
			newObj.ProductName = this.ProductName;						
			newObj.ReorderLevel = this.ReorderLevel;						
			newObj.Barcode = this.Barcode;						
			newObj.CategoryId = this.CategoryId;						
			newObj.Description = this.Description;						
			newObj.Slug = this.Slug;						
			newObj.BasePrice = this.BasePrice;						
			newObj.IsVariantBased = this.IsVariantBased;						
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
			info.AddValue(ProductBase.Property_Id, Id);				
			info.AddValue(ProductBase.Property_CompanyId, CompanyId);				
			info.AddValue(ProductBase.Property_ProductName, ProductName);				
			info.AddValue(ProductBase.Property_ReorderLevel, ReorderLevel);				
			info.AddValue(ProductBase.Property_Barcode, Barcode);				
			info.AddValue(ProductBase.Property_CategoryId, CategoryId);				
			info.AddValue(ProductBase.Property_Description, Description);				
			info.AddValue(ProductBase.Property_Slug, Slug);				
			info.AddValue(ProductBase.Property_BasePrice, BasePrice);				
			info.AddValue(ProductBase.Property_IsVariantBased, IsVariantBased);				
			info.AddValue(ProductBase.Property_IsActive, IsActive);				
			info.AddValue(ProductBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(ProductBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(ProductBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(ProductBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}
