using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "EmailTemplateBase", Namespace = "http://www.piistech.com//entities")]
	public class EmailTemplateBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			TemplateKey = 1,
			Name = 2,
			Description = 3,
			ToEmail = 4,
			CcEmail = 5,
			BccEmail = 6,
			FromEmail = 7,
			FromName = 8,
			ReplyEmail = 9,
			Subject = 10,
			BodyContent = 11,
			BodyFile = 12,
			IsActive = 13,
			LastUpdatedBy = 14,
			LastUpdatedDate = 15
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_TemplateKey = "TemplateKey";		            
		public const string Property_Name = "Name";		            
		public const string Property_Description = "Description";		            
		public const string Property_ToEmail = "ToEmail";		            
		public const string Property_CcEmail = "CcEmail";		            
		public const string Property_BccEmail = "BccEmail";		            
		public const string Property_FromEmail = "FromEmail";		            
		public const string Property_FromName = "FromName";		            
		public const string Property_ReplyEmail = "ReplyEmail";		            
		public const string Property_Subject = "Subject";		            
		public const string Property_BodyContent = "BodyContent";		            
		public const string Property_BodyFile = "BodyFile";		            
		public const string Property_IsActive = "IsActive";		            
		public const string Property_LastUpdatedBy = "LastUpdatedBy";		            
		public const string Property_LastUpdatedDate = "LastUpdatedDate";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private String _TemplateKey;	            
		private String _Name;	            
		private String _Description;	            
		private String _ToEmail;	            
		private String _CcEmail;	            
		private String _BccEmail;	            
		private String _FromEmail;	            
		private String _FromName;	            
		private String _ReplyEmail;	            
		private String _Subject;	            
		private String _BodyContent;	            
		private String _BodyFile;	            
		private Nullable<Boolean> _IsActive;	            
		private String _LastUpdatedBy;	            
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
		public String Name
		{	
			get{ return _Name; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Name, value, _Name);
				if (PropertyChanging(args))
				{
					_Name = value;
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
		public String ReplyEmail
		{	
			get{ return _ReplyEmail; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_ReplyEmail, value, _ReplyEmail);
				if (PropertyChanging(args))
				{
					_ReplyEmail = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String Subject
		{	
			get{ return _Subject; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Subject, value, _Subject);
				if (PropertyChanging(args))
				{
					_Subject = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String BodyContent
		{	
			get{ return _BodyContent; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_BodyContent, value, _BodyContent);
				if (PropertyChanging(args))
				{
					_BodyContent = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public String BodyFile
		{	
			get{ return _BodyFile; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_BodyFile, value, _BodyFile);
				if (PropertyChanging(args))
				{
					_BodyFile = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Boolean> IsActive
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
		public String LastUpdatedBy
		{	
			get{ return _LastUpdatedBy; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_LastUpdatedBy, value, _LastUpdatedBy);
				if (PropertyChanging(args))
				{
					_LastUpdatedBy = value;
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
		public  EmailTemplateBase Clone()
		{
			EmailTemplateBase newObj = new  EmailTemplateBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.TemplateKey = this.TemplateKey;						
			newObj.Name = this.Name;						
			newObj.Description = this.Description;						
			newObj.ToEmail = this.ToEmail;						
			newObj.CcEmail = this.CcEmail;						
			newObj.BccEmail = this.BccEmail;						
			newObj.FromEmail = this.FromEmail;						
			newObj.FromName = this.FromName;						
			newObj.ReplyEmail = this.ReplyEmail;						
			newObj.Subject = this.Subject;						
			newObj.BodyContent = this.BodyContent;						
			newObj.BodyFile = this.BodyFile;						
			newObj.IsActive = this.IsActive;						
			newObj.LastUpdatedBy = this.LastUpdatedBy;						
			newObj.LastUpdatedDate = this.LastUpdatedDate;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(EmailTemplateBase.Property_Id, Id);				
			info.AddValue(EmailTemplateBase.Property_TemplateKey, TemplateKey);				
			info.AddValue(EmailTemplateBase.Property_Name, Name);				
			info.AddValue(EmailTemplateBase.Property_Description, Description);				
			info.AddValue(EmailTemplateBase.Property_ToEmail, ToEmail);				
			info.AddValue(EmailTemplateBase.Property_CcEmail, CcEmail);				
			info.AddValue(EmailTemplateBase.Property_BccEmail, BccEmail);				
			info.AddValue(EmailTemplateBase.Property_FromEmail, FromEmail);				
			info.AddValue(EmailTemplateBase.Property_FromName, FromName);				
			info.AddValue(EmailTemplateBase.Property_ReplyEmail, ReplyEmail);				
			info.AddValue(EmailTemplateBase.Property_Subject, Subject);				
			info.AddValue(EmailTemplateBase.Property_BodyContent, BodyContent);				
			info.AddValue(EmailTemplateBase.Property_BodyFile, BodyFile);				
			info.AddValue(EmailTemplateBase.Property_IsActive, IsActive);				
			info.AddValue(EmailTemplateBase.Property_LastUpdatedBy, LastUpdatedBy);				
			info.AddValue(EmailTemplateBase.Property_LastUpdatedDate, LastUpdatedDate);				
		}
		#endregion

		
	}
}