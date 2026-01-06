using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "ProductCategoryBase", Namespace = "http://www.piistech.com//entities")]
	public class ProductCategoryBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			Name = 1,
			CompanyId = 2
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_Name = "Name";		            
		public const string Property_CompanyId = "CompanyId";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _Name;	            
		private Nullable<Int32> _CompanyId;	            
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

		[DataMember]
		public Nullable<Int32> CompanyId
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

		#endregion
		
		#region Cloning Base Objects
		public  ProductCategoryBase Clone()
		{
			ProductCategoryBase newObj = new  ProductCategoryBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.Name = this.Name;						
			newObj.CompanyId = this.CompanyId;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(ProductCategoryBase.Property_Id, Id);				
			info.AddValue(ProductCategoryBase.Property_Name, Name);				
			info.AddValue(ProductCategoryBase.Property_CompanyId, CompanyId);				
		}
		#endregion

		
	}
}