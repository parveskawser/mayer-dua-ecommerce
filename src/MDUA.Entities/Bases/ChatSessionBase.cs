using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "ChatSessionBase", Namespace = "http://www.piistech.com//entities")]
	public class ChatSessionBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			SessionGuid = 1,
			UserLoginId = 2,
			GuestName = 3,
			Status = 4,
			StartedAt = 5,
			LastMessageAt = 6,
			IsActive = 7
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_SessionGuid = "SessionGuid";		            
		public const string Property_UserLoginId = "UserLoginId";		            
		public const string Property_GuestName = "GuestName";		            
		public const string Property_Status = "Status";		            
		public const string Property_StartedAt = "StartedAt";		            
		public const string Property_LastMessageAt = "LastMessageAt";		            
		public const string Property_IsActive = "IsActive";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Guid _SessionGuid;	            
		private Nullable<Int32> _UserLoginId;	            
		private String _GuestName;	            
		private String _Status;	            
		private DateTime _StartedAt;	            
		private DateTime _LastMessageAt;	            
		private Boolean _IsActive;	            
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
		public Guid SessionGuid
		{	
			get{ return _SessionGuid; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SessionGuid, value, _SessionGuid);
				if (PropertyChanging(args))
				{
					_SessionGuid = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Int32> UserLoginId
		{	
			get{ return _UserLoginId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_UserLoginId, value, _UserLoginId);
				if (PropertyChanging(args))
				{
					_UserLoginId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String GuestName
		{	
			get{ return _GuestName; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_GuestName, value, _GuestName);
				if (PropertyChanging(args))
				{
					_GuestName = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Status
		{	
			get{ return _Status; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Status, value, _Status);
				if (PropertyChanging(args))
				{
					_Status = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime StartedAt
		{	
			get{ return _StartedAt; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_StartedAt, value, _StartedAt);
				if (PropertyChanging(args))
				{
					_StartedAt = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime LastMessageAt
		{	
			get{ return _LastMessageAt; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_LastMessageAt, value, _LastMessageAt);
				if (PropertyChanging(args))
				{
					_LastMessageAt = value;
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

		#endregion
		
		#region Cloning Base Objects
		public  ChatSessionBase Clone()
		{
			ChatSessionBase newObj = new  ChatSessionBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.SessionGuid = this.SessionGuid;						
			newObj.UserLoginId = this.UserLoginId;						
			newObj.GuestName = this.GuestName;						
			newObj.Status = this.Status;						
			newObj.StartedAt = this.StartedAt;						
			newObj.LastMessageAt = this.LastMessageAt;						
			newObj.IsActive = this.IsActive;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(ChatSessionBase.Property_Id, Id);				
			info.AddValue(ChatSessionBase.Property_SessionGuid, SessionGuid);				
			info.AddValue(ChatSessionBase.Property_UserLoginId, UserLoginId);				
			info.AddValue(ChatSessionBase.Property_GuestName, GuestName);				
			info.AddValue(ChatSessionBase.Property_Status, Status);				
			info.AddValue(ChatSessionBase.Property_StartedAt, StartedAt);				
			info.AddValue(ChatSessionBase.Property_LastMessageAt, LastMessageAt);				
			info.AddValue(ChatSessionBase.Property_IsActive, IsActive);				
		}
		#endregion

		
	}
}