using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;

namespace MDUA.Entities.Bases
{
	[Serializable]
    [DataContract(Name = "CompanyPaymentMethodBase", Namespace = "http://www.piistech.com//entities")]
	public class CompanyPaymentMethodBase : BaseBusinessEntity
	{
	
		#region Enum Collection
		public enum Columns
		{
			Id = 0,
			CompanyId = 1,
			PaymentMethodId = 2,
			IsActive = 3,
			CreatedBy = 4,
			CreatedAt = 5,
			UpdatedBy = 6,
			UpdatedAt = 7,
			CustomInstruction = 8,
			IsManualEnabled = 9,
			IsGatewayEnabled = 10
		}
		#endregion
	
		#region Constants
		public const string Property_Id = "Id";		            
		public const string Property_CompanyId = "CompanyId";		            
		public const string Property_PaymentMethodId = "PaymentMethodId";		            
		public const string Property_IsActive = "IsActive";		            
		public const string Property_CreatedBy = "CreatedBy";		            
		public const string Property_CreatedAt = "CreatedAt";		            
		public const string Property_UpdatedBy = "UpdatedBy";		            
		public const string Property_UpdatedAt = "UpdatedAt";		            
		public const string Property_CustomInstruction = "CustomInstruction";		            
		public const string Property_IsManualEnabled = "IsManualEnabled";		            
		public const string Property_IsGatewayEnabled = "IsGatewayEnabled";		            
		#endregion
		
		#region Private Data Types
		private Int32 _Id;	            
		private Int32 _CompanyId;	            
		private Int32 _PaymentMethodId;	            
		private Boolean _IsActive;	            
		private String _CreatedBy;	            
		private Nullable<DateTime> _CreatedAt;	            
		private String _UpdatedBy;	            
		private Nullable<DateTime> _UpdatedAt;	            
		private String _CustomInstruction;	            
		private Boolean _IsManualEnabled;	            
		private Boolean _IsGatewayEnabled;	            
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
		public Int32 CompanyId
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
		public Int32 PaymentMethodId
		{	
			get{ return _PaymentMethodId; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_PaymentMethodId, value, _PaymentMethodId);
				if (PropertyChanging(args))
				{
					_PaymentMethodId = value;
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
		public Nullable<DateTime> CreatedAt
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
		public String CustomInstruction
		{	
			get{ return _CustomInstruction; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_CustomInstruction, value, _CustomInstruction);
				if (PropertyChanging(args))
				{
					_CustomInstruction = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Boolean IsManualEnabled
		{	
			get{ return _IsManualEnabled; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_IsManualEnabled, value, _IsManualEnabled);
				if (PropertyChanging(args))
				{
					_IsManualEnabled = value;
					PropertyChanged(args);					
				}	
			}
        }

		[DataMember]
		public Boolean IsGatewayEnabled
		{	
			get{ return _IsGatewayEnabled; }			
			set
			{
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(Property_IsGatewayEnabled, value, _IsGatewayEnabled);
				if (PropertyChanging(args))
				{
					_IsGatewayEnabled = value;
					PropertyChanged(args);					
				}	
			}
        }

		#endregion
		
		#region Cloning Base Objects
		public  CompanyPaymentMethodBase Clone()
		{
			CompanyPaymentMethodBase newObj = new  CompanyPaymentMethodBase();
			base.CloneBase(newObj);
			newObj.Id = this.Id;						
			newObj.CompanyId = this.CompanyId;						
			newObj.PaymentMethodId = this.PaymentMethodId;						
			newObj.IsActive = this.IsActive;						
			newObj.CreatedBy = this.CreatedBy;						
			newObj.CreatedAt = this.CreatedAt;						
			newObj.UpdatedBy = this.UpdatedBy;						
			newObj.UpdatedAt = this.UpdatedAt;						
			newObj.CustomInstruction = this.CustomInstruction;						
			newObj.IsManualEnabled = this.IsManualEnabled;						
			newObj.IsGatewayEnabled = this.IsGatewayEnabled;						
			
			return newObj;
		}
		#endregion
		
		#region Getting object by adding value of that properties 
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(CompanyPaymentMethodBase.Property_Id, Id);				
			info.AddValue(CompanyPaymentMethodBase.Property_CompanyId, CompanyId);				
			info.AddValue(CompanyPaymentMethodBase.Property_PaymentMethodId, PaymentMethodId);				
			info.AddValue(CompanyPaymentMethodBase.Property_IsActive, IsActive);				
			info.AddValue(CompanyPaymentMethodBase.Property_CreatedBy, CreatedBy);				
			info.AddValue(CompanyPaymentMethodBase.Property_CreatedAt, CreatedAt);				
			info.AddValue(CompanyPaymentMethodBase.Property_UpdatedBy, UpdatedBy);				
			info.AddValue(CompanyPaymentMethodBase.Property_UpdatedAt, UpdatedAt);				
			info.AddValue(CompanyPaymentMethodBase.Property_CustomInstruction, CustomInstruction);				
			info.AddValue(CompanyPaymentMethodBase.Property_IsManualEnabled, IsManualEnabled);				
			info.AddValue(CompanyPaymentMethodBase.Property_IsGatewayEnabled, IsGatewayEnabled);				
		}
		#endregion

		
	}
}