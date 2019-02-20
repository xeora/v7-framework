using System.Collections.Generic;
using Xeora.Web.Global;

namespace Xeora.Web.Directives
{
    public class RenderBag : Dictionary<string, Single>
    {
        private readonly IDirective _Parent;

        public RenderBag(IDirective parent) =>
            this._Parent = parent;

        public void Add(string label, string rawValue, ArgumentCollection arguments)
        {
            Single single = new Single(rawValue, arguments)
            {
                Parent = this._Parent
            };

            base.Add(label, single);
        }

        public void Render(string requesterUniqueID)
        {
            IEnumerator<KeyValuePair<string, Single>> bagEnum = 
                this.GetEnumerator();

            while (bagEnum.MoveNext())
            {
                KeyValuePair<string, Single> item = 
                    bagEnum.Current;

                item.Value.Render(requesterUniqueID);
            }
        }
    }
}