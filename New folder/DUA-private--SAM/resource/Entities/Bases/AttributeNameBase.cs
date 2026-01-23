using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "AttributeNameBase", Namespace = "http://www.piistech.com//entities")]
	public class AttributeNameBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			Name = 1,
			DisplayOrder = 2,
			IsVariantAffecting = 3,
			IsActive = 4,
			CompanyId = 5
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_Name = "Name";		            
		public const string Property_DisplayOrder = "DisplayOrder";		            
		public const string Property_IsVariantAffecting = "IsVariantAffecting";		            
		public const string Property_IsActive = "IsActive";		            
		public const string Property_CompanyId = "CompanyId";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _Name;	            
		private Int32 _DisplayOrder;	            
		private Boolean _IsVariantAffecting;	            
		private Boolean _IsActive;	            
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
		public Int32 DisplayOrder
		{	
			get{ return _DisplayOrder; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_DisplayOrder, value, _DisplayOrder);
				if (PropertyChanging(args))
				{
					_DisplayOrder = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Boolean IsVariantAffecting
		{	
			get{ return _IsVariantAffecting; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_IsVariantAffecting, value, _IsVariantAffecting);
				if (PropertyChanging(args))
				{
					_IsVariantAffecting = value;
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
		public  AttributeNameBase Clone()
		{
			AttributeNameBase newObj = new  AttributeNameBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.Name = this.Name;						
			newObj.DisplayOrder = this.DisplayOrder;						
			newObj.IsVariantAffecting = this.IsVariantAffecting;						
			newObj.IsActive = this.IsActive;						
			newObj.CompanyId = this.CompanyId;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(AttributeNameBase.Property_Id, Id);				
			info.AddValue(AttributeNameBase.Property_Name, Name);				
			info.AddValue(AttributeNameBase.Property_DisplayOrder, DisplayOrder);				
			info.AddValue(AttributeNameBase.Property_IsVariantAffecting, IsVariantAffecting);				
			info.AddValue(AttributeNameBase.Property_IsActive, IsActive);				
			info.AddValue(AttributeNameBase.Property_CompanyId, CompanyId);				
		}
		#endregion

		
	}
}