using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "CarrierBase", Namespace = "http://www.piistech.com//entities")]
	public class CarrierBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			CarrierName = 1,
			ApiEndpoint = 2,
			RequiresApi = 3,
			IsActive = 4
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_CarrierName = "CarrierName";		            
		public const string Property_ApiEndpoint = "ApiEndpoint";		            
		public const string Property_RequiresApi = "RequiresApi";		            
		public const string Property_IsActive = "IsActive";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _CarrierName;	            
		private String _ApiEndpoint;	            
		private Boolean _RequiresApi;	            
		private Boolean _IsActive;	            
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
		public String CarrierName
		{	
			get{ return _CarrierName; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CarrierName, value, _CarrierName);
				if (PropertyChanging(args))
				{
					_CarrierName = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String ApiEndpoint
		{	
			get{ return _ApiEndpoint; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ApiEndpoint, value, _ApiEndpoint);
				if (PropertyChanging(args))
				{
					_ApiEndpoint = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Boolean RequiresApi
		{	
			get{ return _RequiresApi; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_RequiresApi, value, _RequiresApi);
				if (PropertyChanging(args))
				{
					_RequiresApi = value;
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

		#endregion
		
		#region Cloning Base Objects
		public  CarrierBase Clone()
		{
			CarrierBase newObj = new  CarrierBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.CarrierName = this.CarrierName;						
			newObj.ApiEndpoint = this.ApiEndpoint;						
			newObj.RequiresApi = this.RequiresApi;						
			newObj.IsActive = this.IsActive;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(CarrierBase.Property_Id, Id);				
			info.AddValue(CarrierBase.Property_CarrierName, CarrierName);				
			info.AddValue(CarrierBase.Property_ApiEndpoint, ApiEndpoint);				
			info.AddValue(CarrierBase.Property_RequiresApi, RequiresApi);				
			info.AddValue(CarrierBase.Property_IsActive, IsActive);				
		}
		#endregion

		
	}
}