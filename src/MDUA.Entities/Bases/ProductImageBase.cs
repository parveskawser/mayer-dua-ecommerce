using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "ProductImageBase", Namespace = "http://www.piistech.com//entities")]
	public class ProductImageBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			ProductId = 1,
			ImageUrl = 2,
			IsPrimary = 3,
			SortOrder = 4,
			AltText = 5,
			CreatedBy = 6,
			CreatedAt = 7,
			UpdatedBy = 8,
			UpdatedAt = 9
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_ProductId = "ProductId";		            
		public const string Property_ImageUrl = "ImageUrl";		            
		public const string Property_IsPrimary = "IsPrimary";		            
		public const string Property_SortOrder = "SortOrder";		            
		public const string Property_AltText = "AltText";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _ProductId;	            
		private String _ImageUrl;	            
		private Boolean _IsPrimary;	            
		private Int32 _SortOrder;	            
		private String _AltText;	            
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
		public String ImageUrl
		{	
			get{ return _ImageUrl; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ImageUrl, value, _ImageUrl);
				if (PropertyChanging(args))
				{
					_ImageUrl = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Boolean IsPrimary
		{	
			get{ return _IsPrimary; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_IsPrimary, value, _IsPrimary);
				if (PropertyChanging(args))
				{
					_IsPrimary = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 SortOrder
		{	
			get{ return _SortOrder; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SortOrder, value, _SortOrder);
				if (PropertyChanging(args))
				{
					_SortOrder = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String AltText
		{	
			get{ return _AltText; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AltText, value, _AltText);
				if (PropertyChanging(args))
				{
					_AltText = value;
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
		public  ProductImageBase Clone()
		{
			ProductImageBase newObj = new  ProductImageBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.ProductId = this.ProductId;						
			newObj.ImageUrl = this.ImageUrl;						
			newObj.IsPrimary = this.IsPrimary;						
			newObj.SortOrder = this.SortOrder;						
			newObj.AltText = this.AltText;						
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
			info.AddValue(ProductImageBase.Property_Id, Id);				
			info.AddValue(ProductImageBase.Property_ProductId, ProductId);				
			info.AddValue(ProductImageBase.Property_ImageUrl, ImageUrl);				
			info.AddValue(ProductImageBase.Property_IsPrimary, IsPrimary);				
			info.AddValue(ProductImageBase.Property_SortOrder, SortOrder);				
			info.AddValue(ProductImageBase.Property_AltText, AltText);				
			info.AddValue(ProductImageBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(ProductImageBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(ProductImageBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(ProductImageBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}
