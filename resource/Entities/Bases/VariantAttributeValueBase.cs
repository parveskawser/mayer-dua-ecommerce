using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "VariantAttributeValueBase", Namespace = "http://www.piistech.com//entities")]
	public class VariantAttributeValueBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			VariantId = 1,
			AttributeId = 2,
			AttributeValueId = 3,
			DisplayOrder = 4
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_VariantId = "VariantId";		            
		public const string Property_AttributeId = "AttributeId";		            
		public const string Property_AttributeValueId = "AttributeValueId";		            
		public const string Property_DisplayOrder = "DisplayOrder";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _VariantId;	            
		private Int32 _AttributeId;	            
		private Int32 _AttributeValueId;	            
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
		public Int32 AttributeId
		{	
			get{ return _AttributeId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AttributeId, value, _AttributeId);
				if (PropertyChanging(args))
				{
					_AttributeId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 AttributeValueId
		{	
			get{ return _AttributeValueId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AttributeValueId, value, _AttributeValueId);
				if (PropertyChanging(args))
				{
					_AttributeValueId = value;
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
		public  VariantAttributeValueBase Clone()
		{
			VariantAttributeValueBase newObj = new  VariantAttributeValueBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.VariantId = this.VariantId;						
			newObj.AttributeId = this.AttributeId;						
			newObj.AttributeValueId = this.AttributeValueId;						
			newObj.DisplayOrder = this.DisplayOrder;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(VariantAttributeValueBase.Property_Id, Id);				
			info.AddValue(VariantAttributeValueBase.Property_VariantId, VariantId);				
			info.AddValue(VariantAttributeValueBase.Property_AttributeId, AttributeId);				
			info.AddValue(VariantAttributeValueBase.Property_AttributeValueId, AttributeValueId);				
			info.AddValue(VariantAttributeValueBase.Property_DisplayOrder, DisplayOrder);				
		}
		#endregion

		
	}
}