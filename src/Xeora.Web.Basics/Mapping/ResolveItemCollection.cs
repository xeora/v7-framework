using System.Collections.Generic;

namespace Xeora.Web.Basics.Mapping
{
    public class ResolveItemCollection : List<ResolveItem>
    {
        public ResolveItem this[string ID]
        {
            get
            {
                foreach (ResolveItem resolveItem in this)
                {
                    if (string.Compare(resolveItem.ID, ID, true) == 0)
                        return resolveItem;
                }

                return new ResolveItem(ID);
            }
        }
    }
}
