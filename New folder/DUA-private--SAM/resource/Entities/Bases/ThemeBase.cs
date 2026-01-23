using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "ThemeBase", Namespace = "http://www.piistech.com//entities")]
	public class ThemeBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			ThemeKey = 1,
			DisplayName = 2,
			PreviewImageUrl = 3,
			IsActive = 4,
			CreatedBy = 5,
			CreatedAt = 6,
			UpdatedBy = 7,
			UpdatedAt = 8
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_ThemeKey = "ThemeKey";		            
		public const string Property_DisplayName = "DisplayName";		            
		public const string Property_PreviewImageUrl = "PreviewImageUrl";		            
		public const string Property_IsActive = "IsActive";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _ThemeKey;	            
		private String _DisplayName;	            
		private String _PreviewImageUrl;	            
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
		public String ThemeKey
		{	
			get{ return _ThemeKey; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ThemeKey, value, _ThemeKey);
				if (PropertyChanging(args))
				{
					_ThemeKey = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String DisplayName
		{	
			get{ return _DisplayName; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_DisplayName, value, _DisplayName);
				if (PropertyChanging(args))
				{
					_DisplayName = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String PreviewImageUrl
		{	
			get{ return _PreviewImageUrl; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PreviewImageUrl, value, _PreviewImageUrl);
				if (PropertyChanging(args))
				{
					_PreviewImageUrl = value;
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
		public  ThemeBase Clone()
		{
			ThemeBase newObj = new  ThemeBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.ThemeKey = this.ThemeKey;						
			newObj.DisplayName = this.DisplayName;						
			newObj.PreviewImageUrl = this.PreviewImageUrl;						
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
			info.AddValue(ThemeBase.Property_Id, Id);				
			info.AddValue(ThemeBase.Property_ThemeKey, ThemeKey);				
			info.AddValue(ThemeBase.Property_DisplayName, DisplayName);				
			info.AddValue(ThemeBase.Property_PreviewImageUrl, PreviewImageUrl);				
			info.AddValue(ThemeBase.Property_IsActive, IsActive);				
			info.AddValue(ThemeBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(ThemeBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(ThemeBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(ThemeBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}