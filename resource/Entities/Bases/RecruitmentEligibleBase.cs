using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "RecruitmentEligibleBase", Namespace = "http://www.piistech.com//entities")]
	public class RecruitmentEligibleBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			CompanyId = 1,
			RecruitmentEligibleId = 2,
			TestResultMark = 3,
			InterViewResultMark = 4,
			Status = 5,
			CreatedBy = 6,
			CreatedDate = 7,
			LastUpdatedBy = 8,
			LastUpdatedDate = 9,
			CandidateId = 10
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_CompanyId = "CompanyId";		            
		public const string Property_RecruitmentEligibleId = "RecruitmentEligibleId";		            
		public const string Property_TestResultMark = "TestResultMark";		            
		public const string Property_InterViewResultMark = "InterViewResultMark";		            
		public const string Property_Status = "Status";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedDate = "CreatedDate";		            
		public const string Property_LastUpdatedBy = "LastUpdatedBy";		            
		public const string Property_LastUpdatedDate = "LastUpdatedDate";		            
		public const string Property_CandidateId = "CandidateId";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Guid _CompanyId;	            
		private Guid _RecruitmentEligibleId;	            
		private Nullable<Decimal> _TestResultMark;	            
		private Nullable<Decimal> _InterViewResultMark;	            
		private Nullable<Boolean> _Status;	            
		private Guid _CreatedBy;	            
		private Nullable<DateTime> _CreatedDate;	            
		private Guid _LastUpdatedBy;	            
		private Nullable<DateTime> _LastUpdatedDate;	            
		private Guid _CandidateId;	            
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
		public Guid CompanyId
		{	
			get{ return _CompanyId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CompanyId, value, _CompanyId);
				if (PropertyChanging(args))
				{
					_CompanyId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Guid RecruitmentEligibleId
		{	
			get{ return _RecruitmentEligibleId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_RecruitmentEligibleId, value, _RecruitmentEligibleId);
				if (PropertyChanging(args))
				{
					_RecruitmentEligibleId = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Decimal> TestResultMark
		{	
			get{ return _TestResultMark; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_TestResultMark, value, _TestResultMark);
				if (PropertyChanging(args))
				{
					_TestResultMark = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Decimal> InterViewResultMark
		{	
			get{ return _InterViewResultMark; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_InterViewResultMark, value, _InterViewResultMark);
				if (PropertyChanging(args))
				{
					_InterViewResultMark = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Nullable<Boolean> Status
		{	
			get{ return _Status; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_Status, value, _Status);
				if (PropertyChanging(args))
				{
					_Status = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Guid CreatedBy
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
		public Nullable<DateTime> CreatedDate
		{	
			get{ return _CreatedDate; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CreatedDate, value, _CreatedDate);
				if (PropertyChanging(args))
				{
					_CreatedDate = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Guid LastUpdatedBy
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

		[DataMember]
		public Guid CandidateId
		{	
			get{ return _CandidateId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CandidateId, value, _CandidateId);
				if (PropertyChanging(args))
				{
					_CandidateId = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  RecruitmentEligibleBase Clone()
		{
			RecruitmentEligibleBase newObj = new  RecruitmentEligibleBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.CompanyId = this.CompanyId;						
			newObj.RecruitmentEligibleId = this.RecruitmentEligibleId;						
			newObj.TestResultMark = this.TestResultMark;						
			newObj.InterViewResultMark = this.InterViewResultMark;						
			newObj.Status = this.Status;						
			newObj.CreatedBy = this.CreatedBy;						
			newObj.CreatedDate = this.CreatedDate;						
			newObj.LastUpdatedBy = this.LastUpdatedBy;						
			newObj.LastUpdatedDate = this.LastUpdatedDate;						
			newObj.CandidateId = this.CandidateId;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(RecruitmentEligibleBase.Property_Id, Id);				
			info.AddValue(RecruitmentEligibleBase.Property_CompanyId, CompanyId);				
			info.AddValue(RecruitmentEligibleBase.Property_RecruitmentEligibleId, RecruitmentEligibleId);				
			info.AddValue(RecruitmentEligibleBase.Property_TestResultMark, TestResultMark);				
			info.AddValue(RecruitmentEligibleBase.Property_InterViewResultMark, InterViewResultMark);				
			info.AddValue(RecruitmentEligibleBase.Property_Status, Status);				
			info.AddValue(RecruitmentEligibleBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(RecruitmentEligibleBase.Property_CreatedDate, CreatedDate);				
			info.AddValue(RecruitmentEligibleBase.Property_LastUpdatedBy, LastUpdatedBy);				
			info.AddValue(RecruitmentEligibleBase.Property_LastUpdatedDate, LastUpdatedDate);				
			info.AddValue(RecruitmentEligibleBase.Property_CandidateId, CandidateId);				
		}
		#endregion

		
	}
}