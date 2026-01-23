using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "BackInStockRequestBase", Namespace = "http://www.piistech.com//entities")]
	public class BackInStockRequestBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			ProductVariantId = 1,
			ContactNumber = 2,
			RequestDate = 3,
			IsNotified = 4,
			NotifiedDate = 5,
			CreatedBy = 6,
			CreatedAt = 7,
			UpdatedBy = 8,
			UpdatedAt = 9
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_ProductVariantId = "ProductVariantId";		            
		public const string Property_ContactNumber = "ContactNumber";		            
		public const string Property_RequestDate = "RequestDate";		            
		public const string Property_IsNotified = "IsNotified";		            
		public const string Property_NotifiedDate = "NotifiedDate";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _ProductVariantId;	            
		private String _ContactNumber;	            
		private DateTime _RequestDate;	            
		private Boolean _IsNotified;	            
		private Nullable<DateTime> _NotifiedDate;	            
		private String _CreatedBy;	            
		private DateTime _CreatedAt;	            
		private String _UpdatedBy;	            
		private Nullable<DateTime> _UpdatedAt;	            
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
		public Int32 ProductVariantId
		{	
			get{ return _ProductVariantId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ProductVariantId, value, _ProductVariantId);
				if (PropertyChanging(args))
				{
					_ProductVariantId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String ContactNumber
		{	
			get{ return _ContactNumber; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ContactNumber, value, _ContactNumber);
				if (PropertyChanging(args))
				{
					_ContactNumber = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime RequestDate
		{	
			get{ return _RequestDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_RequestDate, value, _RequestDate);
				if (PropertyChanging(args))
				{
					_RequestDate = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Boolean IsNotified
		{	
			get{ return _IsNotified; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_IsNotified, value, _IsNotified);
				if (PropertyChanging(args))
				{
					_IsNotified = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<DateTime> NotifiedDate
		{	
			get{ return _NotifiedDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_NotifiedDate, value, _NotifiedDate);
				if (PropertyChanging(args))
				{
					_NotifiedDate = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String CreatedBy
		{	
			get{ return _CreatedBy; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CreatedBy, value, _CreatedBy);
				if (PropertyChanging(args))
				{
					_CreatedBy = value;
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

		[DataMember]
		public String UpdatedBy
		{	
			get{ return _UpdatedBy; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_UpdatedBy, value, _UpdatedBy);
				if (PropertyChanging(args))
				{
					_UpdatedBy = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<DateTime> UpdatedAt
		{	
			get{ return _UpdatedAt; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_UpdatedAt, value, _UpdatedAt);
				if (PropertyChanging(args))
				{
					_UpdatedAt = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  BackInStockRequestBase Clone()
		{
			BackInStockRequestBase newObj = new  BackInStockRequestBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.ProductVariantId = this.ProductVariantId;						
			newObj.ContactNumber = this.ContactNumber;						
			newObj.RequestDate = this.RequestDate;						
			newObj.IsNotified = this.IsNotified;						
			newObj.NotifiedDate = this.NotifiedDate;						
			newObj.CreatedBy = this.CreatedBy;						
			newObj.CreatedAt = this.CreatedAt;						
			newObj.UpdatedBy = this.UpdatedBy;						
			newObj.UpdatedAt = this.UpdatedAt;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(BackInStockRequestBase.Property_Id, Id);				
			info.AddValue(BackInStockRequestBase.Property_ProductVariantId, ProductVariantId);				
			info.AddValue(BackInStockRequestBase.Property_ContactNumber, ContactNumber);				
			info.AddValue(BackInStockRequestBase.Property_RequestDate, RequestDate);				
			info.AddValue(BackInStockRequestBase.Property_IsNotified, IsNotified);				
			info.AddValue(BackInStockRequestBase.Property_NotifiedDate, NotifiedDate);				
			info.AddValue(BackInStockRequestBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(BackInStockRequestBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(BackInStockRequestBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(BackInStockRequestBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion
	}	
}
