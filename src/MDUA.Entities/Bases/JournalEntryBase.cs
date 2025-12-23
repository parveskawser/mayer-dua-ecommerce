using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "JournalEntryBase", Namespace = "http://www.piistech.com//entities")]
	public class JournalEntryBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			ReferenceType = 1,
			ReferenceId = 2,
			EntryDate = 3,
			Description = 4,
			CreatedBy = 5,
			CreatedAt = 6,
			UpdatedBy = 7,
			UpdatedAt = 8
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_ReferenceType = "ReferenceType";		            
		public const string Property_ReferenceId = "ReferenceId";		            
		public const string Property_EntryDate = "EntryDate";		            
		public const string Property_Description = "Description";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _ReferenceType;	            
		private Nullable<Int32> _ReferenceId;	            
		private DateTime _EntryDate;	            
		private String _Description;	            
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
		public String ReferenceType
		{	
			get{ return _ReferenceType; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ReferenceType, value, _ReferenceType);
				if (PropertyChanging(args))
				{
					_ReferenceType = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Int32> ReferenceId
		{	
			get{ return _ReferenceId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ReferenceId, value, _ReferenceId);
				if (PropertyChanging(args))
				{
					_ReferenceId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime EntryDate
		{	
			get{ return _EntryDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_EntryDate, value, _EntryDate);
				if (PropertyChanging(args))
				{
					_EntryDate = value;
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
		public  JournalEntryBase Clone()
		{
			JournalEntryBase newObj = new  JournalEntryBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.ReferenceType = this.ReferenceType;						
			newObj.ReferenceId = this.ReferenceId;						
			newObj.EntryDate = this.EntryDate;						
			newObj.Description = this.Description;						
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
			info.AddValue(JournalEntryBase.Property_Id, Id);				
			info.AddValue(JournalEntryBase.Property_ReferenceType, ReferenceType);				
			info.AddValue(JournalEntryBase.Property_ReferenceId, ReferenceId);				
			info.AddValue(JournalEntryBase.Property_EntryDate, EntryDate);				
			info.AddValue(JournalEntryBase.Property_Description, Description);				
			info.AddValue(JournalEntryBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(JournalEntryBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(JournalEntryBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(JournalEntryBase.Property_UpdatedAt, UpdatedAt);				
		}
		#endregion

		
	}
}
