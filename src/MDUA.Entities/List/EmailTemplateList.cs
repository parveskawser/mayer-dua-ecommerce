using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "EmailTemplateList", Namespace = "http://www.piistech.com//list")]	
	public class EmailTemplateList : BaseCollection<EmailTemplate>
	{
		#region Constructors
	    public EmailTemplateList() : base() { }
        public EmailTemplateList(EmailTemplate[] list) : base(list) { }
        public EmailTemplateList(List<EmailTemplate> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
