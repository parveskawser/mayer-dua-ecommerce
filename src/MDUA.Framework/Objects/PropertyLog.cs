using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MDUA.Framework.Objects
{
    [Serializable]
    [DataContract]
    public class PropertyLog
    {
        public string Name { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }
}
