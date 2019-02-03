using System;
using System.Collections.Generic;

namespace Xeora.Web.Basics.ControlResult
{
    [Serializable]
    public class VariableBlock : Dictionary<string, object>
    {
        public VariableBlock() : base(StringComparer.InvariantCultureIgnoreCase)
        { }

        public new void Add(string key, object value)
        {
            if (base.ContainsKey(key))
                base[key] = value;
            else
                base.Add(key, value);
        }

        public new object this[string key]
        {
            get
            {
                if (base.ContainsKey(key))
                    return base[key];

                return null;
            }
            set => this.Add(key, value);
        }
    }
}
