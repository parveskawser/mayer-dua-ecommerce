using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "CompanyDomainBase", Namespace = "http://www.piistech.com//entities")]
	public class CompanyDomainBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			CompanyId = 1,
			Domain = 2,
			IsPrimary = 3,
			IsActive = 4,
			VerifiedAt = 5,
			CreatedBy = 6,
			CreatedAt = 7,
			UpdatedBy = 8,
			UpdatedAt = 9
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_CompanyId = "CompanyId";		            
		public const string Property_Domain = "Domain";		            
		public const string Property_IsPrimary = "IsPrimary";		            
		public const string Property_IsActive = "IsActive";		            
		public const string Property_VerifiedAt = "VerifiedAt";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _CompanyId;	            
		private String _Domain;	            
		private Boolean _IsPrimary;	            
		private Boolean _IsActive;	            
		private Nullable<DateTime> _VerifiedAt;	            
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
		public String Domain
		{	
			get{ return _Domain; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Domain, value, _Domain);
				if (PropertyChanging(args))
				{
					_Domain = value;
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
		public Nullable<DateTime> VerifiedAt
		{	
			get{ return _VerifiedAt; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_VerifiedAt, value, _VerifiedAt);
				if (PropertyChanging(args))
				{
					_VerifiedAt = value;
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
		public  CompanyDomainBase Clone()
		{
			CompanyDomainBase newObj = new  CompanyDomainBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.CompanyId = this.CompanyId;						
			newObj.Domain = this.Domain;						
			newObj.IsPrimary = this.IsPrimary;						
			newObj.IsActive = this.IsActive;						
			newObj.VerifiedAt = this.VerifiedAt;						
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
			info.AddValue(CompanyDomainBase.Property_Id, Id);				
			info.AddValue(CompanyDomainBase.Property_CompanyId, CompanyId);				
			info.AddValue(CompanyDomainBase.Property_Domain, Domain);				
			info.AddValue(CompanyDomainBase.Property_IsPrimary, IsPrimary);				
			info.AddValue(CompanyDomainBase.Property_IsActive, IsActive);				
			info.AddValue(CompanyDomainBase.Property_VerifiedAt, VerifiedAt);				
			info.AddValue(CompanyDomainBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(CompanyDomainBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(CompanyDomainBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(CompanyDomainBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}