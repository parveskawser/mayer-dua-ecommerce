using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "PermissionBase", Namespace = "http://www.piistech.com//entities")]
	public class PermissionBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			Name = 1
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_Name = "Name";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _Name;	            
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
		public String Name
		{	
			get{ return _Name; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Name, value, _Name);
				if (PropertyChanging(args))
				{
					_Name = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  PermissionBase Clone()
		{
			PermissionBase newObj = new  PermissionBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.Name = this.Name;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(PermissionBase.Property_Id, Id);				
			info.AddValue(PermissionBase.Property_Name, Name);				
		}
		#endregion

		
	}
}