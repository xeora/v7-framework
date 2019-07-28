using System;
using System.Collections.Generic;
using System.Text;

namespace Xeora.Web.Basics.Domain.Control
{
    public class AttributeCollection : List<Attribute>
    {
        public void Add(string key, string value) =>
            base.Add(new Attribute(key, value));

        public void Remove(string key)
        {
            foreach (Attribute item in this)
            {
                if (string.Compare(key, item.Key, StringComparison.OrdinalIgnoreCase) != 0) continue;
                
                base.Remove(item);
                break;
            }
        }

        public string this[string key]
        {
            get
            {
                foreach (Attribute aI in this)
                {
                    if (string.Compare(key, aI.Key, StringComparison.OrdinalIgnoreCase) == 0)
                        return aI.Value;
                }

                return null;
            }
            set
            {
                this.Remove(key);
                this.Add(key, value);
            }
        }

        public new Attribute this[int index]
        {
            get => base[index];
            set
            {
                this.RemoveAt(index);
                this.Insert(index, value);
            }
        }

        public override string ToString()
        {
            StringBuilder sB = new StringBuilder();

            foreach (Attribute aI in this)
            {
                if (string.Compare(aI.Key, "key", System.StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    if (aI.Key == null || aI.Key.Trim().Length == 0)
                        sB.AppendFormat(" {0}", aI.Value);
                    else
                        sB.AppendFormat(" {0}=\"{1}\"", aI.Key, aI.Value.Replace("\"", "\\\""));
                }
            }

            return sB.ToString();
        }

        public AttributeCollection Clone()
        {
            AttributeCollection attributes =
                new AttributeCollection();

            foreach (Attribute attribute in this)
                attributes.Add(new Attribute(attribute.Key, attribute.Value));

            return attributes;
        }
    }
}