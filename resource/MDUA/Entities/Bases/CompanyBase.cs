using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "CompanyBase", Namespace = "http://www.piistech.com//entities")]
	public class CompanyBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			CompanyName = 1,
			CompanyCode = 2,
			Email = 3,
			Phone = 4,
			Website = 5,
			Address = 6,
			IsActive = 7,
			LogoImg = 8,
			CreatedBy = 9,
			CreatedAt = 10,
			UpdatedBy = 11,
			UpdatedAt = 12
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_CompanyName = "CompanyName";		            
		public const string Property_CompanyCode = "CompanyCode";		            
		public const string Property_Email = "Email";		            
		public const string Property_Phone = "Phone";		            
		public const string Property_Website = "Website";		            
		public const string Property_Address = "Address";		            
		public const string Property_IsActive = "IsActive";		            
		public const string Property_LogoImg = "LogoImg";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _CompanyName;	            
		private String _CompanyCode;	            
		private String _Email;	            
		private String _Phone;	            
		private String _Website;	            
		private String _Address;	            
		private Boolean _IsActive;	            
		private String _LogoImg;	            
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
		public String CompanyName
		{	
			get{ return _CompanyName; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CompanyName, value, _CompanyName);
				if (PropertyChanging(args))
				{
					_CompanyName = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String CompanyCode
		{	
			get{ return _CompanyCode; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CompanyCode, value, _CompanyCode);
				if (PropertyChanging(args))
				{
					_CompanyCode = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Email
		{	
			get{ return _Email; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Email, value, _Email);
				if (PropertyChanging(args))
				{
					_Email = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Phone
		{	
			get{ return _Phone; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Phone, value, _Phone);
				if (PropertyChanging(args))
				{
					_Phone = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Website
		{	
			get{ return _Website; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Website, value, _Website);
				if (PropertyChanging(args))
				{
					_Website = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Address
		{	
			get{ return _Address; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Address, value, _Address);
				if (PropertyChanging(args))
				{
					_Address = value;
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
		public String LogoImg
		{	
			get{ return _LogoImg; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_LogoImg, value, _LogoImg);
				if (PropertyChanging(args))
				{
					_LogoImg = value;
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
		public  CompanyBase Clone()
		{
			CompanyBase newObj = new  CompanyBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.CompanyName = this.CompanyName;						
			newObj.CompanyCode = this.CompanyCode;						
			newObj.Email = this.Email;						
			newObj.Phone = this.Phone;						
			newObj.Website = this.Website;						
			newObj.Address = this.Address;						
			newObj.IsActive = this.IsActive;						
			newObj.LogoImg = this.LogoImg;						
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
			info.AddValue(CompanyBase.Property_Id, Id);				
			info.AddValue(CompanyBase.Property_CompanyName, CompanyName);				
			info.AddValue(CompanyBase.Property_CompanyCode, CompanyCode);				
			info.AddValue(CompanyBase.Property_Email, Email);				
			info.AddValue(CompanyBase.Property_Phone, Phone);				
			info.AddValue(CompanyBase.Property_Website, Website);				
			info.AddValue(CompanyBase.Property_Address, Address);				
			info.AddValue(CompanyBase.Property_IsActive, IsActive);				
			info.AddValue(CompanyBase.Property_LogoImg, LogoImg);				
			info.AddValue(CompanyBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(CompanyBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(CompanyBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(CompanyBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}