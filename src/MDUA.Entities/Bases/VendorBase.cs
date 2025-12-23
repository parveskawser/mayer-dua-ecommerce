using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "VendorBase", Namespace = "http://www.piistech.com//entities")]
	public class VendorBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			VendorName = 1,
			Email = 2,
			Phone = 3,
			CreatedBy = 4,
			CreatedAt = 5,
			UpdatedBy = 6,
			UpdatedAt = 7
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_VendorName = "VendorName";		            
		public const string Property_Email = "Email";		            
		public const string Property_Phone = "Phone";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _VendorName;	            
		private String _Email;	            
		private String _Phone;	            
		private String _CreatedBy;	            
		private Nullable<DateTime> _CreatedAt;	            
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
		public String VendorName
		{	
			get{ return _VendorName; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_VendorName, value, _VendorName);
				if (PropertyChanging(args))
				{
					_VendorName = value;
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
		public  VendorBase Clone()
		{
			VendorBase newObj = new  VendorBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.VendorName = this.VendorName;						
			newObj.Email = this.Email;						
			newObj.Phone = this.Phone;						
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
			info.AddValue(VendorBase.Property_Id, Id);				
			info.AddValue(VendorBase.Property_VendorName, VendorName);				
			info.AddValue(VendorBase.Property_Email, Email);				
			info.AddValue(VendorBase.Property_Phone, Phone);				
			info.AddValue(VendorBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(VendorBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(VendorBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(VendorBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}
