using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "UserPermissionBase", Namespace = "http://www.piistech.com//entities")]
	public class UserPermissionBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			UserId = 1,
			PermissionId = 2,
			PermissionGroupId = 3
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_UserId = "UserId";		            
		public const string Property_PermissionId = "PermissionId";		            
		public const string Property_PermissionGroupId = "PermissionGroupId";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _UserId;	            
		private Nullable<Int32> _PermissionId;	            
		private Nullable<Int32> _PermissionGroupId;	            
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
		public Nullable<Int32> PermissionId
		{	
			get{ return _PermissionId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PermissionId, value, _PermissionId);
				if (PropertyChanging(args))
				{
					_PermissionId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Int32> PermissionGroupId
		{	
			get{ return _PermissionGroupId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PermissionGroupId, value, _PermissionGroupId);
				if (PropertyChanging(args))
				{
					_PermissionGroupId = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  UserPermissionBase Clone()
		{
			UserPermissionBase newObj = new  UserPermissionBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.UserId = this.UserId;						
			newObj.PermissionId = this.PermissionId;						
			newObj.PermissionGroupId = this.PermissionGroupId;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(UserPermissionBase.Property_Id, Id);				
			info.AddValue(UserPermissionBase.Property_UserId, UserId);				
			info.AddValue(UserPermissionBase.Property_PermissionId, PermissionId);				
			info.AddValue(UserPermissionBase.Property_PermissionGroupId, PermissionGroupId);				
		}
		#endregion

		
	}
}