using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "CompanyCarrierBase", Namespace = "http://www.piistech.com//entities")]
	public class CompanyCarrierBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			CompanyId = 1,
			CarrierId = 2,
			IsActive = 3,
			ApiKeyEncrypted = 4,
			ApiSecretEncrypted = 5,
			ApiKeyLast4 = 6,
			SecretUpdatedAt = 7,
			ApiUsernameEncrypted = 8,
			ApiPasswordEncrypted = 9,
			StoreId = 10
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_CompanyId = "CompanyId";		            
		public const string Property_CarrierId = "CarrierId";		            
		public const string Property_IsActive = "IsActive";		            
		public const string Property_ApiKeyEncrypted = "ApiKeyEncrypted";		            
		public const string Property_ApiSecretEncrypted = "ApiSecretEncrypted";		            
		public const string Property_ApiKeyLast4 = "ApiKeyLast4";		            
		public const string Property_SecretUpdatedAt = "SecretUpdatedAt";		            
		public const string Property_ApiUsernameEncrypted = "ApiUsernameEncrypted";		            
		public const string Property_ApiPasswordEncrypted = "ApiPasswordEncrypted";		            
		public const string Property_StoreId = "StoreId";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _CompanyId;	            
		private Int32 _CarrierId;	            
		private Boolean _IsActive;	            
		private String _ApiKeyEncrypted;	            
		private String _ApiSecretEncrypted;	            
		private String _ApiKeyLast4;	            
		private Nullable<DateTime> _SecretUpdatedAt;	            
		private String _ApiUsernameEncrypted;	            
		private String _ApiPasswordEncrypted;	            
		private Nullable<Int32> _StoreId;	            
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
		public Int32 CarrierId
		{	
			get{ return _CarrierId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CarrierId, value, _CarrierId);
				if (PropertyChanging(args))
				{
					_CarrierId = value;
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
		public String ApiKeyEncrypted
		{	
			get{ return _ApiKeyEncrypted; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ApiKeyEncrypted, value, _ApiKeyEncrypted);
				if (PropertyChanging(args))
				{
					_ApiKeyEncrypted = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String ApiSecretEncrypted
		{	
			get{ return _ApiSecretEncrypted; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ApiSecretEncrypted, value, _ApiSecretEncrypted);
				if (PropertyChanging(args))
				{
					_ApiSecretEncrypted = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String ApiKeyLast4
		{	
			get{ return _ApiKeyLast4; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ApiKeyLast4, value, _ApiKeyLast4);
				if (PropertyChanging(args))
				{
					_ApiKeyLast4 = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<DateTime> SecretUpdatedAt
		{	
			get{ return _SecretUpdatedAt; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SecretUpdatedAt, value, _SecretUpdatedAt);
				if (PropertyChanging(args))
				{
					_SecretUpdatedAt = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String ApiUsernameEncrypted
		{	
			get{ return _ApiUsernameEncrypted; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ApiUsernameEncrypted, value, _ApiUsernameEncrypted);
				if (PropertyChanging(args))
				{
					_ApiUsernameEncrypted = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String ApiPasswordEncrypted
		{	
			get{ return _ApiPasswordEncrypted; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ApiPasswordEncrypted, value, _ApiPasswordEncrypted);
				if (PropertyChanging(args))
				{
					_ApiPasswordEncrypted = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Int32> StoreId
		{	
			get{ return _StoreId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_StoreId, value, _StoreId);
				if (PropertyChanging(args))
				{
					_StoreId = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  CompanyCarrierBase Clone()
		{
			CompanyCarrierBase newObj = new  CompanyCarrierBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.CompanyId = this.CompanyId;						
			newObj.CarrierId = this.CarrierId;						
			newObj.IsActive = this.IsActive;						
			newObj.ApiKeyEncrypted = this.ApiKeyEncrypted;						
			newObj.ApiSecretEncrypted = this.ApiSecretEncrypted;						
			newObj.ApiKeyLast4 = this.ApiKeyLast4;						
			newObj.SecretUpdatedAt = this.SecretUpdatedAt;						
			newObj.ApiUsernameEncrypted = this.ApiUsernameEncrypted;						
			newObj.ApiPasswordEncrypted = this.ApiPasswordEncrypted;						
			newObj.StoreId = this.StoreId;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(CompanyCarrierBase.Property_Id, Id);				
			info.AddValue(CompanyCarrierBase.Property_CompanyId, CompanyId);				
			info.AddValue(CompanyCarrierBase.Property_CarrierId, CarrierId);				
			info.AddValue(CompanyCarrierBase.Property_IsActive, IsActive);				
			info.AddValue(CompanyCarrierBase.Property_ApiKeyEncrypted, ApiKeyEncrypted);				
			info.AddValue(CompanyCarrierBase.Property_ApiSecretEncrypted, ApiSecretEncrypted);				
			info.AddValue(CompanyCarrierBase.Property_ApiKeyLast4, ApiKeyLast4);				
			info.AddValue(CompanyCarrierBase.Property_SecretUpdatedAt, SecretUpdatedAt);				
			info.AddValue(CompanyCarrierBase.Property_ApiUsernameEncrypted, ApiUsernameEncrypted);				
			info.AddValue(CompanyCarrierBase.Property_ApiPasswordEncrypted, ApiPasswordEncrypted);				
			info.AddValue(CompanyCarrierBase.Property_StoreId, StoreId);				
		}
		#endregion

		
	}
}