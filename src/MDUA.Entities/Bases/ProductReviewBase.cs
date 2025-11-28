using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "ProductReviewBase", Namespace = "http://www.piistech.com//entities")]
	public class ProductReviewBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			ProductId = 1,
			CustomerName = 2,
			Rating = 3,
			ReviewText = 4,
			IsApproved = 5,
			CreatedBy = 6,
			CreatedAt = 7,
			UpdatedBy = 8,
			UpdatedAt = 9
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_ProductId = "ProductId";		            
		public const string Property_CustomerName = "CustomerName";		            
		public const string Property_Rating = "Rating";		            
		public const string Property_ReviewText = "ReviewText";		            
		public const string Property_IsApproved = "IsApproved";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _ProductId;	            
		private String _CustomerName;	            
		private Int32 _Rating;	            
		private String _ReviewText;	            
		private Boolean _IsApproved;	            
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
		public String CustomerName
		{	
			get{ return _CustomerName; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CustomerName, value, _CustomerName);
				if (PropertyChanging(args))
				{
					_CustomerName = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 Rating
		{	
			get{ return _Rating; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Rating, value, _Rating);
				if (PropertyChanging(args))
				{
					_Rating = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String ReviewText
		{	
			get{ return _ReviewText; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ReviewText, value, _ReviewText);
				if (PropertyChanging(args))
				{
					_ReviewText = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Boolean IsApproved
		{	
			get{ return _IsApproved; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_IsApproved, value, _IsApproved);
				if (PropertyChanging(args))
				{
					_IsApproved = value;
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
		public  ProductReviewBase Clone()
		{
			ProductReviewBase newObj = new  ProductReviewBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.ProductId = this.ProductId;						
			newObj.CustomerName = this.CustomerName;						
			newObj.Rating = this.Rating;						
			newObj.ReviewText = this.ReviewText;						
			newObj.IsApproved = this.IsApproved;						
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
			info.AddValue(ProductReviewBase.Property_Id, Id);				
			info.AddValue(ProductReviewBase.Property_ProductId, ProductId);				
			info.AddValue(ProductReviewBase.Property_CustomerName, CustomerName);				
			info.AddValue(ProductReviewBase.Property_Rating, Rating);				
			info.AddValue(ProductReviewBase.Property_ReviewText, ReviewText);				
			info.AddValue(ProductReviewBase.Property_IsApproved, IsApproved);				
			info.AddValue(ProductReviewBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(ProductReviewBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(ProductReviewBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(ProductReviewBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}
