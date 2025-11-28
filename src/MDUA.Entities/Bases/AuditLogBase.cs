using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "AuditLogBase", Namespace = "http://www.piistech.com//entities")]
	public class AuditLogBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			TableName = 1,
			Operation = 2,
			PrimaryKeyValue = 3,
			OldValues = 4,
			NewValues = 5,
			ChangedBy = 6,
			ChangeDate = 7
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_TableName = "TableName";		            
		public const string Property_Operation = "Operation";		            
		public const string Property_PrimaryKeyValue = "PrimaryKeyValue";		            
		public const string Property_OldValues = "OldValues";		            
		public const string Property_NewValues = "NewValues";		            
		public const string Property_ChangedBy = "ChangedBy";		            
		public const string Property_ChangeDate = "ChangeDate";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _TableName;	            
		private String _Operation;	            
		private Int32 _PrimaryKeyValue;	            
		private String _OldValues;	            
		private String _NewValues;	            
		private String _ChangedBy;	            
		private DateTime _ChangeDate;	            
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
		public String TableName
		{	
			get{ return _TableName; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_TableName, value, _TableName);
				if (PropertyChanging(args))
				{
					_TableName = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Operation
		{	
			get{ return _Operation; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Operation, value, _Operation);
				if (PropertyChanging(args))
				{
					_Operation = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Int32 PrimaryKeyValue
		{	
			get{ return _PrimaryKeyValue; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PrimaryKeyValue, value, _PrimaryKeyValue);
				if (PropertyChanging(args))
				{
					_PrimaryKeyValue = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String OldValues
		{	
			get{ return _OldValues; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_OldValues, value, _OldValues);
				if (PropertyChanging(args))
				{
					_OldValues = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String NewValues
		{	
			get{ return _NewValues; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_NewValues, value, _NewValues);
				if (PropertyChanging(args))
				{
					_NewValues = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String ChangedBy
		{	
			get{ return _ChangedBy; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ChangedBy, value, _ChangedBy);
				if (PropertyChanging(args))
				{
					_ChangedBy = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public DateTime ChangeDate
		{	
			get{ return _ChangeDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ChangeDate, value, _ChangeDate);
				if (PropertyChanging(args))
				{
					_ChangeDate = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  AuditLogBase Clone()
		{
			AuditLogBase newObj = new  AuditLogBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.TableName = this.TableName;						
			newObj.Operation = this.Operation;						
			newObj.PrimaryKeyValue = this.PrimaryKeyValue;						
			newObj.OldValues = this.OldValues;						
			newObj.NewValues = this.NewValues;						
			newObj.ChangedBy = this.ChangedBy;						
			newObj.ChangeDate = this.ChangeDate;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(AuditLogBase.Property_Id, Id);				
			info.AddValue(AuditLogBase.Property_TableName, TableName);				
			info.AddValue(AuditLogBase.Property_Operation, Operation);				
			info.AddValue(AuditLogBase.Property_PrimaryKeyValue, PrimaryKeyValue);				
			info.AddValue(AuditLogBase.Property_OldValues, OldValues);				
			info.AddValue(AuditLogBase.Property_NewValues, NewValues);				
			info.AddValue(AuditLogBase.Property_ChangedBy, ChangedBy);				
			info.AddValue(AuditLogBase.Property_ChangeDate, ChangeDate);				
		}
		#endregion

		
	}
}
