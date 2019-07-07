using System.Collections.Generic;

namespace Xeora.Web.Basics.Mapping
{
    public class ResolveItemCollection : List<ResolveItem>
    {
        public ResolveItem this[string Id]
        {
            get
            {
                foreach (ResolveItem resolveItem in this)
                {
                    if (string.Compare(resolveItem.Id, Id, true) == 0)
                        return resolveItem;
                }

                return new ResolveItem(Id);
            }
        }
    }
}
