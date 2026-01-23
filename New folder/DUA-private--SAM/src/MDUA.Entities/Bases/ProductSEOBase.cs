using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "ProductSEOBase", Namespace = "http://www.piistech.com//entities")]
	public class ProductSEOBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			ProductId = 1,
			MetaTitle = 2,
			MetaKeywords = 3,
			MetaDescription = 4,
			CanonicalUrl = 5,
			OGTitle = 6,
			OGDescription = 7,
			OGImage = 8,
			CreatedBy = 9,
			CreatedAt = 10,
			UpdatedBy = 11,
			UpdatedAt = 12,
            CustomHeaderTags = 13
        }
        #endregion

        #region Constants
        public const string Property_Id = "Id";		            
		public const string Property_ProductId = "ProductId";		            
		public const string Property_MetaTitle = "MetaTitle";		            
		public const string Property_MetaKeywords = "MetaKeywords";		            
		public const string Property_MetaDescription = "MetaDescription";		            
		public const string Property_CanonicalUrl = "CanonicalUrl";		            
		public const string Property_OGTitle = "OGTitle";		            
		public const string Property_OGDescription = "OGDescription";		            
		public const string Property_OGImage = "OGImage";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";
        public const string Property_CustomHeaderTags = "CustomHeaderTags";
        #endregion

        #region Private Data Types
        private Int32 _Id;	            
		private Int32 _ProductId;	            
		private String _MetaTitle;	            
		private String _MetaKeywords;	            
		private String _MetaDescription;	            
		private String _CanonicalUrl;	            
		private String _OGTitle;	            
		private String _OGDescription;	            
		private String _OGImage;	            
		private String _CreatedBy;	            
		private DateTime _CreatedAt;	            
		private String _UpdatedBy;	            
		private Nullable<DateTime> _UpdatedAt;
        private String _CustomHeaderTags;

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
		public String MetaTitle
		{	
			get{ return _MetaTitle; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_MetaTitle, value, _MetaTitle);
				if (PropertyChanging(args))
				{
					_MetaTitle = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String MetaKeywords
		{	
			get{ return _MetaKeywords; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_MetaKeywords, value, _MetaKeywords);
				if (PropertyChanging(args))
				{
					_MetaKeywords = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String MetaDescription
		{	
			get{ return _MetaDescription; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_MetaDescription, value, _MetaDescription);
				if (PropertyChanging(args))
				{
					_MetaDescription = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String CanonicalUrl
		{	
			get{ return _CanonicalUrl; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CanonicalUrl, value, _CanonicalUrl);
				if (PropertyChanging(args))
				{
					_CanonicalUrl = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String OGTitle
		{	
			get{ return _OGTitle; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_OGTitle, value, _OGTitle);
				if (PropertyChanging(args))
				{
					_OGTitle = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String OGDescription
		{	
			get{ return _OGDescription; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_OGDescription, value, _OGDescription);
				if (PropertyChanging(args))
				{
					_OGDescription = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String OGImage
		{	
			get{ return _OGImage; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_OGImage, value, _OGImage);
				if (PropertyChanging(args))
				{
					_OGImage = value;
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
        public String CustomHeaderTags
        {
            get { return _CustomHeaderTags; }
            set
            {
                PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CustomHeaderTags, value, _CustomHeaderTags);
                if (PropertyChanging(args))
                {
                    _CustomHeaderTags = value;
                    PropertyChanged(args);
                }
            }
        }

        #endregion

        #region Cloning Base Objects
        public  ProductSEOBase Clone()
		{
			ProductSEOBase newObj = new  ProductSEOBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.ProductId = this.ProductId;						
			newObj.MetaTitle = this.MetaTitle;						
			newObj.MetaKeywords = this.MetaKeywords;						
			newObj.MetaDescription = this.MetaDescription;						
			newObj.CanonicalUrl = this.CanonicalUrl;						
			newObj.OGTitle = this.OGTitle;						
			newObj.OGDescription = this.OGDescription;						
			newObj.OGImage = this.OGImage;						
			newObj.CreatedBy = this.CreatedBy;						
			newObj.CreatedAt = this.CreatedAt;						
			newObj.UpdatedBy = this.UpdatedBy;						
			newObj.UpdatedAt = this.UpdatedAt;
            newObj.CustomHeaderTags = this.CustomHeaderTags;


            return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(ProductSEOBase.Property_Id, Id);				
			info.AddValue(ProductSEOBase.Property_ProductId, ProductId);				
			info.AddValue(ProductSEOBase.Property_MetaTitle, MetaTitle);				
			info.AddValue(ProductSEOBase.Property_MetaKeywords, MetaKeywords);				
			info.AddValue(ProductSEOBase.Property_MetaDescription, MetaDescription);				
			info.AddValue(ProductSEOBase.Property_CanonicalUrl, CanonicalUrl);				
			info.AddValue(ProductSEOBase.Property_OGTitle, OGTitle);				
			info.AddValue(ProductSEOBase.Property_OGDescription, OGDescription);				
			info.AddValue(ProductSEOBase.Property_OGImage, OGImage);				
			info.AddValue(ProductSEOBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(ProductSEOBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(ProductSEOBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(ProductSEOBase.Property_UpdatedAt, UpdatedAt);	
			info.AddValue(ProductSEOBase.Property_CustomHeaderTags, CustomHeaderTags);
		}
		#endregion

		
	}
}