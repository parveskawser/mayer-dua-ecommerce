using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "ProductAttributeBase", Namespace = "http://www.piistech.com//entities")]
	public class ProductAttributeBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			ProductId = 1,
			AttributeId = 2,
			DisplayOrder = 3
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_ProductId = "ProductId";		            
		public const string Property_AttributeId = "AttributeId";		            
		public const string Property_DisplayOrder = "DisplayOrder";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _ProductId;	            
		private Int32 _AttributeId;	            
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
		public Int32 ProductId
		{	
			get{ return _ProductId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ProductId, value, _ProductId);
				if (PropertyChanging(args))
				{
					_ProductId = value;
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
		public  ProductAttributeBase Clone()
		{
			ProductAttributeBase newObj = new  ProductAttributeBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.ProductId = this.ProductId;						
			newObj.AttributeId = this.AttributeId;						
			newObj.DisplayOrder = this.DisplayOrder;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(ProductAttributeBase.Property_Id, Id);				
			info.AddValue(ProductAttributeBase.Property_ProductId, ProductId);				
			info.AddValue(ProductAttributeBase.Property_AttributeId, AttributeId);				
			info.AddValue(ProductAttributeBase.Property_DisplayOrder, DisplayOrder);				
		}
		#endregion

		
	}
}