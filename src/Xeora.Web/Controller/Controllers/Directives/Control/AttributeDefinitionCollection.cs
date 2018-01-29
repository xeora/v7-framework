using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Xeora.Web.Controller.Directive.Control
{
    public class AttributeDefinitionCollection : List<AttributeDefinition>
    {
        public void Add(string key, string value)
        {
            base.Add(new AttributeDefinition(key, value));
        }

        public void Remove(string key)
        {
            foreach (AttributeDefinition item in this)
            {
                if (string.Compare(key, item.Key, true) == 0)
                {
                    base.Remove(item);

                    break;
                }
            }
        }

        public string this[string key]
        {
            get
            {
                foreach (AttributeDefinition aI in this)
                {
                    if (string.Compare(key, aI.Key, true) == 0)
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

        public new AttributeDefinition this[int index]
        {
            get { return base[index]; }
            set
            {
                this.RemoveAt(index);
                this.Insert(index, value);
            }
        }

        public override string ToString()
        {
            StringBuilder rSB = new StringBuilder();
            CultureInfo compareCulture = new CultureInfo("en-US");

            foreach (AttributeDefinition aI in this)
            {
                if (compareCulture.CompareInfo.Compare(aI.Key, "key", CompareOptions.IgnoreCase) != 0)
                {
                    if (aI.Key == null || aI.Key.Trim().Length == 0)
                        rSB.AppendFormat(" {0}", aI.Value);
                    else
                        rSB.AppendFormat(" {0}=\"{1}\"", aI.Key, aI.Value.Replace("\"", "\\\""));
                }
            }

            return rSB.ToString();
        }
    }
}