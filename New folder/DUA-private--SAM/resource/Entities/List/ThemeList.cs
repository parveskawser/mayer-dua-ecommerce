using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Collections.Generic;

using MDUA.Framework;

namespace MDUA.Entities.List
{
	[Serializable]
	[CollectionDataContract(Name = "ThemeList", Namespace = "http://www.piistech.com//list")]	
	public class ThemeList : BaseCollection<Theme>
	{
		#region Constructors
	    public ThemeList() : base() { }
        public ThemeList(Theme[] list) : base(list) { }
        public ThemeList(List<Theme> list) : base(list) { }
		#endregion
		
		#region Custom Methods
		#endregion
	}	
}
