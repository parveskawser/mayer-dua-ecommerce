using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "UserSessionBase", Namespace = "http://www.piistech.com//entities")]
	public class UserSessionBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			SessionKey = 1,
			UserId = 2,
			IPAddress = 3,
			DeviceInfo = 4,
			CreatedAt = 5,
			LastActiveAt = 6,
			IsActive = 7
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_SessionKey = "SessionKey";		            
		public const string Property_UserId = "UserId";		            
		public const string Property_IPAddress = "IPAddress";		            
		public const string Property_DeviceInfo = "DeviceInfo";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_LastActiveAt = "LastActiveAt";		            
		public const string Property_IsActive = "IsActive";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Guid _SessionKey;	            
		private Int32 _UserId;	            
		private String _IPAddress;	            
		private String _DeviceInfo;	            
		private Nullable<DateTime> _CreatedAt;	            
		private Nullable<DateTime> _LastActiveAt;	            
		private Nullable<Boolean> _IsActive;	            
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
		public Guid SessionKey
		{	
			get{ return _SessionKey; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SessionKey, value, _SessionKey);
				if (PropertyChanging(args))
				{
					_SessionKey = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 UserId
		{	
			get{ return _UserId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_UserId, value, _UserId);
				if (PropertyChanging(args))
				{
					_UserId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String IPAddress
		{	
			get{ return _IPAddress; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_IPAddress, value, _IPAddress);
				if (PropertyChanging(args))
				{
					_IPAddress = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String DeviceInfo
		{	
			get{ return _DeviceInfo; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_DeviceInfo, value, _DeviceInfo);
				if (PropertyChanging(args))
				{
					_DeviceInfo = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<DateTime> CreatedAt
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
		public Nullable<DateTime> LastActiveAt
		{	
			get{ return _LastActiveAt; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_LastActiveAt, value, _LastActiveAt);
				if (PropertyChanging(args))
				{
					_LastActiveAt = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Boolean> IsActive
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

		#endregion
		
		#region Cloning Base Objects
		public  UserSessionBase Clone()
		{
			UserSessionBase newObj = new  UserSessionBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.SessionKey = this.SessionKey;						
			newObj.UserId = this.UserId;						
			newObj.IPAddress = this.IPAddress;						
			newObj.DeviceInfo = this.DeviceInfo;						
			newObj.CreatedAt = this.CreatedAt;						
			newObj.LastActiveAt = this.LastActiveAt;						
			newObj.IsActive = this.IsActive;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(UserSessionBase.Property_Id, Id);				
			info.AddValue(UserSessionBase.Property_SessionKey, SessionKey);				
			info.AddValue(UserSessionBase.Property_UserId, UserId);				
			info.AddValue(UserSessionBase.Property_IPAddress, IPAddress);				
			info.AddValue(UserSessionBase.Property_DeviceInfo, DeviceInfo);				
			info.AddValue(UserSessionBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(UserSessionBase.Property_LastActiveAt, LastActiveAt);				
			info.AddValue(UserSessionBase.Property_IsActive, IsActive);				
		}
		#endregion

		
	}
}