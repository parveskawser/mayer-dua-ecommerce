using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "PostalCodesBase", Namespace = "http://www.piistech.com//entities")]
	public class PostalCodesBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			PostCode = 1,
			SubOfficeEn = 2,
			SubOfficeBn = 3,
			ThanaEn = 4,
			ThanaBn = 5,
			DistrictBn = 6,
			DistrictEn = 7,
			DivisionBn = 8,
			DivisionEn = 9
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_PostCode = "PostCode";		            
		public const string Property_SubOfficeEn = "SubOfficeEn";		            
		public const string Property_SubOfficeBn = "SubOfficeBn";		            
		public const string Property_ThanaEn = "ThanaEn";		            
		public const string Property_ThanaBn = "ThanaBn";		            
		public const string Property_DistrictBn = "DistrictBn";		            
		public const string Property_DistrictEn = "DistrictEn";		            
		public const string Property_DivisionBn = "DivisionBn";		            
		public const string Property_DivisionEn = "DivisionEn";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _PostCode;	            
		private String _SubOfficeEn;	            
		private String _SubOfficeBn;	            
		private String _ThanaEn;	            
		private String _ThanaBn;	            
		private String _DistrictBn;	            
		private String _DistrictEn;	            
		private String _DivisionBn;	            
		private String _DivisionEn;	            
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
		public String PostCode
		{	
			get{ return _PostCode; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PostCode, value, _PostCode);
				if (PropertyChanging(args))
				{
					_PostCode = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String SubOfficeEn
		{	
			get{ return _SubOfficeEn; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SubOfficeEn, value, _SubOfficeEn);
				if (PropertyChanging(args))
				{
					_SubOfficeEn = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String SubOfficeBn
		{	
			get{ return _SubOfficeBn; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_SubOfficeBn, value, _SubOfficeBn);
				if (PropertyChanging(args))
				{
					_SubOfficeBn = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String ThanaEn
		{	
			get{ return _ThanaEn; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ThanaEn, value, _ThanaEn);
				if (PropertyChanging(args))
				{
					_ThanaEn = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String ThanaBn
		{	
			get{ return _ThanaBn; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ThanaBn, value, _ThanaBn);
				if (PropertyChanging(args))
				{
					_ThanaBn = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String DistrictBn
		{	
			get{ return _DistrictBn; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_DistrictBn, value, _DistrictBn);
				if (PropertyChanging(args))
				{
					_DistrictBn = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String DistrictEn
		{	
			get{ return _DistrictEn; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_DistrictEn, value, _DistrictEn);
				if (PropertyChanging(args))
				{
					_DistrictEn = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String DivisionBn
		{	
			get{ return _DivisionBn; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_DivisionBn, value, _DivisionBn);
				if (PropertyChanging(args))
				{
					_DivisionBn = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String DivisionEn
		{	
			get{ return _DivisionEn; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_DivisionEn, value, _DivisionEn);
				if (PropertyChanging(args))
				{
					_DivisionEn = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  PostalCodesBase Clone()
		{
			PostalCodesBase newObj = new  PostalCodesBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.PostCode = this.PostCode;						
			newObj.SubOfficeEn = this.SubOfficeEn;						
			newObj.SubOfficeBn = this.SubOfficeBn;						
			newObj.ThanaEn = this.ThanaEn;						
			newObj.ThanaBn = this.ThanaBn;						
			newObj.DistrictBn = this.DistrictBn;						
			newObj.DistrictEn = this.DistrictEn;						
			newObj.DivisionBn = this.DivisionBn;						
			newObj.DivisionEn = this.DivisionEn;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(PostalCodesBase.Property_Id, Id);				
			info.AddValue(PostalCodesBase.Property_PostCode, PostCode);				
			info.AddValue(PostalCodesBase.Property_SubOfficeEn, SubOfficeEn);				
			info.AddValue(PostalCodesBase.Property_SubOfficeBn, SubOfficeBn);				
			info.AddValue(PostalCodesBase.Property_ThanaEn, ThanaEn);				
			info.AddValue(PostalCodesBase.Property_ThanaBn, ThanaBn);				
			info.AddValue(PostalCodesBase.Property_DistrictBn, DistrictBn);				
			info.AddValue(PostalCodesBase.Property_DistrictEn, DistrictEn);				
			info.AddValue(PostalCodesBase.Property_DivisionBn, DivisionBn);				
			info.AddValue(PostalCodesBase.Property_DivisionEn, DivisionEn);				
		}
		#endregion

		
	}
}
