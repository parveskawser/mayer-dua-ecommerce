using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "LanguageResourceBase", Namespace = "http://www.piistech.com//entities")]
	public class LanguageResourceBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			CompanyId = 1,
			LanguageId = 2,
			LKey = 3,
			LValue = 4,
			Description = 5,
			IsActive = 6,
			CreatedBy = 7,
			CreatedAt = 8,
			UpdatedBy = 9,
			UpdatedAt = 10
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_CompanyId = "CompanyId";		            
		public const string Property_LanguageId = "LanguageId";		            
		public const string Property_LKey = "LKey";		            
		public const string Property_LValue = "LValue";		            
		public const string Property_Description = "Description";		            
		public const string Property_IsActive = "IsActive";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Nullable<Int32> _CompanyId;	            
		private Int32 _LanguageId;	            
		private String _LKey;	            
		private String _LValue;	            
		private String _Description;	            
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
		public Nullable<Int32> CompanyId
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
		public Int32 LanguageId
		{	
			get{ return _LanguageId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_LanguageId, value, _LanguageId);
				if (PropertyChanging(args))
				{
					_LanguageId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String LKey
		{	
			get{ return _LKey; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_LKey, value, _LKey);
				if (PropertyChanging(args))
				{
					_LKey = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String LValue
		{	
			get{ return _LValue; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_LValue, value, _LValue);
				if (PropertyChanging(args))
				{
					_LValue = value;
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
		public  LanguageResourceBase Clone()
		{
			LanguageResourceBase newObj = new  LanguageResourceBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.CompanyId = this.CompanyId;						
			newObj.LanguageId = this.LanguageId;						
			newObj.LKey = this.LKey;						
			newObj.LValue = this.LValue;						
			newObj.Description = this.Description;						
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
			info.AddValue(LanguageResourceBase.Property_Id, Id);				
			info.AddValue(LanguageResourceBase.Property_CompanyId, CompanyId);				
			info.AddValue(LanguageResourceBase.Property_LanguageId, LanguageId);				
			info.AddValue(LanguageResourceBase.Property_LKey, LKey);				
			info.AddValue(LanguageResourceBase.Property_LValue, LValue);				
			info.AddValue(LanguageResourceBase.Property_Description, Description);				
			info.AddValue(LanguageResourceBase.Property_IsActive, IsActive);				
			info.AddValue(LanguageResourceBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(LanguageResourceBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(LanguageResourceBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(LanguageResourceBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}
