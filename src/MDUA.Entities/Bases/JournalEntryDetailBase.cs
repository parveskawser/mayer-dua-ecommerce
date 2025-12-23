using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "JournalEntryDetailBase", Namespace = "http://www.piistech.com//entities")]
	public class JournalEntryDetailBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			JournalEntryId = 1,
			AccountId = 2,
			Debit = 3,
			Credit = 4,
			CreatedBy = 5,
			CreatedAt = 6,
			UpdatedBy = 7,
			UpdatedAt = 8,
			LineDescription = 9,
			CurrencyCode = 10,
			ExchangeRate = 11
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_JournalEntryId = "JournalEntryId";		            
		public const string Property_AccountId = "AccountId";		            
		public const string Property_Debit = "Debit";		            
		public const string Property_Credit = "Credit";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		public const string Property_LineDescription = "LineDescription";		            
		public const string Property_CurrencyCode = "CurrencyCode";		            
		public const string Property_ExchangeRate = "ExchangeRate";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _JournalEntryId;	            
		private Int32 _AccountId;	            
		private Decimal _Debit;	            
		private Decimal _Credit;	            
		private String _CreatedBy;	            
		private DateTime _CreatedAt;	            
		private String _UpdatedBy;	            
		private Nullable<DateTime> _UpdatedAt;	            
		private String _LineDescription;	            
		private String _CurrencyCode;	            
		private Nullable<Decimal> _ExchangeRate;	            
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
		public Int32 JournalEntryId
		{	
			get{ return _JournalEntryId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_JournalEntryId, value, _JournalEntryId);
				if (PropertyChanging(args))
				{
					_JournalEntryId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 AccountId
		{	
			get{ return _AccountId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_AccountId, value, _AccountId);
				if (PropertyChanging(args))
				{
					_AccountId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Decimal Debit
		{	
			get{ return _Debit; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Debit, value, _Debit);
				if (PropertyChanging(args))
				{
					_Debit = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Decimal Credit
		{	
			get{ return _Credit; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Credit, value, _Credit);
				if (PropertyChanging(args))
				{
					_Credit = value;
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

		[DataMember]
		public String LineDescription
		{	
			get{ return _LineDescription; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_LineDescription, value, _LineDescription);
				if (PropertyChanging(args))
				{
					_LineDescription = value;
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
		public Nullable<Decimal> ExchangeRate
		{	
			get{ return _ExchangeRate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ExchangeRate, value, _ExchangeRate);
				if (PropertyChanging(args))
				{
					_ExchangeRate = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  JournalEntryDetailBase Clone()
		{
			JournalEntryDetailBase newObj = new  JournalEntryDetailBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.JournalEntryId = this.JournalEntryId;						
			newObj.AccountId = this.AccountId;						
			newObj.Debit = this.Debit;						
			newObj.Credit = this.Credit;						
			newObj.CreatedBy = this.CreatedBy;						
			newObj.CreatedAt = this.CreatedAt;						
			newObj.UpdatedBy = this.UpdatedBy;						
			newObj.UpdatedAt = this.UpdatedAt;						
			newObj.LineDescription = this.LineDescription;						
			newObj.CurrencyCode = this.CurrencyCode;						
			newObj.ExchangeRate = this.ExchangeRate;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(JournalEntryDetailBase.Property_Id, Id);				
			info.AddValue(JournalEntryDetailBase.Property_JournalEntryId, JournalEntryId);				
			info.AddValue(JournalEntryDetailBase.Property_AccountId, AccountId);				
			info.AddValue(JournalEntryDetailBase.Property_Debit, Debit);				
			info.AddValue(JournalEntryDetailBase.Property_Credit, Credit);				
			info.AddValue(JournalEntryDetailBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(JournalEntryDetailBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(JournalEntryDetailBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(JournalEntryDetailBase.Property_UpdatedAt, UpdatedAt);				
			info.AddValue(JournalEntryDetailBase.Property_LineDescription, LineDescription);				
			info.AddValue(JournalEntryDetailBase.Property_CurrencyCode, CurrencyCode);				
			info.AddValue(JournalEntryDetailBase.Property_ExchangeRate, ExchangeRate);				
		}
		#endregion

		
	}
}
