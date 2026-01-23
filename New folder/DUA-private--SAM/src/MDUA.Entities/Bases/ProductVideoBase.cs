using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "ProductVideoBase", Namespace = "http://www.piistech.com//entities")]
	public class ProductVideoBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			ProductId = 1,
			VideoUrl = 2,
			ThumbnailUrl = 3,
			IsPrimary = 4,
			SortOrder = 5,
			Title = 6,
			CreatedBy = 7,
			CreatedAt = 8,
			UpdatedBy = 9,
			UpdatedAt = 10
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_ProductId = "ProductId";		            
		public const string Property_VideoUrl = "VideoUrl";		            
		public const string Property_ThumbnailUrl = "ThumbnailUrl";		            
		public const string Property_IsPrimary = "IsPrimary";		            
		public const string Property_SortOrder = "SortOrder";		            
		public const string Property_Title = "Title";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _ProductId;	            
		private String _VideoUrl;	            
		private String _ThumbnailUrl;	            
		private Boolean _IsPrimary;	            
		private Int32 _SortOrder;	            
		private String _Title;	            
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
		public String VideoUrl
		{	
			get{ return _VideoUrl; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_VideoUrl, value, _VideoUrl);
				if (PropertyChanging(args))
				{
					_VideoUrl = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String ThumbnailUrl
		{	
			get{ return _ThumbnailUrl; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ThumbnailUrl, value, _ThumbnailUrl);
				if (PropertyChanging(args))
				{
					_ThumbnailUrl = value;
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
		public  ProductVideoBase Clone()
		{
			ProductVideoBase newObj = new  ProductVideoBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.ProductId = this.ProductId;						
			newObj.VideoUrl = this.VideoUrl;						
			newObj.ThumbnailUrl = this.ThumbnailUrl;						
			newObj.IsPrimary = this.IsPrimary;						
			newObj.SortOrder = this.SortOrder;						
			newObj.Title = this.Title;						
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
			info.AddValue(ProductVideoBase.Property_Id, Id);				
			info.AddValue(ProductVideoBase.Property_ProductId, ProductId);				
			info.AddValue(ProductVideoBase.Property_VideoUrl, VideoUrl);				
			info.AddValue(ProductVideoBase.Property_ThumbnailUrl, ThumbnailUrl);				
			info.AddValue(ProductVideoBase.Property_IsPrimary, IsPrimary);				
			info.AddValue(ProductVideoBase.Property_SortOrder, SortOrder);				
			info.AddValue(ProductVideoBase.Property_Title, Title);				
			info.AddValue(ProductVideoBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(ProductVideoBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(ProductVideoBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(ProductVideoBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}
