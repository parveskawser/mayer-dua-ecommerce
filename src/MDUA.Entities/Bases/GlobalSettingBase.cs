using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "GlobalSettingBase", Namespace = "http://www.piistech.com//entities")]
	public class GlobalSettingBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			CompanyId = 1,
			GKey = 2,
			GContent = 3
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_CompanyId = "CompanyId";		            
		public const string Property_GKey = "GKey";		            
		public const string Property_GContent = "GContent";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _CompanyId;	            
		private String _GKey;	            
		private String _GContent;	            
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
		public String GKey
		{	
			get{ return _GKey; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_GKey, value, _GKey);
				if (PropertyChanging(args))
				{
					_GKey = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String GContent
		{	
			get{ return _GContent; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_GContent, value, _GContent);
				if (PropertyChanging(args))
				{
					_GContent = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  GlobalSettingBase Clone()
		{
			GlobalSettingBase newObj = new  GlobalSettingBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.CompanyId = this.CompanyId;						
			newObj.GKey = this.GKey;						
			newObj.GContent = this.GContent;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(GlobalSettingBase.Property_Id, Id);				
			info.AddValue(GlobalSettingBase.Property_CompanyId, CompanyId);				
			info.AddValue(GlobalSettingBase.Property_GKey, GKey);				
			info.AddValue(GlobalSettingBase.Property_GContent, GContent);				
		}
		#endregion

		
	}
}
