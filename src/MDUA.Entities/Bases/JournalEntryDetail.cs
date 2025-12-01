using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using MDUA.Framework;
using MDUA.Entities.Bases;
using MDUA.Entities.List;

namespace MDUA.Entities
{
	[Serializable]
    [DataContract(Name = "JournalEntryDetail", Namespace = "http://www.piistech.com//entities")]
	public partial class JournalEntryDetail : JournalEntryDetailBase
	{
		#region Exernal Properties
		private Account _AccountIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="Account"/>.
		/// </summary>
		/// <value>The source Account for _AccountIdObject.</value>
		[DataMember]
		public Account AccountIdObject
      	{
            get { return this._AccountIdObject; }
            set { this._AccountIdObject = value; }
      	}
		
		private JournalEntry _JournalEntryIdObject = null;
		
		/// <summary>
		/// Gets or sets the source <see cref="JournalEntry"/>.
		/// </summary>
		/// <value>The source JournalEntry for _JournalEntryIdObject.</value>
		[DataMember]
		public JournalEntry JournalEntryIdObject
      	{
            get { return this._JournalEntryIdObject; }
            set { this._JournalEntryIdObject = value; }
      	}
		
		#endregion
		
		#region Orverride Equals
		public override bool Equals(Object obj)		
		{
			if (obj.GetType() != typeof(JournalEntryDetail))
            {
                return false;
            }			
			
			 JournalEntryDetail _paramObj = obj as JournalEntryDetail;
            if (_paramObj != null)
            {			
                return (_paramObj.Id == this.Id && _paramObj.CustomPropertyMatch(this));
            }
            else
            {
                return base.Equals(obj);
            }
		}
		#endregion
		
		#region Orverride HashCode
		 public override int GetHashCode()
        {
            return base.Id.GetHashCode();
        }
		#endregion		
	}
}
