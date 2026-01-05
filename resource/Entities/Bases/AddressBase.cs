using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "AddressBase", Namespace = "http://www.piistech.com//entities")]
	public class AddressBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			CustomerId = 1,
			Country = 2,
			Divison = 3,
			City = 4,
			Thana = 5,
			SubOffice = 6,
			Street = 7,
			PostalCode = 8,
			ZipCode = 9,
			AddressType = 10,
			CreatedBy = 11,
			CreatedAt = 12,
			UpdatedBy = 13,
			UpdatedAt = 14
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_CustomerId = "CustomerId";		            
		public const string Property_Country = "Country";		            
		public const string Property_Divison = "Divison";		            
		public const string Property_City = "City";		            
		public const string Property_Thana = "Thana";		            
		public const string Property_SubOffice = "SubOffice";		            
		public const string Property_Street = "Street";		            
		public const string Property_PostalCode = "PostalCode";		            
		public const string Property_ZipCode = "ZipCode";		            
		public const string Property_AddressType = "AddressType";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _CustomerId;	            
		private String _Country;	            
		private String _Divison;	            
		private String _City;	            
		private String _Thana;	            
		private String _SubOffice;	            
		private String _Street;	            
		private String _PostalCode;	            
		private Char[] _ZipCode;	            
		private String _AddressType;	            
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
		public Int32 CustomerId
		{	
			get{ return _CustomerId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CustomerId, value, _CustomerId);
				if (PropertyChanging(args))
				{
					_CustomerId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Country
		{	
			get{ return _Country; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Country, value, _Country);
				if (PropertyChanging(args))
				{
					_Country = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Divison
		{	
			get{ return _Divison; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Divison, value, _Divison);
				if (PropertyChanging(args))
				{
					_Divison = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String City
		{	
			get{ return _City; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_City, value, _City);
				if (PropertyChanging(args))
				{
					_City = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Thana
		{	
			get{ return _Thana; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Thana, value, _Thana);
				if (PropertyChanging(args))
				{
					_Thana = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String SubOffice
		{	
			get{ return _SubOffice; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SubOffice, value, _SubOffice);
				if (PropertyChanging(args))
				{
					_SubOffice = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Street
		{	
			get{ return _Street; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Street, value, _Street);
				if (PropertyChanging(args))
				{
					_Street = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String PostalCode
		{	
			get{ return _PostalCode; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PostalCode, value, _PostalCode);
				if (PropertyChanging(args))
				{
					_PostalCode = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Char[] ZipCode
		{	
			get{ return _ZipCode; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ZipCode, value, _ZipCode);
				if (PropertyChanging(args))
				{
					_ZipCode = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String AddressType
		{	
			get{ return _AddressType; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AddressType, value, _AddressType);
				if (PropertyChanging(args))
				{
					_AddressType = value;
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
		public  AddressBase Clone()
		{
			AddressBase newObj = new  AddressBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.CustomerId = this.CustomerId;						
			newObj.Country = this.Country;						
			newObj.Divison = this.Divison;						
			newObj.City = this.City;						
			newObj.Thana = this.Thana;						
			newObj.SubOffice = this.SubOffice;						
			newObj.Street = this.Street;						
			newObj.PostalCode = this.PostalCode;						
			newObj.ZipCode = this.ZipCode;						
			newObj.AddressType = this.AddressType;						
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
			info.AddValue(AddressBase.Property_Id, Id);				
			info.AddValue(AddressBase.Property_CustomerId, CustomerId);				
			info.AddValue(AddressBase.Property_Country, Country);				
			info.AddValue(AddressBase.Property_Divison, Divison);				
			info.AddValue(AddressBase.Property_City, City);				
			info.AddValue(AddressBase.Property_Thana, Thana);				
			info.AddValue(AddressBase.Property_SubOffice, SubOffice);				
			info.AddValue(AddressBase.Property_Street, Street);				
			info.AddValue(AddressBase.Property_PostalCode, PostalCode);				
			info.AddValue(AddressBase.Property_ZipCode, ZipCode);				
			info.AddValue(AddressBase.Property_AddressType, AddressType);				
			info.AddValue(AddressBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(AddressBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(AddressBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(AddressBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}