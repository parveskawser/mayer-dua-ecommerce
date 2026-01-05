using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "VariantImageBase", Namespace = "http://www.piistech.com//entities")]
	public class VariantImageBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			VariantId = 1,
			ImageUrl = 2,
			AltText = 3,
			DisplayOrder = 4
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_VariantId = "VariantId";		            
		public const string Property_ImageUrl = "ImageUrl";		            
		public const string Property_AltText = "AltText";		            
		public const string Property_DisplayOrder = "DisplayOrder";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _VariantId;	            
		private String _ImageUrl;	            
		private String _AltText;	            
		private Int32 _DisplayOrder;	            
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
		public Int32 VariantId
		{	
			get{ return _VariantId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_VariantId, value, _VariantId);
				if (PropertyChanging(args))
				{
					_VariantId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String ImageUrl
		{	
			get{ return _ImageUrl; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ImageUrl, value, _ImageUrl);
				if (PropertyChanging(args))
				{
					_ImageUrl = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String AltText
		{	
			get{ return _AltText; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AltText, value, _AltText);
				if (PropertyChanging(args))
				{
					_AltText = value;
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

		#endregion
		
		#region Cloning Base Objects
		public  VariantImageBase Clone()
		{
			VariantImageBase newObj = new  VariantImageBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.VariantId = this.VariantId;						
			newObj.ImageUrl = this.ImageUrl;						
			newObj.AltText = this.AltText;						
			newObj.DisplayOrder = this.DisplayOrder;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(VariantImageBase.Property_Id, Id);				
			info.AddValue(VariantImageBase.Property_VariantId, VariantId);				
			info.AddValue(VariantImageBase.Property_ImageUrl, ImageUrl);				
			info.AddValue(VariantImageBase.Property_AltText, AltText);				
			info.AddValue(VariantImageBase.Property_DisplayOrder, DisplayOrder);				
		}
		#endregion

		
	}
}