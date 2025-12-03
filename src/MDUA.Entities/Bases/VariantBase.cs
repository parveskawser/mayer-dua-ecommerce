using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "VariantBase", Namespace = "http://www.piistech.com//entities")]
	public class VariantBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			ProductId = 1,
			Sku = 2,
			UpcBarcode = 3,
			VariantKeyHash = 4,
			IsActive = 5,
			CreatedAt = 6
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_ProductId = "ProductId";		            
		public const string Property_Sku = "Sku";		            
		public const string Property_UpcBarcode = "UpcBarcode";		            
		public const string Property_VariantKeyHash = "VariantKeyHash";		            
		public const string Property_IsActive = "IsActive";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _ProductId;	            
		private String _Sku;	            
		private String _UpcBarcode;	            
		private String _VariantKeyHash;	            
		private Boolean _IsActive;	            
		private DateTime _CreatedAt;	            
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
		public String Sku
		{	
			get{ return _Sku; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Sku, value, _Sku);
				if (PropertyChanging(args))
				{
					_Sku = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String UpcBarcode
		{	
			get{ return _UpcBarcode; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_UpcBarcode, value, _UpcBarcode);
				if (PropertyChanging(args))
				{
					_UpcBarcode = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String VariantKeyHash
		{	
			get{ return _VariantKeyHash; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_VariantKeyHash, value, _VariantKeyHash);
				if (PropertyChanging(args))
				{
					_VariantKeyHash = value;
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

		#endregion
		
		#region Cloning Base Objects
		public  VariantBase Clone()
		{
			VariantBase newObj = new  VariantBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.ProductId = this.ProductId;						
			newObj.Sku = this.Sku;						
			newObj.UpcBarcode = this.UpcBarcode;						
			newObj.VariantKeyHash = this.VariantKeyHash;						
			newObj.IsActive = this.IsActive;						
			newObj.CreatedAt = this.CreatedAt;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(VariantBase.Property_Id, Id);				
			info.AddValue(VariantBase.Property_ProductId, ProductId);				
			info.AddValue(VariantBase.Property_Sku, Sku);				
			info.AddValue(VariantBase.Property_UpcBarcode, UpcBarcode);				
			info.AddValue(VariantBase.Property_VariantKeyHash, VariantKeyHash);				
			info.AddValue(VariantBase.Property_IsActive, IsActive);				
			info.AddValue(VariantBase.Property_CreatedAt, CreatedAt);				
		}
		#endregion

		
	}
}
