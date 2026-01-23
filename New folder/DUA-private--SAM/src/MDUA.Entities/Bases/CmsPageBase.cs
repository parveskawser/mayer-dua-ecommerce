using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "CmsPageBase", Namespace = "http://www.piistech.com//entities")]
	public class CmsPageBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			CompanyId = 1,
			Title = 2,
			Slug = 3,
			ContentHtml = 4,
			SidebarContentHtml = 5,
			LayoutView = 6,
			MetaTitle = 7,
			MetaDescription = 8,
			CustomCss = 9,
			CustomJs = 10,
			IsActive = 11,
			Version = 12,
			PublishedAt = 13,
			CreatedBy = 14,
			CreatedAt = 15,
			UpdatedBy = 16,
			UpdatedAt = 17,
			CustomHeaderTags = 18
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_CompanyId = "CompanyId";		            
		public const string Property_Title = "Title";		            
		public const string Property_Slug = "Slug";		            
		public const string Property_ContentHtml = "ContentHtml";		            
		public const string Property_SidebarContentHtml = "SidebarContentHtml";		            
		public const string Property_LayoutView = "LayoutView";		            
		public const string Property_MetaTitle = "MetaTitle";		            
		public const string Property_MetaDescription = "MetaDescription";		            
		public const string Property_CustomCss = "CustomCss";		            
		public const string Property_CustomJs = "CustomJs";		            
		public const string Property_IsActive = "IsActive";		            
		public const string Property_Version = "Version";		            
		public const string Property_PublishedAt = "PublishedAt";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		public const string Property_CustomHeaderTags = "CustomHeaderTags";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _CompanyId;	            
		private String _Title;	            
		private String _Slug;	            
		private String _ContentHtml;	            
		private String _SidebarContentHtml;	            
		private String _LayoutView;	            
		private String _MetaTitle;	            
		private String _MetaDescription;	            
		private String _CustomCss;	            
		private String _CustomJs;	            
		private Boolean _IsActive;	            
		private Int32 _Version;	            
		private Nullable<DateTime> _PublishedAt;	            
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
		public String ContentHtml
		{	
			get{ return _ContentHtml; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ContentHtml, value, _ContentHtml);
				if (PropertyChanging(args))
				{
					_ContentHtml = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String SidebarContentHtml
		{	
			get{ return _SidebarContentHtml; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SidebarContentHtml, value, _SidebarContentHtml);
				if (PropertyChanging(args))
				{
					_SidebarContentHtml = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String LayoutView
		{	
			get{ return _LayoutView; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_LayoutView, value, _LayoutView);
				if (PropertyChanging(args))
				{
					_LayoutView = value;
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
		public String CustomCss
		{	
			get{ return _CustomCss; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CustomCss, value, _CustomCss);
				if (PropertyChanging(args))
				{
					_CustomCss = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String CustomJs
		{	
			get{ return _CustomJs; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CustomJs, value, _CustomJs);
				if (PropertyChanging(args))
				{
					_CustomJs = value;
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
		public Int32 Version
		{	
			get{ return _Version; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Version, value, _Version);
				if (PropertyChanging(args))
				{
					_Version = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<DateTime> PublishedAt
		{	
			get{ return _PublishedAt; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PublishedAt, value, _PublishedAt);
				if (PropertyChanging(args))
				{
					_PublishedAt = value;
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
			get{ return _CustomHeaderTags; }			
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
		public  CmsPageBase Clone()
		{
			CmsPageBase newObj = new  CmsPageBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.CompanyId = this.CompanyId;						
			newObj.Title = this.Title;						
			newObj.Slug = this.Slug;						
			newObj.ContentHtml = this.ContentHtml;						
			newObj.SidebarContentHtml = this.SidebarContentHtml;						
			newObj.LayoutView = this.LayoutView;						
			newObj.MetaTitle = this.MetaTitle;						
			newObj.MetaDescription = this.MetaDescription;						
			newObj.CustomCss = this.CustomCss;						
			newObj.CustomJs = this.CustomJs;						
			newObj.IsActive = this.IsActive;						
			newObj.Version = this.Version;						
			newObj.PublishedAt = this.PublishedAt;						
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
			info.AddValue(CmsPageBase.Property_Id, Id);				
			info.AddValue(CmsPageBase.Property_CompanyId, CompanyId);				
			info.AddValue(CmsPageBase.Property_Title, Title);				
			info.AddValue(CmsPageBase.Property_Slug, Slug);				
			info.AddValue(CmsPageBase.Property_ContentHtml, ContentHtml);				
			info.AddValue(CmsPageBase.Property_SidebarContentHtml, SidebarContentHtml);				
			info.AddValue(CmsPageBase.Property_LayoutView, LayoutView);				
			info.AddValue(CmsPageBase.Property_MetaTitle, MetaTitle);				
			info.AddValue(CmsPageBase.Property_MetaDescription, MetaDescription);				
			info.AddValue(CmsPageBase.Property_CustomCss, CustomCss);				
			info.AddValue(CmsPageBase.Property_CustomJs, CustomJs);				
			info.AddValue(CmsPageBase.Property_IsActive, IsActive);				
			info.AddValue(CmsPageBase.Property_Version, Version);				
			info.AddValue(CmsPageBase.Property_PublishedAt, PublishedAt);				
			info.AddValue(CmsPageBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(CmsPageBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(CmsPageBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(CmsPageBase.Property_UpdatedAt, UpdatedAt);				
			info.AddValue(CmsPageBase.Property_CustomHeaderTags, CustomHeaderTags);				
		}
		#endregion

		
	}
}