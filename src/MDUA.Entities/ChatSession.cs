using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Framework;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace MDUA.Entities
{
	public partial class ChatSession 
	{

        [NotMapped]
        public int UnreadCount { get; set; }

    }



}