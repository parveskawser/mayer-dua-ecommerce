using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "CmsAssetBase", Namespace = "http://www.piistech.com//entities")]
	public class CmsAssetBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			CompanyId = 1,
			PageId = 2,
			FileName = 3,
			FilePath = 4,
			FileType = 5,
			Version = 6,
			IsActive = 7,
			CreatedAt = 8
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_CompanyId = "CompanyId";		            
		public const string Property_PageId = "PageId";		            
		public const string Property_FileName = "FileName";		            
		public const string Property_FilePath = "FilePath";		            
		public const string Property_FileType = "FileType";		            
		public const string Property_Version = "Version";		            
		public const string Property_IsActive = "IsActive";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _CompanyId;	            
		private Nullable<Int32> _PageId;	            
		private String _FileName;	            
		private String _FilePath;	            
		private String _FileType;	            
		private Int32 _Version;	            
		private Boolean _IsActive;	            
		private DateTime _CreatedAt;	            
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
		public Nullable<Int32> PageId
		{	
			get{ return _PageId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PageId, value, _PageId);
				if (PropertyChanging(args))
				{
					_PageId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String FileName
		{	
			get{ return _FileName; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_FileName, value, _FileName);
				if (PropertyChanging(args))
				{
					_FileName = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String FilePath
		{	
			get{ return _FilePath; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_FilePath, value, _FilePath);
				if (PropertyChanging(args))
				{
					_FilePath = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String FileType
		{	
			get{ return _FileType; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_FileType, value, _FileType);
				if (PropertyChanging(args))
				{
					_FileType = value;
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

		#endregion
		
		#region Cloning Base Objects
		public  CmsAssetBase Clone()
		{
			CmsAssetBase newObj = new  CmsAssetBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.CompanyId = this.CompanyId;						
			newObj.PageId = this.PageId;						
			newObj.FileName = this.FileName;						
			newObj.FilePath = this.FilePath;						
			newObj.FileType = this.FileType;						
			newObj.Version = this.Version;						
			newObj.IsActive = this.IsActive;						
			newObj.CreatedAt = this.CreatedAt;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(CmsAssetBase.Property_Id, Id);				
			info.AddValue(CmsAssetBase.Property_CompanyId, CompanyId);				
			info.AddValue(CmsAssetBase.Property_PageId, PageId);				
			info.AddValue(CmsAssetBase.Property_FileName, FileName);				
			info.AddValue(CmsAssetBase.Property_FilePath, FilePath);				
			info.AddValue(CmsAssetBase.Property_FileType, FileType);				
			info.AddValue(CmsAssetBase.Property_Version, Version);				
			info.AddValue(CmsAssetBase.Property_IsActive, IsActive);				
			info.AddValue(CmsAssetBase.Property_CreatedAt, CreatedAt);				
		}
		#endregion

		
	}
}