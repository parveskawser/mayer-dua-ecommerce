using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDUA.Framework.Utils.Modificators
{
    public abstract class Modificator
    {

        protected Hashtable _parameters = new Hashtable();

        public Hashtable Parameters
        {
            get { return _parameters; }
        }

        public abstract void Apply(ref string Value, params string[] Parameters);
    }

    class NL2BR : Modificator
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.Replace("\n", "<br>");
        }
    }

    class HTMLENCODE : Modificator
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.Replace("&", "&amp;");
            Value = Value.Replace("<", "&lt;");
            Value = Value.Replace(">", "&gt;");
        }
    }

    class UPPER : Modificator
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.ToUpper();
        }
    }

    class LOWER : Modificator
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.ToLower();
        }
    }

    class TRIM : Modificator
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.Trim();
        }
    }

    class TRIMEND : Modificator
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.TrimEnd();
        }
    }

    class TRIMSTART : Modificator
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            Value = Value.TrimStart();
        }
    }

    class DEFAULT : Modificator
    {
        public override void Apply(ref string Value, params string[] Parameters)
        {
            if (Value == null || Value.Trim() == string.Empty)
                Value = Parameters[0];
        }
    }
}
