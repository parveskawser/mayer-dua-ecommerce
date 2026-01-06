using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "CompanyVendorBase", Namespace = "http://www.piistech.com//entities")]
	public class CompanyVendorBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			CompanyId = 1,
			VendorId = 2
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_CompanyId = "CompanyId";		            
		public const string Property_VendorId = "VendorId";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _CompanyId;	            
		private Int32 _VendorId;	            
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
		public Int32 VendorId
		{	
			get{ return _VendorId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_VendorId, value, _VendorId);
				if (PropertyChanging(args))
				{
					_VendorId = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  CompanyVendorBase Clone()
		{
			CompanyVendorBase newObj = new  CompanyVendorBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.CompanyId = this.CompanyId;						
			newObj.VendorId = this.VendorId;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(CompanyVendorBase.Property_Id, Id);				
			info.AddValue(CompanyVendorBase.Property_CompanyId, CompanyId);				
			info.AddValue(CompanyVendorBase.Property_VendorId, VendorId);				
		}
		#endregion

		
	}
}