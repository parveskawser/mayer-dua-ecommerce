using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "CustomerBase", Namespace = "http://www.piistech.com//entities")]
	public class CustomerBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			CustomerName = 1,
			Email = 2,
			Phone = 3,
			IsActive = 4,
			DateOfBirth = 5,
			Gender = 6,
			Notes = 7,
			CreatedBy = 8,
			CreatedAt = 9,
			UpdatedBy = 10,
			UpdatedAt = 11
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_CustomerName = "CustomerName";		            
		public const string Property_Email = "Email";		            
		public const string Property_Phone = "Phone";		            
		public const string Property_IsActive = "IsActive";		            
		public const string Property_DateOfBirth = "DateOfBirth";		            
		public const string Property_Gender = "Gender";		            
		public const string Property_Notes = "Notes";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _CustomerName;	            
		private String _Email;	            
		private String _Phone;	            
		private Boolean _IsActive;	            
		private Nullable<DateTime> _DateOfBirth;	            
		private String _Gender;	            
		private String _Notes;	            
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
		public String CustomerName
		{	
			get{ return _CustomerName; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CustomerName, value, _CustomerName);
				if (PropertyChanging(args))
				{
					_CustomerName = value;
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
		public Nullable<DateTime> DateOfBirth
		{	
			get{ return _DateOfBirth; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_DateOfBirth, value, _DateOfBirth);
				if (PropertyChanging(args))
				{
					_DateOfBirth = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Gender
		{	
			get{ return _Gender; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Gender, value, _Gender);
				if (PropertyChanging(args))
				{
					_Gender = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Notes
		{	
			get{ return _Notes; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Notes, value, _Notes);
				if (PropertyChanging(args))
				{
					_Notes = value;
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
		public  CustomerBase Clone()
		{
			CustomerBase newObj = new  CustomerBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.CustomerName = this.CustomerName;						
			newObj.Email = this.Email;						
			newObj.Phone = this.Phone;						
			newObj.IsActive = this.IsActive;						
			newObj.DateOfBirth = this.DateOfBirth;						
			newObj.Gender = this.Gender;						
			newObj.Notes = this.Notes;						
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
			info.AddValue(CustomerBase.Property_Id, Id);				
			info.AddValue(CustomerBase.Property_CustomerName, CustomerName);				
			info.AddValue(CustomerBase.Property_Email, Email);				
			info.AddValue(CustomerBase.Property_Phone, Phone);				
			info.AddValue(CustomerBase.Property_IsActive, IsActive);				
			info.AddValue(CustomerBase.Property_DateOfBirth, DateOfBirth);				
			info.AddValue(CustomerBase.Property_Gender, Gender);				
			info.AddValue(CustomerBase.Property_Notes, Notes);				
			info.AddValue(CustomerBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(CustomerBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(CustomerBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(CustomerBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}
