using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "EmailHistoryBase", Namespace = "http://www.piistech.com//entities")]
	public class EmailHistoryBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			TemplateKey = 1,
			ToEmail = 2,
			CcEmail = 3,
			BccEmail = 4,
			FromEmail = 5,
			FromName = 6,
			EmailSubject = 7,
			EmailBodyContent = 8,
			EmailSentDate = 9,
			IsSystemAutoSent = 10,
			IsRead = 11,
			ReadCount = 12,
			LastUpdatedDate = 13
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_TemplateKey = "TemplateKey";		            
		public const string Property_ToEmail = "ToEmail";		            
		public const string Property_CcEmail = "CcEmail";		            
		public const string Property_BccEmail = "BccEmail";		            
		public const string Property_FromEmail = "FromEmail";		            
		public const string Property_FromName = "FromName";		            
		public const string Property_EmailSubject = "EmailSubject";		            
		public const string Property_EmailBodyContent = "EmailBodyContent";		            
		public const string Property_EmailSentDate = "EmailSentDate";		            
		public const string Property_IsSystemAutoSent = "IsSystemAutoSent";		            
		public const string Property_IsRead = "IsRead";		            
		public const string Property_ReadCount = "ReadCount";		            
		public const string Property_LastUpdatedDate = "LastUpdatedDate";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _TemplateKey;	            
		private String _ToEmail;	            
		private String _CcEmail;	            
		private String _BccEmail;	            
		private String _FromEmail;	            
		private String _FromName;	            
		private String _EmailSubject;	            
		private String _EmailBodyContent;	            
		private Nullable<DateTime> _EmailSentDate;	            
		private Nullable<Boolean> _IsSystemAutoSent;	            
		private Nullable<Boolean> _IsRead;	            
		private Nullable<Int32> _ReadCount;	            
		private Nullable<DateTime> _LastUpdatedDate;	            
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
		public String TemplateKey
		{	
			get{ return _TemplateKey; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_TemplateKey, value, _TemplateKey);
				if (PropertyChanging(args))
				{
					_TemplateKey = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String ToEmail
		{	
			get{ return _ToEmail; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ToEmail, value, _ToEmail);
				if (PropertyChanging(args))
				{
					_ToEmail = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String CcEmail
		{	
			get{ return _CcEmail; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CcEmail, value, _CcEmail);
				if (PropertyChanging(args))
				{
					_CcEmail = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String BccEmail
		{	
			get{ return _BccEmail; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_BccEmail, value, _BccEmail);
				if (PropertyChanging(args))
				{
					_BccEmail = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String FromEmail
		{	
			get{ return _FromEmail; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_FromEmail, value, _FromEmail);
				if (PropertyChanging(args))
				{
					_FromEmail = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String FromName
		{	
			get{ return _FromName; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_FromName, value, _FromName);
				if (PropertyChanging(args))
				{
					_FromName = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String EmailSubject
		{	
			get{ return _EmailSubject; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_EmailSubject, value, _EmailSubject);
				if (PropertyChanging(args))
				{
					_EmailSubject = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String EmailBodyContent
		{	
			get{ return _EmailBodyContent; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_EmailBodyContent, value, _EmailBodyContent);
				if (PropertyChanging(args))
				{
					_EmailBodyContent = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<DateTime> EmailSentDate
		{	
			get{ return _EmailSentDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_EmailSentDate, value, _EmailSentDate);
				if (PropertyChanging(args))
				{
					_EmailSentDate = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Boolean> IsSystemAutoSent
		{	
			get{ return _IsSystemAutoSent; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_IsSystemAutoSent, value, _IsSystemAutoSent);
				if (PropertyChanging(args))
				{
					_IsSystemAutoSent = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Boolean> IsRead
		{	
			get{ return _IsRead; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_IsRead, value, _IsRead);
				if (PropertyChanging(args))
				{
					_IsRead = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Int32> ReadCount
		{	
			get{ return _ReadCount; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ReadCount, value, _ReadCount);
				if (PropertyChanging(args))
				{
					_ReadCount = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<DateTime> LastUpdatedDate
		{	
			get{ return _LastUpdatedDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_LastUpdatedDate, value, _LastUpdatedDate);
				if (PropertyChanging(args))
				{
					_LastUpdatedDate = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  EmailHistoryBase Clone()
		{
			EmailHistoryBase newObj = new  EmailHistoryBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.TemplateKey = this.TemplateKey;						
			newObj.ToEmail = this.ToEmail;						
			newObj.CcEmail = this.CcEmail;						
			newObj.BccEmail = this.BccEmail;						
			newObj.FromEmail = this.FromEmail;						
			newObj.FromName = this.FromName;						
			newObj.EmailSubject = this.EmailSubject;						
			newObj.EmailBodyContent = this.EmailBodyContent;						
			newObj.EmailSentDate = this.EmailSentDate;						
			newObj.IsSystemAutoSent = this.IsSystemAutoSent;						
			newObj.IsRead = this.IsRead;						
			newObj.ReadCount = this.ReadCount;						
			newObj.LastUpdatedDate = this.LastUpdatedDate;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(EmailHistoryBase.Property_Id, Id);				
			info.AddValue(EmailHistoryBase.Property_TemplateKey, TemplateKey);				
			info.AddValue(EmailHistoryBase.Property_ToEmail, ToEmail);				
			info.AddValue(EmailHistoryBase.Property_CcEmail, CcEmail);				
			info.AddValue(EmailHistoryBase.Property_BccEmail, BccEmail);				
			info.AddValue(EmailHistoryBase.Property_FromEmail, FromEmail);				
			info.AddValue(EmailHistoryBase.Property_FromName, FromName);				
			info.AddValue(EmailHistoryBase.Property_EmailSubject, EmailSubject);				
			info.AddValue(EmailHistoryBase.Property_EmailBodyContent, EmailBodyContent);				
			info.AddValue(EmailHistoryBase.Property_EmailSentDate, EmailSentDate);				
			info.AddValue(EmailHistoryBase.Property_IsSystemAutoSent, IsSystemAutoSent);				
			info.AddValue(EmailHistoryBase.Property_IsRead, IsRead);				
			info.AddValue(EmailHistoryBase.Property_ReadCount, ReadCount);				
			info.AddValue(EmailHistoryBase.Property_LastUpdatedDate, LastUpdatedDate);				
		}
		#endregion

		
	}
}