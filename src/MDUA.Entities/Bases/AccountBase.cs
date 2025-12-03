using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "AccountBase", Namespace = "http://www.piistech.com//entities")]
	public class AccountBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			AccountCode = 1,
			AccountName = 2,
			AccountTypeId = 3,
			IsActive = 4,
			Description = 5,
			Balance = 6,
			CurrencyCode = 7,
			ParentAccountId = 8,
			CreatedBy = 9,
			CreatedAt = 10,
			UpdatedBy = 11,
			UpdatedAt = 12
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_AccountCode = "AccountCode";		            
		public const string Property_AccountName = "AccountName";		            
		public const string Property_AccountTypeId = "AccountTypeId";		            
		public const string Property_IsActive = "IsActive";		            
		public const string Property_Description = "Description";		            
		public const string Property_Balance = "Balance";		            
		public const string Property_CurrencyCode = "CurrencyCode";		            
		public const string Property_ParentAccountId = "ParentAccountId";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _AccountCode;	            
		private String _AccountName;	            
		private Int32 _AccountTypeId;	            
		private Boolean _IsActive;	            
		private String _Description;	            
		private Nullable<Decimal> _Balance;	            
		private String _CurrencyCode;	            
		private Nullable<Int32> _ParentAccountId;	            
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
		public String AccountCode
		{	
			get{ return _AccountCode; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AccountCode, value, _AccountCode);
				if (PropertyChanging(args))
				{
					_AccountCode = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String AccountName
		{	
			get{ return _AccountName; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AccountName, value, _AccountName);
				if (PropertyChanging(args))
				{
					_AccountName = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 AccountTypeId
		{	
			get{ return _AccountTypeId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AccountTypeId, value, _AccountTypeId);
				if (PropertyChanging(args))
				{
					_AccountTypeId = value;
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
		public String Description
		{	
			get{ return _Description; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Description, value, _Description);
				if (PropertyChanging(args))
				{
					_Description = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Decimal> Balance
		{	
			get{ return _Balance; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Balance, value, _Balance);
				if (PropertyChanging(args))
				{
					_Balance = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String CurrencyCode
		{	
			get{ return _CurrencyCode; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CurrencyCode, value, _CurrencyCode);
				if (PropertyChanging(args))
				{
					_CurrencyCode = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Int32> ParentAccountId
		{	
			get{ return _ParentAccountId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ParentAccountId, value, _ParentAccountId);
				if (PropertyChanging(args))
				{
					_ParentAccountId = value;
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
		public  AccountBase Clone()
		{
			AccountBase newObj = new  AccountBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.AccountCode = this.AccountCode;						
			newObj.AccountName = this.AccountName;						
			newObj.AccountTypeId = this.AccountTypeId;						
			newObj.IsActive = this.IsActive;						
			newObj.Description = this.Description;						
			newObj.Balance = this.Balance;						
			newObj.CurrencyCode = this.CurrencyCode;						
			newObj.ParentAccountId = this.ParentAccountId;						
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
			info.AddValue(AccountBase.Property_Id, Id);				
			info.AddValue(AccountBase.Property_AccountCode, AccountCode);				
			info.AddValue(AccountBase.Property_AccountName, AccountName);				
			info.AddValue(AccountBase.Property_AccountTypeId, AccountTypeId);				
			info.AddValue(AccountBase.Property_IsActive, IsActive);				
			info.AddValue(AccountBase.Property_Description, Description);				
			info.AddValue(AccountBase.Property_Balance, Balance);				
			info.AddValue(AccountBase.Property_CurrencyCode, CurrencyCode);				
			info.AddValue(AccountBase.Property_ParentAccountId, ParentAccountId);				
			info.AddValue(AccountBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(AccountBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(AccountBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(AccountBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}
